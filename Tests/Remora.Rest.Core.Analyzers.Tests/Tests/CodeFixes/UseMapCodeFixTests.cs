//
//  SPDX-FileName: UseMapCodeFixTests.cs
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
/// Tests the <see cref="UseMapCodeFix"/> code fix.
/// </summary>
public class UseMapCodeFixTests : OptionalCodeFixTests<UseMapAnalyzer, UseMapCodeFix>
{
    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesTernaryWithMap()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.HasValue ? new(optional.Value.ToString()) : default;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 82));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.Map(x => x.ToString());
                }
            }
        """;

        this.CompilerDiagnostics = CompilerDiagnostics.None;

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags an inverted simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesInvertedTernaryWithMap()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = !optional.HasValue ? default : new(optional.Value.ToString());
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 83));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.Map(x => x.ToString());
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
    public async Task ReplacesTernaryWithExplicitDefaultWithMap()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.HasValue ? new(optional.Value.ToString()) : default(Optional<string>);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 100));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.Map(x => x.ToString());
                }
            }
        """;

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags an inverted simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesInvertedTernaryWithExplicitDefaultWithMap()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = !optional.HasValue ? default(Optional<string>) : new(optional.Value.ToString());
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 101));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.Map(x => x.ToString());
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
    public async Task ReplacesTernaryWithMapWithExplicitTypeArgumentWhenConvertedTypeDiffers()
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

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<long> result = default;

                    result = optional.Map<long>(x => x);
                }
            }
        """;

        this.CompilerDiagnostics = CompilerDiagnostics.None;

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesTernaryWithMapWithMultipleUsesOfValue()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<(long, long)> result = default;

                    result = optional.HasValue ? new((optional.Value, optional.Value)) : default;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 89));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<(long, long)> result = default;

                    result = optional.Map<(long, long)>(x => (x, x));
                }
            }
        """;

        this.CompilerDiagnostics = CompilerDiagnostics.None;

        await RunAsync();
    }

    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplacesTernaryWithMapWithDiscardWithNoUsesOfValue()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.HasValue ? new("value") : default;
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1002").WithSpan(10, 22, 10, 64));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;
                    Optional<string> result = default;

                    result = optional.Map(_ => "value");
                }
            }
        """;

        this.CompilerDiagnostics = CompilerDiagnostics.None;

        await RunAsync();
    }
}
