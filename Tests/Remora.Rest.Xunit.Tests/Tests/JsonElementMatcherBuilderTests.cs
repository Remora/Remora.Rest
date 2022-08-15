//
//  JsonElementMatcherBuilderTests.cs
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
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonElementMatcherBuilder"/> class, and its constituent matchers.
/// </summary>
public class JsonElementMatcherBuilderTests
{
    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsObject"/> method.
    /// </summary>
    public class IsObject
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsObject"/> method returns true for an element
        /// that is a JSON object.
        /// </summary>
        [Fact]
        public void ReturnsTrueForObjectElement()
        {
            var json = "{ }";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsObject()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsObject"/> method asserts for an element
        /// that is not a JSON object.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("[ ]")]
        [InlineData("\"string\"")]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        public void AssertsForNonObjectElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsObject()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the method asserts when an inner matcher asserts.
        /// </summary>
        [Fact]
        public void AssertsWithInnerMatcher()
        {
            var json = "{ \"value\": 0 }";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsObject(o => o.WithProperty("missing_property"))
                .Build();

            Assert.Throws<ContainsException>(() => matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the method passes when an inner matcher passes.
        /// </summary>
        [Fact]
        public void PassesWithInnerMatcher()
        {
            var json = "{ \"value\": 0 }";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsObject(o => o.WithProperty("value"))
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsArray"/> method.
    /// </summary>
    public class IsArray
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsArray"/> method returns true for an element
        /// that is a JSON array.
        /// </summary>
        [Fact]
        public void ReturnsTrueForArrayElement()
        {
            var json = "[ ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsArray()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsArray"/> method asserts for an element
        /// that is not a JSON array.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("{ }")]
        [InlineData("\"string\"")]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        public void AssertsForNonArrayElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsArray()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the method asserts when an inner matcher asserts.
        /// </summary>
        [Fact]
        public void AssertsWithInnerMatcher()
        {
            var json = "[ ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsArray(a => a.WithCount(1))
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the method passes when an inner matcher passes.
        /// </summary>
        [Fact]
        public void PassesWithInnerMatcher()
        {
            var json = "[ ]";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsArray(a => a.WithCount(0))
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsValue"/> method.
    /// </summary>
    public class IsValue
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsValue"/> method returns true for an element
        /// that is of the specified value type.
        /// </summary>
        /// <param name="valueKind">The type of value the JSON should be.</param>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData(JsonValueKind.Object, "{ }")]
        [InlineData(JsonValueKind.Array, "[ ]")]
        [InlineData(JsonValueKind.String, "\"string\"")]
        [InlineData(JsonValueKind.Number, "0")]
        [InlineData(JsonValueKind.Number, "0.0")]
        [InlineData(JsonValueKind.True, "true")]
        [InlineData(JsonValueKind.False, "false")]
        [InlineData(JsonValueKind.Null, "null")]
        public void ReturnsTrueForCorrectElementType(JsonValueKind valueKind, string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsValue(valueKind)
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsValue"/> method asserts for an element
        /// that is not of the specified value type.
        /// </summary>
        /// <param name="valueKind">The type of value the JSON should be.</param>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData(JsonValueKind.Array, "{ }")]
        [InlineData(JsonValueKind.String, "[ ]")]
        [InlineData(JsonValueKind.Null, "\"string\"")]
        [InlineData(JsonValueKind.False, "0")]
        [InlineData(JsonValueKind.False, "0.0")]
        [InlineData(JsonValueKind.Object, "true")]
        [InlineData(JsonValueKind.Null, "false")]
        [InlineData(JsonValueKind.True, "null")]
        public void AssertsForIncorrectElementType(JsonValueKind valueKind, string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsValue(valueKind)
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsNull"/> method.
    /// </summary>
    public class IsNull
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsNull"/> method returns true for an element
        /// that is null.
        /// </summary>
        [Fact]
        public void ReturnsTrueForNullElement()
        {
            var json = "null";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsNull()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsNull"/> method asserts for an element
        /// that is not null.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("{ }")]
        [InlineData("[ ]")]
        [InlineData("\"string\"")]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("true")]
        [InlineData("false")]
        public void AssertsForNonNullElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsNull()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsBoolean"/> method.
    /// </summary>
    public class IsBoolean
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsBoolean"/> method returns true for an element
        /// that is a JSON boolean.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        public void ReturnsTrueForBooleanElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsBoolean()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsBoolean"/> method asserts for an element
        /// that is not a JSON boolean.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("{ }")]
        [InlineData("[ ]")]
        [InlineData("\"string\"")]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("null")]
        public void AssertsForNonBooleanElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsBoolean()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsNumber"/> method.
    /// </summary>
    public class IsNumber
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsNumber"/> method returns true for an element
        /// that is a JSON number.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("0")]
        [InlineData("0.0")]
        public void ReturnsTrueForNumberElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsNumber()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsNumber"/> method asserts for an element
        /// that is not a JSON number.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("{ }")]
        [InlineData("[ ]")]
        [InlineData("\"string\"")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        public void AssertsForNonNumberElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsNumber()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.IsString"/> method.
    /// </summary>
    public class IsString
    {
        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsString"/> method returns true for an element
        /// that is a JSON string.
        /// </summary>
        [Fact]
        public void ReturnsTrueForStringElement()
        {
            var json = "\"string\"";
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsString()
                .Build();

            Assert.True(matcher.Matches(document.RootElement));
        }

        /// <summary>
        /// Tests whether the <see cref="JsonElementMatcherBuilder.IsString"/> method asserts for an element
        /// that is not a JSON string.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("{ }")]
        [InlineData("[ ]")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("null")]
        public void AssertsForNonStringElement(string json)
        {
            var document = JsonDocument.Parse(json);

            var matcher = new JsonElementMatcherBuilder()
                .IsString()
                .Build();

            Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
        }
    }

    /// <summary>
    /// Tests the <see cref="JsonElementMatcherBuilder.Is(sbyte)"/> method and its overloads.
    /// </summary>
    public class Is
    {
        /// <summary>
        /// Tests the <see cref="sbyte"/> overload.
        /// </summary>
        public class SByte
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(sbyte)"/> method returns true for an
            /// element that is an <see cref="sbyte"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForSByteElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((sbyte)0)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(sbyte)"/> method asserts for an
            /// element that is an <see cref="sbyte"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForSByteElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((sbyte)1)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(sbyte)"/> method asserts for an
            /// element that is not an <see cref="sbyte"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonSByteElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((sbyte)0)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(sbyte)"/> method asserts for an
            /// element that is not representable as a <see cref="sbyte"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("128")]
            [InlineData("-129")]
            public void AssertsForSByteUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((sbyte)0)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="short"/> overload.
        /// </summary>
        public class Int16
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(short)"/> method returns true for an
            /// element that is an <see cref="short"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForInt16ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((short)0)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(short)"/> method asserts for an
            /// element that is an <see cref="short"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForInt16ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((short)1)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(short)"/> method asserts for an
            /// element that is not an <see cref="short"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonInt16Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((short)0)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(short)"/> method asserts for an
            /// element that is not representable as a <see cref="short"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("32768")]
            [InlineData("-32769")]
            public void AssertsForInt16UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((short)0)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="int"/> overload.
        /// </summary>
        public class Int32
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(int)"/> method returns true for an
            /// element that is an <see cref="int"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForInt32ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(int)"/> method asserts for an
            /// element that is an <see cref="int"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForInt32ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(int)"/> method asserts for an
            /// element that is not an <see cref="int"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonInt32Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(int)"/> method asserts for an
            /// element that is not representable as a <see cref="int"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("2147483648")]
            [InlineData("-2147483649")]
            public void AssertsForInt32UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="long"/> overload.
        /// </summary>
        public class Int64
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(long)"/> method returns true for an
            /// element that is an <see cref="long"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForInt64ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0L)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(long)"/> method asserts for an
            /// element that is an <see cref="long"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForInt64ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1L)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(long)"/> method asserts for an
            /// element that is not an <see cref="long"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonInt64Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0L)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(long)"/> method asserts for an
            /// element that is not representable as a <see cref="long"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("9223372036854775808")]
            [InlineData("-9223372036854775809")]
            public void AssertsForInt64UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0L)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="byte"/> overload.
        /// </summary>
        public class Byte
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(byte)"/> method returns true for an
            /// element that is an <see cref="byte"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForByteElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((byte)0)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(byte)"/> method asserts for an
            /// element that is an <see cref="byte"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForByteElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((byte)1)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(byte)"/> method asserts for an
            /// element that is not an <see cref="byte"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonByteElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((byte)0)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(byte)"/> method asserts for an
            /// element that is not representable as a <see cref="byte"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("256")]
            [InlineData("-1")]
            public void AssertsForByteUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((byte)0)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="ushort"/> overload.
        /// </summary>
        public class UInt16
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ushort)"/> method returns true for an
            /// element that is an <see cref="ushort"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForUInt16ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((ushort)0)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ushort)"/> method asserts for an
            /// element that is an <see cref="ushort"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForUInt16ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((ushort)1)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ushort)"/> method asserts for an
            /// element that is not an <see cref="ushort"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonUInt16Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((ushort)0)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ushort)"/> method asserts for an
            /// element that is not representable as a <see cref="ushort"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("65536")]
            [InlineData("-1")]
            public void AssertsForUInt16UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is((ushort)0)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="uint"/> overload.
        /// </summary>
        public class UInt32
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(uint)"/> method returns true for an
            /// element that is an <see cref="uint"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForUInt32ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0U)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(uint)"/> method asserts for an
            /// element that is an <see cref="uint"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForUInt32ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1U)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(uint)"/> method asserts for an
            /// element that is not an <see cref="uint"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonUInt32Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0U)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(uint)"/> method asserts for an
            /// element that is not representable as a <see cref="uint"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("4294967296")]
            [InlineData("-1")]
            public void AssertsForUInt32UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0U)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="ulong"/> overload.
        /// </summary>
        public class UInt64
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ulong)"/> method returns true for an
            /// element that is an <see cref="ulong"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForUInt64ElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0UL)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ulong)"/> method asserts for an
            /// element that is an <see cref="ulong"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForUInt64ElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1UL)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ulong)"/> method asserts for an
            /// element that is not an <see cref="ulong"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonUInt64Element(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0UL)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(ulong)"/> method asserts for an
            /// element that is not representable as a <see cref="ulong"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("0.0")]
            [InlineData("18446744073709551616")]
            [InlineData("-1")]
            public void AssertsForUInt64UnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0UL)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="string"/> overload.
        /// </summary>
        public class String
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(string)"/> method returns true for an
            /// element that is an <see cref="string"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForStringElementWithEqualValue()
            {
                var json = "\"string\"";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is("string")
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(string)"/> method asserts for an
            /// element that is an <see cref="string"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForStringElementWithUnequalValue()
            {
                var json = "\"string\"";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is("not string")
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(string)"/> method asserts for an
            /// element that is not an <see cref="string"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("0")]
            [InlineData("0.0")]
            [InlineData("null")]
            public void AssertsForNonStringElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is("string")
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="bool"/> overload.
        /// </summary>
        public class Boolean
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(bool)"/> method returns true for an
            /// element that is an <see cref="bool"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForBooleanElementWithEqualValue()
            {
                var json = "true";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(true)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(bool)"/> method asserts for an
            /// element that is an <see cref="bool"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForBooleanElementWithUnequalValue()
            {
                var json = "true";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(false)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(bool)"/> method asserts for an
            /// element that is not an <see cref="bool"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("0")]
            [InlineData("0.0")]
            [InlineData("null")]
            public void AssertsForNonBoolElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(true)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="decimal"/> overload.
        /// </summary>
        public class Decimal
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(decimal)"/> method returns true for an
            /// element that is an <see cref="decimal"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForDecimalElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0M)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(decimal)"/> method asserts for an
            /// element that is an <see cref="decimal"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForDecimalElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1M)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(decimal)"/> method asserts for an
            /// element that is not an <see cref="decimal"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonDecimalElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1M)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(decimal)"/> method asserts for an
            /// element that is not representable as a <see cref="decimal"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("8.5070592e+37")]
            public void AssertsForDecimalUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1M)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="float"/> overload.
        /// </summary>
        public class Float
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(float)"/> method returns true for an
            /// element that is an <see cref="float"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForFloatElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0f)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(float)"/> method asserts for an
            /// element that is an <see cref="float"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForFloatElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1f)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(float)"/> method asserts for an
            /// element that is not an <see cref="float"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonFloatElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1f)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="double"/> overload.
        /// </summary>
        public class Double
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(double)"/> method returns true for an
            /// element that is an <see cref="double"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForDoubleElementWithEqualValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(0d)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(double)"/> method asserts for an
            /// element that is an <see cref="double"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForDoubleElementWithUnequalValue()
            {
                var json = "0";
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1d)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(double)"/> method asserts for an
            /// element that is not an <see cref="double"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            public void AssertsForNonDoubleElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(1d)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="System.Guid"/> overload.
        /// </summary>
        public class Guid
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.Guid)"/> method returns true for an
            /// element that is an <see cref="System.Guid"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForGuidElementWithEqualValue()
            {
                var guid = System.Guid.NewGuid();
                var json = $"\"{guid.ToString()}\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(guid)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.Guid)"/> method asserts for an
            /// element that is an <see cref="System.Guid"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForGuidElementWithUnequalValue()
            {
                var guid = System.Guid.NewGuid();
                var json = $"\"{guid.ToString()}\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.Guid.NewGuid())
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.Guid)"/> method asserts for an
            /// element that is not an <see cref="System.Guid"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            [InlineData("0")]
            [InlineData("0.0")]
            public void AssertsForNonGuidElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.Guid.NewGuid())
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.Guid)"/> method asserts for an
            /// element that is not representable as an <see cref="System.Guid"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("\"somestring\"")]
            public void AssertsForGuidUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.Guid.NewGuid())
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="System.DateTime"/> overload.
        /// </summary>
        public class DateTime
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTime)"/> method returns true for an
            /// element that is an <see cref="System.DateTime"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForDateTimeElementWithEqualValue()
            {
                var dateTime = System.DateTime.Parse("2000-01-01T00:00:00");
                var json = "\"2000-01-01T00:00:00\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(dateTime)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTime)"/> method asserts for an
            /// element that is an <see cref="System.DateTime"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForDateTimeElementWithUnequalValue()
            {
                var dateTime = System.DateTime.Parse("2000-01-01T00:00:00");
                var json = "\"2000-01-01T00:00:00\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(dateTime + TimeSpan.FromDays(1))
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTime)"/> method asserts for an
            /// element that is not an <see cref="System.DateTime"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            [InlineData("0")]
            [InlineData("0.0")]
            public void AssertsForNonDateTimeElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.DateTime.Today)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTime)"/> method asserts for an
            /// element that is not representable as an <see cref="System.DateTime"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("\"somestring\"")]
            public void AssertsForDateTimeUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.DateTime.UtcNow)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="System.DateTimeOffset"/> overload.
        /// </summary>
        public class DateTimeOffset
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTimeOffset)"/> method returns true for an
            /// element that is an <see cref="System.DateTimeOffset"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForDateTimeOffsetElementWithEqualValue()
            {
                var dateTimeOffset = System.DateTimeOffset.Parse("2000-01-01T00:00:00Z");
                var json = "\"2000-01-01T00:00:00Z\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(dateTimeOffset)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTimeOffset)"/> method asserts for an
            /// element that is an <see cref="System.DateTimeOffset"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForDateTimeOffsetElementWithUnequalValue()
            {
                var dateTimeOffset = System.DateTimeOffset.Parse("2000-01-01T00:00:00Z");
                var json = "\"2000-01-01T00:00:00Z\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(dateTimeOffset + TimeSpan.FromDays(1))
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTimeOffset)"/> method asserts for an
            /// element that is not an <see cref="System.DateTimeOffset"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            [InlineData("0")]
            [InlineData("0.0")]
            public void AssertsForNonDateTimeOffsetElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.DateTimeOffset.UtcNow)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(System.DateTimeOffset)"/> method asserts for an
            /// element that is not representable as an <see cref="System.DateTimeOffset"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("\"somestring\"")]
            public void AssertsForDateTimeOffsetUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(System.DateTimeOffset.UtcNow)
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }

