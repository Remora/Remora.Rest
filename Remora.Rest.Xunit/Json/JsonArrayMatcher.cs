//
//  SPDX-FileName: JsonArrayMatcher.cs
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
/// Matches against Json arrays.
/// </summary>
[PublicAPI]
public class JsonArrayMatcher
{
    private readonly IReadOnlyList<Func<JsonElement.ArrayEnumerator, bool>> _matchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonArrayMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public JsonArrayMatcher(IReadOnlyList<Func<JsonElement.ArrayEnumerator, bool>> matchers)
    {
        _matchers = matchers;
    }

    /// <summary>
    /// Determines whether the matcher fully matches the given array.
    /// </summary>
    /// <param name="arrayEnumerator">The array enumerator.</param>
    /// <returns>Whether the matcher matches the array.</returns>
    public bool Matches(JsonElement.ArrayEnumerator arrayEnumerator)
    {
        return _matchers.All(m => m(arrayEnumerator));
    }
}
