//
//  SPDX-FileName: IOptional.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Defines basic functionality for an optional.
/// </summary>
[PublicAPI]
public interface IOptional
{
    /// <summary>
    /// Gets a value indicating whether the optional contains a value.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    /// Determines whether the option has a defined value; that is, whether it both has a value and that value is
    /// non-null.
    /// </summary>
    /// <returns>true if the optional has a value and that value is non-null; otherwise, false.</returns>
    bool IsDefined();
}
