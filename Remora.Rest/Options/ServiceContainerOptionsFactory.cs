//
//  SPDX-FileName: ServiceContainerOptionsFactory.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Remora.Rest.Options;

/// <summary>
/// Enables service-activated option objects.
/// </summary>
/// <typeparam name="TOptions">The options type.</typeparam>
internal class ServiceContainerOptionsFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors)] TOptions> : IOptionsFactory<TOptions>
    where TOptions : class
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceContainerOptionsFactory{TOptions}"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    public ServiceContainerOptionsFactory(IServiceProvider services)
    {
        _services = services;
    }

    /// <inheritdoc/>
    public TOptions Create(string name)
    {
        var service = ActivatorUtilities.CreateInstance<TOptions>(_services, name);

        var configureService = _services.GetServices<IConfigureOptions<TOptions>>();
        var postConfigureOptions = _services.GetServices<IPostConfigureOptions<TOptions>>();

        foreach (var configure in configureService)
        {
            if (configure is IConfigureNamedOptions<TOptions> configureNamed)
            {
                configureNamed.Configure(name, service);
            }
            else
            {
                configure.Configure(service);
            }
        }

        foreach (var postConfigure in postConfigureOptions)
        {
            postConfigure.PostConfigure(name, service);
        }

        return service;
    }
}
