//
//  SPDX-FileName: UseMapAnalyzerTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Remora.Rest.Core.Analyzers.Analyzers;
using Remora.Rest.Core.Analyzers.Tests.TestBases;
using Xunit;

namespace Remora.Rest.Core.Analyzers.Tests.Analyzers;

/// <summary>
/// Tests the <see cref="UseMapAnalyzer"/> analyzer.
/// </summary>
public class UseMapAnalyzerTests : OptionalAnalyzerTests<UseMapAnalyzer>
{
    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForTernary()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<long> result = default;

                    result = optional.HasValue ? new(optional.Value) : default;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 71));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags an inverted simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForInvertedTernary()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<long> result = default;

                    result = !optional.HasValue ? default : new(optional.Value);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 72));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForTernaryWithExplicitDefault()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<long> result = default;

                    result = optional.HasValue ? new(optional.Value) : default(Optional<long>);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 87));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags an inverted simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForInvertedTernaryWithExplicitDefault()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<long> result = default;

                    result = !optional.HasValue ? default(Optional<long>) : new(optional.Value);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 88));

        await RunAsync();
    }
}
