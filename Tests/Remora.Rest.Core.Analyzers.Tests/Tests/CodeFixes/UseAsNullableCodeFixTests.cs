//
//  SPDX-FileName: UseAsNullableCodeFixTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Remora.Rest.Core.Analyzers.Analyzers;
using Remora.Rest.Core.Analyzers.CodeFixes;
using Remora.Rest.Core.Analyzers.Tests.TestBases;
using Xunit;

namespace Remora.Rest.Core.Analyzers.Tests.CodeFixes;

/// <summary>
/// Tests the <see cref="UseAsNullableCodeFix"/> code fix.
/// </summary>
public class UseAsNullableCodeFixTests : OptionalCodeFixTests<UseAsNullableAnalyzer, UseAsNullableCodeFix>
{
    /// <summary>
    /// Tests whether the code fix replaces the entire conditional with a call to AsNullable in the simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesSimpleConditionalWithAsNullableCall()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    result = optional.AsNullable();
                }
            }
        """;

        await RunAsync();
    }

    /// <summary>
    /// Tests whether the code fix replaces the entire inverted conditional with a call to AsNullable in the simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesInvertedSimpleConditionalWithAsNullableCall()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    result = optional.AsNullable();
                }
            }
        """;

        await RunAsync();
    }

    /// <summary>
    /// Tests whether the code fix replaces the entire nested-access conditional with a call to AsNullable in the simple
    /// case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesNestedAccessConditionalWithAsNullableCall()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    var tuple = (optional, 1);

                    result = tuple.Item1.AsNullable();
                }
            }
        """;

        await RunAsync();
    }

    /// <summary>
    /// Tests whether the code fix replaces the entire nested-access conditional with a call to AsNullable in the simple
    /// case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesInvertedNestedAccessConditionalWithAsNullableCall()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    int? result = default;

                    var tuple = (optional, 1);

                    result = tuple.Item1.AsNullable();
                }
            }
        """;

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesSubObjectAccessTernaryWithAsNullableCallAndConditionalAccess()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<string> optional = default;
                    int? result = default;

                    result = optional.AsNullable()?.Length;
                }
            }
        """;

        await RunAsync();
    }
}
