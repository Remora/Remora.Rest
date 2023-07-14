//
//  SPDX-FileName: ConstructorArgumentData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Rest.Core;

namespace Remora.Rest.Tests.Data.DataObjects;

/// <inheritdoc />
public record ConstructorArgumentData
(
    int ValueType = 0,
    int? NullableValueType = null,
    Optional<int> OptionalValueType = default,
    Optional<int?> OptionalNullableValueType = default,
    string ReferenceType = "",
    string? NullableReferenceType = null,
    Optional<string> OptionalReferenceType = default,
    Optional<string?> OptionalNullableReferenceType = default
) : IConstructorArgumentData;
