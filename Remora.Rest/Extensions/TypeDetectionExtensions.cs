//
//  SPDX-FileName: TypeDetectionExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using JetBrains.Annotations;

namespace Remora.Rest.Extensions;

/// <summary>
/// Extension methods for <see cref="byte"/> arrays, used to detect file types.
/// </summary>
[PublicAPI]
public static class TypeDetectionExtensions
{
    /// <summary>
    /// Determines whether the array contains PNG data.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <returns>true if the array contains PNG data; otherwise, false.</returns>
    public static bool IsPNG(this byte[] array) => IsPNG(array.AsSpan());

    /// <summary>
    /// Determines whether the span is a view over PNG data.
    /// </summary>
    /// <param name="array">The span.</param>
    /// <returns>true if the span contains PNG data; otherwise, false.</returns>
    public static bool IsPNG(this ReadOnlySpan<byte> array)
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
    public static bool IsGIF(this byte[] array) => IsGIF(array.AsSpan());

    /// <summary>
    /// Determines whether the span is a view over GIF data.
    /// </summary>
    /// <param name="array">The span.</param>
    /// <returns>true if the span contains GIF data; otherwise, false.</returns>
    public static bool IsGIF(this ReadOnlySpan<byte> array)
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
    public static bool IsJPG(this byte[] array) => IsJPG(array.AsSpan());

    /// <summary>
    /// Determines whether the span is a view over JPG data.
    /// </summary>
    /// <param name="array">The span.</param>
    /// <returns>true if the span contains JPG data; otherwise, false.</returns>
    public static bool IsJPG(this ReadOnlySpan<byte> array)
    {
        if (array.Length < 3)
        {
            return false;
        }

        return array[0] == 0xFF && array[1] == 0xD8 && array[2] == 0xFF;
    }
}
