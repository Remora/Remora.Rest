//
//  SPDX-FileName: BoundDataObjectConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Rest.Json.Reflection;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// The actual implementation of <see cref="DataObjectConverter{TInterface, TImplementation}"/>, created by that same
/// type. These instances are bound to a certain set of <see cref="JsonSerializerOptions"/> and cache any data it might
/// need.
/// </summary>
/// <typeparam name="T">The type this instance converts.</typeparam>
internal sealed class BoundDataObjectConverter<T> : JsonConverter<T>
{
    private readonly ObjectFactory<T> _dtoFactory;
    private readonly bool _allowExtraProperties;

    // Split the property list to avoid "CanWrite" checks everywhere and instead perform that preemptively

    // Properties relevant for Write(...)
    private readonly DTOPropertyInfo[] _writeProperties;

    // Properties relevant for Read(...)
    private readonly DTOPropertyInfo[] _readProperties;

    // Speed up looking up the correct property. Also implicitly means property names can't be duplicated
    private readonly Dictionary<string, (bool IsPrimary, DTOPropertyInfo DTOProperty)> _readPropertiesByName;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundDataObjectConverter{T}"/> class.
    /// </summary>
    /// <param name="dtoFactory">The DTO factory.</param>
    /// <param name="allowExtraProperties">Whether extra undefined properties should be allowed.</param>
    /// <param name="writeProperties">Properties relevant when writing the DTO to JSON.</param>
    /// <param name="readProperties">Properties relevant when reading the DTO from JSON.</param>
    public BoundDataObjectConverter
    (
        ObjectFactory<T> dtoFactory,
        bool allowExtraProperties,
        DTOPropertyInfo[] writeProperties,
        DTOPropertyInfo[] readProperties
    )
    {
        _dtoFactory = dtoFactory;
        _allowExtraProperties = allowExtraProperties;
        _writeProperties = writeProperties;
        _readProperties = readProperties;

        _readPropertiesByName = _readProperties
            .SelectMany(p => p.ReadNames.Select((n, i) => (IsPrimary: i == 0, n, DTOProperty: p)))
            .ToDictionary(x => x.n, x => (x.IsPrimary, x.DTOProperty));
    }

    /// <inheritdoc />
    public override T Read
    (
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject || !reader.Read())
        {
            throw new JsonException();
        }

        var readProperties = _readProperties;

        // Possible Optimization: Cache this array and reuse it.
        // F.e. in a thread-static field. This method isn't async anyways.

        // Avoid creating a dictionary and fill the arguments array directly
        var constructorArguments = new object?[readProperties.Length];

        // We use "Missing" as a sentinel to indicate that a value hasn't
        // been seen yet without needing to allocate additional memory.
        constructorArguments.AsSpan().Fill(DataObjectConverterHelpers.Missing);

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            var propertyName = reader.GetString()!;
            if (!reader.Read())
            {
                throw new JsonException();
            }

            var (isPrimaryChoice, dtoProperty) = GetReadPropertyInfo(propertyName);

            if (dtoProperty is null)
            {
                if (!_allowExtraProperties)
                {
                    throw new JsonException
                    (
                        $"DTO disallows extra properties and has no matching property for JSON property {propertyName}."
                    );
                }

                // No matching property - we'll skip it
                if (!reader.TrySkip())
                {
                    throw new JsonException("Couldn't skip elements.");
                }

                if (!reader.Read())
                {
                    throw new JsonException("Unexpectedly reached end of JSON.");
                }

                continue;
            }

            var propertyValue = JsonSerializer.Deserialize
            (
                ref reader,
                dtoProperty.Property.PropertyType,
                dtoProperty.Options
            );

            // Verify nullability
            if (propertyValue is null && !dtoProperty.AllowsNull)
            {
                throw new JsonException($"null is not a valid value for DTO property \"{dtoProperty.Property.Name}\".");
            }

            var index = dtoProperty.ReadIndex;
            if (isPrimaryChoice || constructorArguments[index] == DataObjectConverterHelpers.Missing)
            {
                constructorArguments[index] = propertyValue;
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }
        }

        // Polyfill/check properties that weren't found yet
        for (var i = 0; i < constructorArguments.Length; i++)
        {
            if (constructorArguments[i] != DataObjectConverterHelpers.Missing)
            {
                continue;
            }

            var dtoProperty = readProperties[i];
            if (dtoProperty.DefaultValue.TryGet(out var defaultValue))
            {
                constructorArguments[i] = defaultValue;
            }
            else
            {
                throw new JsonException
                (
                    $"The data property \"{dtoProperty.Property.Name}\" did not have a corresponding value in the JSON."
                );
            }
        }

        return _dtoFactory(constructorArguments);
    }

    /// <inheritdoc />
    public override void Write
    (
        Utf8JsonWriter writer,
        T value,
        JsonSerializerOptions options
    )
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        foreach (var dtoProperty in _writeProperties)
        {
            dtoProperty.Writer(writer, dtoProperty, value);
        }

        writer.WriteEndObject();
    }

    private (bool IsPrimaryChoice, DTOPropertyInfo? DTOProperty) GetReadPropertyInfo(string name)
    {
        return _readPropertiesByName.TryGetValue(name, out var v) ? v : default;
    }
}
