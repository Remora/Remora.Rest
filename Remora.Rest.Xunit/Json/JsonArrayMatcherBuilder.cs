//
//  JsonArrayMatcherBuilder.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using JetBrains.Annotations;
using Xunit;
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
    public JsonArrayMatcherBuilder WithCount(Func<long, bool> countPredicate)
    {
        _matchers.Add(j =>
        {
            var match = countPredicate(j.LongCount());
            if (!match)
            {
                throw new XunitException("Count predicate did not match.");
            }

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
    public JsonArrayMatcherBuilder WithCount(long count) => WithCount(c =>
    {
        Assert.Equal(count, c);
        return true;
    });

    /// <summary>
    /// Adds a requirement that the array is of an exact length.
    /// </summary>
    /// <param name="count">The required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonArrayMatcherBuilder WithAtLeastCount(long count) => WithCount(c =>
    {
        Assert.NotInRange(c, 0, count - 1);
        return true;
    });

    /// <summary>
    /// Adds a requirement that the array is of an exact length.
    /// </summary>
    /// <param name="count">The required length.</param>
    /// <returns>The builder, with the added requirement.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonArrayMatcherBuilder WithNoMoreThanCount(long count) => WithCount(c =>
    {
        Assert.InRange(c, 0, count);
        return true;
    });

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
        _matchers.Add(j =>
        {
            var anyMatch = j.Any(e =>
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

            if (!anyMatch)
            {
                throw new XunitException("No elements in the JSON array matched.");
            }

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
                > 1 => throw SingleException.MoreThanOne(),
                < 1 => throw SingleException.Empty(),
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

        _matchers.Add(j =>
        {
            Assert.InRange(index, 0, j.Count() - 1);
            return matcher.Matches(j.ElementAt(index));
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
