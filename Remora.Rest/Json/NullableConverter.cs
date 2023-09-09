//
//  SPDX-FileName: NullableConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Converts from to and from <see cref="Nullable{T}"/>.
/// </summary>
/// <typeparam name="TValue">The inner nullable value.</typeparam>
[PublicAPI]
public class NullableConverter<TValue> : JsonConverter<TValue?>
    where TValue : struct
{
    /// <inheritdoc />
    public override TValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            _ => JsonSerializer.Deserialize<TValue>(ref reader, options)
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TValue? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
