//
//  SPDX-FileName: ContextConfiguration.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Rest.Generators.Data;

/// <summary>
/// Represents information gathered about a JSON serialization context that should be generated.
/// </summary>
/// <param name="Name">The name of the context.</param>
/// <param name="Call">The call that starts the context's definition.</param>
/// <param name="DataObjects">The  interface/implementation pairs that are associated with the context.</param>
public record ContextConfiguration
(
    string Name,
    InvocationExpressionSyntax Call,
    IReadOnlyDictionary<TypeSyntax, TypeSyntax> DataObjects
);
