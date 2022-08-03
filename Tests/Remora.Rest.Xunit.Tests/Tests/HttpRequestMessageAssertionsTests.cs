//
//  HttpRequestMessageAssertionsTests.cs
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

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Remora.Rest.Xunit.Extensions;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="Extensions.HttpRequestMessageAssertions"/> class.
/// </summary>
public static class HttpRequestMessageAssertionsTests
{
    /// <summary>
    /// Tests the <see cref="Extensions.HttpRequestMessageAssertions.HasNoContent"/> method.
    /// </summary>
    public static class HasNoContent
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request contains any content.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestContainsContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("wooga");

            Assert.Throws<NullException>(() => request.HasNoContent());
        }

        /// <summary>
        /// Tests that the method passes if the request does not contain any content.
        /// </summary>
        [Fact]
        public static void PassesIfRequestDoesNotContainContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.HasNoContent();
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.HttpRequestMessageAssertions.HasAuthentication(HttpRequestMessage)"/> method and
    /// its overloads.
    /// </summary>
    public static class HasAuthentication
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks an authorization header.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNoAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            Assert.Throws<NotNullException>(() => request.HasAuthentication());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has an authorization header that does not match
        /// the predicate.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasAuthorizationHeaderThatDoesNotMatchExpectation()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "booga");

            Assert.Throws<EqualException>(() => request.HasAuthentication(v => Assert.Equal("unexpected", v.Parameter)));
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has an authorization header that does not match
        /// the predicate.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasAuthorizationHeaderThatDoesNotMatchPredicate()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "booga");

            Assert.Throws<TrueException>(() => request.HasAuthentication(v => v.Parameter == "unexpected"));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            request.HasAuthentication();
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the expectation.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasExpectationMatchingAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            request.HasAuthentication(v => Assert.Equal("wooga", v.Parameter));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasPredicateMatchingAuthorizationHeader()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "wooga");

            request.HasAuthentication(v => v.Parameter == "wooga");
        }
    }

    /// <summary>
    /// Tests the <see cref="Extensions.HttpRequestMessageAssertions.HasJson(HttpRequestMessage)"/> method and its
    /// overloads.
    /// </summary>
    public static class HasJson
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNoContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            Assert.Throws<NotNullException>(() => request.HasJson());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks a JSON body.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNonJsonContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("not json");

            Assert.Throws<IsTypeException>(() => request.HasJson());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a JSON body that does not match the
        /// predicate.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasJsonContentThatDoesNotMatch()
        {
            var json = "{ \"value\": 1 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            Assert.Throws<EqualException>(() => request.HasJson(j => j.IsArray()));
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasJsonContent()
        {
            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            request.HasJson();
        }

        /// <summary>
        /// Tests that the method passes if the request has an authorization header that matches the predicate.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasMatchingJsonContent()
        {
            var json = "{ \"value\": 0 }";
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            request.HasJson(j => j.IsObject(o => o.WithProperty("value", p => p.Is(0))));
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.HttpRequestMessageAssertions.HasMultipartJsonPayload(HttpRequestMessage, string)"/> method
    /// and its overloads.
    /// </summary>
    public static class HasMultipartJsonPayload
    {
        /// <summary>
        /// Tests that the method raises an assertion if the request lacks any content.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNoContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            Assert.Throws<NotNullException>(() => request.HasMultipartJsonPayload());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks multipart content.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNonMultipartContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("ooga");

            Assert.Throws<IsTypeException>(() => request.HasMultipartJsonPayload());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks multipart content with a string content part
        /// of the expected name.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasMultipartContentWithoutJsonPayload()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("ooga"), "something else");

            request.Content = multipart;

            Assert.Throws<SingleException>(() => request.HasMultipartJsonPayload());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has multipart content with a string part of the
        /// expected name, but that does not contain valid JSON.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasMultipartContentWithInvalidJsonPayload()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("ooga"), "payload_json");

            request.Content = multipart;

            Assert.Throws<IsTypeException>(() => request.HasMultipartJsonPayload());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request has a JSON body that does not match the
        /// JSON matcher.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasMultipartJsonPayloadThatDoesNotMatch()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            Assert.Throws<EqualException>(() => request.HasMultipartJsonPayload(j => j.IsArray()));
        }

        /// <summary>
        /// Tests that the method passes if the request has a valid JSON multipart payload.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasMultipartJsonPayload()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            request.HasMultipartJsonPayload();
        }

        /// <summary>
        /// Tests that the method passes if the request has a valid JSON multipart payload that matches the predicate.
        /// </summary>
        [Fact]
        public static void PassesIfRequestHasMatchingJsonContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

            var multipart = new MultipartFormDataContent();
            multipart.Add(new StringContent("{ \"value\": 1 }", Encoding.UTF8, "application/json"), "payload_json");

            request.Content = multipart;

            request.HasMultipartJsonPayload(j => j.IsObject(o => o.WithProperty("value", p => p.Is(1))));
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.HttpRequestMessageAssertions.HasMultipartFormData(HttpRequestMessage)"/> method
    /// and its overloads.
    /// </summary>
    public static class HasMultipartFormData
    {
        /// <summary>
        /// Tests the overload with no arguments.
        /// </summary>
        public static class NoArguments
        {
            /// <summary>
            /// Tests that the method raises an assertion if the request has no content.
            /// </summary>
            [Fact]
            public static void AssertsIfFieldRequestHasNoContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                Assert.Throws<NotNullException>(() => request.HasMultipartFormData());
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request content is not multipart content.
            /// </summary>
            [Fact]
            public static void AssertsIfFieldRequestIsNotMultipartContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new StringContent("fail");

                Assert.Throws<IsTypeException>(() => request.HasMultipartFormData());
            }
        }

        /// <summary>
        /// Tests the overload that deals with string fields.
        /// </summary>
        public static class StringField
        {
            /// <summary>
            /// Tests that the method raises an assertion if the request has no form data field with the correct name.
            /// </summary>
            [Fact]
            public static void AssertsIfRequestHasNoFormDataWithName()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                var multipart = new MultipartFormDataContent();
                multipart.Add(new StringContent("something"), "not value");

                request.Content = multipart;

                Assert.Throws<ContainsException>(() => request.HasMultipartFormData("value", "something"));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has a form data field with the correct name but
            /// the wrong type.
            /// </summary>
            [Fact]
            public static void AssertsIfRequestHasFormDataWithCorrectNameButWrongType()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                var multipart = new MultipartFormDataContent();
                multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "value");

                request.Content = multipart;

                Assert.Throws<IsTypeException>(() => request.HasMultipartFormData("value", "something"));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has a form data field with the correct name but
            /// the wrong value.
            /// </summary>
            [Fact]
            public static void AssertsIfRequestHasFormDataWithCorrectNameButWrongValue()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                var multipart = new MultipartFormDataContent();
                multipart.Add(new StringContent("not something"), "value");

                request.Content = multipart;

                Assert.Throws<EqualException>(() => request.HasMultipartFormData("value", "something"));
            }

            /// <summary>
            /// Tests that the method passes if the request has a form data field where the name and value match.
            /// </summary>
            [Fact]
            public static void PassesIfRequestHasFormDataWithCorrectNameAndCorrectValue()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                var multipart = new MultipartFormDataContent();
                multipart.Add(new StringContent("something"), "value");

                request.Content = multipart;

                request.HasMultipartFormData("value", "something");
            }
        }

        /// <summary>
        /// Tests the overload that deals with file fields.
        /// </summary>
        public static class File
        {
            /// <summary>
            /// Tests that the method raises an assertion if the request has no content.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestHasNoContent()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                Assert.Throws<NotNullException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request content is not multipart content.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestIsNotMultipartContent()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new StringContent("fail");

                Assert.Throws<IsTypeException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has no form data field with the correct name.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestHasNoFormDataWithName()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(stream), "not file");

                request.Content = multipart;

                Assert.Throws<ContainsException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has a form data field with the correct name but
            /// the wrong type.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongType()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                var multipart = new MultipartFormDataContent();
                multipart.Add(new ByteArrayContent(Array.Empty<byte>()), "file", "filename.txt");

                request.Content = multipart;

                Assert.Throws<IsTypeException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has a form data field with the correct name but
            /// the wrong value.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestHasFormDataWithCorrectNameAndValueButWrongFilename()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(stream), "file", "wrong.txt");

                request.Content = multipart;

                Assert.Throws<ContainsException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request has a form data field with the correct name but
            /// the wrong value.
            /// </summary>
            [Fact]
            public static void AssertsIfFileRequestHasFormDataWithCorrectNameAndFilenameButWrongValue()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(new MemoryStream()), "file", "filename.txt");

                request.Content = multipart;

                Assert.Throws<EqualException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
            }

            /// <summary>
            /// Tests that the method passes if the request has a form data field where the name and value match.
            /// </summary>
            [Fact]
            public static void PassesIfFileRequestHasFormDataWithCorrectNameAndFilenameAndValue()
            {
                using var stream = new MemoryStream();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                var multipart = new MultipartFormDataContent();
                multipart.Add(new StreamContent(stream), "file", "filename.txt");

                request.Content = multipart;

                request.HasMultipartFormData("file", "filename.txt", stream);
            }
        }
    }
}
