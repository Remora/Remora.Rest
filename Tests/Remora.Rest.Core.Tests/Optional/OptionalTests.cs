//
//  SPDX-FileName: OptionalTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using Xunit;

// ReSharper disable SA1600
#pragma warning disable 1591, SA1600

namespace Remora.Rest.Core.Tests;

/// <summary>
/// Tests the <see cref="Optional{TValue}"/> struct.
/// </summary>
public static class OptionalTests
{
    /// <summary>
    /// Tests the <see cref="Optional{TValue}.HasValue"/> property.
    /// </summary>
    public static class HasValue
    {
        [Fact]
        public static void ReturnsTrueWhenOptionalValueTypeContainsValue()
        {
            var optional = new Optional<int>(0);

            Assert.True(optional.HasValue);
        }

        [Fact]
        public static void ReturnsFalseWhenOptionalValueTypeDoesNotContainValue()
        {
            Optional<int> optional = default;

            Assert.False(optional.HasValue);
        }

        [Fact]
        public static void ReturnsTrueWhenOptionalNullableValueTypeContainsValue()
        {
            var optional = new Optional<int?>(0);

            Assert.True(optional.HasValue);
        }

        [Fact]
        public static void ReturnsTrueWhenOptionalNullableValueTypeContainsNull()
        {
            var optional = new Optional<int?>(null);

            Assert.True(optional.HasValue);
        }

        [Fact]
        public static void ReturnsFalseWhenOptionalNullableValueTypeDoesNotContainValue()
        {
            Optional<int?> optional = default;

            Assert.False(optional.HasValue);
        }

        [Fact]
        public static void ReturnsTrueWhenOptionalReferenceTypeContainsValue()
        {
            var optional = new Optional<string>("Hello world!");

            Assert.True(optional.HasValue);
        }

        [Fact]
        public static void ReturnsFalseWhenOptionalReferenceTypeDoesNotContainValue()
        {
            Optional<string> optional = default;

            Assert.False(optional.HasValue);
        }

        [Fact]
        public static void ReturnsTrueWhenOptionalNullableReferenceTypeContainsValue()
        {
            var optional = new Optional<string?>(null);

            Assert.True(optional.HasValue);
        }

        [Fact]
        public static void ReturnsFalseWhenOptionalNullableReferenceTypeDoesNotContainValue()
        {
            Optional<string?> optional = default;

            Assert.False(optional.HasValue);
        }
    }

    /// <summary>
    /// Tests the <see cref="Optional{TValue}.Value"/> property.
    /// </summary>
    public static class Value
    {
        [Fact]
        public static void ReturnsCorrectValueIfValueTypeOptionalContainsValue()
        {
            var optional = new Optional<int>(64);

            Assert.Equal(64, optional.Value);
        }

        [Fact]
        public static void ThrowsIfValueTypeOptionalDoesNotContainValue()
        {
            Optional<int> optional = default;

            Assert.Throws<InvalidOperationException>(() => optional.Value);
        }

        [Fact]
        public static void ReturnsCorrectValueIfNullableValueTypeOptionalContainsValue()
        {
            var optional = new Optional<int?>(64);

            Assert.Equal(64, optional.Value);
        }

        [Fact]
        public static void ReturnsCorrectValueIfNullableValueTypeOptionalContainsNullValue()
        {
            var optional = new Optional<int?>(null);

            Assert.Null(optional.Value);
        }

        [Fact]
        public static void ThrowsIfNullableValueTypeOptionalDoesNotContainValue()
        {
            Optional<int?> optional = default;

            Assert.Throws<InvalidOperationException>(() => optional.Value);
        }

        [Fact]
        public static void ReturnsCorrectValueIfReferenceTypeOptionalContainsValue()
        {
            var optional = new Optional<string>("Hello world!");

            Assert.Equal("Hello world!", optional.Value);
        }

        [Fact]
        public static void ThrowsIfReferenceTypeOptionalDoesNotContainValue()
        {
            Optional<string> optional = default;

            Assert.Throws<InvalidOperationException>(() => optional.Value);
        }

