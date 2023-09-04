//
//  SPDX-FileName: OptionalConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Json;

/// <summary>
/// Converts optional fields to their JSON representation.
/// </summary>
/// <typeparam name="TValue">The underlying type.</typeparam>
[PublicAPI]
public class OptionalConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<TValue>(ref reader, options));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Optional<TValue?> value, JsonSerializerOptions options)
    {
        if (value.Value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
