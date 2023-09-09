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
    required public string Value { get; init; }

    /// <inheritdoc/>
    required public int OtherValue { get; init; }
}
