//
//  SPDX-FileName: IExcludedData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface with an excluded member.
/// </summary>
public interface IExcludedData
{
    /// <summary>
    /// Gets a value that should be serialized.
    /// </summary>
    string Serialize { get; }

    /// <summary>
    /// Gets a value that should not be serialized.
    /// </summary>
    string DoNotSerialize { get; }
}
