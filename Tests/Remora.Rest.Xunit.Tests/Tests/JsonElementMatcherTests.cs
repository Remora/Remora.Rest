//
//  SPDX-FileName: JsonElementMatcherTests.cs
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
/// Tests the <see cref="JsonElementMatcher"/> class.
/// </summary>
public class JsonElementMatcherTests
{
    /// <summary>
    /// Tests the <see cref="JsonElementMatcher.Matches"/> method.
    /// </summary>
    public class Matches
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcher.Matches"/> method returns true if the matcher contains
        /// no matching functions.
        /// </summary>
        [Fact]
        public void ReturnTrueIfNoMatchersAreConfigured()
        {
            var matcher = new JsonElementMatcher(new List<Func<JsonElement, bool>>());
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcher.Matches"/> method returns true if the matcher contains
        /// a single matching function, which returns true.
        /// </summary>
        [Fact]
        public void ReturnTrueIfSingleMatcherMatches()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => true
            };

            var matcher = new JsonElementMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcher.Matches"/> method returns false if the matcher contains
        /// a single matching function, which returns false.
        /// </summary>
        [Fact]
        public void ReturnFalseIfSingleMatcherDoesNotMatch()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => false
            };

            var matcher = new JsonElementMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcher.Matches"/> method returns true if the matcher contains
        /// multiple matching functions, which all return true.
        /// </summary>
        [Fact]
        public void ReturnTrueIfAllMatchersMatch()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => true,
                _ => true,
                _ => true
            };

            var matcher = new JsonElementMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcher.Matches"/> method returns false if the matcher contains
        /// multiple matching functions, at least one of which returns false.
        /// </summary>
        [Fact]
        public void ReturnFalseIfOneMatcherDoesNotMatch()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => true,
                _ => false,
                _ => true
            };

            var matcher = new JsonElementMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }
    }
}
