//
//  SPDX-FileName: ConstructorInitializationInfo.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remora.Rest.Json.Reflection;

/// <summary>
/// Represents the information required to initialize a DTO via its constructor.
/// </summary>
internal class ConstructorInitializationInfo : IInitializationInfo
{
    /// <inheritdoc/>
    public Type TargetType { get; init; }

    /// <summary>
    /// Gets the exact constructor used to create objects.
    /// </summary>
    public ConstructorInfo Constructor { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorInitializationInfo"/> class.
    /// </summary>
    /// <param name="constructor">The constructor used to create objects.</param>
    public ConstructorInitializationInfo
    (
        ConstructorInfo constructor
    )
    {
        if (constructor.DeclaringType is null)
        {
            throw new ArgumentException
            (
                "Constructor does not belong to a type",
                nameof(constructor)
            );
        }

        this.TargetType = constructor.DeclaringType;
        this.Constructor = constructor;
    }

    /// <inheritdoc/>
    public ObjectFactory<T> CreateObjectFactory<T>()
    {
        var arguments = Expression.Parameter(typeof(object[]), "arguments");

        var parameters = this.Constructor.GetParameters()
            .Select((p, i) => Expression.Convert
                (
                    Expression.ArrayIndex(arguments, Expression.Constant(i)),
                    p.ParameterType
                )
            );

        /*
         * (object[] arguments) => new constructor(
         *     (Param0Type) arguments[0],
         *     (Param1Type) arguments[1],
         *     ...
         * );
         */
        return Expression.Lambda<ObjectFactory<T>>
        (
            Expression.New(this.Constructor, parameters),
            arguments
        ).Compile();
    }
}
