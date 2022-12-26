//
//  SPDX-FileName: DataObjectConverterHelpers.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Json.Internal;

/// <summary>
/// Shared, non-generic code for <see cref="DataObjectConverter{TInterface, TImplementation}"/>.
/// </summary>
internal static class DataObjectConverterHelpers
{
    /// <summary>
    /// Type of <see cref="Missing"/>.
    /// </summary>
    private sealed class MissingValue
    {
    }

    /// <summary>
    /// Sentinel value for use by <see cref="BoundDataObjectConverter{T}"/>.
    /// It indicates a specific property has not been read from JSON yet.
    /// </summary>
    public static readonly object Missing = new MissingValue();
}
