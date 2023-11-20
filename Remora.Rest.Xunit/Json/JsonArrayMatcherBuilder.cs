//
//  SPDX-FileName: JsonArrayMatcherBuilder.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Json;

/// <summary>
/// Builds instances of the <see cref="JsonArrayMatcher"/> class.
/// </summary>
[PublicAPI]
public class JsonArrayMatcherBuilder
{
    private readonly List<Func<JsonElement.ArrayEnumerator, bool>> _matchers = new();

    /// <summary>
    /// Adds a requirement that the array is of an exact length.
    /// </summary>
    /// <param name="countPredicate">The function of the required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    public JsonArrayMatcherBuilder WithCount(Expression<Func<int, bool>> countPredicate)
    {
        _matchers.Add(array =>
        {
            array
                .Should().HaveCount(countPredicate);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the array is of an exact length.
    /// </summary>
    /// <param name="count">The required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonArrayMatcherBuilder WithCount(int count)
    {
        _matchers.Add(array =>
        {
            array
                .Should().HaveCount(count);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the array is greater than or equal to a specified length.
    /// </summary>
    /// <param name="count">The required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonArrayMatcherBuilder WithGreaterOrEqualToCount(int count)
    {
        _matchers.Add(array =>
        {
            array
                .Should().HaveCountGreaterOrEqualTo(count);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the array is less than or equal to a specified length.
    /// </summary>
    /// <param name="count">The required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonArrayMatcherBuilder WithLessThanOrEqualCount(int count)
    {
        _matchers.Add(array =>
        {
            array
                .Should().HaveCountLessThanOrEqualTo(count);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that any element matches the given element builder.
    /// </summary>
    /// <param name="elementMatcherBuilder">The element matcher.</param>
    /// <returns>The builder, with the added requirement.</returns>
    public JsonArrayMatcherBuilder WithAnyElement(Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null)
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        var matcher = elementMatcher.Build();
        _matchers.Add(array =>
        {
            array
                .Should().Contain(e => matcher.Matches(e));

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that a single element matches the given element builder.
    /// </summary>
    /// <param name="elementMatcherBuilder">The element matcher.</param>
    /// <returns>The builder, with the added requirement.</returns>
    public JsonArrayMatcherBuilder WithSingleElement(Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null)
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        var matcher = elementMatcher.Build();
        _matchers.Add(j =>
        {
            var matchingCount = j.Count(e =>
            {
                try
                {
                    return matcher.Matches(e);
                }
                catch (XunitException)
                {
                    return false;
                }
            });

            return matchingCount switch
            {
                > 1 => throw SingleException.MoreThanOne(matchingCount, null, "element", Array.Empty<int>()),
                < 1 => throw SingleException.Empty(null, "element"),
                _ => true
            };
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that an element at the given index matches the given element builder.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <param name="elementMatcherBuilder">The element matcher.</param>
    /// <returns>The builder, with the added requirement.</returns>
    public JsonArrayMatcherBuilder WithElement
    (
        int index,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        var matcher = elementMatcher.Build();

        _matchers.Add(array =>
        {
            index.Should().BeInRange(0, array.Count() - 1);

            var element = array.ElementAt(index);
            element.Should().Match<JsonElement>(e => matcher.Matches(e));

            return true;
        });

        return this;
    }

    /// <summary>
    /// Builds the array matcher.
    /// </summary>
    /// <returns>The built array matcher.</returns>
    public JsonArrayMatcher Build()
    {
        return new(_matchers);
    }
}
