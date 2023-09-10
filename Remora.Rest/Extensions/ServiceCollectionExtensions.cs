//
//  SPDX-FileName: ServiceCollectionExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Net.Http;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Rest.Json;

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
                    .AddConverter<OptionalConverterFactory>()
                    .AddConverter<NullableConverterFactory>()
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
}
