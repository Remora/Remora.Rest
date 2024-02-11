//
//  SPDX-FileName: RestHttpClient.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Rest.Extensions;
using Remora.Rest.Results;
using Remora.Results;

namespace Remora.Rest;

/// <summary>
/// Represents a specialized HTTP client for the REST APIs.
/// </summary>
/// <typeparam name="TError">A type which represents an error payload returned by the API.</typeparam>
[PublicAPI]
public class RestHttpClient<TError> : IRestHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly List<RestRequestCustomization> _customizations;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestHttpClient{TError}"/> class.
    /// </summary>
    /// <param name="httpClient">The Http client.</param>
    /// <param name="serializerOptions">The serialization options.</param>
    public RestHttpClient
    (
        HttpClient httpClient,
        JsonSerializerOptions serializerOptions
    )
    {
        _httpClient = httpClient;
        _serializerOptions = serializerOptions;

        _customizations = new List<RestRequestCustomization>();
    }

    /// <inheritdoc />
    public RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer)
    {
        var customization = new RestRequestCustomization(this, requestCustomizer);
        _customizations.Add(customization);

        return customization;
    }

    /// <inheritdoc />
    void IRestCustomizable.RemoveCustomization(RestRequestCustomization customization)
    {
        _customizations.Remove(customization);
    }

    /// <inheritdoc />
    public Task<Result<TEntity>> GetAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
        => GetAsync<TEntity>(endpoint, string.Empty, configureRequestBuilder, allowNullReturn, ct);

    /// <inheritdoc />
    public async Task<Result<TEntity>> GetAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Get);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync<TEntity>(response, jsonPath, allowNullReturn, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result<Stream>> GetContentAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Get);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            var unpackedResponse = await UnpackResponseAsync(response, ct);
            if (!unpackedResponse.IsSuccess)
            {
                return Result<Stream>.FromError(unpackedResponse);
            }

            #if NETSTANDARD
            var responseContent = await response.Content.ReadAsStreamAsync();
            #else
            var responseContent = await response.Content.ReadAsStreamAsync(ct);
            #endif

            return responseContent;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Task<Result<TEntity>> PostAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
        => PostAsync<TEntity>(endpoint, string.Empty, configureRequestBuilder, allowNullReturn, ct);

    /// <inheritdoc />
    public async Task<Result<TEntity>> PostAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Post);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync<TEntity>(response, jsonPath, allowNullReturn, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result> PostAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Post);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync(response, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Task<Result<TEntity>> PatchAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
        => PatchAsync<TEntity>(endpoint, string.Empty, configureRequestBuilder, allowNullReturn, ct);

    /// <inheritdoc />
    public async Task<Result<TEntity>> PatchAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Patch);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync<TEntity>(response, jsonPath, allowNullReturn, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result> PatchAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Patch);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync(response, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result> DeleteAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Delete);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync(response, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Task<Result<TEntity>> DeleteAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
        => DeleteAsync<TEntity>(endpoint, string.Empty, configureRequestBuilder, allowNullReturn, ct);

    /// <inheritdoc />
    public async Task<Result<TEntity>> DeleteAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Delete);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync<TEntity>(response, jsonPath, allowNullReturn, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public Task<Result<TEntity>> PutAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
        => PutAsync<TEntity>(endpoint, string.Empty, configureRequestBuilder, allowNullReturn, ct);

    /// <inheritdoc />
    public async Task<Result<TEntity>> PutAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Put);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync<TEntity>(response, jsonPath, allowNullReturn, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <inheritdoc />
    public async Task<Result> PutAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    )
    {
        configureRequestBuilder ??= _ => { };

        var requestBuilder = new RestRequestBuilder(endpoint);
        configureRequestBuilder(requestBuilder);

        requestBuilder.WithMethod(HttpMethod.Put);

        foreach (var customization in _customizations)
        {
            customization.RequestCustomizer(requestBuilder);
        }

        try
        {
            using var request = requestBuilder.Build();
            using var response = await _httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );

            return await UnpackResponseAsync(response, ct);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Unpacks a response from the API, attempting to deserialize either a plain success or a parsed
    /// error.
    /// </summary>
    /// <param name="response">The response to unpack.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    private async Task<Result> UnpackResponseAsync
    (
        HttpResponseMessage response,
        CancellationToken ct = default
    )
    {
        if (response.IsSuccessStatusCode)
        {
            return Result.Success;
        }

        // See if we have a JSON error to get some more details from
        if (response.Content.Headers.ContentLength is <= 0)
        {
            return new HttpResultError(response.StatusCode, response.ReasonPhrase);
        }

        try
        {
            #if NETSTANDARD
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            #else
            await using var contentStream = await response.Content.ReadAsStreamAsync(ct);
            #endif

            var error = await JsonSerializer.DeserializeAsync<TError>
            (
                contentStream,
                _serializerOptions,
                ct
            );

            if (error is null)
            {
                return new HttpResultError(response.StatusCode, response.ReasonPhrase);
            }

            return new RestResultError<TError>(error);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Unpacks a response from the API, attempting to either get the requested entity type or a parsed
    /// error.
    /// </summary>
    /// <param name="response">The response to unpack.</param>
    /// <param name="jsonPath">The path to the json node to deserialize.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to unpack.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    private async Task<Result<TEntity>> UnpackResponseAsync<TEntity>
    (
        HttpResponseMessage response,
        string jsonPath = "",
        bool allowNullReturn = false,
        CancellationToken ct = default
    )
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentLength == 0 || response.StatusCode == HttpStatusCode.NoContent)
            {
                if (!allowNullReturn)
                {
                    throw new InvalidOperationException("Response content null, but null returns not allowed.");
                }

                return default;
            }

            #if NETSTANDARD
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            #else
            await using var contentStream = await response.Content.ReadAsStreamAsync(ct);
            #endif

            TEntity? entity;

            if (string.IsNullOrEmpty(jsonPath))
            {
                entity = await JsonSerializer.DeserializeAsync<TEntity>
                (
                    contentStream,
                    _serializerOptions,
                    ct
                );
            }
            else
            {
                var doc = await JsonSerializer.DeserializeAsync<JsonDocument>
                (
                    contentStream,
                    _serializerOptions,
                    ct
                );

                var element = doc.SelectElement(jsonPath);

                if (!element.HasValue)
                {
                    return allowNullReturn
                        ? default
                        : throw new InvalidOperationException
                        (
                            "The requested path does not exist or the found content is empty."
                        );
                }

                entity = element.Value.ToObject<TEntity>(_serializerOptions);
            }

            if (entity is not null)
            {
                return entity;
            }

            if (!allowNullReturn)
            {
                throw new InvalidOperationException("Response content null, but null returns not allowed.");
            }

            return default;
        }

        // See if we have a JSON error to get some more details from
        if (response.Content.Headers.ContentLength == 0)
        {
            return new HttpResultError(response.StatusCode, response.ReasonPhrase);
        }

        try
        {
            #if NETSTANDARD
            await using var contentStream = await response.Content.ReadAsStreamAsync();
            #else
            await using var contentStream = await response.Content.ReadAsStreamAsync(ct);
            #endif

            var error = await JsonSerializer.DeserializeAsync<TError>
            (
                contentStream,
                _serializerOptions,
                ct
            );

            if (error is null)
            {
                return new HttpResultError(response.StatusCode, response.ReasonPhrase);
            }

            return new RestResultError<TError>(error);
        }
        catch (Exception e)
        {
            return e;
        }
    }
}
