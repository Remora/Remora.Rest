//
//  SPDX-FileName: ConfiguredJsonTypeInfoResolverOptionsFactory.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Linq;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Creates instances of the <see cref="ConfiguredJsonTypeInfoResolver"/> based on named configuration.
/// </summary>
public class ConfiguredJsonTypeInfoResolverOptionsFactory : IOptionsFactory<ConfiguredJsonTypeInfoResolver>
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredJsonTypeInfoResolverOptionsFactory"/> class.
    /// </summary>
    /// <param name="services">The available services.</param>
    public ConfiguredJsonTypeInfoResolverOptionsFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc/>
    public ConfiguredJsonTypeInfoResolver Create(string name)
    {
        var factory = _services.GetRequiredService<IOptionsFactory<FluentJsonTypeInfoResolver>>();
        var configuration = factory.Create(name);

        var resolvers = _services.GetServices<IJsonTypeInfoResolver>()
            .Where(s => configuration.ContextTypes.Contains(s.GetType()))
            .ToArray();

        foreach (var contextType in configuration.ContextTypes)
        {
            if (resolvers.Any(r => r.GetType() == contextType))
            {
                continue;
            }

            var displayName = name == string.Empty
                ? nameof(ConfiguredJsonTypeInfoResolver)
                : $"{nameof(ConfiguredJsonTypeInfoResolver)} ({name})";

            throw new InvalidOperationException
            (
                $"Missing a registered implementation of {contextType} associated with {displayName}"
            );
        }

        return new ConfiguredJsonTypeInfoResolver
        (
            resolvers,
            configuration.TypeInfoConfigurations,
            configuration.InterfaceConverters
        );
    }
}
