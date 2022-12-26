//
//  SPDX-FileName: IRestHttpClient.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Rest;

/// <summary>
/// Represents the public API of a REST HTTP client.
/// </summary>
[PublicAPI]
public interface IRestHttpClient : IRestCustomizable
{
    /// <summary>
    /// Performs a GET request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to retrieve.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> GetAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a GET request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="jsonPath">The path to the element to deserialize.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to retrieve.</typeparam>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> GetAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a GET request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A retrieval result which may or may not have succeeded.</returns>
    Task<Result<Stream>> GetContentAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a POST request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to create.</typeparam>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PostAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a POST request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="jsonPath">The path to the element to deserialize.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to create.</typeparam>
    /// <returns>A creation result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PostAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a POST request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A post result which may or may not have succeeded.</returns>
    Task<Result> PostAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PATCH request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to modify.</typeparam>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PatchAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PATCH request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="jsonPath">The path to the element to deserialize.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The entity type to modify.</typeparam>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PatchAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PATCH request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A modification result which may or may not have succeeded.</returns>
    Task<Result> PatchAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a DELETE request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> DeleteAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a DELETE request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="jsonPath">The path to the element to deserialize.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> DeleteAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a DELETE request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A deletion result which may or may not have succeeded.</returns>
    Task<Result> DeleteAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PUT request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PutAsync<TEntity>
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PUT request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="jsonPath">The path to the element to deserialize.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="allowNullReturn">Whether to allow null return values inside the creation result.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <typeparam name="TEntity">The type of entity to create.</typeparam>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result<TEntity>> PutAsync<TEntity>
    (
        string endpoint,
        string jsonPath,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        bool allowNullReturn = false,
        CancellationToken ct = default
    );

    /// <summary>
    /// Performs a PUT request to the REST API at the given endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <param name="configureRequestBuilder">The request builder for the request.</param>
    /// <param name="ct">The cancellation token for this operation.</param>
    /// <returns>A result which may or may not have succeeded.</returns>
    Task<Result> PutAsync
    (
        string endpoint,
        Action<RestRequestBuilder>? configureRequestBuilder = null,
        CancellationToken ct = default
    );
}
