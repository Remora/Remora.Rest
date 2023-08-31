//
//  SPDX-FileName: JsonTypeInfoConfiguration.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Remora.Rest.Core;
using Remora.Rest.Extensions;
using Remora.Rest.Json.Converters;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Represents a runtime configuration applied to a <see cref="JsonTypeInfo"/>.
/// </summary>
    /// <typeparam name="TType">The type.</typeparam>
public class JsonTypeInfoConfiguration<TType> : IJsonTypeInfoConfiguration
{
    private readonly List<Action<JsonTypeInfo>> _modifiers = new();
    private readonly Dictionary<string, List<Action<JsonPropertyInfo>>> _propertyModifiers = new();

    /// <inheritdoc/>
    Type IJsonTypeInfoConfiguration.Type => typeof(TType);

    /// <summary>
    /// Applies a generic modification to a type.
    /// </summary>
    /// <param name="modifier">The modifier.</param>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> Modify(Action<JsonTypeInfo> modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }

    /// <summary>
    /// Applies a generic modification to a property on a type.
    /// </summary>
    /// <param name="propertyExpression">The property to modify.</param>
    /// <param name="modifier">The modification.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The builder, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> ModifyProperty<TProperty>
    (
        Expression<Func<TType, TProperty>> propertyExpression,
        Action<JsonPropertyInfo> modifier
    )
    {
        if (propertyExpression.Body is not MemberExpression memberExpression)
        {
            throw new InvalidOperationException();
        }

        var member = memberExpression.Member;
        if (member is not PropertyInfo property)
        {
            throw new InvalidOperationException();
        }

        ModifyProperty(property.Name, modifier);
        return this;
    }

    /// <summary>
    /// Sets whether additional properties should be accepted or rejected when encountered in the deserialization
    /// logic.
    /// </summary>
    /// <param name="allowExtraProperties">
    /// true if additional properties should be accepted; otherwise, false.
    /// </param>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> AllowExtraProperties(bool allowExtraProperties = true)
    {
        if (allowExtraProperties)
        {
            return this;
        }

        return Modify
        (
            t =>
            {
                var overflowProperty = t.Properties.FirstOrDefault(p => p.IsExtensionData);
                if (overflowProperty is not null)
                {
                    return;
                }

                #pragma warning disable IL2026, IL3050
                overflowProperty = t.CreateJsonPropertyInfo
                (
                    typeof(Dictionary<string, JsonElement>),
                    "ExtraProperties"
                );
                #pragma warning restore

                overflowProperty.IsExtensionData = true;
                overflowProperty.Get = static _ => null;
                overflowProperty.Set = static (_, val) =>
                {
                    var dictionary = (Dictionary<string, JsonElement>?)val;
                    if (dictionary != null)
                    {
                        throw new JsonException();
                    }
                };

                t.Properties.Add(overflowProperty);
            }
        );
    }

    /// <summary>
    /// Explicitly marks the property as included when serializing, ignoring other settings.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> IncludeWhenSerializing<TProperty>
    (
        Expression<Func<TType, TProperty>> propertyExpression
    ) => ModifyProperty(propertyExpression, p => p.ShouldSerialize = (_, _) => true);

    /// <summary>
    /// Explicitly marks the property as excluded when serializing, ignoring other settings.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> ExcludeWhenSerializing<TProperty>
    (
        Expression<Func<TType, TProperty>> propertyExpression
    ) => ModifyProperty(propertyExpression, p => p.ShouldSerialize = (_, _) => false);

    /// <summary>
    /// Overrides the serialized name of the property.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <param name="name">The new name of the property.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> WithPropertyName<TProperty>
    (
        Expression<Func<TType, TProperty>> propertyExpression,
        string name
    ) => ModifyProperty(propertyExpression, p => p.Name = name);

    /// <summary>
    /// Explicitly specifies a converter to use for the property, ignoring other settings.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <param name="converter">The converter to use.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> WithPropertyConverter<TProperty>
    (
        Expression<Func<TType, TProperty>> propertyExpression,
        JsonConverter<TProperty> converter
    ) => ModifyProperty(propertyExpression, p => p.CustomConverter = converter);

    /// <summary>
    /// Explicitly specifies a converter to use for the property, ignoring other settings.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <param name="converter">The converter to use.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> WithPropertyConverter<TProperty>
    (
        Expression<Func<TType, Optional<TProperty>>> propertyExpression,
        JsonConverter<TProperty> converter
    ) => ModifyProperty
    (
        propertyExpression, p => p.CustomConverter = new ForwardingOptionalConverter<TProperty>(converter)
    );

    /// <summary>
    /// Explicitly specifies a converter to use for the property, ignoring other settings.
    /// </summary>
    /// <param name="propertyExpression">The property.</param>
    /// <param name="converter">The converter to use.</param>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <returns>The configuration, with the modification.</returns>
    public JsonTypeInfoConfiguration<TType> WithPropertyConverter<TProperty>
    (
        Expression<Func<TType, TProperty?>> propertyExpression,
        JsonConverter<TProperty> converter
    )
        where TProperty : struct
    {
        return ModifyProperty
        (
            propertyExpression,
            p => p.CustomConverter = new ForwardingNullableConverter<TProperty>(converter)
        );
    }

    /// <inheritdoc/>
    public void Configure(JsonTypeInfo typeInfo)
    {
        if (typeInfo.IsReadOnly)
        {
            throw new InvalidOperationException("The type information has already been configured.");
        }

        // do the property modifiers first to ensure names aren't clobbered
        foreach (var (propertyName, modifiers) in _propertyModifiers)
        {
            var property = typeInfo.Properties.Single(p => p.Name == propertyName);
            foreach (var modifier in modifiers)
            {
                modifier(property);
            }
        }

        // then more general modifiers
        foreach (var modifier in _modifiers)
        {
            modifier(typeInfo);
        }

        // and finally built-in modifiers
        ApplyOptionalSerializationModifications(typeInfo);
    }

    /// <summary>
    /// Applies a generic modification to a property on a type.
    /// </summary>
    /// <param name="propertyName">The name of the property to modify.</param>
    /// <param name="modifier">The modifier.</param>
    private void ModifyProperty(string propertyName, Action<JsonPropertyInfo> modifier)
    {
        if (!_propertyModifiers.TryGetValue(propertyName, out var modifiers))
        {
            modifiers = new();
            _propertyModifiers.Add(propertyName, modifiers);
        }

        modifiers.Add(modifier);
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
}
