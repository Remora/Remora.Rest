//
//  SPDX-FileName: NullableConverterFactory.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Extensions;

namespace Remora.Rest.Json;

/// <summary>
/// Creates instances of <see cref="NullableConverter{TValue}"/>.
/// </summary>
[PublicAPI]
public class NullableConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsNullable();
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converter = (JsonConverter?)Activator.CreateInstance
        (
            typeof(NullableConverter<>).MakeGenericType(typeToConvert.GetGenericArguments())
        );

        if (converter is null)
        {
            throw new InvalidOperationException();
        }

        return converter;
    }
}
