//
//  JsonObjectMatcherBuilderTests.cs
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

using System.Text.Json;
using Remora.Rest.Xunit.Json;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonObjectMatcherBuilder"/> class, and its constituent matchers.
/// </summary>
public class JsonObjectMatcherBuilderTests
{
    /// <summary>
    /// Tests the <see cref="JsonObjectMatcherBuilder.WithProperty"/> method.
    /// </summary>
    public class WithProperty
    {
        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithProperty"/> method returns true if the
        /// required property is present.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfPropertyIsPresent()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithProperty("value")
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithProperty"/> method returns true if the
        /// required property is present, and the property value matches.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfPropertyIsPresentAndMatches()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithProperty("value", p => p.Is(0))
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithProperty"/> method asserts if the
        /// required property is not present.
        /// </summary>
        [Fact]
        public void AssertsIfPropertyIsNotPresent()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithProperty("missing_property")
                .Build();

            Assert.Throws<ContainsException>(() => matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithProperty"/> method asserts if the
        /// required property is present, but the property value does not match.
        /// </summary>
        [Fact]
        public void AssertsIfPropertyIsPresentButDoesNotMatch()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithProperty("value", p => p.Is(1))
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonObjectMatcherBuilder.WithoutProperty"/> method.
    /// </summary>
    public class WithoutProperty
    {
        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithoutProperty"/> method returns true if the
        /// forbidden property is not present.
        /// </summary>
        [Fact]
        public void ReturnsTrueIfPropertyIsNotPresent()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithoutProperty("forbidden_property")
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonObjectMatcherBuilder.WithoutProperty"/> method asserts if the
        /// forbidden property is present.
        /// </summary>
        [Fact]
        public void AssertsIfPropertyIsPresent()
        {
            var json = "{ \"value\": 0 }";

            var document = JsonDocument.Parse(json);

            var matcher = new JsonObjectMatcherBuilder()
                .WithoutProperty("value")
                .Build();

            Assert.Throws<DoesNotContainException>(() => matcher.Matches(document.RootElement));
        }
    }
}
