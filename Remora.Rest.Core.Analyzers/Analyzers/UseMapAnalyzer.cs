//
//  SPDX-FileName: UseMapAnalyzer.cs
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
/// Detects and flags opportunities to use AsNullable instead of the flagged code.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseMapAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptors.REM1002UseMap);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );

        context.RegisterSyntaxNodeAction(action: AnalyzeConditionalExpression, SyntaxKind.ConditionalExpression);
    }

    private void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ConditionalExpressionSyntax conditional)
        {
            return;
        }

        var isFirstArmTheSuccessCase = true;
        MemberAccessExpressionSyntax conditionAccess;

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
                conditionAccess = innerAccess;
                break;
            }
            case MemberAccessExpressionSyntax innerAccess:
            {
                conditionAccess = innerAccess;
                break;
            }
            default:
            {
                return;
            }
        }

        // check type & property of the condition
        var semanticModel = context.SemanticModel;
        var conditionalTypeInfo = semanticModel.GetTypeInfo(conditionAccess.Expression);
        if (conditionalTypeInfo.Type?.Name is not "Optional")
        {
            return;
        }

        if (conditionAccess.Name.Identifier.Text != nameof(Optional<int>.HasValue))
        {
            return;
        }

        var valueArm = isFirstArmTheSuccessCase ? conditional.WhenTrue : conditional.WhenFalse;
        if (valueArm is not BaseObjectCreationExpressionSyntax objectCreation)
        {
            return;
        }

        var valueTypeInfo = semanticModel.GetTypeInfo(objectCreation);
        if (valueTypeInfo.Type?.Name is not "Optional")
        {
            return;
        }

        var noValueArm = isFirstArmTheSuccessCase ? conditional.WhenFalse : conditional.WhenTrue;
        switch (noValueArm)
        {
            case LiteralExpressionSyntax literalExpression:
            {
                if (!literalExpression.IsKind(SyntaxKind.DefaultLiteralExpression))
                {
                    return;
                }

                break;
            }
            case DefaultExpressionSyntax:
            {
                break;
            }
            default:
            {
                return;
            }
        }

        // Bad!
        context.ReportDiagnostic
        (
            Diagnostic.Create
            (
                descriptor: Descriptors.REM1002UseMap,
                conditional.GetLocation()
            )
        );
    }
}
