//
//  SPDX-FileName: DataObjectConverterTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
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

        var value = new SimpleData("booga");
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

    /// <summary>
    /// Tests whether the converter fails correctly when deserializing data record with an excluded member.
    /// </summary>
    [Fact]
    public void CannotDeserializeDataWithExcludedMemberWithoutDefaultValue()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedData>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"serialize\": \"booga\" }";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<IExcludedData>(payload, jsonOptions));
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize a data record with an excluded member.
    /// </summary>
    [Fact]
    public void CanSerializeDataWithExcludedMemberWithoutDefaultValue()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedData>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        IExcludedData value = new ExcludedData("booga", "wooga");

        var expectedPayload = JsonDocument.Parse("{ \"serialize\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize a data record with an excluded member where the excluded
    /// member has a default value.
    /// </summary>
    [Fact]
    public void CanDeserializeDataWithExcludedMemberWithDefaultValue()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedDataWithDefaultValue>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"serialize\": \"booga\" }";

        var value = JsonSerializer.Deserialize<IExcludedData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Serialize);
        Assert.Equal("value", value.DoNotSerialize);
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize a data record with an excluded member where the excluded
    /// member has a default value.
    /// </summary>
    [Fact]
    public void CanSerializeDataWithExcludedMemberWithDefaultValue()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedDataWithDefaultValue>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        IExcludedData value = new ExcludedDataWithDefaultValue("booga");

        var expectedPayload = JsonDocument.Parse("{ \"serialize\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize a data record with an excluded member where the excluded
    /// member is read-only.
    /// </summary>
    [Fact]
    public void CanDeserializeDataWithExcludedMemberWithReadOnlyMember()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedDataWithReadOnlyMember>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"serialize\": \"booga\" }";

        var value = JsonSerializer.Deserialize<IExcludedData>(payload, jsonOptions);
        Assert.NotNull(value);
        Assert.Equal("booga", value.Serialize);
        Assert.Equal("value", value.DoNotSerialize);
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize a data record with an excluded member where the excluded
    /// member is read-only.
    /// </summary>
    [Fact]
    public void CanSerializeDataWithExcludedMemberWithReadOnlyMember()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IExcludedData, ExcludedDataWithDefaultValue>()
                        .ExcludeWhenSerializing(d => d.DoNotSerialize);
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        IExcludedData value = new ExcludedDataWithReadOnlyMember("booga");

        var expectedPayload = JsonDocument.Parse("{ \"serialize\": \"booga\" }");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize a data record where data values are sourced from the
    /// type's primary constructor.
    /// </summary>
    [Fact]
    public void CanDeserializeDataWithConstructorProvidedDefaults()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IConstructorArgumentData, ConstructorArgumentData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ }";

        var value = JsonSerializer.Deserialize<IConstructorArgumentData>(payload, jsonOptions);
        Assert.NotNull(value);

        Assert.Equal(0, value.ValueType);
        Assert.Null(value.NullableValueType);
        Assert.Equal(default, value.OptionalValueType);
        Assert.Equal(default, value.OptionalNullableValueType);

        Assert.Equal(string.Empty, value.ReferenceType);
        Assert.Null(value.NullableReferenceType);
        Assert.Equal(default, value.OptionalReferenceType);
        Assert.Equal(default, value.OptionalNullableReferenceType);
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize a data record where data values are sourced from the
    /// type's primary constructor.
    /// </summary>
    [Fact]
    public void CanSerializeDataWithConstructorProvidedDefaults()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IConstructorArgumentData, ConstructorArgumentData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IConstructorArgumentData value = new ConstructorArgumentData();

        var expectedPayload = JsonDocument.Parse("{ \n  \"value_type\": 0,\n  \"nullable_value_type\": null,\n  \"reference_type\": \"\",\n  \"nullable_reference_type\": null\n}");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize a data record where default values are sourced from the
    /// type's primary constructor, but the real values come from the payload.
    /// </summary>
    [Fact]
    public void CanDeserializeDataWithConstructorProvidedDefaultsWhereDataIsFromPayload()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();

                    json.AddConverter<OptionalConverterFactory>();
                    json.AddDataObjectConverter<IConstructorArgumentData, ConstructorArgumentData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \n  \"value_type\": 1,\n  \"nullable_value_type\": 2,\n  \"optional_value_type\": 3,\n  \"optional_nullable_value_type\": 4,\n  \"reference_type\": \"ooga\",\n  \"nullable_reference_type\": \"booga\",\n  \"optional_reference_type\": \"wooga\",\n  \"optional_nullable_reference_type\": \"mooga\"\n}";

        var value = JsonSerializer.Deserialize<IConstructorArgumentData>(payload, jsonOptions);
        Assert.NotNull(value);

        Assert.Equal(1, value.ValueType);
        Assert.Equal(2, value.NullableValueType);
        Assert.Equal(3, value.OptionalValueType);
        Assert.Equal(4, value.OptionalNullableValueType);

        Assert.Equal("ooga", value.ReferenceType);
        Assert.Equal("booga", value.NullableReferenceType);
        Assert.Equal("wooga", value.OptionalReferenceType);
        Assert.Equal("mooga", value.OptionalNullableReferenceType);
    }

    /// <summary>
    /// Tests whether the converter can correctly serialize a data record where data values are sourced from the
    /// type's primary constructor.
    /// </summary>
    [Fact]
    public void CanSerializeDataWithConstructorProvidedDefaultsWhereDataIsFromInstance()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IConstructorArgumentData, ConstructorArgumentData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        IConstructorArgumentData value = new ConstructorArgumentData
        (
            1,
            2,
            3,
            4,
            "ooga",
            "booga",
            "wooga",
            "mooga"
        );

        var expectedPayload = JsonDocument.Parse("{ \n  \"value_type\": 1,\n  \"nullable_value_type\": 2,\n  \"optional_value_type\": 3,\n  \"optional_nullable_value_type\": 4,\n  \"reference_type\": \"ooga\",\n  \"nullable_reference_type\": \"booga\",\n  \"optional_reference_type\": \"wooga\",\n  \"optional_nullable_reference_type\": \"mooga\"\n}");

        var serialized = JsonDocument.Parse(JsonSerializer.Serialize(value, jsonOptions));
        JsonAssert.Equivalent(expectedPayload, serialized);
    }

    /// <summary>
    /// Tests whether the converter can correctly deserialize into an old-style object without a constructor.
    /// </summary>
    [Fact]
    public void CanDeserializeOldStyleObjects()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddDataObjectConverter<IOldStyleData, OldStyleData>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

        var payload =
            """
            {
                "value": "ooga",
                "other_value": 4
            }
            """;

        OldStyleData deserialized = JsonSerializer.Deserialize<OldStyleData>(payload, jsonOptions)!;

        Assert.Equal("ooga", deserialized.Value);
        Assert.Equal(4, deserialized.OtherValue);
    }
}
