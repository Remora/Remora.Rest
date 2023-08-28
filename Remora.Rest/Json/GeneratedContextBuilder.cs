//
//  SPDX-FileName: GeneratedContextBuilder.cs
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
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Remora.Rest.Core;
using Remora.Rest.Json.Contexts;
using Remora.Rest.Json.Converters;

namespace Remora.Rest.Json;

/// <summary>
/// Provides a fluent API for building a combined compile time- and runtime-defined model of a JSON de/serialization
/// model.
/// </summary>
[PublicAPI]
public class GeneratedContextBuilder
{
    private readonly IServiceCollection _serviceCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedContextBuilder"/> class.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    public GeneratedContextBuilder(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    /// <summary>
    /// Registers a data object, enabling source-generated serialization logic for the types and associating the
    /// implementation type as the default type used when deserializing interface-typed properties and values.
    /// </summary>
    /// <param name="configureInterface">
    /// The configuration function for the interface type. Any type implementing the interface that is de/serialized by
    /// the JSON model will have this configuration applied.
    /// </param>
    /// <param name="configureImplementation">
    /// The configuration function for the implementation type. Only the implementation type will have this
    /// configuration applied.
    /// </param>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <returns>The builder.</returns>
    public GeneratedContextBuilder WithDataObject<TInterface, TImplementation>
    (
        Func<GeneratedContextTypeBuilder<TInterface>, GeneratedContextTypeBuilder<TInterface>>? configureInterface = null,
        Func<GeneratedContextTypeBuilder<TImplementation>, GeneratedContextTypeBuilder<TImplementation>>? configureImplementation = null
    )
        where TImplementation : TInterface
        where TInterface : notnull
    {
        configureInterface?.Invoke(new GeneratedContextTypeBuilder<TInterface>(this));
        configureImplementation?.Invoke(new GeneratedContextTypeBuilder<TImplementation>(this));

        return this;
    }

    /// <summary>
    /// Finishes configuration of this model.
    /// </summary>
    /// <returns>The service collection.</returns>
    public IServiceCollection Finish() => _serviceCollection;

    /// <summary>
    /// Provides a fluent API for building a combined compile time- and runtime-defined model of a JSON de/serialization
    /// model for a single type.
    /// </summary>
    /// <typeparam name="TType">The type.</typeparam>
    public class GeneratedContextTypeBuilder<TType>
    {
        private readonly GeneratedContextBuilder _contextBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedContextTypeBuilder{TType}"/> class.
        /// </summary>
        /// <param name="contextBuilder">The context builder this instance belongs to.</param>
        public GeneratedContextTypeBuilder(GeneratedContextBuilder contextBuilder)
        {
            _contextBuilder = contextBuilder;
        }

        /// <summary>
        /// Applies a generic modification to the type.
        /// </summary>
        /// <param name="modifier">The modification.</param>
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> Modify(Action<JsonTypeInfo> modifier)
        {
            _contextBuilder._serviceCollection.Configure<FluentSerializerContext>(c => c.Modify<TType>(modifier));
            return this;
        }

        /// <summary>
        /// Applies a generic modification to a property on the type.
        /// </summary>
        /// <param name="propertyExpression">The property to modify.</param>
        /// <param name="modifier">The modification.</param>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> ModifyProperty<TProperty>
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

            return Modify(property.Name, modifier);
        }

        /// <summary>
        /// Sets whether additional properties should be accepted or rejected when encountered in the deserialization
        /// logic.
        /// </summary>
        /// <param name="allowExtraProperties">
        /// true if additional properties should be accepted; otherwise, false.
        /// </param>
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> AllowExtraProperties(bool allowExtraProperties = true)
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
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> IncludeWhenSerializing<TProperty>
        (
            Expression<Func<TType, TProperty>> propertyExpression
        ) => ModifyProperty(propertyExpression, p => p.ShouldSerialize = (_, _) => true);

        /// <summary>
        /// Explicitly marks the property as excluded when serializing, ignoring other settings.
        /// </summary>
        /// <param name="propertyExpression">The property.</param>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> ExcludeWhenSerializing<TProperty>
        (
            Expression<Func<TType, TProperty>> propertyExpression
        ) => ModifyProperty(propertyExpression, p => p.ShouldSerialize = (_, _) => false);

        /// <summary>
        /// Overrides the serialized name of the property.
        /// </summary>
        /// <param name="propertyExpression">The property.</param>
        /// <param name="name">The new name of the property.</param>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> WithPropertyName<TProperty>
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
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> WithPropertyConverter<TProperty>
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
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> WithPropertyConverter<TProperty>
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
        /// <returns>The builder, with the modification.</returns>
        public GeneratedContextTypeBuilder<TType> WithPropertyConverter<TProperty>
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

        private GeneratedContextTypeBuilder<TType> Modify(string propertyName, Action<JsonPropertyInfo> modifier)
        {
            _contextBuilder._serviceCollection.Configure<FluentSerializerContext>
            (
                c => c.Modify<TType>(propertyName, modifier)
            );

            return this;
        }
    }
}
