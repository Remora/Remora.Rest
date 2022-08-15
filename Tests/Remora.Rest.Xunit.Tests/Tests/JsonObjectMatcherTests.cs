//
//  JsonObjectMatcherTests.cs
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
using System.Text.Json;
using Remora.Rest.Xunit.Json;
using Xunit;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonObjectMatcher"/> class.
/// </summary>
public class JsonObjectMatcherTests
{
    /// <summary>
    /// Tests the <see cref="JsonObjectMatcher.Matches"/> method.
    /// </summary>
    public class Matches
    {
        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcher.Matches"/> method returns true if the matcher contains
        /// no matching functions.
        /// </summary>
        [Fact]
        public void ReturnTrueIfNoMatchersAreConfigured()
        {
            var matcher = new JsonObjectMatcher(new List<Func<JsonElement, bool>>());
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcher.Matches"/> method returns true if the matcher contains
        /// a single matching function, which returns true.
        /// </summary>
        [Fact]
        public void ReturnTrueIfSingleMatcherMatches()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => true
            };

            var matcher = new JsonObjectMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcher.Matches"/> method returns false if the matcher contains
        /// a single matching function, which returns false.
        /// </summary>
        [Fact]
        public void ReturnFalseIfSingleMatcherDoesNotMatch()
        {
            var matchers = new List<Func<JsonElement, bool>>
            {
                _ => false
            };

            var matcher = new JsonObjectMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcher.Matches"/> method returns true if the matcher contains
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

            var matcher = new JsonObjectMatcher(matchers);
            Assert.True(matcher.Matches(default));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcher.Matches"/> method returns false if the matcher contains
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

            var matcher = new JsonObjectMatcher(matchers);
            Assert.False(matcher.Matches(default));
        }
    }
}
