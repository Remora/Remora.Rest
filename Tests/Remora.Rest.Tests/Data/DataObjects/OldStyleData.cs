//
//  SPDX-FileName: OldStyleData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// An old style data object.
/// </summary>
public sealed class OldStyleData : IOldStyleData
{
    /// <inheritdoc/>
    public required string Value { get; init; }

    /// <inheritdoc/>
    public required int OtherValue { get; init; }
}
