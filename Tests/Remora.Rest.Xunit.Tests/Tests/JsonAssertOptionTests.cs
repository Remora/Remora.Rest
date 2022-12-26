//
//  SPDX-FileName: JsonAssertOptionTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
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
                .GetConstructor
                (
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    new[] { typeof(JsonElement), typeof(string) }
                )!
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
                .GetConstructor
                (
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    new[] { typeof(JsonElement), typeof(string) }
                )!
                .Invoke(new object[] { default(JsonElement), "property" });

            Assert.False(options.AllowMissingBy(property));
        }
    }
}
