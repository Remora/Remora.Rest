//
//  HttpRequestMessageAssertions.cs
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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using Remora.Rest.Xunit.Json;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="HttpRequestMessage"/> class.
/// </summary>
[PublicAPI]
public static class HttpRequestMessageAssertions
{
    /// <summary>
    /// Asserts that the message does not contain any content.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasNoContent(this HttpRequestMessage message)
    {
        Assert.Null(message.Content);
    }

    /// <summary>
    /// Asserts that the message has an authorization header.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasAuthentication(this HttpRequestMessage message)
    {
        Assert.NotNull(message.Headers.Authorization);
    }

    /// <summary>
    /// Asserts that the message has an authorization header.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="headerPredicate">The predicate to check the header against.</param>
    public static void HasAuthentication
    (
        this HttpRequestMessage message,
        Func<AuthenticationHeaderValue, bool> headerPredicate
    )
    {
        message.HasAuthentication();
        Assert.True(headerPredicate(message.Headers.Authorization!), "The authentication predicate did not match.");
    }

    /// <summary>
    /// Asserts that the message has an authorization header.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="expectation">The expectation to check the header against.</param>
    public static void HasAuthentication
    (
        this HttpRequestMessage message,
        Action<AuthenticationHeaderValue> expectation
    )
    {
        message.HasAuthentication();
        expectation(message.Headers.Authorization!);
    }

    /// <summary>
    /// Asserts that the message has a JSON body.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasJson(this HttpRequestMessage message)
    {
        message.HasJson(out _);
    }

    /// <summary>
    /// Asserts that the message has a JSON body.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="json">The contained json body.</param>
    public static void HasJson(this HttpRequestMessage message, out JsonDocument json)
    {
        Assert.NotNull(message.Content);
        var content = message.Content!.ReadAsStreamAsync().GetAwaiter().GetResult();

        try
        {
            json = JsonDocument.Parse(content);
        }
        catch (JsonException)
        {
            throw new IsTypeException("JSON", "Unknown");
        }
    }

    /// <summary>
    /// Asserts that the message has a JSON body.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the JSON body.</param>
    public static void HasJson
    (
        this HttpRequestMessage message,
        Action<JsonElementMatcherBuilder> elementMatcherBuilder
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder.Invoke(elementMatcher);

        var matcher = elementMatcher.Build();
        message.HasJson(matcher);
    }

    /// <summary>
    /// Asserts that the message has a JSON body.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="matcher">The additional requirements on the Json body.</param>
    public static void HasJson
    (
        this HttpRequestMessage message,
        JsonElementMatcher matcher
    )
    {
        message.HasJson(out var json);

        if (matcher is not null)
        {
            Assert.True(matcher.Matches(json.RootElement));
        }
    }

    /// <summary>
    /// Asserts that the message has a multipart payload with a JSON part.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="partName">The name of the JSON part.</param>
    public static void HasMultipartJsonPayload
    (
        this HttpRequestMessage message,
        string partName = "payload_json"
    )
    {
        message.HasMultipartJsonPayload(out _, partName);
    }

    /// <summary>
    /// Asserts that the message has a multipart payload with a JSON part.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="json">The contained json body.</param>
    /// <param name="partName">The name of the JSON part.</param>
    public static void HasMultipartJsonPayload
    (
        this HttpRequestMessage message,
        out JsonDocument json,
        string partName = "payload_json"
    )
    {
        Assert.NotNull(message.Content);
        Assert.IsType<MultipartFormDataContent>(message.Content);
        var multipart = (MultipartFormDataContent)message.Content!;

        Assert.Single
        (
            multipart,
            c => c is StringContent s && s.Headers.ContentDisposition?.Name == partName
        );

        var payloadContent = multipart.Single
        (
            c => c is StringContent s && s.Headers.ContentDisposition?.Name == partName
        );

        var content = payloadContent.ReadAsStreamAsync().GetAwaiter().GetResult();

        try
        {
            json = JsonDocument.Parse(content);
        }
        catch (JsonException)
        {
            throw new IsTypeException("JSON", "Unknown");
        }
    }

    /// <summary>
    /// Asserts that the message has a multipart payload with a JSON part.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the JSON body.</param>
    /// <param name="partName">The name of the JSON part.</param>
    public static void HasMultipartJsonPayload
    (
        this HttpRequestMessage message,
        Action<JsonElementMatcherBuilder> elementMatcherBuilder,
        string partName = "payload_json"
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder(elementMatcher);

        var matcher = elementMatcher.Build();

        message.HasMultipartJsonPayload(matcher, partName);
    }

    /// <summary>
    /// Asserts that the message has a multipart payload with a JSON part.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="matcher">The additional requirements on the JSON body.</param>
    /// <param name="partName">The name of the JSON part.</param>
    public static void HasMultipartJsonPayload
    (
        this HttpRequestMessage message,
        JsonElementMatcher matcher,
        string partName = "payload_json"
    )
    {
        message.HasMultipartJsonPayload(out var json, partName);
        Assert.True(matcher.Matches(json.RootElement));
    }

    /// <summary>
    /// Asserts that the message has a a multipart payload.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasMultipartFormData(this HttpRequestMessage message)
    {
        Assert.NotNull(message.Content);
        Assert.IsType<MultipartFormDataContent>(message.Content);
    }

    /// <summary>
    /// Asserts that the message has a a multipart payload with a specific named part that passes the given assertion.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="name">The name of the part.</param>
    /// <param name="expectation">The assertion to make on the value.</param>
    /// <typeparam name="TContent">The content type the part should be.</typeparam>
    public static void HasMultipartFormData<TContent>
    (
        this HttpRequestMessage message,
        string name,
        Action<TContent>? expectation = null
    )
        where TContent : HttpContent
    {
        message.HasMultipartFormData();

        var formContent = (MultipartFormDataContent)message.Content!;
        Assert.Contains(formContent, c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}"))));

        var contentWithName = formContent.Single
        (
            c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}")))
        );

        Assert.IsType<TContent>(contentWithName);
        expectation?.Invoke((TContent)contentWithName);
    }

    /// <summary>
    /// Asserts that the message has a a multipart payload with a specific named string.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="name">The name of the part.</param>
    /// <param name="expected">The expected value.</param>
    public static void HasMultipartFormData
    (
        this HttpRequestMessage message,
        string name,
        string expected
    )
    {
        message.HasMultipartFormData<StringContent>(name, c =>
        {
            var actualValue = c.ReadAsStringAsync().GetAwaiter().GetResult();
            Assert.Equal(expected, actualValue);
        });
    }

    /// <summary>
    /// Asserts that the message has a a multipart payload with a specific named file.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="name">The name of the part.</param>
    /// <param name="expectedFilename">The expected name of the file.</param>
    /// <param name="expectedContent">The expected contents of the file.</param>
    public static void HasMultipartFormData
    (
        this HttpRequestMessage message,
        string name,
        string expectedFilename,
        Stream expectedContent
    )
    {
        message.HasMultipartFormData<StreamContent>(name, c =>
        {
            Assert.Contains(c.Headers, h => h.Value.Any(v => v.Contains($"filename={expectedFilename}")));

            // Reflection hackery
            var innerStream = (Stream)typeof(StreamContent)
                .GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(c)!;

            Assert.Equal(expectedContent, innerStream);
        });
    }
}
