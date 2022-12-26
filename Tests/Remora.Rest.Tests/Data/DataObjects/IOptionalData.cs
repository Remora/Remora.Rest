//
//  SPDX-FileName: IOptionalData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Rest.Core;

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface that contains an optional members.
/// </summary>
public interface IOptionalData
{
    /// <summary>
    /// Gets an optional string.
    /// </summary>
    Optional<string> Value { get; }
}
