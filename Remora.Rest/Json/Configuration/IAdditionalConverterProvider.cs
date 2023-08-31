//
//  SPDX-FileName: IAdditionalConverterProvider.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Remora.Rest.Json.Configuration;

/// <summary>
/// Represents a type that provides additional JSON converters in de/serialization scenarios.
/// </summary>
/// <remarks>
/// This interface is generally only used in a source-generated context.
/// </remarks>
public interface IAdditionalConverterProvider
{
    /// <summary>
    /// Gets the additional converters.
    /// </summary>
    IReadOnlyList<JsonConverter> AdditionalConverters { get; }
}
