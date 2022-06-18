//
//  OneOfConverter.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneOf;
using Remora.Rest.Core;
using Remora.Rest.Extensions;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// Converts instances of <see cref="IOneOf"/> to and from JSON.
/// </summary>
/// <typeparam name="TOneOf">The OneOf type.</typeparam>
internal class OneOfConverter<TOneOf> : JsonConverter<TOneOf>
    where TOneOf : IOneOf
{
    /// <summary>
    /// Holds the member types, sorted in the order they should be attempted to be deserialized.
    /// </summary>
    /// <remarks>
    /// The order is constructed to produce correct results in as many cases as possible, giving leeway to the fact
    /// that multiple type parsers can take primitive elements from JSON as an input. A typical example of this is
    /// integers vs <see cref="Snowflake"/>.
    ///
    /// The order is as follows:
    ///   * Numeric C# types
    ///   * Collection types
    ///   * Complex types (classes, records, etc)
    ///   * Builtin C# types (string, etc)
    ///
    /// Hopefully, this works for most cases.
    /// </remarks>
    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyList<Type> OrderedUnionMemberTypes;

    /// <summary>
    /// Holds a mapping between the member types and the FromT methods of the union.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static readonly IReadOnlyDictionary<Type, MethodInfo> FromValueMethods;

    /// <summary>
    /// Holds a mapping between the union members types and their property names.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    private static IReadOnlyDictionary<Type, IReadOnlyList<JsonEncodedText>>? _unionTypePropertyNames;

    static OneOfConverter()
    {
        var unionType = typeof(TOneOf);
        var unionMemberTypes = unionType.GetGenericArguments();

        OrderedUnionMemberTypes = unionMemberTypes
            .OrderByDescending(t => t.IsNumeric())
            .ThenByDescending(t => t.IsCollection())
            .ThenBy(t => t.IsBuiltin())
            .ToList();

        var fromValueMethods = new Dictionary<Type, MethodInfo>();
        for (var i = 0; i < unionMemberTypes.Length; ++i)
        {
            var methodInfo = unionType.GetMethod($"FromT{i}");
            if (methodInfo is null)
            {
                throw new InvalidOperationException();
            }

            fromValueMethods.Add(unionMemberTypes[i], methodInfo);
        }

        FromValueMethods = fromValueMethods;
    }

    /// <inheritdoc />
    public override TOneOf Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        InitialiseUnionTypePropertyNames(options.PropertyNamingPolicy);

        if (reader.TokenType is JsonTokenType.StartObject)
        {
            if (TryCreateOneOfFromObject(ref reader, options, _unionTypePropertyNames, out var result))
            {
                return result;
            }
        }

        if (TryCreateOneOf(ref reader, OrderedUnionMemberTypes, options, out var primitiveResult))
        {
            return primitiveResult;
        }

        throw new JsonException("Could not parse value as a member of the union.");
    }

    private static bool TryCreateOneOfFromObject
    (
        ref Utf8JsonReader reader,
        JsonSerializerOptions options,
        IReadOnlyDictionary<Type, IReadOnlyList<JsonEncodedText>> unionTypePropertyNames,
        [NotNullWhen(true)] out TOneOf? oneOf
    )
    {
        oneOf = default;
        using var document = JsonDocument.ParseValue(ref reader);

        foreach (KeyValuePair<Type, IReadOnlyList<JsonEncodedText>> pair in unionTypePropertyNames)
        {
            bool match = true;
            foreach (JsonEncodedText propName in pair.Value)
            {
                if (!document.RootElement.TryGetProperty(propName.EncodedUtf8Bytes, out _))
                {
                    match = false;
                    break;
                }
            }

            if (!match)
            {
                continue;
            }

            object? value = document.Deserialize(pair.Key, options);
            var method = FromValueMethods[pair.Key];
            oneOf = (TOneOf)method.Invoke(null, new[] { value })!;
            return true;
        }

        // Nothing matched. Perhaps it's a simple type?
        return false;
    }

    private static bool TryCreateOneOf
    (
        ref Utf8JsonReader reader,
        IEnumerable<Type> types,
        JsonSerializerOptions options,
        [NotNullWhen(true)] out TOneOf? oneOf
    )
    {
        oneOf = default;

        // This method is used to promote the most likely type to successfully deserialize to the start of the
        // considered types. The original most-likely order is generally preserved, but this increases our chances
        // significantly.
        static bool CouldMatchJson(Type type, JsonTokenType tokenType)
        {
            return tokenType switch
            {
                JsonTokenType.StartArray => type.IsCollection(),
                JsonTokenType.String => type == typeof(string),
                JsonTokenType.Number => type.IsNumeric(),
                JsonTokenType.True => type == typeof(bool),
                JsonTokenType.False => type == typeof(bool),
                JsonTokenType.Null => type.IsNullable(),
                JsonTokenType.StartObject => !type.IsPrimitive,
                _ => false
            };
        }

        var currentTokenType = reader.TokenType;
        types = types.OrderByDescending(t => CouldMatchJson(t, currentTokenType));

        // Try deserializing from one of the "other" types
        foreach (var type in types)
        {
            object? value;
            try
            {
                value = JsonSerializer.Deserialize(ref reader, type, options);
            }
            catch (JsonException)
            {
                // Pass, we'll try the next one
                continue;
            }

            // It worked!
            var method = FromValueMethods[type];

            oneOf = (TOneOf)method.Invoke(null, new[] { value })!;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TOneOf value, JsonSerializerOptions options)
    {
        var declaredType = typeof(TOneOf).GetGenericArguments()[value.Index];
        JsonSerializer.Serialize(writer, value.Value, declaredType, options);
    }

    /// <summary>
    /// Initializes the <see cref="_unionTypePropertyNames"/> field with the property
    /// names of each member type of the OneOf union. <see cref="Optional{TValue}"/>
    /// properties are ignored.
    /// </summary>
    /// <param name="namePolicy">The naming policy used to convert property names.</param>
    [MemberNotNull(nameof(_unionTypePropertyNames))]
    private static void InitialiseUnionTypePropertyNames(JsonNamingPolicy? namePolicy = null)
    {
        if (_unionTypePropertyNames is not null)
        {
            return;
        }

        var unionTypePropertyNames = new Dictionary<Type, IReadOnlyList<JsonEncodedText>>();
        foreach (Type unionType in typeof(TOneOf).GetGenericArguments())
        {
            var propNames = new List<JsonEncodedText>();
            unionTypePropertyNames.Add(unionType, propNames);

            foreach (var prop in unionType.GetProperties())
            {
                // No need to consider optional properties. If they're not present in the payload,
                // we can't match on them and it's an expected worst-case scenario
                if (prop.PropertyType == typeof(Optional<>))
                {
                    continue;
                }

                var encoded = JsonEncodedText.Encode
                (
                    namePolicy is null
                        ? prop.Name
                        : namePolicy.ConvertName(prop.Name)
                );
                propNames.Add(encoded);
            }
        }

        _unionTypePropertyNames = unionTypePropertyNames;
    }
}
