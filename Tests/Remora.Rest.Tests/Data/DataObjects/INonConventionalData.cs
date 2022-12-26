//
//  SPDX-FileName: INonConventionalData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface that does not contain any complex members.
/// </summary>
public interface INonConventionalData
{
    /// <summary>
    /// Gets an arbitrary string.
    /// </summary>
    string Value { get; }
}
