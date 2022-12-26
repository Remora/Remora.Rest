//
//  SPDX-FileName: MockedRequestExtensionsTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Remora.Rest.Xunit.Extensions;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="Extensions.MockedRequestExtensions"/> class.
/// </summary>
public static class MockedRequestExtensionsTests
{
    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithNoContent"/> method.
    /// </summary>
    public static class WithNoContent
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request contains any content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestContainsContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithNoContent()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("wooga");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request does not contain any content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestDoesNotContainContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithNoContent()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithAuthentication"/> method.
    /// </summary>
    public static class WithAuthentication
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasNoAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has an authorization header that does not match
        /// the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasAuthorizationHeaderThatDoesNotMatch()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication(a => a.Scheme == "Bearer" && a.Parameter == "wooga")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "booga");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasMatchingAuthorizationHeader()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithAuthentication(a => a.Scheme == "Bearer" && a.Parameter == "wooga")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.MockedRequestExtensions.WithJson"/> method.
    /// </summary>
    public static class WithJson
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasNoContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasNonJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("not json");

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a JSON body that does not match the
        /// predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasJsonContentThatDoesNotMatch()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson(j => j.IsObject(o => o.WithProperty("value", p => p.Is(0))))
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 1 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            await Assert.ThrowsAnyAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasMatchingJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithJson(j => j.IsObject(o => o.WithProperty("value", p => p.Is(0))))
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.MockedRequestExtensions.WithMultipartJsonPayload"/> method.
    /// </summary>
    public static class WithMultipartJsonPayload
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks any content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasNoContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks multipart content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasNonMultipartContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("ooga");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks multipart content with a string content part
        /// of the expected name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasMultipartContentWithoutJsonPayload()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("ooga"), "something else");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has multipart content with a string part of the
        /// expected name, but that does not contain valid JSON.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasMultipartContentWithInvalidJsonPayload()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("ooga"), "payload_json");

            request.Content = multipart;

            await Assert.ThrowsAsync<IsTypeException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a JSON body that does not match the
        /// JSON matcher.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfRequestHasMultipartJsonPayloadThatDoesNotMatch()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload(j => j.IsArray())
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method passes if the request has a valid JSON multipart payload.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasMultipartJsonPayload()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload()
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method passes if the request has a valid JSON multipart payload that matches the predicate.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfRequestHasMatchingJsonContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartJsonPayload(j => j.IsObject(o => o.WithProperty("value", p => p.Is(1))))
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.MockedRequestExtensions.WithMultipartFormData(MockedRequest,string,string)"/> method
    /// and its overloads.
    /// </summary>
    public static class WithMultipartFormData
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request has no content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestHasNoContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request content is not multipart content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestIsNotMultipartContent()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("fail");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no form data field with the correct name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestHasNoFormDataWithName()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("something else"), "not value");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestHasFormDataWithCorrectNameButWrongType()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "value");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestHasFormDataWithCorrectNameButWrongValue()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("not value"), "value");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method returns true if the request has a form data field where the name and value match.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFieldRequestHasFormDataWithCorrectNameAndCorrectValue()
        {
            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("value", "0")
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("0"), "value");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestHasNoContent()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request content is not multipart content.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestIsNotMultipartContent()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("fail");

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has no form data field with the correct name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestHasNoFormDataWithName()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("something else"), "not value");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong type.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongType()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "file", "filename.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndValueButWrongFilename()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", "wrong.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a form data field with the correct name but
        /// the wrong value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongValue()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(new MemoryStream()), "file", "filename.txt");

            request.Content = multipart;

            await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
        }

        /// <summary>
        /// Tests that the method returns true if the request has a form data field where the name and value match.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public static async Task PassesIfFileRequestHasFormDataWithCorrectNameAndFilenameAndValue()
        {
            await using var stream = new MemoryStream();

            var mockHandler = new MockHttpMessageHandler();
            mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                .WithMultipartFormData("file", "filename.txt", stream)
                .Respond(HttpStatusCode.OK);

            var client = mockHandler.ToHttpClient();

            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StreamContent(stream), "file", "filename.txt");

            request.Content = multipart;

            var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.MockedRequestExtensions.WithUrlEncodedFormData(MockedRequest)"/> method
    /// and its overloads.
    /// </summary>
    public static class WithUrlEncodedFormData
    {
        /// <summary>
        /// Tests the overload with no arguments.
        /// </summary>
        public static class NoArguments
        {
            /// <summary>
            /// Tests that the method raises an assertion if the request has no content.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task AssertsIfRequestHasNoContent()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData()
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request content is not URL-encoded form data content.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task AssertsIfRequestContainsNotUrlEncodedFormDataContent()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData()
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new StringContent("fail");

                await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
            }

            /// <summary>
            /// Tests that the method passes if the request content is URL-encoded form data content.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task PassesIfRequestContainsUrlEncodedFormDataContent()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData()
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(Array.Empty<KeyValuePair<string, string>>());

                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        /// <summary>
        /// Tests the overload with a set of expected values.
        /// </summary>
        public static class Expectations
        {
            /// <summary>
            /// Tests that the method raises an assertion if an expected key is missing.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task AssertsIfKeyIsMissing()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData(new Dictionary<string, string>
                    {
                        { "other", "value" }
                    })
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
            }

            /// <summary>
            /// Tests that the method raises an assertion if an expected value differs.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task AssertsIfValueDiffers()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData(new Dictionary<string, string>
                    {
                        { "some", "type" }
                    })
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the content contains more than the expected data and strict
            /// checking is enabled.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task AssertsIfContentContainsMoreAndExpectationsAreStrict()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData
                    (
                        new Dictionary<string, string>
                        {
                            { "some", "value" }
                        },
                        true
                    )
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value"),
                    new("other", "thing")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                await Assert.ThrowsAsync<XunitException>(async () => await client.SendAsync(request));
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqual()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData(new Dictionary<string, string>
                    {
                        { "some", "value" }
                    })
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqualAndExpectationsAreStrict()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData
                    (
                        new Dictionary<string, string>
                        {
                            { "some", "value" }
                        },
                        true
                    )
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            /// <summary>
            /// Tests that the method passes if the content contains more than the expected data and strict checking is
            /// disabled.
            /// </summary>
            /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
            [Fact]
            public static async Task PassesIfContentContainsMoreAndExpectationsAreNotStrict()
            {
                await using var stream = new MemoryStream();

                var mockHandler = new MockHttpMessageHandler();
                mockHandler.Expect(HttpMethod.Get, "https://unit-test")
                    .WithUrlEncodedFormData(new Dictionary<string, string>
                    {
                        { "some", "value" }
                    })
                    .Respond(HttpStatusCode.OK);

                var client = mockHandler.ToHttpClient();

                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value"),
                    new("other", "thing")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                var response = await client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
