//
//  SPDX-FileName: Descriptors.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Microsoft.CodeAnalysis;

namespace Remora.Rest.Core.Analyzers;

/// <summary>
/// Defines analyzer descriptors for various analyzers.
/// </summary>
internal static class Descriptors
{
    /// <summary>
    /// Holds the descriptor for the opportunity to use AsNullable.
    /// </summary>
    internal static readonly DiagnosticDescriptor REM1001UseAsNullable = new
    (
        id: "REM1001",
        title: "Use AsNullable",
        messageFormat: "Use AsNullable instead of manually accessing the optional",
        category: DiagnosticCategories.APIUsageOpportunities,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Holds the descriptor for the opportunity to use Map.
    /// </summary>
    internal static readonly DiagnosticDescriptor REM1002UseMap = new
    (
        id: "REM1002",
        title: "Use Map",
        messageFormat: "Use Map instead of manually constructing the optional",
        category: DiagnosticCategories.APIUsageOpportunities,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Holds the descriptor for redundant use of IsDefined.
    /// </summary>
    internal static readonly DiagnosticDescriptor REM1003UseTryGet = new
    (
        id: "REM1003",
        title: "Use TryGet",
        messageFormat: "IsDefined behaves identically to TryGet when the optional's value is not nullable",
        category: DiagnosticCategories.APIUsageOpportunities,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
}
