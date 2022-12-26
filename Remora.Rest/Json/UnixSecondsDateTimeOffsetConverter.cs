//
//  SPDX-FileName: UnixSecondsDateTimeOffsetConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Converts a <see cref="DateTimeOffset"/> to and from unix time, in seconds.
/// </summary>
[PublicAPI]
public class UnixSecondsDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
            {
                var milliseconds = reader.GetInt64();
                return DateTimeOffset.FromUnixTimeSeconds(milliseconds);
            }
            default:
            {
                throw new JsonException();
            }
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        var milliseconds = value.ToUnixTimeSeconds();
        writer.WriteNumberValue(milliseconds);
    }
}
