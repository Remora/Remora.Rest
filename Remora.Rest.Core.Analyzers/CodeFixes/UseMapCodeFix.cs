//
//  SPDX-FileName: UseMapCodeFix.cs
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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Remora.Rest.Core.Analyzers.CodeFixes;

/// <summary>
/// Provides a code fix for instances of REM1001.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class UseMapCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Descriptors.REM1002UseMap.Id);

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

        ExpressionSyntax optional;
        var isFirstArmTheSuccessCase = true;

        switch (conditional.Condition)
        {
            case PrefixUnaryExpressionSyntax prefixExpression
                when prefixExpression.IsKind(SyntaxKind.LogicalNotExpression):
            {
                if (prefixExpression.Operand is not MemberAccessExpressionSyntax memberAccess)
                {
                    return;
                }

                optional = memberAccess.Expression;
                isFirstArmTheSuccessCase = false;

                break;
            }
            case MemberAccessExpressionSyntax memberAccess:
            {
                optional = memberAccess.Expression;

                break;
            }
            default:
            {
                return;
            }
        }

        var valueArm = isFirstArmTheSuccessCase ? conditional.WhenTrue : conditional.WhenFalse;
        if (valueArm is not BaseObjectCreationExpressionSyntax objectCreation)
        {
            return;
        }

        if (objectCreation.ArgumentList is null)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync();
        if (semanticModel is null)
        {
            return;
        }

        var optionalSymbol = semanticModel.GetSymbolInfo(optional);
        if (optionalSymbol.Symbol is null)
        {
            return;
        }

        var valueExpression = objectCreation.ArgumentList.Arguments[0].Expression;

        var valueExpressionType = semanticModel.GetTypeInfo(valueExpression);
        if (valueExpressionType.ConvertedType is null)
        {
            return;
        }

        var needsExplicitGenericArgument = !SymbolEqualityComparer.IncludeNullability.Equals
        (
            valueExpressionType.Type,
            valueExpressionType.ConvertedType
        );

        // walk the value expression and look for access to optional.Value
        var lambdaParameter = Identifier("x");
        var rewriter = new ValueAccessRewriter(semanticModel, optionalSymbol, lambdaParameter);
        var mapExpression = (ExpressionSyntax)rewriter.Visit(valueExpression);

        if (!rewriter.ChangedAny)
        {
            // use a discard instead of an explicit parameter
            lambdaParameter = Identifier("_");
        }

        var explictTypeSyntax = ParseTypeName
        (
            valueExpressionType.ConvertedType.ToMinimalDisplayString(semanticModel, node.SpanStart)
        );

        SimpleNameSyntax mapMethodName = needsExplicitGenericArgument
            ? GenericName("Map").WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList(explictTypeSyntax)))
            : IdentifierName("Map");

        var mapCall = InvocationExpression
        (
            MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                optional,
                mapMethodName
            )
        ).WithArgumentList
        (
            ArgumentList(SingletonSeparatedList
            (
                Argument
                (
                    SimpleLambdaExpression(Parameter(lambdaParameter))
                    .WithExpressionBody(mapExpression)
                )
            ))
        );

        var fixTitle = $"Replace with \"{mapCall.GetText()}\"";
        context.RegisterCodeFix
        (
            CodeAction.Create
            (
                title: fixTitle,
                createChangedDocument: _ =>
                {
                    var newRoot = root.ReplaceNode(node, mapCall.WithTriviaFrom(node));
                    return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                },
                equivalenceKey: fixTitle
            ),
            context.Diagnostics
        );
    }

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    private class ValueAccessRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _semanticModel;
        private readonly SymbolInfo _optionalSymbol;
        private readonly SyntaxToken _lambdaArgument;

        /// <summary>
        /// Gets a value indicating whether any Value accesses were rewritten.
        /// </summary>
        public bool ChangedAny { get; private set; }

        public ValueAccessRewriter(SemanticModel semanticModel, SymbolInfo optionalSymbol, SyntaxToken lambdaArgument)
        {
            _semanticModel = semanticModel;
            _optionalSymbol = optionalSymbol;
            _lambdaArgument = lambdaArgument;
        }

        public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (_optionalSymbol.Symbol is null)
            {
                throw new InvalidOperationException();
            }

            var nodeSymbolInfo = _semanticModel.GetSymbolInfo(node.Expression);
            if (nodeSymbolInfo.Symbol is null)
            {
                return base.VisitMemberAccessExpression(node);
            }

            if (!SymbolEqualityComparer.Default.Equals(_optionalSymbol.Symbol, nodeSymbolInfo.Symbol))
            {
                return base.VisitMemberAccessExpression(node);
            }

            this.ChangedAny = true;
            return IdentifierName(_lambdaArgument);
        }
    }
}