        [Fact]
        public static void ReturnsCorrectValueIfNullableReferenceTypeOptionalContainsValue()
        {
            var optional = new Optional<string?>("Hello world!");

            Assert.Equal("Hello world!", optional.Value);
        }

        [Fact]
        public static void ReturnsCorrectValueIfNullableReferenceTypeOptionalContainsNullValue()
        {
            var optional = new Optional<string?>(null);

            Assert.Null(optional.Value);
        }

        [Fact]
        public static void ThrowsIfNullableReferenceTypeOptionalDoesNotContainValue()
        {
            Optional<string?> optional = default;

            Assert.Throws<InvalidOperationException>(() => optional.Value);
        }
    }

    /// <summary>
    /// Tests the <see cref="Optional{TValue}.IsDefined()"/> method and its overloads.
    /// </summary>
    public static class IsDefined
    {
        [Fact]
        public static void ReturnsFalseIfNullableOptionalHasNoValue()
        {
            Optional<int?> noValue = default;
            Assert.False(noValue.IsDefined());

            Assert.False(noValue.IsDefined(out var value));
            Assert.Null(value);
        }

        [Fact]
        public static void ReturnsFalseIfNullableOptionalHasNullValue()
        {
            Optional<int?> noValue = null;
            Assert.False(noValue.IsDefined());

            Assert.False(noValue.IsDefined(out var value));
            Assert.Null(value);
        }

        [Fact]
        public static void ReturnsTrueIfNullableOptionalHasNonNullValue()
        {
            Optional<int?> noValue = 1;
            Assert.True(noValue.IsDefined());

            Assert.True(noValue.IsDefined(out var value));
            Assert.NotNull(value);
        }

        [Fact]
        public static void ReturnsFalseIfOptionalHasNoValue()
        {
            Optional<int> noValue = default;
            Assert.False(noValue.IsDefined());
            Assert.False(noValue.IsDefined(out _));
        }

        [Fact]
        public static void ReturnsTrueIfOptionalHasNonNullValue()
        {
            Optional<int> noValue = 1;
            Assert.True(noValue.IsDefined());

            Assert.True(noValue.IsDefined(out var value));
            Assert.Equal(1, value);
        }
    }

    /// <summary>
    /// Tests the <see cref="Optional{TValue}.ToString"/> method.
    /// </summary>
    public new class ToString
    {
        [Fact]
        public static void ResultContainsValueIfValueTypeOptionalContainsValue()
        {
            var optional = new Optional<int>(64);

            Assert.Contains(64.ToString(), optional.ToString());
        }

        [Fact]
        public static void ResultIsEmptyIfValueTypeOptionalDoesNotContainValue()
        {
            var optional = default(Optional<int>);

            Assert.Equal("Empty", optional.ToString());
        }

        [Fact]
        public static void ResultContainsValueIfNullableValueTypeOptionalContainsValue()
        {
            var optional = new Optional<int?>(64);

            Assert.Contains(64.ToString(), optional.ToString());
        }

        [Fact]
        public static void ResultContainsNullIfNullableValueTypeOptionalContainsNullValue()
        {
            var optional = new Optional<int?>(null);

            Assert.Contains("null", optional.ToString());
        }

        [Fact]
        public static void ResultIsEmptyIfNullableValueTypeOptionalDoesNotContainValue()
        {
            var optional = default(Optional<int?>);

            Assert.Equal("Empty", optional.ToString());
        }

        [Fact]
        public static void ResultContainsValueIfReferenceTypeOptionalContainsValue()
        {
            var optional = new Optional<string>("Hello world!");

            Assert.Contains("Hello world!", optional.ToString());
        }

        [Fact]
        public static void ResultIsEmptyIfReferenceTypeOptionalDoesNotContainValue()
        {
            var optional = default(Optional<string>);

            Assert.Equal("Empty", optional.ToString());
        }

        [Fact]
        public static void ResultContainsValueIfNullableReferenceTypeOptionalContainsValue()
        {
            var optional = new Optional<string?>("Hello world!");

            Assert.Contains("Hello world!", optional.ToString());
        }

        [Fact]
        public static void ResultContainsNullIfNullableReferenceTypeOptionalContainsNullValue()
        {
            var optional = new Optional<string?>(null);

            Assert.Contains("null", optional.ToString());
        }

        [Fact]
        public static void ResultIsEmptyIfNullableReferenceTypeOptionalDoesNotContainValue()
        {
            var optional = default(Optional<string?>);

            Assert.Equal("Empty", optional.ToString());
        }
    }

