//
//  BoundDataObjectConverter.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Rest.Extensions;
using Remora.Rest.Json.Reflection;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// The actual implementation of <see cref="DataObjectConverter{TInterface, TImplementation}"/>, created by that same type.
/// These instances are bound to a certain set of <see cref="JsonSerializerOptions"/> and cache any data it might need.
/// </summary>
/// <typeparam name="TInterface">The interface that is seen in the objects.</typeparam>
/// <typeparam name="TImplementation">The concrete implementation.</typeparam>
internal sealed class BoundDataObjectConverter<TInterface, TImplementation> : JsonConverter<TInterface>
    where TImplementation : TInterface
{
    private readonly ObjectFactory<TInterface> _dtoFactory;
    private readonly bool _allowExtraProperties;

    // Split the property list to avoid "CanWrite" checks everywhere and instead perform that preemptively

    // Properties relevant for Write(...)
    private readonly DTOPropertyInfo[] _writeProperties;

    // Properties relevant for Read(...)
    private readonly DTOPropertyInfo[] _readProperties;

    // Speed up looking up the correct property. Also implicitly means property names can't be duplicated
    private readonly Dictionary<string, (bool IsPrimary, DTOPropertyInfo DTOProperty)> _readPropertiesByName;

    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(TImplementation) || base.CanConvert(typeToConvert);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundDataObjectConverter{TInterface, TImplementation}"/> class.
    /// </summary>
    /// <param name="source">The source converter factory that created this instance.</param>
    /// <param name="options">The serializer options this instance is bound to.</param>
    public BoundDataObjectConverter(DataObjectConverter<TInterface, TImplementation> source, JsonSerializerOptions options)
    {
        _dtoFactory = source.DTOFactory;
        _allowExtraProperties = source.AllowsExtraProperties;

        var dtoProperties = source.DTOProperties;

        var writeProperties = new List<DTOPropertyInfo>();
        var readProperties = new List<DTOPropertyInfo>();

        var interfaceMap = typeof(TImplementation).GetInterfaceMap(typeof(TInterface));

        foreach (var property in dtoProperties)
        {
            var converter = source.GetConverter(property, options);
            var propertyOptions = CreatePropertyConverterOptions(options, converter);
            var defaultValue = source.GetDefaultValueForType(property.PropertyType);
            var readNames = source.GetReadJsonPropertyName(property, options);
            var writeNames = source.GetWriteJsonPropertyName(property, options);
            var writer = source.GetPropertyWriter(property);

            // We cache this as well since the check is somewhat complex
            bool allowsNull = property.AllowsNull();

            var data = new DTOPropertyInfo
            (
                property,
                readNames,
                writeNames,
                writer,
                allowsNull,
                defaultValue,
                propertyOptions,
                readProperties.Count
            );

            if (property.CanWrite)
            {
                // If a property is writable, it can be *read* from JSON.
                readProperties.Add(data);
            }

            if (property.CanWrite || source.ShouldIncludeReadOnlyProperty(property))
            {
                // Any property that is writable and not excluded due to being read-only,
                // can be *written* to JSON.
                writeProperties.Add(data);
            }
        }

        _writeProperties = writeProperties.ToArray();
        _readProperties = readProperties.ToArray();

        _readPropertiesByName = readProperties
            .SelectMany(p => p.ReadNames.Select((n, i) => (IsPrimary: i == 0, n, DTOProperty: p)))
            .ToDictionary(x => x.n, x => (x.IsPrimary, x.DTOProperty));
    }

    private static JsonSerializerOptions CreatePropertyConverterOptions(JsonSerializerOptions options, JsonConverter? converter)
    {
        if (converter == null)
        {
            return options;
        }

        var cloned = new JsonSerializerOptions(options);
        cloned.Converters.Insert(0, converter);

        return cloned;
    }

    /// <inheritdoc />
    public override TInterface Read
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
        constructorArguments.AsSpan().Fill(DataObjectConverterShared.Missing);

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
                    throw new JsonException();
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

            var propertyValue = JsonSerializer.Deserialize(ref reader, dtoProperty.Property.PropertyType, dtoProperty.Options);

            // Verify nullability
            if (propertyValue is null && !dtoProperty.AllowsNull)
            {
                throw new JsonException($"null isn't supported for DTO property \"{dtoProperty.Property.Name}\".");
            }

            int index = dtoProperty.ReadIndex;
            if (isPrimaryChoice || constructorArguments[index] == DataObjectConverterShared.Missing)
            {
                constructorArguments[index] = propertyValue;
            }

            if (!reader.Read())
            {
                throw new JsonException();
            }
        }

        // Polyfill/check properties that weren't found yet
        for (int i = 0; i < constructorArguments.Length; i++)
        {
            if (constructorArguments[i] == DataObjectConverterShared.Missing)
            {
                var dtoProperty = readProperties[i];
                if (dtoProperty.DefaultValue != null)
                {
                    constructorArguments[i] = dtoProperty.DefaultValue;
                }
                else
                {
                    throw new JsonException
                    (
                        $"The data property \"{dtoProperty.Property.Name}\" did not have a corresponding value in the JSON."
                    );
                }
            }
        }

        return _dtoFactory(constructorArguments);
    }

    /// <inheritdoc />
    public override void Write
    (
        Utf8JsonWriter writer,
        TInterface value,
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
