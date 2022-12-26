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

using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Contains extension methods for <see cref="Optional{TValue}"/> where the contained value is a value type.
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
}
