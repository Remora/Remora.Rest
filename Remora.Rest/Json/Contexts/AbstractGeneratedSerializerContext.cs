//
//  SPDX-FileName: AbstractGeneratedSerializerContext.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Remora.Rest.Json.Contexts;

/// <summary>
/// Serves as a base class for source-generated contexts.
/// </summary>
/// <typeparam name="TRealContext">The implementing type.</typeparam>
[SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "Intentional")]
[Obsolete("This type intended for source-generated code and should not be used directly. Its API is not stable and may change at any time.")]
public abstract class AbstractGeneratedSerializerContext<TRealContext> : JsonSerializerContext, IJsonTypeInfoResolver
    where TRealContext : AbstractGeneratedSerializerContext<TRealContext>
{
    private readonly JsonSerializerOptions _generatedOptions;
    private readonly JsonSerializerContext _realContext;

    /// <summary>
    /// Gets or sets a factory function that creates a new instance of the raw generated context.
    /// </summary>
    protected static Func<JsonSerializerOptions, JsonSerializerContext> GeneratedContextFactory { get; set; } = null!;

    /// <summary>
    /// Gets or sets a factory function that creates a new instance of the implementing type.
    /// </summary>
    public static Func<JsonSerializerOptions, TRealContext> ContextFactory { get; protected set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractGeneratedSerializerContext{TRealContext}"/> class.
    /// </summary>
    /// <param name="options">The serializer options to base the context on.</param>
    /// <param name="generatedConverters">The generated converters to augment the options with.</param>
    protected AbstractGeneratedSerializerContext(JsonSerializerOptions options, JsonConverter[] generatedConverters)
        : base(AugmentOptions(options, generatedConverters))
    {
        _generatedOptions = AugmentOptions(options, generatedConverters);
        _realContext = GeneratedContextFactory(_generatedOptions);
    }

    private static JsonSerializerOptions AugmentOptions
    (
        JsonSerializerOptions options,
        IEnumerable<JsonConverter> extraConverters
    )
    {
        var copy = new JsonSerializerOptions(options);
        copy.Converters.Clear();

        foreach (var converter in extraConverters)
        {
            copy.Converters.Add(converter);
        }

        foreach (var converter in options.Converters)
        {
            copy.Converters.Add(converter);
        }

        return copy;
    }

    /// <inheritdoc/>
    public override JsonTypeInfo? GetTypeInfo(Type type) => _realContext.GetTypeInfo(type);

    /// <inheritdoc/>
    JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
        => ((IJsonTypeInfoResolver)_realContext).GetTypeInfo(type, options);

    /// <inheritdoc/>
    protected override JsonSerializerOptions GeneratedSerializerOptions => _generatedOptions;
}
