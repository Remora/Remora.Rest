//
//  IRestCustomizable.cs
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

using System;

namespace Remora.Rest;

/// <summary>
/// Represents a type that can accept customizations for REST requests.
/// </summary>
public interface IRestCustomizable
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
    void RemoveCustomization(RestRequestCustomization customization);
}
