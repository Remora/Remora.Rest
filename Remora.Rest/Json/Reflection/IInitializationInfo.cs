//
//  SPDX-FileName: IInitializationInfo.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;

namespace Remora.Rest.Json.Reflection;

/// <summary>
/// Represents an abstraction for the information required to initialize a DTO.
/// </summary>
internal interface IInitializationInfo
{
    /// <summary>
    /// Gets the type being initialized.
    /// </summary>
    Type TargetType { get; }

    /// <summary>
    /// Creates a new <seealso cref="ObjectFactory{T}"/> for this type.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    /// <returns>The <seealso cref="ObjectFactory{T}"/> for this type. Results need not be cached, and it
    /// is advised to cache them at callsites.</returns>
    ObjectFactory<T> CreateObjectFactory<T>();
}
