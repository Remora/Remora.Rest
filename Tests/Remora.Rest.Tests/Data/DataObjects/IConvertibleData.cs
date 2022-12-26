//
//  SPDX-FileName: IConvertibleData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface that contains a member that needs a custom converter.
/// </summary>
public interface IConvertibleData
{
    /// <summary>
    /// Gets the enumeration value.
    /// </summary>
    StringifiedEnum Value { get; }
}
