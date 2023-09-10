//
//  SPDX-FileName: SimpleDataTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Text.Json.Serialization;
using Remora.Rest.Json.Configuration;
using Remora.Rest.Tests.Data.DataObjects;
using Remora.Rest.Tests.TestBases;

#pragma warning disable CS1591

namespace Remora.Rest.Tests.Json.Basic;

/// <summary>
/// Tests de/serialization of simple data.
/// </summary>
public partial class SimpleDataTests : ConfiguredJsonTypeInfoResolverTestBase<SimpleData, SimpleDataTests.SimpleDataSerializerContext>
{
    [JsonSerializable(typeof(SimpleData))]
    public partial class SimpleDataSerializerContext : JsonSerializerContext
    {
    }

    /// <inheritdoc />
    protected override Action<FluentJsonTypeInfoResolver> ConfigureDefaultResolver => r => r
        .WithContext<SimpleDataSerializerContext>();

    /// <inheritdoc />
    protected override string Payload => """{ "Value": "booga" }""";

    /// <inheritdoc />
    protected override SimpleData Object { get; } = new("booga");
}