        /// <summary>
        /// Tests the <see cref="IEnumerable{T}"/> overload.
        /// </summary>
        public class ByteEnumeration
        {
            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(IEnumerable{byte})"/> method returns true for an
            /// element that is an <see cref="IEnumerable{T}"/> value, equal to the value under test.
            /// </summary>
            [Fact]
            public void ReturnsTrueForByteEnumerationElementWithEqualValue()
            {
                var bytes = new byte[]
                {
                    0x6e,
                    0x6f,
                    0x73,
                    0x79,
                    0x20,
                    0x66,
                    0x75,
                    0x63,
                    0x6b,
                    0x65,
                    0x72,
                    0x20,
                    0x61,
                    0x69,
                    0x6e,
                    0x74,
                    0x20,
                    0x79,
                    0x61
                };

                var json = "\"bm9zeSBmdWNrZXIgYWludCB5YQ==\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(bytes)
                    .Build();

                Assert.True(matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(IEnumerable{byte})"/> method asserts for an
            /// element that is an <see cref="IEnumerable{T}"/> value, not equal to the value under test.
            /// </summary>
            [Fact]
            public void AssertsForByteEnumerationElementWithUnequalValue()
            {
                var bytes = Array.Empty<byte>();
                var json = "\"bm9zeSBmdWNrZXIgYWludCB5YQ==\"";

                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(bytes)
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(IEnumerable{byte})"/> method asserts for an
            /// element that is not an <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("{ }")]
            [InlineData("[ ]")]
            [InlineData("true")]
            [InlineData("false")]
            [InlineData("null")]
            [InlineData("0")]
            [InlineData("0.0")]
            public void AssertsForNonByteEnumerationElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(Array.Empty<byte>())
                    .Build();

                Assert.Throws<EqualException>(() => matcher.Matches(document.RootElement));
            }

            /// <summary>
            /// Tests whether the <see cref="JsonElementMatcherBuilder.Is(IEnumerable{byte})"/> method asserts for an
            /// element that is not representable as an <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <param name="json">The JSON to test.</param>
            [Theory]
            [InlineData("\"somestring\"")]
            public void AssertsForByteEnumerationUnrepresentableElement(string json)
            {
                var document = JsonDocument.Parse(json);

                var matcher = new JsonElementMatcherBuilder()
                    .Is(Array.Empty<byte>())
                    .Build();

                Assert.Throws<IsTypeException>(() => matcher.Matches(document.RootElement));
            }
        }
    }
}
