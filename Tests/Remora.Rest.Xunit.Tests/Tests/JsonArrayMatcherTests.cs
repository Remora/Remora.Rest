//
//  SPDX-FileName: JsonArrayMatcherTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using Remora.Rest.Xunit.Json;
using Xunit;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonArrayMatcher"/> class.
/// </summary>
public class JsonArrayMatcherTests
{
    /// <summary>
    /// Tests the <see cref="JsonArrayMatcher.Matches"/> method.
    /// </summary>
    public class Matches
    {
        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcher.Matches"/> method returns true if the matcher contains
        /// no matching functions.
        /// </summary>
        [Fact]
        public void ReturnTrueIfNoMatchersAreConfigured()
        {
            var matcher = new JsonArrayMatcher(new List<Func<JsonElement.ArrayEnumerator, bool>>());
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcher.Matches"/> method returns true if the matcher contains
        /// a single matching function, which returns true.
        /// </summary>
        [Fact]
        public void ReturnTrueIfSingleMatcherMatches()
        {
            var matchers = new List<Func<JsonElement.ArrayEnumerator, bool>>
            {
                _ => true
            };

            var matcher = new JsonArrayMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcher.Matches"/> method returns false if the matcher contains
        /// a single matching function, which returns false.
        /// </summary>
        [Fact]
        public void ReturnFalseIfSingleMatcherDoesNotMatch()
        {
            var matchers = new List<Func<JsonElement.ArrayEnumerator, bool>>
            {
                _ => false
            };

            var matcher = new JsonArrayMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcher.Matches"/> method returns true if the matcher contains
        /// multiple matching functions, which all return true.
        /// </summary>
        [Fact]
        public void ReturnTrueIfAllMatchersMatch()
        {
            var matchers = new List<Func<JsonElement.ArrayEnumerator, bool>>
            {
                _ => true,
                _ => true,
                _ => true
            };

            var matcher = new JsonArrayMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonArrayMatcher.Matches"/> method returns false if the matcher contains
        /// multiple matching functions, at least one of which returns false.
        /// </summary>
        [Fact]
        public void ReturnFalseIfOneMatcherDoesNotMatch()
        {
            var matchers = new List<Func<JsonElement.ArrayEnumerator, bool>>
            {
                _ => true,
                _ => false,
                _ => true
            };

            var matcher = new JsonArrayMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }
    }
}
