//
//  SPDX-FileName: UseAsNullableCodeFix.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Rest.Core.Analyzers.CodeFixes;

/// <summary>
/// Provides a code fix for instances of REM1001.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class UseAsNullableCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Descriptors.REM1001UseAsNullable.Id);

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync();
        if (root is null)
        {
            return;
        }

        var node = root.FindNode(context.Span);

        if (node is not ConditionalExpressionSyntax conditional)
        {
            return;
        }

        var isFirstArmTheSuccessCase = true;
        ExpressionSyntax optional;

        switch (conditional.Condition)
        {
            case PrefixUnaryExpressionSyntax prefixExpression
                when prefixExpression.IsKind(SyntaxKind.LogicalNotExpression):
            {
                if (prefixExpression.Operand is not MemberAccessExpressionSyntax innerAccess)
                {
                    return;
                }

                isFirstArmTheSuccessCase = false;
                optional = innerAccess.Expression;
                break;
            }
            case MemberAccessExpressionSyntax innerAccess:
            {
                optional = innerAccess.Expression;
                break;
            }
            default:
            {
                return;
            }
        }

        ExpressionSyntax replacement = SyntaxFactory.InvocationExpression
        (
            SyntaxFactory.MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                optional,
                SyntaxFactory.IdentifierName("AsNullable")
            )
        );

        var valueArm = isFirstArmTheSuccessCase ? conditional.WhenTrue : conditional.WhenFalse;
        if (valueArm is MemberAccessExpressionSyntax memberAccess && !memberAccess.Expression.IsEquivalentTo(optional))
        {
            replacement = SyntaxFactory.ConditionalAccessExpression
            (
                replacement,
                SyntaxFactory.MemberBindingExpression(memberAccess.Name)
            );
        }

        var fixTitle = $"Replace with \"{replacement.GetText()}\"";
        context.RegisterCodeFix
        (
            CodeAction.Create
            (
                title: fixTitle,
                createChangedDocument: _ =>
                {
                    var newRoot = root.ReplaceNode(node, replacement.WithTriviaFrom(node));
                    return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                },
                equivalenceKey: fixTitle
            ),
            context.Diagnostics
        );
    }

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
}
