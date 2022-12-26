//
//  SPDX-FileName: SnakeCaseNamingPolicyTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Rest.Json.Policies;
using Xunit;

namespace Remora.Rest.Tests.Json;

/// <summary>
/// Tests the <see cref="SnakeCaseNamingPolicy"/> class.
/// </summary>
public class SnakeCaseNamingPolicyTests
{
    /// <summary>
    /// Tests whether the naming policy converts names correctly for a variety of cases.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [InlineData("", "")]
    [InlineData("A", "a")]
    [InlineData("AB", "ab")]
    [InlineData("ABC", "abc")]
    [InlineData("ABCD", "abcd")]
    [InlineData("IOStream", "io_stream")]
    [InlineData("IOStreamAPI", "io_stream_api")]
    [InlineData("already_snake", "already_snake")]
    [InlineData("SCREAMING_CASE", "screaming_case")]
    [InlineData("Ada_Case", "ada_case")]
    [InlineData("NormalPascalCase", "normal_pascal_case")]
    [InlineData("camelCase", "camel_case")]
    [InlineData("camelCaseAPI", "camel_case_api")]
    [InlineData("IOStreamAPIForReal", "io_stream_api_for_real")]
    [InlineData("OnceUponATime", "once_upon_a_time")]
    public void ConvertsCorrectly(string input, string expected)
    {
        var snakeCase = new SnakeCaseNamingPolicy();

        Assert.Equal(expected, snakeCase.ConvertName(input));
    }
}
