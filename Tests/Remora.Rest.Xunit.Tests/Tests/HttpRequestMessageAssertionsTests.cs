//
//  SPDX-FileName: HttpRequestMessageAssertionsTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
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

            Assert.Throws<XunitException>(() => request.HasNoContent());
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

            Assert.Throws<XunitException>(() => request.HasAuthentication());
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

            Assert.Throws<EqualException>
            (
                () => request.HasAuthentication(v => Assert.Equal("unexpected", v.Parameter))
            );
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

            Assert.Throws<XunitException>(() => request.HasAuthentication(v => v.Parameter == "unexpected"));
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

            Assert.Throws<XunitException>(() => request.HasJson());
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

            Assert.Throws<XunitException>(() => request.HasJson(j => j.IsArray()));
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

            Assert.Throws<XunitException>(() => request.HasMultipartJsonPayload());
        }

        /// <summary>
        /// Tests that the method raises an assertion if the request lacks multipart content.
        /// </summary>
        [Fact]
        public static void AssertsIfRequestHasNonMultipartContent()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
            request.Content = new StringContent("ooga");

            Assert.Throws<XunitException>(() => request.HasMultipartJsonPayload());
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

            Assert.Throws<XunitException>(() => request.HasMultipartJsonPayload());
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

            Assert.Throws<XunitException>(() => request.HasMultipartJsonPayload(j => j.IsArray()));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData());
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request content is not multipart content.
            /// </summary>
            [Fact]
            public static void AssertsIfFieldRequestIsNotMultipartContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new StringContent("fail");

                Assert.Throws<XunitException>(() => request.HasMultipartFormData());
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("value", "something"));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("value", "something"));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("value", "something"));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

                Assert.Throws<XunitException>(() => request.HasMultipartFormData("file", "filename.txt", stream));
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

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.HttpRequestMessageAssertions.HasUrlEncodedFormData(HttpRequestMessage)"/> method
    /// and its overloads.
    /// </summary>
    public static class HasUrlEncodedFormData
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
            public static void AssertsIfRequestHasNoContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                Assert.Throws<XunitException>(() => request.HasUrlEncodedFormData());
            }

            /// <summary>
            /// Tests that the method raises an assertion if the request content is not URL-encoded form data content.
            /// </summary>
            [Fact]
            public static void AssertsIfRequestContainsNotUrlEncodedFormDataContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new StringContent("fail");

                Assert.Throws<XunitException>(() => request.HasUrlEncodedFormData());
            }

            /// <summary>
            /// Tests that the method passes if the request content is URL-encoded form data content.
            /// </summary>
            [Fact]
            public static void PassesIfRequestContainsUrlEncodedFormDataContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(Array.Empty<KeyValuePair<string, string>>());

                request.HasUrlEncodedFormData();
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
            [Fact]
            public static void AssertsIfKeyIsMissing()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                Assert.Throws<XunitException>(() => request.HasUrlEncodedFormData(new Dictionary<string, string>
                {
                    { "other", "value" }
                }));
            }

            /// <summary>
            /// Tests that the method raises an assertion if an expected value differs.
            /// </summary>
            [Fact]
            public static void AssertsIfValueDiffers()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                Assert.Throws<XunitException>(() => request.HasUrlEncodedFormData(new Dictionary<string, string>
                {
                    { "some", "type" }
                }));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the content contains more than the expected data and strict
            /// checking is enabled.
            /// </summary>
            [Fact]
            public static void AssertsIfContentContainsMoreAndExpectationsAreStrict()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value"),
                    new("other", "thing")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                Assert.Throws<XunitException>(() => request.HasUrlEncodedFormData
                (
                    new Dictionary<string, string>
                    {
                        { "some", "value" }
                    },
                    true
                ));
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqual()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                request.HasUrlEncodedFormData(new Dictionary<string, string>
                {
                    { "some", "value" }
                });
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqualAndExpectationsAreStrict()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                request.HasUrlEncodedFormData
                (
                    new Dictionary<string, string>
                    {
                        { "some", "value" }
                    },
                    true
                );
            }

            /// <summary>
            /// Tests that the method passes if the content contains more than the expected data and strict checking is
            /// disabled.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsMoreAndExpectationsAreNotStrict()
            {
                var actual = new KeyValuePair<string, string>[]
                {
                    new("some", "value"),
                    new("other", "thing")
                };

                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");
                request.Content = new FormUrlEncodedContent(actual);

                request.HasUrlEncodedFormData(new Dictionary<string, string>
                {
                    { "some", "value" }
                });
            }
        }
    }

    /// <summary>
    /// Tests the
    /// <see cref="Extensions.HttpRequestMessageAssertions.HasQueryParameters(HttpRequestMessage)"/> method
    /// and its overloads.
    /// </summary>
    public static class HasQueryParameters
    {
        /// <summary>
        /// Tests the overload with no arguments.
        /// </summary>
        public static class NoArguments
        {
            /// <summary>
            /// Tests that the method raises an assertion if the request has no query parameters.
            /// </summary>
            [Fact]
            public static void AssertsIfRequestHasNoQueryParameters()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test");

                Assert.Throws<XunitException>(() => request.HasQueryParameters());
            }

            /// <summary>
            /// Tests that the method passes if the request content is URL-encoded form data content.
            /// </summary>
            [Fact]
            public static void PassesIfRequestContainsUrlEncodedFormDataContent()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?query=parameter");

                request.HasQueryParameters();
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
            [Fact]
            public static void AssertsIfKeyIsMissing()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?query=parameter");

                Assert.Throws<XunitException>(() => request.HasQueryParameters(new Dictionary<string, string>
                {
                    { "other", "value" }
                }));
            }

            /// <summary>
            /// Tests that the method raises an assertion if an expected value differs.
            /// </summary>
            [Fact]
            public static void AssertsIfValueDiffers()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?some=value");

                Assert.Throws<XunitException>(() => request.HasQueryParameters(new Dictionary<string, string>
                {
                    { "some", "type" }
                }));
            }

            /// <summary>
            /// Tests that the method raises an assertion if the query string contains more than the expected data and
            /// strict checking is enabled.
            /// </summary>
            [Fact]
            public static void AssertsIfContentContainsMoreAndExpectationsAreStrict()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?some=value&other=thing");

                Assert.Throws<XunitException>(() => request.HasQueryParameters
                (
                    new Dictionary<string, string>
                    {
                        { "some", "value" }
                    },
                    true
                ));
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqual()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?some=value");

                request.HasQueryParameters(new Dictionary<string, string>
                {
                    { "some", "value" }
                });
            }

            /// <summary>
            /// Tests that the method passes if all expected keys are present and all expected values are equal.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsAllExpectedKeysAndAllValuesAreEqualAndExpectationsAreStrict()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?some=value");

                request.HasQueryParameters
                (
                    new Dictionary<string, string>
                    {
                        { "some", "value" }
                    },
                    true
                );
            }

            /// <summary>
            /// Tests that the method passes if the query string contains more than the expected data and strict
            /// checking is disabled.
            /// </summary>
            [Fact]
            public static void PassesIfContentContainsMoreAndExpectationsAreNotStrict()
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://unit-test?some=value&other=thing");

                request.HasQueryParameters(new Dictionary<string, string>
                {
                    { "some", "value" }
                });
            }
        }
    }
}
