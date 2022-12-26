//
//  SPDX-FileName: IReadOnlyData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface with a read-only data member.
/// </summary>
public interface IReadOnlyData
{
    /// <summary>
    /// Gets an arbitrary string.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets a read-only value.
    /// </summary>
    int ReadOnly { get; }
}
