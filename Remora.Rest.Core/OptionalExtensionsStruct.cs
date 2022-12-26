//
//  OptionalExtensionsStruct.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Contains extension methods for <see cref="Optional{TValue}"/>.
/// </summary>
[PublicAPI]
public static class OptionalExtensionsStruct
{
    /// <summary>
    /// Casts the current <see cref="Optional{TValue}"/> to a nullable <typeparamref name="T"/>?.
    /// </summary>
    /// <param name="optional">The optional.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>
    /// An <see cref="Optional{TValue}"/> with the type parameter changed to <typeparamref name="T"/>?.
    /// </returns>
    public static Optional<T?> AsNullableOptional<T>(this Optional<T> optional) where T : struct
    {
        return optional.TryGet(out var value)
            ? value
            : default(Optional<T?>);
    }

    /// <summary>
    /// Converts an <see cref="Optional{TValue}"/> with a null value to an Optional with no value; otherwise, returns
    /// the unmodified <see cref="Optional{TValue}"/>.
    /// </summary>
    /// <param name="optional">The optional.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The <see cref="Optional{TValue}"/>.</returns>
    public static Optional<T> ConvertNullToEmpty<T>(this Optional<T?> optional) where T : struct
    {
        return optional.IsDefined(out var value)
            ? new Optional<T>(value.Value)
            : default;
    }

    /// <summary>
    /// Converts a nullable value to a non-nullable <see cref="Optional{TValue}"/> which is empty if the input value is
    /// <c>null</c>.
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

    /// <summary>
    /// Converts an <see cref="Optional{TValue}"/> to a <see cref="Nullable{T}"/>, which is <c>null</c> if the
    /// <see cref="Optional{TValue}"/> is null or has no value, and non-null otherwise.
    /// </summary>
    /// <param name="optional">The optional input value.</param>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>The nullable value.</returns>
    public static T? AsNullable<T>(this Optional<T> optional) where T : struct
    {
        return optional.TryGet(out var value)
            ? value
            : null;
    }
}
