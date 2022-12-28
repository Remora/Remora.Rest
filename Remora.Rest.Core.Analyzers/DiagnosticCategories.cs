//
//  SPDX-FileName: DiagnosticCategories.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Core.Analyzers;

/// <summary>
/// Provides uniform access to Roslyn analyzer diagnostic categories.
/// </summary>
internal static class DiagnosticCategories
{
    /// <summary>
    /// Gets the category string for analyzers that flag redundant code.
    /// </summary>
    internal static string Redundancies => "Redundancies in Code";

    /// <summary>
    /// Gets the category string for analyzers that flag API usage opportunities.
    /// </summary>
    internal static string APIUsageOpportunities => "API Usage Opportunities";
}
