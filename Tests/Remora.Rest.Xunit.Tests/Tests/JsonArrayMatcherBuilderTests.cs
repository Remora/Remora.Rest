//
//  SPDX-FileName: JsonArrayMatcherBuilderTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Linq.Expressions;
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
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithCount(Expression{Func{int, bool}})"/> and its overloads.
    /// </summary>
    public class WithCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(int)"/> method
        /// returns true for an array with a matching number of elements.
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
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(int)"/> method
        /// asserts for an array with a mismatching number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithMismatchingConstantCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(2)
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(Expression{Func{int, bool}})"/> method
        /// returns true for an array with a matching predicate.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithMatchingPredicateCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(c => c > 0 && c < 2)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithCount(Expression{Func{int, bool}})"/> method
        /// asserts for an array with a mismatching number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithMismatchingPredicateCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithCount(c => c > 1 && c < 3)
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithGreaterOrEqualToCount"/> method.
    /// </summary>
    public class WithGreaterOrEqualToCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithGreaterOrEqualToCount"/> method returns true
        /// for an array with an exactly matching number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithExactCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithGreaterOrEqualToCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithGreaterOrEqualToCount"/> method returns true
        /// for an array with a greater number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithGreaterCount()
        {
            var json = "[ 1, 2 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithGreaterOrEqualToCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithGreaterOrEqualToCount"/> method asserts for an
        /// array with a lesser number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithLesserCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithGreaterOrEqualToCount(2)
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonArrayMatcherBuilder.WithLessThanOrEqualCount"/> method.
    /// </summary>
    public class WithLessThanOrEqualCount
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithLessThanOrEqualCount"/> method returns true for an
        /// array with an exactly matching number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithExactCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithLessThanOrEqualCount(1)
                .Build();

            Assert.True(matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithLessThanOrEqualCount"/> method asserts for an
        /// array with a greater number of elements.
        /// </summary>
        [Fact]
        public void AssertsForArrayWithGreaterCount()
        {
            var json = "[ 1, 2 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithLessThanOrEqualCount(1)
                .Build();

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcherBuilder.WithLessThanOrEqualCount"/> method returns true for an
        /// array with a lesser number of elements.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayWithLesserCount()
        {
            var json = "[ 1 ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonArrayMatcherBuilder()
                .WithLessThanOrEqualCount(2)
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

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
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

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
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

            Assert.Throws<XunitException>(() => matcher.Matches(document.RootElement.EnumerateArray()));
        }
    }
}
