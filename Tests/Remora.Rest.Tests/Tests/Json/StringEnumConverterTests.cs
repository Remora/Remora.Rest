//
//  SPDX-FileName: StringEnumConverterTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Rest.Json;
using Remora.Rest.Json.Policies;
using Remora.Rest.Tests.Data.DataObjects;
using Xunit;

namespace Remora.Rest.Tests.Json;

/// <summary>
/// Tests for <see cref="StringEnumConverter{TEnum}"/>.
/// </summary>
public class StringEnumConverterTests
{
    /// <summary>
    /// Tests that the converter can serialize a dictionary where the key is an enum.
    /// </summary>
    [Fact]
    public void CanSerializeDictionaryKeyAsInteger()
    {
        // Arrange
        var services = new ServiceCollection()
                       .Configure<JsonSerializerOptions>
                       (
                           json =>
                           {
                               json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                               json.Converters.Add(new StringEnumConverter<StringifiedEnum>(asInteger: true));
                           })
                       .BuildServiceProvider();

        var dictionary = new Dictionary<StringifiedEnum, string>
        {
            { StringifiedEnum.First, "first" },
            { StringifiedEnum.Second, "second" },
            { StringifiedEnum.Third, "third" },
        };

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        // Act
        var result = JsonSerializer.Serialize(dictionary, jsonOptions);
        var expected = "{\"0\":\"first\",\"1\":\"second\",\"2\":\"third\"}";

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that the converter can serialize a dictionary where the key is an enum.
    /// </summary>
    [Fact]
    public void CanSerializeDictionaryKeyAsString()
    {
        // Arrange
        var services = new ServiceCollection()
                       .Configure<JsonSerializerOptions>
                       (
                           json =>
                           {
                               json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                               json.Converters.Add(new StringEnumConverter<StringifiedEnum>(json.PropertyNamingPolicy));
                           })
                       .BuildServiceProvider();

        var dictionary = new Dictionary<StringifiedEnum, string>
        {
            { StringifiedEnum.First, "first" },
            { StringifiedEnum.Second, "second" },
            { StringifiedEnum.Third, "third" },
        };

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        // Act
        var result = JsonSerializer.Serialize(dictionary, jsonOptions);
        var expected = "{\"first\":\"first\",\"second\":\"second\",\"third\":\"third\"}";

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that the converter can read a dictionary where the key is an enum.
    /// </summary>
    [Fact]
    public void CanDeserializeDictionaryKeyAsInteger()
    {
        // Arrange
        var services = new ServiceCollection()
                       .Configure<JsonSerializerOptions>
                       (
                           json =>
                           {
                               json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                               json.Converters.Add(new StringEnumConverter<StringifiedEnum>(asInteger: true));
                           })
                       .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        var json = "{\"0\":\"first\",\"1\":\"second\",\"2\":\"third\"}";

        // Act
        var result = JsonSerializer.Deserialize<Dictionary<StringifiedEnum, string>>(json, jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("first", result[StringifiedEnum.First]);
        Assert.Equal("second", result[StringifiedEnum.Second]);
        Assert.Equal("third", result[StringifiedEnum.Third]);
    }

    /// <summary>
    /// Tests that the converter can read a dictionary where the key is an enum.
    /// </summary>
    [Fact]
    public void CanDeserializeDictionaryKeyAsString()
    {
        // Arrange
        var services = new ServiceCollection()
                       .Configure<JsonSerializerOptions>
                       (
                           json =>
                           {
                               json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                               json.Converters.Add(new StringEnumConverter<StringifiedEnum>(json.PropertyNamingPolicy));
                           })
                       .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        var json = "{\"first\":\"first\",\"second\":\"second\",\"third\":\"third\"}";

        // Act
        var result = JsonSerializer.Deserialize<Dictionary<StringifiedEnum, string>>(json, jsonOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("first", result[StringifiedEnum.First]);
        Assert.Equal("second", result[StringifiedEnum.Second]);
        Assert.Equal("third", result[StringifiedEnum.Third]);
    }
}
