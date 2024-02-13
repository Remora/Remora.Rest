//
//  SPDX-FileName: OptionalAnalyzerTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.IO;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Remora.Rest.Core.Analyzers.Tests.TestBases;

/// <summary>
/// Serves as a base class for Optional-targeted analyzer tests.
/// </summary>
/// <typeparam name="TAnalyzer">The analyzer under test.</typeparam>
public abstract class OptionalAnalyzerTests<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalAnalyzerTests{TAnalyzer}"/> class.
    /// </summary>
    protected OptionalAnalyzerTests()
    {
        this.ReferenceAssemblies = new ReferenceAssemblies
        (
            "net8.0",
            new PackageIdentity("Microsoft.NETCore.App.Ref", "8.0.1"),
            Path.Combine("ref", "net8.0")
        );

        this.TestState.AdditionalReferences.Add(typeof(Optional<>).Assembly);
    }
}
