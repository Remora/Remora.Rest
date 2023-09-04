//
//  SPDX-FileName: OptionalConverterFactory.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Remora.Rest.Json;

/// <summary>
/// Creates converters for <see cref="Optional{TValue}"/>.
/// </summary>
[PublicAPI]
public class OptionalConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        var typeInfo = typeToConvert.GetTypeInfo();
        if (!typeInfo.IsGenericType || typeInfo.IsGenericTypeDefinition)
        {
            return false;
        }

        var genericType = typeInfo.GetGenericTypeDefinition();
        return genericType == typeof(Optional<>);
    }

    /// <inheritdoc />
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var typeInfo = typeToConvert.GetTypeInfo();

        var optionalType = typeof(OptionalConverter<>).MakeGenericType(typeInfo.GenericTypeArguments);

        if (Activator.CreateInstance(optionalType) is not JsonConverter createdConverter)
        {
            throw new JsonException();
        }

        return createdConverter;
    }
}
