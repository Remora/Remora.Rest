//
//  SPDX-FileName: OneOfConverterFactory.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// Creates OneOf converters.
/// </summary>
internal class OneOfConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        var genericType = typeToConvert.GetGenericTypeDefinition();
        return genericType switch
        {
            _ when genericType == typeof(OneOf<>) => true,
            _ when genericType == typeof(OneOf<,>) => true,
            _ when genericType == typeof(OneOf<,,>) => true,
            _ when genericType == typeof(OneOf<,,,>) => true,
            _ when genericType == typeof(OneOf<,,,,>) => true,
            _ when genericType == typeof(OneOf<,,,,,>) => true,
            _ when genericType == typeof(OneOf<,,,,,,>) => true,
            _ when genericType == typeof(OneOf<,,,,,,,>) => true,
            _ when genericType == typeof(OneOf<,,,,,,,,>) => true,
            _ => false
        };
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var typeInfo = typeToConvert.GetTypeInfo();
        var optionalType = typeof(OneOfConverter<>).MakeGenericType(typeInfo);

        if (Activator.CreateInstance(optionalType) is not JsonConverter createdConverter)
        {
            throw new JsonException();
        }

        return createdConverter;
    }
}
