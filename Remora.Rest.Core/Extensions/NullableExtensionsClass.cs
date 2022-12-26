//
//  NullableExtensionsClass.cs
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
