//
//  SPDX-FileName: RestResultError.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using JetBrains.Annotations;
using Remora.Results;

#pragma warning disable CS1591

namespace Remora.Rest.Results;

/// <summary>
/// Represents an error returned by the REST API.
/// </summary>
/// <typeparam name="TError">A type which represents an error payload returned by the API.</typeparam>
[PublicAPI]
public record RestResultError<TError>(TError Error)
    : ResultError($"REST request failed: {Error}");
