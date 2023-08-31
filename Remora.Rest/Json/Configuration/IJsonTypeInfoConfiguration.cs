//
//  SPDX-FileName: IJsonTypeInfoConfiguration.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json.Serialization.Metadata;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Defines the public interface for untyped interaction with a <see cref="JsonTypeInfoConfiguration{TType}"/>.
/// </summary>
public interface IJsonTypeInfoConfiguration
{
    /// <summary>
    /// Gets the type that the configuration applies to.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Configures the given <see cref="JsonTypeInfo"/> instance based on the contained configuration.
    /// </summary>
    /// <param name="typeInfo">The type information to configure.</param>
    void Configure(JsonTypeInfo typeInfo);
}
