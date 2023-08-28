//
//  SPDX-FileName: InterfaceConverter.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Remora.Rest.Json.Converters;

/// <summary>
/// Maps de/serialization of an interface to an appropriate concrete type. A single concrete type is used when
/// deserializing, while the actual type of the object is used when serializing.
/// </summary>
/// <typeparam name="TInterface">The interface type.</typeparam>
/// <typeparam name="TImplementation">The implementation type used for deserialization.</typeparam>
[Obsolete("This type intended for source-generated code and should not be used directly. Its API is not stable and may change at any time.")]
public class InterfaceConverter<TInterface, TImplementation> : JsonConverter<TInterface>
    where TImplementation : TInterface
    where TInterface : notnull
{
    /// <inheritdoc/>
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var typeInfo = (JsonTypeInfo<TImplementation>)options.GetTypeInfo(typeof(TImplementation));
        return JsonSerializer.Deserialize(ref reader, typeInfo);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
    {
        var typeInfo = options.GetTypeInfo(value.GetType());

        // use the type information of the concrete type, or fall back to the interface
        // the use of Unsafe.As here is a little scary, but it's just to trick the compiler into accepting the
        // type info. Under the hood, in IL, the two values are compatible since we know for a fact that the real type
        // of value is TReal, and the runtime type of typeInfo is JsonTypeInfo<TReal>.
        //
        JsonSerializer.Serialize(writer, value, Unsafe.As<JsonTypeInfo<TInterface>>(typeInfo));
    }
}
