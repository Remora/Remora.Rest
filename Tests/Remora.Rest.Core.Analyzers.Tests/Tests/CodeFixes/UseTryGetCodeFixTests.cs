//
//  SPDX-FileName: UseTryGetCodeFixTests.cs
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
/// Tests the <see cref="UseTryGetCodeFix"/> code fix.
/// </summary>
public class UseTryGetCodeFixTests : OptionalCodeFixTests<UseTryGetAnalyzer, UseTryGetCodeFix>
{
    /// <summary>
    /// Tests that the analyzer flags a simple case.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task ReplaceIsDefinedWithTryGetForNonNullableStruct()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;

                    _ = optional.IsDefined(out _);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1003").WithSpan(9, 17, 9, 42));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;

                    _ = optional.TryGet(out _);
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
    public async Task ReplaceIsDefinedWithTryGetForNonNullableClass()
    {
        this.TestCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;

                    _ = optional.IsDefined(out _);
                }
            }
        """;

        this.ExpectedDiagnostics.Clear();
        this.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning("REM1003").WithSpan(9, 17, 9, 42));

        this.FixedCode = """
            using Remora.Rest.Core;

            public static class Test
            {
                public static void Method()
                {
                    Optional<int> optional = default;

                    _ = optional.TryGet(out _);
                }
            }
        """;

        await RunAsync();
    }
}
