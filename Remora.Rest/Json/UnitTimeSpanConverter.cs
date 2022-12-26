//
//  SPDX-FileName: UnitTimeSpanConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Converts a <see cref="TimeSpan"/> to and from a specified time unit in JSON.
/// </summary>
/// <remarks>
/// This converter does not take fractions into account, and only serializes whole integers.
/// </remarks>
[PublicAPI]
public class UnitTimeSpanConverter : JsonConverter<TimeSpan>
{
    private readonly TimeUnit _unit;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitTimeSpanConverter"/> class.
    /// </summary>
    /// <param name="unit">The unit to convert to and from.</param>
    public UnitTimeSpanConverter(TimeUnit unit)
    {
        _unit = unit;
    }

    /// <inheritdoc />
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!reader.TryGetDouble(out var value))
        {
            throw new JsonException();
        }

        return _unit switch
        {
            TimeUnit.Days => TimeSpan.FromDays(value),
            TimeUnit.Hours => TimeSpan.FromHours(value),
            TimeUnit.Minutes => TimeSpan.FromMinutes(value),
            TimeUnit.Seconds => TimeSpan.FromSeconds(value),
            TimeUnit.Milliseconds => TimeSpan.FromMilliseconds(value),
            _ => throw new ArgumentOutOfRangeException(nameof(_unit), "Unknown configured time unit")
        };
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        switch (_unit)
        {
            case TimeUnit.Days:
            {
                writer.WriteNumberValue((int)value.TotalDays);
                break;
            }
            case TimeUnit.Hours:
            {
                writer.WriteNumberValue((int)value.TotalHours);
                break;
            }
            case TimeUnit.Minutes:
            {
                writer.WriteNumberValue((int)value.TotalMinutes);
                break;
            }
            case TimeUnit.Seconds:
            {
                writer.WriteNumberValue((int)value.TotalSeconds);
                break;
            }
            case TimeUnit.Milliseconds:
            {
                writer.WriteNumberValue((int)value.TotalMilliseconds);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(_unit), "Unknown configured time unit");
            }
        }
    }
}
