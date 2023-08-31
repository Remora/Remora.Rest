//
//  SPDX-FileName: ContextAssociationCallSyntaxReceiver.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Rest.Generators;

/// <summary>
/// Collects information about source generation-enabled calls.
/// </summary>
internal class ContextAssociationCallSyntaxReceiver : ISyntaxReceiver
{
    private readonly List<IdentifierNameSyntax> _contextNames = new();

    /// <summary>
    /// Gets the discovered context configurations.
    /// </summary>
    public IReadOnlyList<IdentifierNameSyntax> ContextNames => _contextNames;

    /// <inheritdoc/>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not InvocationExpressionSyntax ies)
        {
            return;
        }

        if (ies.Expression is not MemberAccessExpressionSyntax maes)
        {
            return;
        }

        if (maes.Name is not GenericNameSyntax gns)
        {
            return;
        }

        if (gns.Identifier.Text is not "WithContext")
        {
            return;
        }

        if (gns.TypeArgumentList.Arguments.Count is not 1)
        {
            return;
        }

        if (gns.TypeArgumentList.Arguments[0] is not IdentifierNameSyntax ins)
        {
            return;
        }

        if (_contextNames.Any(c => c.Identifier.Equals(ins.Identifier)))
        {
            return;
        }

        _contextNames.Add(ins);
    }
}
