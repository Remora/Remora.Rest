//
//  SPDX-FileName: ConfiguredJsonTypeInfoResolver.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Represents a runtime configuration applied to a <see cref="IJsonTypeInfoResolver"/>.
/// </summary>
public class ConfiguredJsonTypeInfoResolver : IJsonTypeInfoResolver
{
    private readonly IReadOnlyList<IJsonTypeInfoResolver> _resolvers;
    private readonly IReadOnlyList<JsonConverter> _additionalConverters;
    private readonly IReadOnlyDictionary<Type, IJsonTypeInfoConfiguration> _typeInfoConfigurations;
    private readonly SameTypeEqualityComparer<JsonConverter> _sameTypeComparer = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredJsonTypeInfoResolver"/> class.
    /// </summary>
    /// <param name="resolvers">The resolvers that should be used to resolve the base type information.</param>
    /// <param name="typeInfoConfigurations">The runtime type info configurations to use.</param>
    /// <param name="interfaceConverters">The interface converters required by the resolver.</param>
    public ConfiguredJsonTypeInfoResolver
    (
        IEnumerable<IJsonTypeInfoResolver> resolvers,
        IEnumerable<IJsonTypeInfoConfiguration> typeInfoConfigurations,
        IEnumerable<JsonConverter> interfaceConverters
    )
    {
        _resolvers = resolvers.OrderBy(r => r is not DefaultJsonTypeInfoResolver).ToArray();

        // it is expected that an exhaustive list of applicable Optional<T> converters are provided via this mechanism
        _additionalConverters = _resolvers.OfType<IAdditionalConverterProvider>()
            .SelectMany(i => i.AdditionalConverters)
            .Concat(interfaceConverters)
            .Distinct(_sameTypeComparer)
            .ToArray();

        _typeInfoConfigurations = typeInfoConfigurations.ToDictionary(c => c.Type);
    }

    /// <summary>
    /// Resolves a strongly-typed <see cref="JsonTypeInfo"/> for the given type.
    /// </summary>
    /// <param name="options">The serializer options to use.</param>
    /// <typeparam name="T">The type to resolve information for.</typeparam>
    /// <returns>The type information.</returns>
    public JsonTypeInfo<T>? GetTypeInfo<T>(JsonSerializerOptions options)
        => Unsafe.As<JsonTypeInfo<T>>(GetTypeInfo(typeof(T), options));

    /// <inheritdoc/>
    public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        // augment the options
        foreach (var additionalConverter in _additionalConverters)
        {
            if (options.Converters.Contains(additionalConverter, _sameTypeComparer))
            {
                continue;
            }

            options.Converters.Add(additionalConverter);
        }

        var typeInfo = _resolvers.Select(r => r.GetTypeInfo(type, options)).FirstOrDefault(r => r is not null);
        if (typeInfo is null)
        {
            return null;
        }

        // pull out all configurations that are type-compatible with the base type information
        var applicableConfigurationTypes = _typeInfoConfigurations.Keys.Where(k => k.IsAssignableFrom(type));
        var applicableConfigurations = applicableConfigurationTypes.Select(k => _typeInfoConfigurations[k]);

        foreach (var configuration in applicableConfigurations)
        {
            configuration.Configure(typeInfo);
        }

        if (!options.IsReadOnly)
        {
            options.TypeInfoResolver = this;
        }

        typeInfo.OriginatingResolver = this;
        typeInfo.MakeReadOnly();

        return typeInfo;
    }
}
