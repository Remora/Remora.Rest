//
//  SPDX-FileName: ConfiguredJsonTypeInfoResolverTestBase.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Rest.Json.Configuration;
using Remora.Rest.Tests.Data.DataObjects;
using Remora.Rest.Xunit;
using Xunit;

namespace Remora.Rest.Tests.TestBases;

/// <summary>
/// Serves as an abstract base class for JSON de/serialization tests through a <see cref="ConfiguredJsonTypeInfoResolver"/>.
/// </summary>
/// <typeparam name="T">The type to test.</typeparam>
/// <typeparam name="TContext">The serializer context in use.</typeparam>
public abstract class ConfiguredJsonTypeInfoResolverTestBase<T, TContext> : IDisposable
    where TContext : JsonSerializerContext
{
    private readonly ServiceProvider _services;

    /// <summary>
    /// Gets a function that configures the service collection.
    /// </summary>
    protected virtual Func<IServiceCollection, IServiceCollection> ConfigureServices => s => s;

    /// <summary>
    /// Gets a function that configures the default serializer options.
    /// </summary>
    protected virtual Action<JsonSerializerOptions> ConfigureDefaultSerializerOptions => _ => { };

    /// <summary>
    /// Gets a function that configures the default type info resolver.
    /// </summary>
    protected virtual Action<FluentJsonTypeInfoResolver> ConfigureDefaultResolver => _ => { };

    /// <summary>
    /// Gets the payload to test.
    /// </summary>
    protected abstract string Payload { get; }

    /// <summary>
    /// Gets the expected object representation of the payload.
    /// </summary>
    protected abstract T Object { get; }

    /// <summary>
    /// Gets the default serializer options.
    /// </summary>
    protected JsonSerializerOptions DefaultSerializerOptions { get; }

    /// <summary>
    /// Gets the default type info resolver.
    /// </summary>
    protected ConfiguredJsonTypeInfoResolver DefaultResolver { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredJsonTypeInfoResolverTestBase{T, TContext}"/> class.
    /// </summary>
    protected ConfiguredJsonTypeInfoResolverTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton
        <
            IOptionsFactory<ConfiguredJsonTypeInfoResolver>,
            ConfiguredJsonTypeInfoResolverOptionsFactory
        >();

        serviceCollection.AddSingleton<IJsonTypeInfoResolver, TContext>();

        this.ConfigureServices(serviceCollection);

        serviceCollection.Configure(this.ConfigureDefaultResolver);
        serviceCollection.Configure(this.ConfigureDefaultSerializerOptions);

        _services = serviceCollection.BuildServiceProvider();

        this.DefaultSerializerOptions = _services.GetRequiredService<IOptionsFactory<JsonSerializerOptions>>()
            .Create(string.Empty);

        this.DefaultResolver = _services.GetRequiredService<IOptionsFactory<ConfiguredJsonTypeInfoResolver>>()
            .Create(string.Empty);
    }

    /// <summary>
    /// Gets configured type information for the given type from the default type info resolver.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>The type information.</returns>
    protected JsonTypeInfo<T> GetTypeInfo<T>() => this.DefaultResolver.GetTypeInfo<T>(this.DefaultSerializerOptions)
        ?? throw new InvalidOperationException();

    /// <summary>
    /// Tests whether the type under test can be deserialized without errors.
    /// </summary>
    [Fact]
    public void CanDeserialize()
    {
        var typeInfo = GetTypeInfo<T>();
        _ = JsonSerializer.Deserialize(this.Payload, typeInfo);
    }

    /// <summary>
    /// Tests whether the type under test can be serialized without errors.
    /// </summary>
    [Fact]
    public void CanSerialize()
    {
        var typeInfo = GetTypeInfo<T>();
        _ = JsonSerializer.Serialize(this.Object, typeInfo);
    }

    /// <summary>
    /// Tests whether the type can be round-tripped to JSON and back without information loss.
    /// </summary>
    [Fact]
    public void SurvivesRoundTrip()
    {
        var typeInfo = GetTypeInfo<T>();

        var actual = JsonSerializer.Deserialize(this.Payload, typeInfo);

        Assert.NotNull(actual);
        Assert.Equal(this.Object, actual);

        var serialized = JsonSerializer.Serialize(actual, typeInfo);

        JsonAssert.Equivalent(JsonDocument.Parse(this.Payload), JsonDocument.Parse(serialized));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _services.Dispose();
    }
}
