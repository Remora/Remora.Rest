//
//  SPDX-FileName: SameTypeEqualityComparer.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Determines equality between two objects based on whether they are the same type.
/// </summary>
/// <typeparam name="T">The base type shared by the objects.</typeparam>
public class SameTypeEqualityComparer<T> : IEqualityComparer<T> where T : notnull
{
    /// <inheritdoc/>
    public bool Equals(T x, T y) => x.GetType() == y.GetType();

    /// <inheritdoc/>
    public int GetHashCode(T obj) => obj.GetType().GetHashCode();
}
