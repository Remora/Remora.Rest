//
//  RestRequestCustomization.cs
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
using JetBrains.Annotations;

namespace Remora.Rest;

/// <summary>
/// Represents a set of customizations that will be applied to a REST request made to the API.
/// </summary>
[PublicAPI]
public class RestRequestCustomization : IDisposable
{
    private readonly IRestCustomizable _customizable;

    /// <summary>
    /// Gets the request customizer.
    /// </summary>
    internal Action<RestRequestBuilder> RequestCustomizer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RestRequestCustomization"/> class.
    /// </summary>
    /// <param name="customizable">The instance that the customization originated from.</param>
    /// <param name="requestCustomizer">The request customizer.</param>
    internal RestRequestCustomization
    (
        IRestCustomizable customizable,
        Action<RestRequestBuilder> requestCustomizer
    )
    {
        _customizable = customizable;
        this.RequestCustomizer = requestCustomizer;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _customizable.RemoveCustomization(this);
    }
}
