//
//  SPDX-FileName: ObjectInitializationInfo.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remora.Rest.Json.Reflection;

/// <summary>
/// Represents the information required to initialize a DTO via its setters.
/// </summary>
internal class ObjectInitializationInfo : IInitializationInfo
{
    /// <inheritdoc/>
    public Type TargetType { get; }

    /// <summary>
    /// Gets the visible properties on this DTO.
    /// </summary>
    public IReadOnlyList<PropertyInfo> Properties { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectInitializationInfo"/> class.
    /// </summary>
    /// <param name="target">The target type being initialized.</param>
    /// <param name="visibleProperties">The visible properties on this type.</param>
    public ObjectInitializationInfo
    (
        Type target,
        IReadOnlyList<PropertyInfo> visibleProperties
    )
    {
        this.TargetType = target;
        this.Properties = visibleProperties;
    }

    /// <inheritdoc/>
    public ObjectFactory<T> CreateObjectFactory<T>()
    {
        var arguments = Expression.Parameter(typeof(object[]), "arguments");
        var constructor = Expression.New(this.TargetType);

        var bindings = this.Properties
            .Select
            (
                (property, index) => Expression.Bind
                (
                    property,
                    Expression.Convert
                    (
                        Expression.ArrayIndex(arguments, Expression.Constant(index)),
                        property.PropertyType
                    )
                )
            );

        var init = Expression.MemberInit(constructor, bindings);

        return Expression.Lambda<ObjectFactory<T>>(init, arguments).Compile();
    }
}
