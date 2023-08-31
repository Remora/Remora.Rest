//
//  SPDX-FileName: OptionalConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Remora.Rest.Core;

namespace Remora.Rest.Json.Converters;

/// <summary>
/// Converts optional fields to their JSON representation.
/// </summary>
/// <typeparam name="TValue">The underlying type.</typeparam>
internal sealed class OptionalConverter<TValue> : JsonConverter<Optional<TValue?>>
{
    /// <inheritdoc />
    public override Optional<TValue?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));

        return new(JsonSerializer.Deserialize(ref reader, typeInfo));
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Optional<TValue?> value, JsonSerializerOptions options)
    {
        // direct access is safe here since logical omission is handled via JsonTypeInfo
        if (value.Value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var typeInfo = (JsonTypeInfo<TValue>)options.GetTypeInfo(typeof(TValue));
        JsonSerializer.Serialize(writer, value.Value, typeInfo);
    }
}
