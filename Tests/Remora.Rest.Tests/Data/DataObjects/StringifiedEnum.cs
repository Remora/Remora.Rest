//
//  SPDX-FileName: StringifiedEnum.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// An enum that is stringified when (de)serializing.
/// </summary>
public enum StringifiedEnum
{
    /// <summary>
    /// The first value.
    /// </summary>
    First,

    /// <summary>
    /// The second value.
    /// </summary>
    Second,

    /// <summary>
    /// The third value.
    /// </summary>
    Third
}
