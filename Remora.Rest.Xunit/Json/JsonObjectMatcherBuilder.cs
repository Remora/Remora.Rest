//
//  SPDX-FileName: JsonObjectMatcherBuilder.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using JetBrains.Annotations;

namespace Remora.Rest.Xunit.Json;

/// <summary>
/// Builds instances of the <see cref="JsonObjectMatcher"/> class.
/// </summary>
[PublicAPI]
public class JsonObjectMatcherBuilder
{
    private readonly List<Func<JsonElement, bool>> _matchers = new();

    /// <summary>
    /// Adds a requirement that a given property should exist.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the property value.</param>
    /// <returns>The builder, with the requirement.</returns>
    public JsonObjectMatcherBuilder WithProperty
    (
        string name,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        _matchers.Add
        (
            obj =>
            {
                obj.TryGetProperty(name, out var property)
                    .Should().NotBe(false, $"because a property named {name} should be present");

                if (elementMatcherBuilder is null)
                {
                    return true;
                }

                var matcherBuilder = new JsonElementMatcherBuilder();
                elementMatcherBuilder(matcherBuilder);

                return matcherBuilder.Build().Matches(property);
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that a given property should not exist.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <returns>The builder, with the requirement.</returns>
    public JsonObjectMatcherBuilder WithoutProperty
    (
        string name
    )
    {
        _matchers.Add
        (
            obj =>
            {
                obj.TryGetProperty(name, out _)
                    .Should().Be(false, $"because a property named {name} should not be present");

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Builds the object matcher.
    /// </summary>
    /// <returns>The built object matcher.</returns>
    public JsonObjectMatcher Build()
    {
        return new(_matchers);
    }
}
