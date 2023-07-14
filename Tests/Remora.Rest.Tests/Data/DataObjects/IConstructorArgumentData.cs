//
//  SPDX-FileName: IConstructorArgumentData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Rest.Core;

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface with various permutations of data that can be assigned in the type's constructor.
/// </summary>
public interface IConstructorArgumentData
{
    /// <summary>
    /// Gets a value type.
    /// </summary>
    int ValueType { get; }

    /// <summary>
    /// Gets a nullable value type.
    /// </summary>
    int? NullableValueType { get; }

    /// <summary>
    /// Gets an optional value type.
    /// </summary>
    Optional<int> OptionalValueType { get; }

    /// <summary>
    /// Gets an optional nullable value type.
    /// </summary>
    Optional<int?> OptionalNullableValueType { get; }

    /// <summary>
    /// Gets a reference type.
    /// </summary>
    string ReferenceType { get; }

    /// <summary>
    /// Gets a nullable reference type.
    /// </summary>
    string? NullableReferenceType { get; }

    /// <summary>
    /// Gets an optional reference type.
    /// </summary>
    Optional<string> OptionalReferenceType { get; }

    /// <summary>
    /// Gets an optional nullable reference type.
    /// </summary>
    Optional<string?> OptionalNullableReferenceType { get; }
}
