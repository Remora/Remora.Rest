//
//  SPDX-FileName: UseAsNullableAnalyzerTests.cs
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
/// Tests the <see cref="UseAsNullableAnalyzer"/> analyzer.
/// </summary>
public class UseAsNullableAnalyzerTests : OptionalAnalyzerTests<UseAsNullableAnalyzer>
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
                    int? result = default;

                    result = optional.HasValue ? optional.Value : null;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1001").WithSpan(10, 22, 10, 63));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
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
                    int? result = default;

                    result = !optional.HasValue ? null : optional.Value;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1001").WithSpan(10, 22, 10, 64));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a case where the optional is inside another object.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForNestedAccessTernary()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    var tuple = (optional, 1);

                    result = tuple.Item1.HasValue ? tuple.Item1.Value : null;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1001").WithSpan(12, 22, 12, 69));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a case where the inverted optional is inside another object.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForInvertedNestedAccessTernary()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    var tuple = (optional, 1);

                    result = !tuple.Item1.HasValue ? null : tuple.Item1.Value;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1001").WithSpan(12, 22, 12, 70));

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task RaisesWarningForSubObjectAccessTernary()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<string> optional = default;
                    int? result = default;

                    result = optional.HasValue ? optional.Value.Length : null;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1001").WithSpan(10, 22, 10, 70));

        await RunAsync();
    }
}
