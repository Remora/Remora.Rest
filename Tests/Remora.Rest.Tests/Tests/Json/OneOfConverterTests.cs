//
//  OneOfConverterTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using Remora.Rest.Json.Internal;
using Remora.Rest.Json.Policies;
using Remora.Rest.Tests.Data.DataObjects;
using Xunit;

namespace Remora.Rest.Tests.Json;

/// <summary>
/// Tests the <see cref="OneOfConverter{TOneOf}"/> class.
/// </summary>
public class OneOfConverterTests
{
    /// <summary>
    /// Tests whether the converter can deserialize a OneOf object containing primitive types.
    /// </summary>
    [Fact]
    public void CanDeserializeSimpleObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddConverter<OneOfConverterFactory>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": \"booga\" }";

        var valueMaybeNull = JsonSerializer.Deserialize<SimpleOneOfData>(payload, jsonOptions);
        Assert.NotNull(valueMaybeNull);

        var value = valueMaybeNull!.Value;
        Assert.True(value.IsT0);
        Assert.Equal("booga", value.AsT0);

        payload = "{ \"value\": 10 }";
        valueMaybeNull = JsonSerializer.Deserialize<SimpleOneOfData>(payload, jsonOptions);
        Assert.NotNull(valueMaybeNull);

        value = valueMaybeNull!.Value;
        Assert.True(value.IsT1);
        Assert.Equal(10, value.AsT1);
    }

    /// <summary>
    /// Tests whether the converter can deserialize a OneOf object containing objects.
    /// </summary>
    [Fact]
    public void CanDeserializeComplexObject()
    {
        var services = new ServiceCollection()
            .Configure<JsonSerializerOptions>
            (
                json =>
                {
                    json.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                    json.AddConverter<OneOfConverterFactory>();
                })
            .BuildServiceProvider();

        var jsonOptions = services.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
        var payload = "{ \"value\": { \"custom_id\": \"Next\", \"component_type\": 2 } }";

        var valueMaybeNull = JsonSerializer.Deserialize<ComplexOneOfData>(payload, jsonOptions);
        Assert.NotNull(valueMaybeNull);

        var value = valueMaybeNull!.Value;
        Assert.True(value.IsT1);
        Assert.Equal(new MessageComponentData("Next", 2), value.AsT1);
    }
}
