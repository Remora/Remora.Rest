//
//  SPDX-FileName: ColorConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Converts instances of the <see cref="Color"/> struct to and from JSON.
/// </summary>
[PublicAPI]
public class ColorConverter : JsonConverter<Color>
{
    /// <inheritdoc />
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException();
        }

        var value = reader.GetUInt32();
        var clrValue = value | 0xFF000000;

        return Color.FromArgb((int)clrValue);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        var val = value.ToArgb() & 0x00FFFFFF;
        writer.WriteNumberValue((uint)val);
    }
}
