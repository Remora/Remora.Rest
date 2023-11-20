//
//  SPDX-FileName: NonConventionalData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

#pragma warning disable SA1402

/// <summary>
/// A data class that serves as an implementation for <see cref="INonConventionalData"/>.
/// </summary>
public record NonConventionalData(string Value) : INonConventionalData;

/// <summary>
/// A data class that unconventionally alters the type of the interface's property.
/// </summary>
/// <param name="Value">Gets an arbitrary string.</param>
public record NonConventionalData2(string Value) : INonConventionalData
{
    /// <inheritdoc/>
    string INonConventionalData.Value { get; } = "this should be serialized";
}
