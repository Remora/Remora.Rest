//
//  SPDX-FileName: ForwardingNullableConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remora.Rest.Json.Converters;

/// <summary>
/// Forwards de/serialization of an <see cref="Nullable{TValue}"/> to an overridden converter.
/// </summary>
/// <typeparam name="TValue">The forwarded-to type.</typeparam>
internal class ForwardingNullableConverter<TValue> : JsonConverter<TValue?> where TValue : struct
{
    private readonly JsonConverter<TValue> _valueConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardingNullableConverter{TValue}"/> class.
    /// </summary>
    /// <param name="valueConverter">The forwarded-to converter.</param>
    public ForwardingNullableConverter(JsonConverter<TValue> valueConverter)
    {
        _valueConverter = valueConverter;
    }

    /// <inheritdoc />
    public override TValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
        {
            return null;
        }

        return _valueConverter.Read(ref reader, typeof(TValue), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TValue? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        _valueConverter.Write(writer, value.Value, options);
    }
}
