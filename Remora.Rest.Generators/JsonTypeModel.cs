//
//  SPDX-FileName: JsonTypeModel.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Immutable;

namespace Remora.Rest.Generators;

/// <summary>
/// Contains necessary metadata for the generator to emit code without passing symbols or syntax models
/// through the incremental pipeline, thus breaking incrementality.
/// </summary>
public readonly record struct JsonTypeModel
{
    /// <summary>
    /// Gets initialization strings for all converters necessary.
    /// </summary>
    public ImmutableArray<string> ConverterInitializations { get; init; }

    /// <summary>
    /// Gets the namespace into which we're generating.
    /// </summary>
    public string Namespace { get; init; }

    /// <summary>
    /// Gets the name of the type we're generating for.
    /// </summary>
    public string Type { get; init; }
}
