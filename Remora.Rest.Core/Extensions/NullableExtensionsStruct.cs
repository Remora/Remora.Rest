//
//  SPDX-FileName: NullableExtensionsStruct.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Defines extension methods for the <see cref="Nullable{T}"/> struct.
/// </summary>
[PublicAPI]
public static class NullableExtensionsStruct
{
    /// <summary>
    /// Converts a <see cref="Nullable{T}"/> to a non-nullable <see cref="Optional{TValue}"/> which is empty if the
    /// input value is <c>null</c>.
    /// </summary>
    /// <param name="nullable">The nullable input value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The <see cref="Optional{TValue}"/>.</returns>
    public static Optional<T> AsOptional<T>(this T? nullable) where T : struct
    {
        return nullable is { } value
            ? new Optional<T>(value)
            : default;
    }
}
