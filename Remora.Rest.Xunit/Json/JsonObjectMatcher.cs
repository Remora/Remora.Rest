//
//  SPDX-FileName: JsonObjectMatcher.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;

namespace Remora.Rest.Xunit.Json;

/// <summary>
/// Matches against Json objects.
/// </summary>
[PublicAPI]
public class JsonObjectMatcher
{
    private readonly IReadOnlyList<Func<JsonElement, bool>> _matchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonObjectMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public JsonObjectMatcher(IReadOnlyList<Func<JsonElement, bool>> matchers)
    {
        _matchers = matchers;
    }

    /// <summary>
    /// Determines whether the matcher fully matches the given object.
    /// </summary>
    /// <param name="jsonObject">The json object.</param>
    /// <returns>Whether the matcher matches the object.</returns>
    public bool Matches(JsonElement jsonObject)
    {
        return _matchers.All(m => m(jsonObject));
    }
}
