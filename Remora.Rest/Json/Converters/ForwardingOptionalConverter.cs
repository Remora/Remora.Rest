//
//  SPDX-FileName: ForwardingOptionalConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Rest.Core;

namespace Remora.Rest.Json.Converters;

/// <summary>
/// Forwards de/serialization of an <see cref="Optional{TValue}"/> to an overridden converter.
/// </summary>
/// <typeparam name="TValue">The forwarded-to type.</typeparam>
internal class ForwardingOptionalConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    private readonly JsonConverter<TValue> _valueConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForwardingOptionalConverter{TValue}"/> class.
    /// </summary>
    /// <param name="valueConverter">The forwarded-to converter.</param>
    public ForwardingOptionalConverter(JsonConverter<TValue> valueConverter)
    {
        _valueConverter = valueConverter;
    }

    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => _valueConverter.Read(ref reader, typeof(TValue), options);

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Optional<TValue?> value, JsonSerializerOptions options)
    {
        if (value.Value is null)
        {
            writer.WriteNullValue();
            return;
        }

        _valueConverter.Write(writer, value.Value, options);
    }
}
