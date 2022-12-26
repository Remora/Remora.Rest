//
//  SPDX-FileName: PropertyInfoExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Remora.Rest.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="PropertyInfo"/> class.
/// </summary>
[PublicAPI]
public static class PropertyInfoExtensions
{
    /// <summary>
    /// Enumerates the various nullability possibilities.
    /// </summary>
    private enum Nullability
    {
        Oblivious = 0,
        NotNull = 1,
        Nullable = 2
    }

    /// <summary>
    /// Determines whether the given property allows null as a value.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>true if the property allows null; otherwise, false.</returns>
    public static bool AllowsNull(this PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return true;
        }

        var nullableAttributeType = Type.GetType("System.Runtime.CompilerServices.NullableAttribute");
        if (nullableAttributeType is null)
        {
            // If we don't have access to nullability attributes, assume that we're not in a nullable context.
            return !propertyType.IsValueType;
        }

        // We're in a nullable context, and we can assume that the lack of an attribute means the property is not
        // nullable.
        var nullableAttribute = property.CustomAttributes.FirstOrDefault
        (
            s => s.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute"
        );

        var topLevelNullability = Nullability.Oblivious;

        if (nullableAttribute is not null)
        {
            var nullableArgument = nullableAttribute.ConstructorArguments.Single();
            switch (nullableArgument.Value)
            {
                case byte singleArg:
                {
                    topLevelNullability = (Nullability)singleArg;
                    break;
                }
                case IReadOnlyCollection<CustomAttributeTypedArgument> multiArg:
                {
                    if (multiArg.First().Value is not byte firstArg)
                    {
                        throw new InvalidOperationException();
                    }

                    topLevelNullability = (Nullability)firstArg;
                    break;
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        switch (topLevelNullability)
        {
            case Nullability.Oblivious:
            {
                // Check the context instead
                var nullableContextAttribute = property.DeclaringType?.CustomAttributes.FirstOrDefault
                (
                    s => s.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute"
                );

                if (nullableContextAttribute is null)
                {
                    return !propertyType.IsValueType;
                }

                var nullableArgument = nullableContextAttribute.ConstructorArguments.Single();
                if (nullableArgument.Value is byte singleArg)
                {
                    return singleArg == 2;
                }

                throw new InvalidOperationException();
            }
            case Nullability.NotNull:
            {
                return false;
            }
            case Nullability.Nullable:
            {
                return true;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(topLevelNullability), "Unknown nullability state");
            }
        }
    }
}
