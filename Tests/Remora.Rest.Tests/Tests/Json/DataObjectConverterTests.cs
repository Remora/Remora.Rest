//
//  DataObjectConverterTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Rest.Extensions;
using Remora.Rest.Json;
using Remora.Rest.Json.Policies;
using Remora.Rest.Tests.Data.DataObjects;
using Remora.Rest.Xunit;
using Xunit;

namespace Remora.Rest.Tests.Json;

/// <summary>
/// Tests the <see cref="DataObjectConverter{TInterface,TImplementation}"/> class.
/// </summary>
public class DataObjectConverterTests
{
    /// <summary>
    /// Tests whether the converter can deserialize a simple data object.
    /// </summary>
    [Fact]
    public void CanDeserializeSimpleDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"booga\" }";

        var value = JsonSerializer.Deserialize<ISimpleData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Value);
    }

    /// <summary>
    /// Tests whether the converter can serialize a simple data object.
    /// </summary>
    [Fact]
    public void CanSerializeSimpleDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        ISimpleData value = new SimpleData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can deserialize an optional data object.
    /// </summary>
    [Fact]
    public void CanDeserializeOptionalDataObject()
    {
        var services = new ServiceCollection()
            .ConfigureRestJsonConverters()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json
                        .AddDataObjectConverter<IOptionalData, OptionalData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"booga\" }";

        var value = JsonSerializer.Deserialize<IOptionalData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.True(value.Value.HasValue);
        Assert.Equal("booga", value.Value.Value);
    }

    /// <summary>
    /// Tests whether the converter can deserialize an optional data object.
    /// </summary>
    [Fact]
    public void CanDeserializeOptionalDataObjectWithMissingProperties()
    {
        var services = new ServiceCollection()
            .ConfigureRestJsonConverters()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IOptionalData, OptionalData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{  }";

        var value = JsonSerializer.Deserialize<IOptionalData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.False(value.Value.HasValue);
    }

    /// <summary>
    /// Tests whether the converter can serialize an optional data object.
    /// </summary>
    [Fact]
    public void CanSerializeOptionalDataObject()
    {
        var services = new ServiceCollection()
            .ConfigureRestJsonConverters()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IOptionalData, OptionalData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IOptionalData value = new OptionalData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can serialize an optional data object where one or more properties do not have a
    /// value.
    /// </summary>
    [Fact]
    public void CanSerializeOptionalDataObjectWithMissingProperties()
    {
        var services = new ServiceCollection()
            .ConfigureRestJsonConverters()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IOptionalData, OptionalData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IOptionalData value = new OptionalData(default);
        var expectedPayload = JsonDocument.Parse("{ }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can deserialize a data object with a member that required a custom converter.
    /// </summary>
    [Fact]
    public void CanDeserializeConvertibleDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    var policy = new SnakeCaseNamingPolicy();
                    json.PropertyNamingPolicy = policy;
                    json.AddDataObjectConverter<IConvertibleData, ConvertibleData>()
                        .WithPropertyConverter(d => d.Value, new StringEnumConverter<StringifiedEnum>(policy));
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"first\" }";

        var value = JsonSerializer.Deserialize<IConvertibleData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal(StringifiedEnum.First, value.Value);
    }

    /// <summary>
    /// Tests whether the converter can serialize a data object with a member that required a custom converter.
    /// </summary>
    [Fact]
    public void CanSerializeConvertibleDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    var policy = new SnakeCaseNamingPolicy();
                    json.PropertyNamingPolicy = policy;
                    json.AddDataObjectConverter<IConvertibleData, ConvertibleData>()
                        .WithPropertyConverter(d => d.Value, new StringEnumConverter<StringifiedEnum>(policy));
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IConvertibleData value = new ConvertibleData(StringifiedEnum.First);
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"first\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can deserialize a simple data object where a property has been renamed.
    /// </summary>
    [Fact]
    public void CanDeserializeSimpleDataObjectWithRenamedProperty()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>()
                        .WithPropertyName(d => d.Value, "other_value");
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"other_value\": \"booga\" }";

        var value = JsonSerializer.Deserialize<ISimpleData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Value);
    }

    /// <summary>
    /// Tests whether the converter can serialize a simple data object where a property has been renamed.
    /// </summary>
    [Fact]
    public void CanSerializeSimpleDataObjectWithRenamedProperty()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>()
                        .WithPropertyName(d => d.Value, "other_value");
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        ISimpleData value = new SimpleData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"other_value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can deserialize a data object with a read-only member.
    /// </summary>
    [Fact]
    public void CanDeserializeReadOnlyDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IReadOnlyData, ReadOnlyData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"booga\", \"read_only\": 1 }";

        var value = JsonSerializer.Deserialize<IReadOnlyData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Value);
    }

    /// <summary>
    /// Tests whether the converter can serialize a data object with a read-only member.
    /// </summary>
    [Fact]
    public void CanSerializeReadOnlyDataObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IReadOnlyData, ReadOnlyData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IReadOnlyData value = new ReadOnlyData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter does not serialize read-only members by default.
    /// </summary>
    [Fact]
    public void SerializedDataDoesNotContainReadOnlyMemberByDefault()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IReadOnlyData, ReadOnlyData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IReadOnlyData value = new ReadOnlyData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);

        Assert.False(serialized.RootElement.TryGetProperty("read_only", out _));
    }

    /// <summary>
    /// Tests whether the converter includes read-only data when asked to.
    /// </summary>
    [Fact]
    public void CanIncludeReadOnlyData()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IReadOnlyData, ReadOnlyData>()
                        .IncludeWhenSerializing(d => d.ReadOnly);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IReadOnlyData value = new ReadOnlyData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\", \"read_only\": 1 }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can serialize a data object containing unconventional explicit interface
    /// implementations.
    /// </summary>
    [Fact]
    public void CanSerializeExplicitInterfaceImplementations()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<INonConventionalData, NonConventionalData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        INonConventionalData value = new NonConventionalData2("this should not be serialized");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"this should be serialized\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize its data if provided as the implementation type.
    /// </summary>
    [Fact]
    public void CanSerializeAsConcreteType()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        SimpleData value = new SimpleData("booga");
        var expectedPayload = JsonDocument.Parse("{ \"value\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize its data if provided as the implementation type.
    /// </summary>
    [Fact]
    public void CanDeserializeToConcreteType()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<ISimpleData, SimpleData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"booga\" }";

        var value = JsonSerializer.Deserialize<SimpleData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Value);
    }
}
