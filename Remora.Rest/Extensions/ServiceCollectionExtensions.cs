//
//  ServiceCollectionExtensions.cs
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

using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Remora.Rest.Extensions
{
    /// <summary>
    /// Defines extension methods for the <see cref="IServiceCollection"/> interface.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a REST-specialized HTTP client, allowing subsequent optional configuration of the backend client.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <typeparam name="TRestHttpClient">The HTTP client type to add.</typeparam>
        /// <returns>The client builder for the <typeparamref name="TRestHttpClient"/> type.</returns>
        public static IHttpClientBuilder AddRestHttpClient<TRestHttpClient>
        (
            this IServiceCollection services
        ) where TRestHttpClient : class, IRestHttpClient
        {
            services.TryAddTransient<TRestHttpClient>();
            return services.AddHttpClient<TRestHttpClient>();
        }
    }
}
