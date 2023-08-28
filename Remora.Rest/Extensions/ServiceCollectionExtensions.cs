//
//  SPDX-FileName: ServiceCollectionExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Rest.Json;
using Remora.Rest.Json.Contexts;
using Remora.Rest.Json.Internal;
using Remora.Rest.Options;

namespace Remora.Rest.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
[PublicAPI]
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures supporting JSON converters for REST APIs, optionally scoped to a set of named options.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="optionsName">The name of the JSON options to configure, if any.</param>
    /// <returns>The services, with the converters configured.</returns>
    public static IServiceCollection ConfigureRestJsonConverters
    (
        this IServiceCollection services,
        string? optionsName = null
    )
    {
        return services.Configure<JsonSerializerOptions>
        (
            optionsName,
            options =>
            {
                options
                    .AddConverter<ColorConverter>()
                    .AddConverter<OneOfConverterFactory>()
                    .AddConverter<ISO8601DateTimeOffsetConverter>();
            }
        );
    }

    /// <summary>
    /// Adds a REST-specialized HTTP client, allowing subsequent optional configuration of the backend client.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="optionsName">The name of the JSON options to retrieve, if any.</param>
    /// <typeparam name="TError">The error that the created client will handle.</typeparam>
    /// <returns>The client builder for the REST client.</returns>
    public static IHttpClientBuilder AddRestHttpClient<TError>
    (
        this IServiceCollection services,
        string? optionsName = null
    )
    {
        var httpClientBuilder = services.AddHttpClient<RestHttpClient<TError>>();

        services.Replace(ServiceDescriptor.Transient(s =>
        {
            var client = s.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientBuilder.Name);
            var options = s.GetRequiredService<IOptionsMonitor<JsonSerializerOptions>>().Get(optionsName);

            return new RestHttpClient<TError>(client, options);
        }));

        services.TryAddTransient<IRestHttpClient>(s => s.GetRequiredService<RestHttpClient<TError>>());
        return httpClientBuilder;
    }

    /// <summary>
    /// Creates a source-generated JSON de/serialization context. This method serves both as a hook point for source
    /// generation and as runtime configuration.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TContext">The context type to generate.</typeparam>
    /// <returns>A fluent builder that can be used to configure the model further.</returns>
    #pragma warning disable CS0618 // Type or member is obsolete
    public static GeneratedContextBuilder CreateGeneratedJsonContext<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TContext>
    (
        this IServiceCollection serviceCollection
    )
        where TContext : AbstractGeneratedSerializerContext<TContext>
    {
        serviceCollection.AddSingleton
        <
            IOptionsFactory<FluentSerializerContext>,
            ServiceContainerOptionsFactory<FluentSerializerContext>
        >();

        serviceCollection.TryAddSingleton<FluentSerializerContext>
        (
            s => s.GetRequiredService<IOptions<FluentSerializerContext>>().Value
        );

        serviceCollection.AddSingleton<TContext>
        (
            s =>
            {
                var options = s.GetService<JsonSerializerOptions>() ?? new JsonSerializerOptions();
                return AbstractGeneratedSerializerContext<TContext>.ContextFactory(options);
            }
        );

        serviceCollection.AddSingleton<JsonSerializerContext, TContext>();

        return new GeneratedContextBuilder(serviceCollection);
    }
    #pragma warning restore CS0618 // Type or member is obsolete
}
