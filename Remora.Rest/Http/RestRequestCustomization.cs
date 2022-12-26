//
//  SPDX-FileName: RestRequestCustomization.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
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
