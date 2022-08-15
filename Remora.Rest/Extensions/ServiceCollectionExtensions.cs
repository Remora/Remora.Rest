//
//  ServiceCollectionExtensions.cs
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

using System.Net.Http;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Remora.Rest.Json;
using Remora.Rest.Json.Internal;

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
