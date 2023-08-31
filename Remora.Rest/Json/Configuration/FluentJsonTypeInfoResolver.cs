//
//  SPDX-FileName: FluentJsonTypeInfoResolver.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Remora.Rest.Json.Converters;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Represents mutable configuration used to build a <see cref="ConfiguredJsonTypeInfoResolver"/>.
/// </summary>
public class FluentJsonTypeInfoResolver
{
    private readonly List<Type> _contextTypes = new();
    private readonly Dictionary<Type, IJsonTypeInfoConfiguration> _typeInfoConfigurations = new();
    private readonly List<JsonConverter> _interfaceConverters = new();

    /// <summary>
    /// Gets the context types associated with this configuration.
    /// </summary>
    public IReadOnlyList<Type> ContextTypes => _contextTypes;

    /// <summary>
    /// Gets the type info configurations associated with this configuration.
    /// </summary>
    public IReadOnlyCollection<IJsonTypeInfoConfiguration> TypeInfoConfigurations => _typeInfoConfigurations.Values;

    /// <summary>
    /// Gets the required interface converters.
    /// </summary>
    public IReadOnlyList<JsonConverter> InterfaceConverters => _interfaceConverters;

    /// <summary>
    /// Apply runtime configurations to the given type.
    /// </summary>
    /// <param name="configure">The configuration for the type.</param>
    /// <typeparam name="TType">The type to configure.</typeparam>
    /// <returns>The configuration, with the type configurations applied.</returns>
    public FluentJsonTypeInfoResolver ConfigureType<TType>
    (
        Action<JsonTypeInfoConfiguration<TType>> configure
    )
    {
        var type = typeof(TType);

        if (!_typeInfoConfigurations.TryGetValue(type, out var storedConfiguration))
        {
            storedConfiguration = new JsonTypeInfoConfiguration<TType>();
            _typeInfoConfigurations.Add(type, storedConfiguration);
        }

        var configuration = Unsafe.As<JsonTypeInfoConfiguration<TType>>(storedConfiguration);
        configure(configuration);

        return this;
    }

    /// <summary>
    /// Configures a data object, associating the implementation type as the default type used when deserializing
    /// interface-typed properties and values.
    /// </summary>
    /// <param name="configureInterface">
    /// The configuration function for the interface type. Any type implementing the interface that is de/serialized by
    /// the JSON model will have this configuration applied.
    /// </param>
    /// <param name="configureImplementation">
    /// The configuration function for the implementation type. Only the implementation type will have this
    /// configuration applied.
    /// </param>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The builder.</returns>
    public FluentJsonTypeInfoResolver ConfigureDataObject<TInterface, TImplementation>
    (
        Action<JsonTypeInfoConfiguration<TInterface>>? configureInterface = null,
        Action<JsonTypeInfoConfiguration<TImplementation>>? configureImplementation = null
    )
        where TImplementation : TInterface
        where TInterface : notnull
    {
        if (!_interfaceConverters.Any(c => c is InterfaceConverter<TInterface, TImplementation>))
        {
            _interfaceConverters.Add(new InterfaceConverter<TInterface, TImplementation>());
        }

        if (configureInterface is not null)
        {
            ConfigureType(configureInterface);
        }

        if (configureImplementation is not null)
        {
            ConfigureType(configureImplementation);
        }

        return this;
    }

    /// <summary>
    /// Adds the given context type to the contexts used for type resolution by this configured resolver.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    /// <returns>The configuration, with the context type added.</returns>
    #pragma warning disable IL3050
    public FluentJsonTypeInfoResolver WithContext<TContext>() => WithContext(typeof(TContext));
    #pragma warning restore IL3050

    /// <summary>
    /// Adds the given context type to the contexts used for type resolution by this configured resolver.
    /// </summary>
    /// <param name="contextType">The context type.</param>
    /// <returns>The configuration, with the context type added.</returns>
    [RequiresDynamicCode("Adding contexts without providing static type information may not ensure that all optional properties can be correctly de/serialized.")]
    public FluentJsonTypeInfoResolver WithContext(Type contextType)
    {
        if (_contextTypes.Contains(contextType))
        {
            return this;
        }

        _contextTypes.Add(contextType);
        return this;
    }
}
