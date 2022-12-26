//
//  SPDX-FileName: Utf8JsonWriterExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Text.Json;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="Utf8JsonWriter"/> class.
/// </summary>
[PublicAPI]
public static class Utf8JsonWriterExtensions
{
    /// <summary>
    /// Writes the given optional to the json writer as its serialized representation. If the value is null, a null
    /// is written.
    /// </summary>
    /// <param name="json">The JSON writer.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="jsonOptions">The json options, if any.</param>
    /// <typeparam name="T">The underlying type.</typeparam>
    public static void Write<T>
    (
        this Utf8JsonWriter json,
        string name,
        in T value,
        JsonSerializerOptions? jsonOptions = default
    ) => json.Write(name, new Optional<T>(value), jsonOptions);

    /// <summary>
    /// Writes the given optional to the json writer as its serialized representation. If the value is null, a null
    /// is written.
    /// </summary>
    /// <param name="json">The JSON writer.</param>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The value.</param>
    /// <param name="jsonOptions">The json options, if any.</param>
    /// <typeparam name="T">The underlying type.</typeparam>
    public static void Write<T>
    (
        this Utf8JsonWriter json,
        string name,
        in Optional<T> value,
        JsonSerializerOptions? jsonOptions = default
    )
    {
        if (!value.HasValue)
        {
            return;
        }

        if (value.Value is null)
        {
            json.WriteNull(name);
            return;
        }

        json.WritePropertyName(name);
        JsonSerializer.Serialize(json, value.Value, jsonOptions);
    }
}
