//
//  SPDX-FileName: GeneratorRunner.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Remora.Rest.Generators.Runners;

/// <summary>
/// Explicitly runs one or more source generators against a set of syntax trees.
/// </summary>
internal static class GeneratorRunner
{
    /// <summary>
    /// Runs the given generators against the given syntax trees.
    /// </summary>
    /// <param name="context">The generator context.</param>
    /// <param name="hintNamePrefix">The prefix to use for generated files.</param>
    /// <param name="generators">The generators to run.</param>
    /// <param name="syntaxTrees">The syntax trees to run against.</param>
    public static void Run
    (
        GeneratorExecutionContext context,
        string hintNamePrefix,
        IEnumerable<ISourceGenerator> generators,
        params SyntaxTree[] syntaxTrees
    )
    {
        var argsField = typeof(Diagnostic)
            .GetNestedType("SimpleDiagnostic", BindingFlags.NonPublic)
            .GetField("_messageArgs", BindingFlags.Instance | BindingFlags.NonPublic);

        var compilation = context.Compilation
            .RemoveAllSyntaxTrees()
            .AddSyntaxTrees(syntaxTrees);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators: generators);

        driver = driver.RunGenerators(compilation);
        var runResult = driver.GetRunResult();

        foreach (var diagnostic in runResult.Diagnostics)
        {
            ReportDiagnostic(diagnostic, argsField, context);
        }

        foreach (var generatedSource in runResult.Results.SelectMany(result => result.GeneratedSources))
        {
            context.AddSource(GetHintName(generatedSource.HintName, hintNamePrefix), generatedSource.SourceText);
        }
    }

    private static string GetHintName(string nestedHintName, string hintNamePrefix)
    {
        return hintNamePrefix switch
        {
            _ when hintNamePrefix.EndsWith(".g.cs") => hintNamePrefix[..^".g.cs".Length],
            _ when hintNamePrefix.EndsWith(".cs") => hintNamePrefix[..^".cs".Length],
            _ => hintNamePrefix,
        } + "__" + nestedHintName;
    }

    private static void ReportDiagnostic(Diagnostic diagnostic, FieldInfo? argsField, GeneratorExecutionContext context)
    {
        // There will be an error if we report a diagnostic
        // from a different compilation so we create a new one.
        var newDiagnostic = Diagnostic.Create(
            diagnostic.Descriptor,
            Location.None,
            diagnostic.Severity,
            diagnostic.AdditionalLocations,
            diagnostic.Properties,
            (object?[]?)argsField!.GetValue(diagnostic)
        );

        context.ReportDiagnostic(newDiagnostic);
    }
}