    /// <summary>
    /// Tests the implicit operator.
    /// </summary>
    public static class ImplicitOperator
    {
        [Fact]
        public static void CanCreateValueTypeOptionalImplicitly()
        {
            Optional<int> optional = 64;

            Assert.True(optional.HasValue);
            Assert.Equal(64, optional.Value);
        }

        [Fact]
        public static void CanCreateNullableValueTypeOptionalImplicitly()
        {
            Optional<int?> optional = 64;

            Assert.True(optional.HasValue);
            Assert.Equal(64, optional.Value);

            Optional<int?> nullOptional = null;

            Assert.True(nullOptional.HasValue);
            Assert.Null(nullOptional.Value);
        }

        [Fact]
        public static void CanCreateReferenceTypeOptionalImplicitly()
        {
            Optional<string> optional = "Hello world!";

            Assert.True(optional.HasValue);
            Assert.Equal("Hello world!", optional.Value);
        }

        [Fact]
        public static void CanCreateNullableReferenceTypeOptionalImplicitly()
        {
            Optional<string?> optional = "Hello world!";

            Assert.True(optional.HasValue);
            Assert.Equal("Hello world!", optional.Value);

            Optional<string?> nullOptional = null;

            Assert.True(nullOptional.HasValue);
            Assert.Null(nullOptional.Value);
        }
    }

    /// <summary>
    /// Tests the equality operator.
    /// </summary>
    public static class EqualityOperator
    {
        [Fact]
        public static void ReturnsTrueForDefaultValues()
        {
            var a = default(Optional<int>);
            var b = default(Optional<int>);

            Assert.True(a == b);
            Assert.True(b == a);
        }

        [Fact]
        public static void ReturnsTrueForSameContainedValues()
        {
            Optional<int> a = 1;
            Optional<int> b = 1;

            Assert.True(a == b);
            Assert.True(b == a);
        }

        [Fact]
        public static void ReturnsFalseForDefaultValueComparedToContainedValue()
        {
            var a = default(Optional<int>);
            Optional<int> b = 1;

            Assert.False(a == b);
            Assert.False(b == a);
        }

        [Fact]
        public static void ReturnsFalseForDifferentContainedValues()
        {
            Optional<int> a = 1;
            Optional<int> b = 2;

            Assert.False(a == b);
            Assert.False(b == a);
        }
    }

    /// <summary>
    /// Tests the inequality operator.
    /// </summary>
    public static class InequalityOperator
    {
        [Fact]
        public static void ReturnsFalseForDefaultValues()
        {
            var a = default(Optional<int>);
            var b = default(Optional<int>);

            Assert.False(a != b);
            Assert.False(b != a);
        }

        [Fact]
        public static void ReturnsFalseForSameContainedValues()
        {
            Optional<int> a = 1;
            Optional<int> b = 1;

            Assert.False(a != b);
            Assert.False(b != a);
        }

        [Fact]
        public static void ReturnsTrueForDefaultValueComparedToContainedValue()
        {
            var a = default(Optional<int>);
            Optional<int> b = 1;

            Assert.True(a != b);
            Assert.True(b != a);
        }

        [Fact]
        public static void ReturnsTrueForDifferentContainedValues()
        {
            Optional<int> a = 1;
            Optional<int> b = 2;

            Assert.True(a != b);
            Assert.True(b != a);
        }
    }

