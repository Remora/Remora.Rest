//
//  SPDX-FileName: ISO8601DateTimeOffsetConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Converts instances of the <see cref="DateTimeOffset"/> struct to and from an ISO8601 representation in JSON.
/// </summary>
[PublicAPI]
public class ISO8601DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                if (!DateTimeOffset.TryParse(reader.GetString(), out var time))
                {
                    throw new JsonException();
                }

                return time;
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
        var offset = value.Offset;
        writer.WriteStringValue
        (
            value.ToString($"yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'+'{offset.Hours:D2}':'{offset.Minutes:D2}")
        );
    }
}
