//
//  ExpressionFactoryUtilities.cs
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remora.Rest.Json.Reflection;

/// <summary>
/// Represents a typeless factory that invokes the parameterless constructor of an unspecified type and returns the new
/// instance cast to <c>object</c>.
/// </summary>
/// <returns>The newly created object.</returns>
internal delegate object ParameterlessObjectFactory();

/// <summary>
/// Represents a factory that invokes a predefined constructor of type <typeparamref name="T"/> or a subclass thereof,
/// without requiring a strongly-typed invocation.
/// </summary>
/// <param name="args">The positional arguments to pass to the constructor.</param>
/// <typeparam name="T">The type of the object to instantiate.</typeparam>
/// <returns>The newly created object.</returns>
internal delegate T ObjectFactory<out T>(params object?[] args);

/// <summary>
/// Represents a typeless function that retrieves a predefined property from a predefined type using the instance given
/// in <paramref name="instance"/> and returns it cast to <c>object</c>.
/// </summary>
/// <param name="instance">The instance whose property to retrieve.</param>
/// <returns>The retrieved property.</returns>
internal delegate object InstancePropertyGetter(object instance);

/// <summary>
/// Handles application-specific creation of delegates for performing reflective operations using Linq Expressions as a
/// substitute for traditional reflection APIs using runtime-compiled .NET IL.
/// </summary>
internal static class ExpressionFactoryUtilities
{
    /// <summary>
    /// Creates an <see cref="ObjectFactory{T}"/> for the given type <typeparamref name="T"/> using the constructor
    /// <paramref name="constructor"/>.
    /// </summary>
    /// <param name="constructor">The constructor to be used to create the instance.</param>
    /// <typeparam name="T">The type of the instance to create.</typeparam>
    /// <returns>A factory that can be used to create instances of the specified type.</returns>
    /// <exception cref="ArgumentException">
    /// If the type <typeparamref name="T"/> is not assignable to the declaring type of the constructor, or the
    /// constructor does not belong to a type.
    /// </exception>
    public static ObjectFactory<T> CreateFactory<T>(ConstructorInfo constructor)
    {
        if (constructor.DeclaringType == null)
        {
            throw new ArgumentException
            (
                $"Constructor does not belong to a type",
                nameof(constructor)
            );
        }

        if (constructor.DeclaringType.IsAssignableFrom(typeof(T)))
        {
            throw new ArgumentException
            (
                $"Constructor does not belong to a type corresponding to {nameof(T)}",
                nameof(constructor)
            );
        }

        var arguments = Expression.Parameter(typeof(object[]), "arguments");

        var parameters = constructor.GetParameters()
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
            Expression.New(constructor, parameters),
            arguments
        ).Compile();
    }

    /// <summary>
    /// Creates a <see cref="ParameterlessObjectFactory"/> for the given type <paramref name="type"/>, using the type's
    /// public parameterless constructor.
    /// </summary>
    /// <param name="type">The type whose instances will be created by the factory.</param>
    /// <returns>A factory that can be used to create instances of the specified type.</returns>
    /// <exception cref="ArgumentException">
    /// If the type <paramref name="type"/> does not have a public parameterless constructor.
    /// </exception>
    public static ParameterlessObjectFactory CreateFactoryParameterless(Type type)
    {
        var constructor = type.GetConstructor(Array.Empty<Type>()) ?? throw new ArgumentException
        (
            "Type did not have a public parameterless constructor",
            nameof(type)
        );

        /*
         * () => new constructor();
         */
        return Expression.Lambda<ParameterlessObjectFactory>(Expression.New(constructor)).Compile();
    }

    /// <summary>
    /// Creates a <see cref="InstancePropertyGetter"/> that retrieves the property <paramref name="property"/> in the
    /// type <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type the factory will convert the instance to when retrieving the property.</param>
    /// <param name="property">The property whose value will be retrieved by the factory.</param>
    /// <returns>A factory that can be used to retrieve the specified property from a given instance.</returns>
    /// <remarks>
    /// We allow for manually specifying the <paramref name="type"/> instead of simply using
    /// <see cref="MemberInfo.DeclaringType"/> because the instance is converted to that type specifically when
    /// retrieving the property, and this doesn't necessarily have to align with the declaring type of the property, e.g
    /// for edge cases such as explicit interface implementations.
    /// </remarks>
    public static InstancePropertyGetter CreatePropertyGetter(Type type, PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");

        /*
         * (instance) => (object) ((InstanceType)instance).property
         */
        return Expression.Lambda<InstancePropertyGetter>
        (
            Expression.Convert(Expression.Property(Expression.Convert(instance, type), property), typeof(object)),
            instance
        ).Compile();
    }
}
