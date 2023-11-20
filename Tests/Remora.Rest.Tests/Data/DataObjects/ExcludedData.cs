//
//  SPDX-FileName: ExcludedData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

#pragma warning disable SA1402

/// <summary>
/// A data class that serves as an implementation for <see cref="IExcludedData"/>.
/// </summary>
public record ExcludedData(string Serialize, string DoNotSerialize) : IExcludedData;

/// <summary>
/// A data class that serves as an implementation for <see cref="IExcludedData"/>.
/// </summary>
public record ExcludedDataWithDefaultValue(string Serialize, string DoNotSerialize = "value") : IExcludedData;

/// <summary>
/// A data class that serves as an implementation for <see cref="IExcludedData"/>.
/// </summary>
public record ExcludedDataWithReadOnlyMember(string Serialize) : IExcludedData
{
    /// <inheritdoc />
    public string DoNotSerialize => "value";
}
