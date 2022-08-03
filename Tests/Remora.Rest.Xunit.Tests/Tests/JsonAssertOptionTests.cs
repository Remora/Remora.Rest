//
//  JsonAssertOptionTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Xunit;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonAssertOptions"/> record.
/// </summary>
public static class JsonAssertOptionTests
{
    /// <summary>
    /// Tests construction of the object through various means.
    /// </summary>
    public static class Construction
    {
        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property is not null on a default-constructed
        /// object.
        /// </summary>
        [Fact]
        public static void AllowMissingIsNotNullOnDefaultConstructedObject()
        {
            var options = new JsonAssertOptions();
            Assert.NotNull(options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property is not null on a
        /// default-constructed object.
        /// </summary>
        [Fact]
        public static void AllowMissingByIsNotNullOnDefaultConstructedObject()
        {
            var options = new JsonAssertOptions();
            Assert.NotNull(options.AllowMissingBy);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowSkip"/> property is not null on a default-constructed
        /// object.
        /// </summary>
        [Fact]
        public static void AllowSkipIsNotNullOnDefaultConstructedObject()
        {
            var options = new JsonAssertOptions();
            Assert.NotNull(options.AllowSkip);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property is empty on a default-constructed
        /// object.
        /// </summary>
        [Fact]
        public static void AllowMissingIsEmptyOnDefaultConstructedObject()
        {
            var options = new JsonAssertOptions();
            Assert.Empty(options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property can be set through the constructor.
        /// </summary>
        [Fact]
        public static void CanSetAllowMissingViaConstructor()
        {
            var expected = new[] { "property" };

            var options = new JsonAssertOptions(AllowMissing: expected);
            Assert.Equal(expected, options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property can be set through the
        /// constructor.
        /// </summary>
        [Fact]
        public static void CanSetAllowMissingByViaConstructor()
        {
            bool Expected(JsonProperty discard) => true;

            var options = new JsonAssertOptions(AllowMissingBy: Expected);
            Assert.Equal(Expected, options.AllowMissingBy);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowSkip"/> property can be set through the
        /// constructor.
        /// </summary>
        [Fact]
        public static void CanSetAllowSkipViaConstructor()
        {
            bool Expected(JsonElement discard) => true;

            var options = new JsonAssertOptions(AllowSkip: Expected);
            Assert.Equal(Expected, options.AllowSkip);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property can be set through an object
        /// initializer.
        /// </summary>
        [Fact]
        public static void CanSetAllowMissingViaInitializer()
        {
            var expected = new[] { "property" };

            var options = new JsonAssertOptions
            {
                AllowMissing = expected
            };

            Assert.Equal(expected, options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property can be set through an object
        /// initializer.
        /// </summary>
        [Fact]
        public static void CanSetAllowMissingByViaInitializer()
        {
            bool Expected(JsonProperty discard) => true;

            var options = new JsonAssertOptions
            {
                AllowMissingBy = Expected
            };

            Assert.Equal(Expected, options.AllowMissingBy);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowSkip"/> property can be set through an object
        /// initializer.
        /// </summary>
        [Fact]
        public static void CanSetAllowSkipViaInitializer()
        {
            bool Expected(JsonElement discard) => true;

            var options = new JsonAssertOptions
            {
                AllowSkip = Expected
            };

            Assert.Equal(Expected, options.AllowSkip);
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonAssertOptions.Default"/> property.
    /// </summary>
    public static class Default
    {
        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property is not null on the default
        /// instance.
        /// </summary>
        [Fact]
        public static void AllowMissingIsNotNullOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            Assert.NotNull(options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property is not null on a
        /// default-constructed instance.
        /// </summary>
        [Fact]
        public static void AllowMissingByIsNotNullOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            Assert.NotNull(options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowSkip"/> property is not null on the default
        /// instance.
        /// </summary>
        [Fact]
        public static void AllowSkipIsNotNullOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            Assert.NotNull(options.AllowSkip);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissing"/> property is empty on the default
        /// instance.
        /// </summary>
        [Fact]
        public static void AllowMissingIsEmptyOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            Assert.Empty(options.AllowMissing);
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property is a function that returns true
        /// for underscore-prefixed values on the default instance.
        /// </summary>
        [Fact]
        public static void AllowMissingByReturnsTrueForUnderscorePrefixedOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            var property = (JsonProperty)typeof(JsonProperty)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(JsonElement), typeof(string) })!
                .Invoke(new object[] { default(JsonElement), "_property" });

            Assert.True(options.AllowMissingBy(property));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonAssertOptions.AllowMissingBy"/> property is a function that returns false
        /// for non-underscore-prefixed values on the default instance.
        /// </summary>
        [Fact]
        public static void AllowMissingByReturnsFalseForNonUnderscorePrefixedOnDefaultInstance()
        {
            var options = JsonAssertOptions.Default;
            var property = (JsonProperty)typeof(JsonProperty)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, new[] { typeof(JsonElement), typeof(string) })!
                .Invoke(new object[] { default(JsonElement), "property" });

            Assert.False(options.AllowMissingBy(property));
        }
    }
}
