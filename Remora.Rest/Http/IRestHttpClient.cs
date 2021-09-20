//
//  IRestHttpClient.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Remora.Results;

namespace Remora.Rest
{
    /// <summary>
    /// Represents the public API of a REST HTTP client.
    /// </summary>
    public interface IRestHttpClient
    {
        /// <summary>
        /// Creates a customization that will be applied to all requests made by the
        /// <see cref="RestHttpClient{TError}"/>. The customization is removed when it is disposed.
        /// </summary>
        /// <param name="requestCustomizer">The action that customizes the request.</param>
        /// <returns>The created customization.</returns>
        RestRequestCustomization WithCustomization(Action<RestRequestBuilder> requestCustomizer);

        /// <summary>
        /// Removes a customization from the client.
        /// </summary>
        /// <param name="customization">The customization to remove.</param>
        internal void RemoveCustomization(RestRequestCustomization customization);

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
}
