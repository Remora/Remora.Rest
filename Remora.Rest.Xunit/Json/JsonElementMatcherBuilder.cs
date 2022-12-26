//
//  SPDX-FileName: JsonElementMatcherBuilder.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentAssertions;
using JetBrains.Annotations;

namespace Remora.Rest.Xunit.Json;

/// <summary>
/// Builds instances of the <see cref="JsonElementMatcher"/> class.
/// </summary>
[PublicAPI]
public class JsonElementMatcherBuilder
{
    private readonly List<Func<JsonElement, bool>> _matchers = new();

    /// <summary>
    /// Adds a requirement that the element should be an object, with optional additional requirements.
    /// </summary>
    /// <param name="objectMatcherBuilder">The optional requirement builder.</param>
    /// <returns>The request matcher builder, with the requirements added.</returns>
    public JsonElementMatcherBuilder IsObject(Action<JsonObjectMatcherBuilder>? objectMatcherBuilder = null)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind.Should().Be(JsonValueKind.Object);
                if (objectMatcherBuilder is null)
                {
                    return true;
                }

                var objectMatcher = new JsonObjectMatcherBuilder();
                objectMatcherBuilder(objectMatcher);

                objectMatcher.Build().Matches(j);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be an array, with optional additional requirements.
    /// </summary>
    /// <param name="arrayMatcherBuilder">The optional requirement builder.</param>
    /// <returns>The request matcher builder, with the requirements added.</returns>
    public JsonElementMatcherBuilder IsArray(Action<JsonArrayMatcherBuilder>? arrayMatcherBuilder = null)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind.Should().Be(JsonValueKind.Array);
                if (arrayMatcherBuilder is null)
                {
                    return true;
                }

                var arrayMatcher = new JsonArrayMatcherBuilder();
                arrayMatcherBuilder(arrayMatcher);

                arrayMatcher.Build().Matches(j.EnumerateArray());

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a value, with optional additional requirements.
    /// </summary>
    /// <param name="valueKind">The type of value the element should be.</param>
    /// <param name="valueMatcher">The optional requirement.</param>
    /// <returns>The request matcher builder, with the requirements added.</returns>
    public JsonElementMatcherBuilder IsValue(JsonValueKind valueKind, Func<JsonElement, bool>? valueMatcher = null)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(valueKind);

                return valueMatcher is null || valueMatcher(j);
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a null value.
    /// </summary>
    /// <returns>The builder, with the requirement added.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonElementMatcherBuilder IsNull()
    {
        _matchers.Add(j =>
        {
            j.ValueKind
                .Should().Be(JsonValueKind.Null);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a boolean.
    /// </summary>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder IsBoolean()
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().BeOneOf(JsonValueKind.True, JsonValueKind.False);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a number.
    /// </summary>
    /// <returns>The builder, with the requirement added.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonElementMatcherBuilder IsNumber()
    {
        _matchers.Add(j =>
        {
            j.ValueKind
                .Should().Be(JsonValueKind.Number);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a boolean.
    /// </summary>
    /// <returns>The builder, with the requirement added.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonElementMatcherBuilder IsString()
    {
        _matchers.Add(j =>
        {
            j.ValueKind
                .Should().Be(JsonValueKind.String);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be an 8-bit integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(sbyte value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetSByte(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a signed byte");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 16-bit integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(short value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetInt16(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a short");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 32-bit integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(int value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetInt32(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to an int");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 64-bit integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(long value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetInt64(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a long");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be an 8-bit unsigned integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(byte value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetByte(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a byte");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 16-bit unsigned integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(ushort value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetUInt16(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to an ushort");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 32-bit unsigned integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(uint value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetUInt32(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to an uint");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 64-bit unsigned integer with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(ulong value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetUInt64(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to an ulong");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a string with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(string value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.String);

                j.GetString()
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a boolean with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Intentional.")]
    public JsonElementMatcherBuilder Is(bool value)
    {
        _matchers.Add(j =>
        {
            j.ValueKind
                .Should().Be(value ? JsonValueKind.True : JsonValueKind.False);

            return true;
        });

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a decimal with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(decimal value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetDecimal(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a decimal");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 32-bit floating point value with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(float value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetSingle(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a float");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a 64-bit floating point value with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(double value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.Number);

                j.TryGetDouble(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a double");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a <see cref="Guid"/>-representable string with the given
    /// value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(Guid value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.String);

                j.TryGetGuid(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a GUID");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a <see cref="DateTime"/> with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(DateTime value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.String);

                j.TryGetDateTime(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a DateTime");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a <see cref="DateTimeOffset"/> with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(DateTimeOffset value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.String);

                j.TryGetDateTimeOffset(out var actualValue)
                    .Should().BeTrue("because the value should be convertible to a DateTimeOffset");

                actualValue
                    .Should().Be(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Adds a requirement that the element should be a base64-encoded byte array with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The builder, with the requirement added.</returns>
    public JsonElementMatcherBuilder Is(IEnumerable<byte> value)
    {
        _matchers.Add
        (
            j =>
            {
                j.ValueKind
                    .Should().Be(JsonValueKind.String);

                j.TryGetBytesFromBase64(out var actualValue)
                    .Should().BeTrue("because the value should be a base64-encoded byte array");

                actualValue
                    .Should().BeEquivalentTo(value);

                return true;
            }
        );

        return this;
    }

    /// <summary>
    /// Builds the element matcher.
    /// </summary>
    /// <returns>The built element matcher.</returns>
    public JsonElementMatcher Build()
    {
        return new(_matchers);
    }
}
