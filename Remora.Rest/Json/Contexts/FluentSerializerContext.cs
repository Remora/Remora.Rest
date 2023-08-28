//
//  SPDX-FileName: FluentSerializerContext.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using JetBrains.Annotations;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Rest.Json.Converters;

namespace Remora.Rest.Json.Contexts;

/// <summary>
/// Provides the ability to configure modifications to a set of JSON serialization contexts with a fluent API.
/// </summary>
[PublicAPI]
public class FluentSerializerContext : JsonSerializerContext
{
    private readonly Dictionary<Type, JsonTypeInfo> _typeInfoCache = new();
    private readonly IReadOnlyList<JsonSerializerContext> _contexts;

    private readonly Dictionary<Type, List<Action<JsonTypeInfo>>> _modifiers = new();
    private readonly Dictionary<Type, Dictionary<string, List<Action<JsonPropertyInfo>>>> _propertyModifiers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FluentSerializerContext"/> class.
    /// </summary>
    /// <param name="options">The serializer options used by the contexts.</param>
    /// <param name="contexts">The contexts.</param>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Unavoidable")]
    public FluentSerializerContext(JsonSerializerOptions options, IEnumerable<JsonSerializerContext> contexts)
        : base(CombineOptions(options, contexts.Select(c => c.Options)))
    {
        _contexts = contexts.ToArray();
    }

    private static JsonSerializerOptions CombineOptions
    (
        JsonSerializerOptions system,
        IEnumerable<JsonSerializerOptions> contextOptions
    )
    {
        var options = new JsonSerializerOptions(system);
        foreach (var converter in contextOptions.SelectMany(o => o.Converters))
        {
            if (options.Converters.Any(c => c.GetType() == converter.GetType()))
            {
                continue;
            }

            options.Converters.Add(converter);
        }

        return options;
    }

    /// <summary>
    /// Adds a modification to the fluent context.
    /// </summary>
    /// <param name="modifier">The modifier.</param>
    /// <typeparam name="TTarget">The target type to modify.</typeparam>
    public void Modify<TTarget>(Action<JsonTypeInfo> modifier)
    {
        if (!_modifiers.TryGetValue(typeof(TTarget), out var modifiers))
        {
            modifiers = new();
            _modifiers.Add(typeof(TTarget), modifiers);
        }

        modifiers.Add(modifier);
    }

    /// <summary>
    /// Adds a modification to the fluent context.
    /// </summary>
    /// <param name="propertyName">The name of the property to modify.</param>
    /// <param name="modifier">The modifier.</param>
    /// <typeparam name="TTarget">The target type to modify.</typeparam>
    public void Modify<TTarget>(string propertyName, Action<JsonPropertyInfo> modifier)
    {
        if (!_propertyModifiers.TryGetValue(typeof(TTarget), out var propertyModifiers))
        {
            propertyModifiers = new();
            _propertyModifiers.Add(typeof(TTarget), propertyModifiers);
        }

        if (!propertyModifiers.TryGetValue(propertyName, out var modifiers))
        {
            modifiers = new();
            propertyModifiers.Add(propertyName, modifiers);
        }

        modifiers.Add(modifier);
    }

    /// <inheritdoc/>
    public override JsonTypeInfo GetTypeInfo(Type type)
    {
        if (_typeInfoCache.TryGetValue(type, out var baseInfo))
        {
            // skip potentially expensive mutations
            return baseInfo;
        }

        foreach (var context in _contexts)
        {
            baseInfo = ((IJsonTypeInfoResolver)context).GetTypeInfo(type, this.Options);
            if (baseInfo is not null)
            {
                break;
            }
        }

        if (baseInfo is null)
        {
            throw new InvalidOperationException();
        }

        #pragma warning disable CS0618
        var isShimInfo = baseInfo.Converter.GetType().IsGenericType
                         && baseInfo.Converter.GetType().GetGenericTypeDefinition() == typeof(InterfaceConverter<,>);
        #pragma warning restore CS0618

        if (isShimInfo)
        {
            // skip modification of our own shim type information
            return baseInfo;
        }

        // do the property modifiers first to ensure names aren't clobbered
        var applicablePropertyModifierTypes = _propertyModifiers.Keys.Where(k => k.IsAssignableFrom(type));
        var applicablePropertyModifiers = applicablePropertyModifierTypes
            .SelectMany(k => _propertyModifiers[k])
            .ToLookup(k => k.Key, v => v.Value);

        foreach (var propertyModifiers in applicablePropertyModifiers)
        {
            var property = baseInfo.Properties.Single(p => p.Name == propertyModifiers.Key);
            foreach (var modifier in propertyModifiers.SelectMany(m => m))
            {
                modifier(property);
            }
        }

        // then more general modifiers
        var applicableTypes = _modifiers.Keys.Where(k => k.IsAssignableFrom(type));
        var applicableModifiers = applicableTypes.SelectMany(k => _modifiers[k]);
        foreach (var modifier in applicableModifiers)
        {
            modifier(baseInfo);
        }

        // and finally built-in modifiers
        ApplyOptionalSerializationModifications(baseInfo);

        _typeInfoCache.Add(type, baseInfo);
        return baseInfo;
    }

    /// <summary>
    /// Applies required modifications to support <see cref="Optional{TValue}"/> types.
    /// </summary>
    /// <param name="typeInfo">The type info to modify.</param>
    private void ApplyOptionalSerializationModifications(JsonTypeInfo typeInfo)
    {
        foreach (var property in typeInfo.Properties)
        {
            if (!property.PropertyType.IsOptional())
            {
                continue;
            }

            property.ShouldSerialize = (_, optional) => ((IOptional)optional!).HasValue;
        }
    }

    /// <inheritdoc/>
    protected override JsonSerializerOptions GeneratedSerializerOptions => DefaultOptions;

    private static JsonSerializerOptions DefaultOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        IncludeFields = false,
        WriteIndented = false
    };
}
