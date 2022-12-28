//
//  SPDX-FileName: UseTryGetAnalyzer.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remora.Rest.Core.Analyzers.Analyzers;

/// <summary>
/// Detects and flags opportunities to use TryGet instead of the flagged code.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseTryGetAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptors.REM1003UseTryGet);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );

        context.RegisterSyntaxNodeAction(action: AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var sourceType = TryGetSourceType(invocation.Expression, context.SemanticModel);
        if (sourceType is null)
        {
            return;
        }

        if (sourceType.Name is not "Optional")
        {
            return;
        }

        if (sourceType is not INamedTypeSymbol namedSourceType)
        {
            return;
        }

        var firstArgument = namedSourceType.TypeArguments.FirstOrDefault();
        if (firstArgument is null)
        {
            return;
        }

        if (firstArgument.NullableAnnotation is NullableAnnotation.Annotated)
        {
            // this is nullable, we don't care
            return;
        }

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        if (invocation.ArgumentList.Arguments.Count is not 1)
        {
            return;
        }

        if (memberAccess.Name.Identifier.Text is not "IsDefined")
        {
            return;
        }

        // Bad!
        context.ReportDiagnostic
        (
            Diagnostic.Create
            (
                descriptor: Descriptors.REM1003UseTryGet,
                invocation.GetLocation()
            )
        );
    }

    private static ITypeSymbol? TryGetSourceType(ExpressionSyntax invocationExpression, SemanticModel semanticModel)
    {
        switch (invocationExpression)
        {
            case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
            {
                var symbol = semanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Expression).Symbol;
                return symbol switch
                {
                    ILocalSymbol local => local.Type,
                    IParameterSymbol param => param.Type,
                    IFieldSymbol field => field.Type,
                    IPropertySymbol prop => prop.Type,
                    IMethodSymbol method => method.MethodKind == MethodKind.Constructor
                        ? method.ReceiverType
                        : method.ReturnType,
                    _ => null
                };
            }
            default:
            {
                return null;
            }
        }
    }
}
