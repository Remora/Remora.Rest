//
//  SnowflakeConverter.cs
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
using System.Text.Json;
using System.Text.Json.Serialization;
using Remora.Rest.Core;

namespace Remora.Rest.Json;

/// <inheritdoc />
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