    /// <summary>
    /// Tests the <see cref="Optional{TValue}.Equals(Optional{TValue})"/> method and its overloads.
    /// </summary>
    public new class Equals
    {
        public static class Typed
        {
            [Fact]
            public static void ReturnsTrueForDefaultValues()
            {
                var a = default(Optional<int>);
                var b = default(Optional<int>);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public static void ReturnsTrueForSameContainedValues()
            {
                Optional<int> a = 1;
                Optional<int> b = 1;

                Assert.True(a.Equals(b));
            }

            [Fact]
            public static void ReturnsFalseForDefaultValueComparedToContainedValue()
            {
                var a = default(Optional<int>);
                Optional<int> b = 1;

                Assert.False(a.Equals(b));
            }

            [Fact]
            public static void ReturnsFalseForDifferentContainedValues()
            {
                Optional<int> a = 1;
                Optional<int> b = 2;

                Assert.False(a.Equals(b));
            }
        }

        public static class Object
        {
            [Fact]
            public static void ReturnsTrueForDefaultValues()
            {
                var a = default(Optional<int>);
                object b = default(Optional<int>);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public static void ReturnsTrueForSameContainedValues()
            {
                Optional<int> a = 1;
                object b = new Optional<int>(1);

                Assert.True(a.Equals(b));
            }

            [Fact]
            public static void ReturnsFalseForDefaultValueComparedToContainedValue()
            {
                var a = default(Optional<int>);
                object b = new Optional<int>(1);

                Assert.False(a.Equals(b));
            }

            [Fact]
            public static void ReturnsFalseForDifferentContainedValues()
            {
                Optional<int> a = 1;
                object b = new Optional<int>(2);

                Assert.False(a.Equals(b));
            }
        }
    }

    /// <summary>
    /// Tests the <see cref="Optional{TValue}.GetHashCode"/> method.
    /// </summary>
    public new class GetHashCode
    {
        [Fact]
        public static void ReturnsDefaultContainedCombinedWithFalseForDefault()
        {
            var a = default(Optional<int>);
            Assert.Equal(HashCode.Combine(default(int), false), a.GetHashCode());
        }

        [Fact]
        public static void ReturnsContainedCombinedWithTrueForContainedValue()
        {
            Optional<int> a = 1;
            Assert.Equal(HashCode.Combine(1, true), a.GetHashCode());
        }
    }

    public static class OrDefault
    {
        [Fact]
        public static void ReturnsDefaultValueOfTypeForStructs()
        {
            var a = default(Optional<int>);
            Assert.Equal(default, a.OrDefault());
        }

        [Fact]
        public static void ReturnsNullValueForClasses()
        {
            var a = default(Optional<object>);
            Assert.Null(a.OrDefault());
        }

        [Fact]
        public static void ReturnsExistingValueIfContainsValue()
        {
            Optional<int> a = 1;
            Assert.Equal(1, a.OrDefault());
            Assert.Equal(1, a.OrDefault(2));
        }

        [Fact]
        public static void ReturnsProvidedValueIfNull()
        {
            Optional<string?> a = null;
            Assert.Equal("Expected", a.OrDefault("Expected"));

            Optional<int?> b = null;
            Assert.Equal(1, b.OrDefault(1));
        }

        [Fact]
        public static void ReturnsProvidedDefaultValueIfDoesNotContainValue()
        {
            var a = default(Optional<string>);
            Assert.Equal("Expected", a.OrDefault("Expected"));

            var b = default(Optional<int>);
            Assert.Equal(1, b.OrDefault(1));
        }
    }

    public static class OrThrow
    {
        [Fact]
        public static void ThrowsIfDoesNotContainValue()
        {
            var a = default(Optional<int>);
            Assert.Throws<InvalidOperationException>(() => a.OrThrow(static () => new InvalidOperationException("Expected")));
        }

        [Fact]
        public static void DoesNotThrowIfContainsValue()
        {
            Optional<int> a = 1;

            var exception = Record.Exception(() => a.OrThrow(static () => new InvalidOperationException("Not expected")));
            Assert.Null(exception);
        }
    }

    public static class AsNullableOptional
    {
        public static class Struct
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                var a = default(Optional<int>);
                var result = a.AsNullableOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(default(Optional<int?>), result);
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                Optional<int> a = 1;
                var result = a.AsNullableOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(new Optional<int?>(1), result);
                Assert.True(result.HasValue);
            }
        }

        public static class Class
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                var a = default(Optional<object>);
                var result = a.AsNullableOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(default(Optional<object?>), result);
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                Optional<string> a = "Hello world";
                var result = a.AsNullableOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(new Optional<string?>("Hello world"), result);
                Assert.True(result.HasValue);
            }
        }
    }

    public static class TryGet
    {
        [Fact]
        public static void ReturnsTrueIfOptionalHasValue()
        {
            Optional<int> a = 1;
            Assert.True(a.TryGet(out var value));
            Assert.Equal(1, value);
        }

        [Fact]
        public static void ReturnsTrueAndProducesNullIfOptionalHasNullValue()
        {
            Optional<object?> nullValue = null;
            Assert.True(nullValue.TryGet(out var value));
            Assert.Null(value);
        }

        [Fact]
        public static void ReturnsFalseIfOptionalHasNoValue()
        {
            var a = default(Optional<int>);
            Assert.False(a.TryGet(out var value));
            Assert.Equal(default, value);
        }
    }

    public static class Map
    {
        [Fact]
        public static void CanMap()
        {
            Optional<int> a = 1;

            var mapped = a.Map(x => x + 1);
            Assert.Equal(new Optional<int>(2), mapped);
        }

        [Fact]
        public static void ReturnsOptionalWithNoValueIfInputHasNoValue()
        {
            var a = default(Optional<int>);

            var mapped = a.Map(x => x + 1);
            Assert.False(mapped.HasValue);
        }
    }

    public static class ConvertNullToEmpty
    {
        public static class Struct
        {
            [Fact]
            public static void ReturnsOptionalWithNoValueIfInputIsNull()
            {
                Optional<int?> a = null;

                var result = a.ConvertNullToEmpty();
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ReturnsOptionalWithNoValueIfInputIsEmpty()
            {
                var a = default(Optional<int?>);

                var result = a.ConvertNullToEmpty();
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ReturnsOptionalWithValueIfInputHasValue()
            {
                Optional<int?> a = 1;

                var result = a.ConvertNullToEmpty();
                Assert.True(result.IsDefined(out var value));
                Assert.Equal(1, value);
            }
        }

        public static class Class
        {
            [Fact]
            public static void ReturnsOptionalWithNoValueIfInputIsNull()
            {
                Optional<string?> a = null;

                var result = a.ConvertNullToEmpty();
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ReturnsOptionalWithNoValueIfInputIsEmpty()
            {
                var a = default(Optional<string?>);

                var result = a.ConvertNullToEmpty();
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ReturnsOptionalWithValueIfInputHasValue()
            {
                Optional<string?> a = "Expected";

                var result = a.ConvertNullToEmpty();
                Assert.True(result.IsDefined(out var value));
                Assert.Equal("Expected", value);
            }
        }
    }

    public static class AsOptional
    {
        public static class Struct
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                var a = default(int?);
                var result = a.AsOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(default(Optional<int>), result);
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                int? a = 1;
                var result = a.AsOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(new Optional<int>(1), result);
                Assert.True(result.HasValue);
            }
        }

        public static class Class
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                string? a = null;
                var result = a.AsOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(default(Optional<string>), result);
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                // ReSharper disable once SuggestVarOrType_BuiltInTypes
                // ReSharper disable once VariableCanBeNotNullable
                string? a = "Hello world";
                var result = a.AsOptional();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(new Optional<string>("Hello world"), result);
                Assert.True(result.HasValue);
            }
        }
    }

    public static class AsNullable
    {
        public static class Struct
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                var a = default(Optional<int>);
                var result = a.AsNullable();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(default(int?), result);
                Assert.False(result.HasValue);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                Optional<int> a = 1;
                var result = a.AsNullable();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal(1, result);
                Assert.True(result.HasValue);
            }
        }

        public static class Class
        {
            [Fact]
            public static void ProducesCorrectValueWhenEmpty()
            {
                var a = default(Optional<string>);
                var result = a.AsNullable();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Null(result);
            }

            [Fact]
            public static void ProducesCorrectValueWhenPresent()
            {
                // ReSharper disable once SuggestVarOrType_BuiltInTypes
                // ReSharper disable once VariableCanBeNotNullable
                Optional<string> a = "Hello world";
                var result = a.AsNullable();

                // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
                Assert.Equal("Hello world", result);
            }
        }
    }
}
