//
//  SPDX-FileName: ContextConfiguration.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Rest.Generators.Data;

public record ContextConfiguration
(
    string Name,
    InvocationExpressionSyntax Call,
    IReadOnlyDictionary<TypeSyntax, TypeSyntax> DataObjects
);
