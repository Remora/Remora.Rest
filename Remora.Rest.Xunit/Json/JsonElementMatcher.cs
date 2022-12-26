//
//  SPDX-FileName: JsonElementMatcher.cs
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
/// Matches against Json elements.
/// </summary>
[PublicAPI]
public class JsonElementMatcher
{
    private readonly IReadOnlyList<Func<JsonElement, bool>> _matchers;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonElementMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public JsonElementMatcher(IReadOnlyList<Func<JsonElement, bool>> matchers)
    {
        _matchers = matchers;
    }

    /// <summary>
    /// Determines whether the matcher fully matches the given element.
    /// </summary>
    /// <param name="elementEnumerator">The element enumerator.</param>
    /// <returns>Whether the matcher matches the element.</returns>
    public bool Matches(JsonElement elementEnumerator)
    {
        return _matchers.All(m => m(elementEnumerator));
    }
}
