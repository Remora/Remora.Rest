//
//  SPDX-FileName: ReadOnlyData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data record with a read-only data member.
/// </summary>
/// <param name="Value">An arbitrary string.</param>
public record ReadOnlyData(string Value) : IReadOnlyData
{
    /// <inheritdoc/>
    public int ReadOnly => 1;
}
