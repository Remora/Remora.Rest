//
//  SPDX-FileName: SnowflakeDictionaryConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Json;

/// <inheritdoc />
[PublicAPI]
public class SnowflakeDictionaryConverter<TElement> : JsonConverter<IReadOnlyDictionary<Snowflake, TElement>>
{
    /// <summary>
    /// Gets the epoch used for converting snowflakes.
    /// </summary>
    public ulong Epoch { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SnowflakeDictionaryConverter{TElement}"/> class.
    /// </summary>
    /// <param name="epoch">The epoch to use.</param>
    public SnowflakeDictionaryConverter(ulong epoch)
    {
        this.Epoch = epoch;
    }

    /// <inheritdoc />
    public override IReadOnlyDictionary<Snowflake, TElement>? Read
    (
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var dictionary = JsonSerializer.Deserialize<IReadOnlyDictionary<string, TElement>>(ref reader, options);
        if (dictionary is null)
        {
            return null;
        }

        var mappedDictionary = new Dictionary<Snowflake, TElement>();
        foreach (var (key, element) in dictionary)
        {
            if (!Snowflake.TryParse(key, out var snowflakeKey, this.Epoch))
            {
                throw new JsonException();
            }

            mappedDictionary.Add(snowflakeKey.Value, element);
        }

        return mappedDictionary;
    }

    /// <inheritdoc />
    public override void Write
    (
        Utf8JsonWriter writer,
        IReadOnlyDictionary<Snowflake, TElement> value,
        JsonSerializerOptions options
    )
    {
        var mappedDictionary = new Dictionary<string, TElement>();
        foreach (var (key, element) in value)
        {
            mappedDictionary.Add(key.ToString(), element);
        }

        JsonSerializer.Serialize(writer, mappedDictionary, options);
    }
}
