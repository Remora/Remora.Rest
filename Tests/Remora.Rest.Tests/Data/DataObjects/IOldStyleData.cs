//
//  SPDX-FileName: IOldStyleData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface for implementation by an old-style, pre-primary-constructor type.
/// </summary>
internal interface IOldStyleData
{
    /// <summary>
    /// Gets an inconsequential value.
    /// </summary>
    string Value { get; }

    /// <summary>
    /// Gets an equally inconsequential other value.
    /// </summary>
    int OtherValue { get; }
}
