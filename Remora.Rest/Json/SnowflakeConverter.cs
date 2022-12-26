//
//  SPDX-FileName: SnowflakeConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Json;

/// <inheritdoc />
[PublicAPI]
public class SnowflakeConverter : JsonConverter<Snowflake>
{
    /// <summary>
    /// Gets the epoch used for converting snowflakes.
    /// </summary>
    public ulong Epoch { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeConverter"/> class.
    /// </summary>
    /// <param name="epoch">The epoch to use.</param>
    public SnowflakeConverter(ulong epoch)
    {
        this.Epoch = epoch;
    }

    /// <inheritdoc />
    public override Snowflake Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                var value = reader.GetString()!;
                if (!Snowflake.TryParse(value, out var snowflake, this.Epoch))
                {
                    throw new JsonException();
                }

                return snowflake.Value;
            }
            case JsonTokenType.Number:
            {
                return new Snowflake(reader.GetUInt64(), this.Epoch);
            }
            default:
            {
                throw new JsonException();
            }
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
