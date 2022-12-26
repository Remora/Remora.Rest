//
//  SPDX-FileName: RestRequestBuilderExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Extensions;

/// <summary>
/// Defines extensions to the <see cref="RestRequestBuilder"/> class.
/// </summary>
[PublicAPI]
public static class RestRequestBuilderExtensions
{
    /// <summary>
    /// Adds a header to the request, provided the value is defined.
    /// </summary>
    /// <param name="builder">The request builder.</param>
    /// <param name="name">The name of the header.</param>
    /// <param name="value">The value of the header.</param>
    /// <returns>The builder, potentially with the header.</returns>
    public static RestRequestBuilder AddHeader(this RestRequestBuilder builder, string name, Optional<string> value)
    {
        if (!value.HasValue)
        {
            return builder;
        }

        builder.AddHeader(name, value.Value);
        return builder;
    }
}
