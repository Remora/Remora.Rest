//
//  SPDX-FileName: NullableExtensionsClass.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Defines extension methods for reference types with nullability annotations.
/// </summary>
[PublicAPI]
public static class NullableExtensionsClass
{
    /// <summary>
    /// Converts a <see cref="Nullable{T}"/> to a non-nullable <see cref="Optional{TValue}"/> which is empty if the
    /// input value is <c>null</c>.
    /// </summary>
    /// <param name="nullable">The nullable input value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The <see cref="Optional{TValue}"/>.</returns>
    public static Optional<T> AsOptional<T>(this T? nullable) where T : class
    {
        return nullable is not null
            ? new Optional<T>(nullable)
            : default;
    }
}
