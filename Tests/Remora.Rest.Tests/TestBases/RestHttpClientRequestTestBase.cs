//
//  RestHttpClientRequestTestBase.cs
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

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Remora.Rest.Extensions;
using Remora.Rest.Tests.Data;
using Remora.Rest.Xunit.Extensions;
using Remora.Results;
using RichardSzalay.MockHttp;
using Xunit;

namespace Remora.Rest.Tests.TestBases;

/// <summary>
/// Acts as a test base for the base cases of a single HTTP request method.
/// </summary>
public abstract class RestHttpClientRequestTestBase
{
    /// <summary>
    /// Gets the request method to test.
    /// </summary>
    protected abstract HttpMethod RequestMethod { get; }

    private Func<IRestHttpClient, string, Task<IResult>> RequestFunction => this.RequestMethod switch
    {
        { Method: "GET" } => async (c, e) => await c.GetAsync<int>(e),
        { Method: "POST" } => async (c, e) => await c.PostAsync<int>(e),
        { Method: "PATCH" } => async (c, e) => await c.PatchAsync<int>(e),
        { Method: "DELETE" } => async (c, e) => await c.DeleteAsync<int>(e),
        { Method: "PUT" } => async (c, e) => await c.PutAsync<int>(e),
        _ => throw new InvalidOperationException()
    };

    private Func<IRestHttpClient, string, Task<IResult>> NullableRequestFunction => this.RequestMethod switch
    {
        { Method: "GET" } => async (c, e) => await c.GetAsync<int?>(e, allowNullReturn: true),
        { Method: "POST" } => async (c, e) => await c.PostAsync<int?>(e, allowNullReturn: true),
        { Method: "PATCH" } => async (c, e) => await c.PatchAsync<int?>(e, allowNullReturn: true),
        { Method: "DELETE" } => async (c, e) => await c.DeleteAsync<int?>(e, allowNullReturn: true),
        { Method: "PUT" } => async (c, e) => await c.PutAsync<int?>(e, allowNullReturn: true),
        _ => throw new InvalidOperationException()
    };

    /// <summary>
    /// Tests whether the Http client can perform simple requests.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformSimpleRequestAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .Respond("application/json", "1")
        );

        var result = await this.RequestFunction(client, "https://unit-test");
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the Http client can perform nullable requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformNullableRequestWithNullLiteralBodyAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .Respond("application/json", "null")
        );

        var result = await this.NullableRequestFunction(client, "https://unit-test");
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the Http client can perform nullable requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformNullableRequestWithEmptyBodyAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .Respond("application/json", string.Empty)
        );

        var result = await this.NullableRequestFunction(client, "https://unit-test");
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the Http client can perform nullable requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformNullableRequestWith204NoContentAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .Respond(HttpStatusCode.NoContent)
        );

        var result = await this.NullableRequestFunction(client, "https://unit-test");
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the Http client can perform nullable requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformNullableRequestWith204NoContentWithoutContentLengthAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .Respond
                (
                    _ => new HttpResponseMessage(HttpStatusCode.NoContent)
                    {
                        Content = null
                    }
                )
        );

        var result = await this.NullableRequestFunction(client, "https://unit-test");
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests whether the Http client can perform customized requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CanPerformCustomizedRequestAsync()
    {
        var client = CreateClient
        (
            b => b
                .Expect(this.RequestMethod, "https://unit-test")
                .WithJson(json => json.IsObject(o => o.WithProperty("name", p => p.Is("value"))))
                .Respond("application/json", "1")
        );

        using (_ = client.WithCustomization(r => r.WithJson(json => json.WriteString("name", "value"))))
        {
            var result = await this.RequestFunction(client, "https://unit-test");
            Assert.True(result.IsSuccess);
        }
    }

    /// <summary>
    /// Tests whether the Http client can perform customized requests.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CustomizationElapsesAfterScopeAsync()
    {
        var client = CreateClient
        (
            b =>
            {
                b
                    .Expect(this.RequestMethod, "https://unit-test")
                    .WithJson
                    (
                        json => json.IsObject(o => o.WithProperty("name", p => p.Is("value")))
                    )
                    .Respond("application/json", "1");

                b
                    .Expect(this.RequestMethod, "https://unit-test/elapsed")
                    .WithNoContent()
                    .Respond("application/json", "1");
            }
        );

        using (_ = client.WithCustomization(r => r.WithJson(json => json.WriteString("name", "value"))))
        {
            var result = await this.RequestFunction(client, "https://unit-test");
            Assert.True(result.IsSuccess);
        }

        var elapsedResult = await this.RequestFunction(client, "https://unit-test/elapsed");
        Assert.True(elapsedResult.IsSuccess);
    }

    /// <summary>
    /// Creates a <see cref="RestHttpClient{TError}"/> with related mocking rules.
    /// </summary>
    /// <param name="builder">The mock builder.</param>
    /// <returns>The created client.</returns>
    private static IRestHttpClient CreateClient(Action<MockHttpMessageHandler> builder)
    {
        var serviceCollection = new ServiceCollection();

        var clientBuilder = serviceCollection.AddRestHttpClient<TestError>();
        clientBuilder.ConfigurePrimaryHttpMessageHandler
        (
            _ =>
            {
                var mockHandler = new MockHttpMessageHandler();
                builder(mockHandler);

                return mockHandler;
            }
        );

        return serviceCollection.BuildServiceProvider().GetRequiredService<RestHttpClient<TestError>>();
    }
}
