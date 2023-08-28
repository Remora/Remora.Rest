//
//  SPDX-FileName: ContextCreationCallSyntaxReceiver.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remora.Rest.Generators.Data;

namespace Remora.Rest.Generators;

/// <summary>
/// Collects information about source generation-enabled calls.
/// </summary>
internal class ContextCreationCallSyntaxReceiver : ISyntaxReceiver
{
    private readonly Dictionary<string, List<SyntaxNode>> _duplicateContextCreations = new();
    private readonly List<ContextConfiguration> _contexts = new();
    private readonly Dictionary<ContextConfiguration, List<SyntaxNode>> _duplicateDataObjectRegistrations = new();

    /// <summary>
    /// Gets any locations in the source code where a context is created more than once.
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyList<SyntaxNode>> DuplicateContextCreations
        => _duplicateContextCreations.ToDictionary(c => c.Key, c => c.Value as IReadOnlyList<SyntaxNode>);

    /// <summary>
    /// Gets any locations in the source code where a data object is registered more than once.
    /// </summary>
    public IReadOnlyDictionary<ContextConfiguration, IReadOnlyList<SyntaxNode>> DuplicateDataObjectRegistrations
        => _duplicateDataObjectRegistrations.ToDictionary(c => c.Key, c => c.Value as IReadOnlyList<SyntaxNode>);

    /// <summary>
    /// Gets the discovered context configurations.
    /// </summary>
    public IReadOnlyList<ContextConfiguration> Contexts => _contexts;

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

        if (gns.Identifier.Text is not "CreateGeneratedJsonContext")
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

        var contextName = ins.Identifier.Text;
        if (_contexts.Any(c => c.Name == contextName))
        {
            if (!_duplicateContextCreations.TryGetValue(contextName, out var nodes))
            {
                nodes = new();
                _duplicateContextCreations.Add(contextName, nodes);
            }

            nodes.Add(syntaxNode);
            return;
        }

        var contextConfiguration = CreateContextConfiguration(contextName, ies);

        _contexts.Add(contextConfiguration);
    }

    private ContextConfiguration CreateContextConfiguration(string contextName, InvocationExpressionSyntax ies)
    {
        var dataObjects = new Dictionary<TypeSyntax, TypeSyntax>();
        var contextConfiguration = new ContextConfiguration(contextName, ies, dataObjects);

        var parent = ies.Parent;
        while (parent is not null && parent is not MemberAccessExpressionSyntax { Name.Identifier.Text: "Finish" })
        {
            // collect known calls
            switch (parent)
            {
                case InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name: GenericNameSyntax
                        {
                            Identifier.Text: "WithDataObject",
                            TypeArgumentList.Arguments:
                            [
                                { } interfaceType,
                                { } implementationType
                            ]
                        }
                    }
                }:
                {
                    if (dataObjects.ContainsKey(interfaceType))
                    {
                        if (!_duplicateDataObjectRegistrations.TryGetValue(contextConfiguration, out var dupes))
                        {
                            dupes = new();
                            _duplicateDataObjectRegistrations.Add(contextConfiguration, dupes);
                        }

                        dupes.Add(parent);
                    }

                    dataObjects[interfaceType] = implementationType;
                    break;
                }
            }

            parent = parent.Parent;
        }

        return contextConfiguration;
    }
}
