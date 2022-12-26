//
//  SPDX-FileName: HttpResultError.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Net;
using JetBrains.Annotations;
using Remora.Results;

namespace Remora.Rest.Results;

/// <summary>
/// Represents a HTTP error returned by an endpoint.
/// </summary>
[PublicAPI]
public record HttpResultError : ResultError
{
    /// <summary>
    /// Gets the status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpResultError"/> class.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="message">The human-readable error message.</param>
    public HttpResultError(HttpStatusCode statusCode, string? message = null)
        : base(message ?? $"An HTTP error occurred ({(ulong)statusCode} {statusCode})")
    {
        this.StatusCode = statusCode;
    }

    /// <summary>
    /// Creates an error from a status code.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <returns>The error.</returns>
    public static implicit operator HttpResultError(HttpStatusCode statusCode) => new(statusCode);
}
