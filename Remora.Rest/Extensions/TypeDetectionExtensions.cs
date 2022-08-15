//
//  TypeDetectionExtensions.cs
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

namespace Remora.Rest.Extensions;

/// <summary>
/// Extension methods for <see cref="byte"/> arrays, used to detect file types.
/// </summary>
public static class TypeDetectionExtensions
{
    /// <summary>
    /// Determines whether the array contains PNG data.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>true if the array contains PNG data; otherwise, false.</returns>
    public static bool IsPNG(this byte[] array)
    {
        if (array.Length < 8)
        {
            return false;
        }

        return array[0] == 0x89 &&
               array[1] == 0x50 &&
               array[2] == 0x4E &&
               array[3] == 0x47 &&
               array[4] == 0x0D &&
               array[5] == 0x0A &&
               array[6] == 0x1A &&
               array[7] == 0x0A;
    }

    /// <summary>
    /// Determines whether the array contains GIF data.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>true if the array contains GIF data; otherwise, false.</returns>
    public static bool IsGIF(this byte[] array)
    {
        if (array.Length < 3)
        {
            return false;
        }

        return array[0] == 0x47 && array[1] == 0x49 && array[2] == 0x46;
    }

    /// <summary>
    /// Determines whether the array contains JPG data.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>true if the array contains JPG data; otherwise, false.</returns>
    public static bool IsJPG(this byte[] array)
    {
        if (array.Length < 3)
        {
            return false;
        }

        return array[0] == 0xFF && array[1] == 0xD8 && array[2] == 0xFF;
    }
}
