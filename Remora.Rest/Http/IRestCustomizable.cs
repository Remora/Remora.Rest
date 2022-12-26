//
//  SPDX-FileName: IRestCustomizable.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using JetBrains.Annotations;

namespace Remora.Rest;

/// <summary>
/// Represents a type that can accept customizations for REST requests.
/// </summary>
[PublicAPI]
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
