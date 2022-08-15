//
//  JsonArrayMatcherBuilderTests.cs
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
using System.Text.Json;
using Remora.Rest.Xunit.Json;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonArrayMatcherBuilder"/> class, and its constituent matchers.
/// </summary>
public class JsonArrayMatcherBuilderTests
{
    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithCount(long)"/> and its overloads.
    /// </summary>
    public class WithCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(long)"/> method returns true for an array
        /// with a matching number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithMatchingConstantCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(long)"/> method asserts for an array
        /// with a mismatching number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithMismatchingConstantCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(2)
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(Func{long, bool})"/> method returns true
        /// for an array with a matching predicate.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithMatchingPredicateCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(c => c is > 0 and < 2)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(Func{long, bool})"/> method asserts
        /// for an array with a mismatching number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithMismatchingPredicateCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(c => c is > 1 and < 3)
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithAtLeastCount"/> method.
    /// </summary>
    public class WithAtLeastCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAtLeastCount"/> method returns true for an
        /// array with an exactly matching number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithExactCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAtLeastCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAtLeastCount"/> method returns true for an
        /// array with a greater number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithGreaterCount()
        {
            var json = "[ 1, 2 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAtLeastCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAtLeastCount"/> method asserts for an
        /// array with a lesser number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithLesserCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAtLeastCount(2)
                .Build();

            Assert.Throws<NotInRangeException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithNoMoreThanCount"/> method.
    /// </summary>
    public class WithNoMoreThanCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithNoMoreThanCount"/> method returns true for an
        /// array with an exactly matching number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithExactCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithNoMoreThanCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithNoMoreThanCount"/> method asserts for an
        /// array with a greater number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithGreaterCount()
        {
            var json = "[ 1, 2 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithNoMoreThanCount(1)
                .Build();

            Assert.Throws<InRangeException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithNoMoreThanCount"/> method returns true for an
        /// array with a lesser number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithLesserCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithNoMoreThanCount(2)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method.
    /// </summary>
    public class WithAnyElement
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method returns true if a single
        /// element in the array matches.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfSingleElementMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAnyElement(e => e.Is(1))
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method returns true if multiple
        /// elements in the array match.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfMultipleElementsMatch()
        {
            var json = "[ 1, 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAnyElement(e => e.Is(1))
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method asserts if no
        /// element in the array matches.
        /// </summary>
        [Fact]
        public void AssertsIfNoElementMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithAnyElement(e => e.Is(4))
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithSingleElement"/> method.
    /// </summary>
    public class WithSingleElement
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithSingleElement"/> method returns true if a
        /// single  element in the array matches.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfSingleElementMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithSingleElement(e => e.Is(1))
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method asserts if multiple
        /// elements in the array match.
        /// </summary>
        [Fact]
        public void AssertsIfMultipleElementsMatch()
        {
            var json = "[ 1, 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithSingleElement(e => e.Is(1))
                .Build();

            Assert.Throws<SingleException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithAnyElement"/> method asserts if no
        /// element in the array matches.
        /// </summary>
        [Fact]
        public void AssertsIfNoElementMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithSingleElement(e => e.Is(4))
                .Build();

            Assert.Throws<SingleException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithElement"/> and its overloads.
    /// </summary>
    public class WithElement
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithElement"/> method returns true if the element
        /// at the specified index matches.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfElementAtIndexMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithElement(0, e => e.Is(1))
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithElement"/> method asserts if the element
        /// at the specified index does not match.
        /// </summary>
        [Fact]
        public void AssertsIfElementAtIndexDoesNotMatch()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithElement(0, e => e.Is(4))
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithElement"/> method asserts if the element
        /// at the specified index does not match, but another element does.
        /// </summary>
        [Fact]
        public void AssertsIfElementAtOtherIndexMatches()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithElement(1, e => e.Is(1))
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithElement"/> method asserts if requested
        /// element index is out of range.
        /// </summary>
        [Fact]
        public void AssertsIfIndexIsOutOfRange()
        {
            var json = "[ 1, 2, 3 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithElement(3, e => e.Is(4))
                .Build();

            Assert.Throws<InRangeException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }
}
