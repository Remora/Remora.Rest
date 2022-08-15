//
//  JsonElementMatcherBuilder.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using JetBrains.Annotations;
using Xunit;
using Xunit.Sdk;

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
                Assert.Equal(JsonValueKind.Object, j.ValueKind);
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
                Assert.Equal(JsonValueKind.Array, j.ValueKind);
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
                Assert.Equal(valueKind, j.ValueKind);
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
            Assert.Equal(JsonValueKind.Null, j.ValueKind);
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
                if (j.ValueKind is JsonValueKind.True or JsonValueKind.False)
                {
                    return true;
                }

                throw new EqualException("True or False", j.ValueKind.ToString());
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
            Assert.Equal(JsonValueKind.Number, j.ValueKind);
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
            Assert.Equal(JsonValueKind.String, j.ValueKind);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetSByte(out var actualValue))
                {
                    throw new IsTypeException(nameof(SByte), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetInt16(out var actualValue))
                {
                    throw new IsTypeException(nameof(Int16), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetInt32(out var actualValue))
                {
                    throw new IsTypeException(nameof(Int32), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetInt64(out var actualValue))
                {
                    throw new IsTypeException(nameof(Int64), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetByte(out var actualValue))
                {
                    throw new IsTypeException(nameof(Byte), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetUInt16(out var actualValue))
                {
                    throw new IsTypeException(nameof(UInt16), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetUInt32(out var actualValue))
                {
                    throw new IsTypeException(nameof(UInt32), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetUInt64(out var actualValue))
                {
                    throw new IsTypeException(nameof(UInt64), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.String, j.ValueKind);
                Assert.Equal(value, j.GetString());
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
            Assert.Equal(value ? JsonValueKind.True : JsonValueKind.False, j.ValueKind);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetDecimal(out var actualValue))
                {
                    throw new IsTypeException(nameof(Decimal), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetSingle(out var actualValue))
                {
                    throw new IsTypeException(nameof(Single), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.Number, j.ValueKind);

                if (!j.TryGetDouble(out var actualValue))
                {
                    throw new IsTypeException(nameof(Double), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.String, j.ValueKind);

                if (!j.TryGetGuid(out var actualValue))
                {
                    throw new IsTypeException(nameof(Guid), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.String, j.ValueKind);

                if (!j.TryGetDateTime(out var actualValue))
                {
                    throw new IsTypeException(nameof(DateTime), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.String, j.ValueKind);

                if (!j.TryGetDateTimeOffset(out var actualValue))
                {
                    throw new IsTypeException(nameof(DateTimeOffset), "Number");
                }

                Assert.Equal(value, actualValue);
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
                Assert.Equal(JsonValueKind.String, j.ValueKind);

                if (!j.TryGetBytesFromBase64(out var actualValue))
                {
                    throw new IsTypeException("IEnumerable<byte>", "Number");
                }

                Assert.Equal(value, actualValue);
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
