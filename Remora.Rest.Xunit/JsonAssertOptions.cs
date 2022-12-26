//
//  SPDX-FileName: JsonAssertOptions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using JetBrains.Annotations;

namespace Remora.Rest.Xunit;

/// <summary>
/// Contains various options for the json assertions.
/// </summary>
[PublicAPI]
public record JsonAssertOptions
(
    IReadOnlyCollection<string>? AllowMissing = null,
    Func<JsonProperty, bool>? AllowMissingBy = null,
    Func<JsonElement, bool>? AllowSkip = null
)
{
    /// <summary>
    /// Gets a list of property names that are allowed to be missing from the serialized result.
    /// </summary>
    public IReadOnlyCollection<string> AllowMissing { get; init; } = AllowMissing ?? new List<string>();

    /// <summary>
    /// Gets a function that inspects a property and determines if it's allowed to be missing in the serialized
    /// result.
    /// </summary>
    public Func<JsonProperty, bool> AllowMissingBy { get; init; } = AllowMissingBy ?? (_ => false);

    /// <summary>
    /// Gets a function that inspects an element and determines if validation of it should be skipped.
    /// </summary>
    public Func<JsonElement, bool> AllowSkip { get; init; } = AllowSkip ?? (_ => false);

    /// <summary>
    /// Gets a default instance of the assertion options. This default option set allows underscore-prefixed fields
    /// to be missing.
    /// </summary>
    public static JsonAssertOptions Default { get; } = new()
    {
        AllowMissingBy = p => p.Name.StartsWith("_")
    };
}
