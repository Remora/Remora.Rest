//
//  SPDX-FileName: HttpRequestMessageAssertions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Web;
using FluentAssertions;
using JetBrains.Annotations;
using Remora.Rest.Xunit.Json;
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
        message.Content
            .Should().BeNull();
    }

    /// <summary>
    /// Asserts that the message has an authorization header.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasAuthentication(this HttpRequestMessage message)
    {
        message.Headers.Authorization
            .Should().NotBeNull();
    }

    /// <summary>
    /// Asserts that the message has an authorization header.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="headerPredicate">The predicate to check the header against.</param>
    public static void HasAuthentication
    (
        this HttpRequestMessage message,
        Expression<Func<AuthenticationHeaderValue, bool>> headerPredicate
    )
    {
        message.HasAuthentication();
        message.Headers.Authorization.Should().Match(headerPredicate);
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
        message.Content.Should().NotBeNull();
        var content = message.Content!.ReadAsStream();

        try
        {
            json = JsonDocument.Parse(content);
        }
        catch (JsonException)
        {
            throw IsTypeException.ForMismatchedType("JSON", "Unknown");
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
        json.RootElement
            .Should().Match<JsonElement>(e => matcher.Matches(e));
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
        message.Content
            .Should().NotBeNull();

        message.Content
            .Should().BeOfType<MultipartFormDataContent>();

        var multipart = (MultipartFormDataContent)message.Content!;

        multipart
            .Should().ContainSingle(c => c is StringContent)
                .Which
                .Should().BeOfType<StringContent>()
                    .Which.Headers.ContentDisposition!.Name
                    .Should().NotBeNull().And.Be(partName);

        var payloadContent = multipart.Single
        (
            c => c is StringContent s && s.Headers.ContentDisposition?.Name == partName
        );

        var content = payloadContent.ReadAsStream();

        try
        {
            json = JsonDocument.Parse(content);
        }
        catch (JsonException)
        {
            throw IsTypeException.ForMismatchedType("JSON", "Unknown");
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
        json.RootElement.Should().Match<JsonElement>(e => matcher.Matches(e));
    }

    /// <summary>
    /// Asserts that the message has a a multipart payload.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasMultipartFormData(this HttpRequestMessage message)
    {
        message.Content
            .Should().NotBeNull();

        message.Content
            .Should().BeOfType<MultipartFormDataContent>();
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
        formContent
            .Should().Contain(c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}"))));

        var contentWithName = formContent.Single
        (
            c => c.Headers.Any(h => h.Value.Any(v => v.Contains($"name={name}")))
        );

        contentWithName
            .Should().BeOfType<TContent>();

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
            using var stream = c.ReadAsStream();
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            content.Should().Be(expected);
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
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            c.Headers
                .Should().Contain(h => h.Value.Any(v => v.Contains($"filename={expectedFilename}")));

            // Reflection hackery
            var innerStream = (Stream)typeof(StreamContent)
                .GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(c)!;

            innerStream
                .Should().BeSameAs(expectedContent);
        });
    }

    /// <summary>
    /// Asserts that the message has URL-encoded form data as its content.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasUrlEncodedFormData(this HttpRequestMessage message)
    {
        message.HasUrlEncodedFormData(out _);
    }

    /// <summary>
    /// Asserts that the message has URL-encoded form data as its content.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="data">The decoded form data.</param>
    public static void HasUrlEncodedFormData
    (
        this HttpRequestMessage message,
        out IReadOnlyDictionary<string, string> data
    )
    {
        message.Content
            .Should().NotBeNull();

        message.Content
            .Should().BeOfType<FormUrlEncodedContent>();

        var formContent = (FormUrlEncodedContent)message.Content!;

        using var stream = formContent.ReadAsStream();
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        var collection = HttpUtility.ParseQueryString(content);

        data = collection.AllKeys.ToDictionary
        (
            key => key ?? throw new InvalidOperationException(),
            key => collection[key] ?? throw new InvalidOperationException()
        );
    }

    /// <summary>
    /// Asserts that the message has URL-encoded form data as its content.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="expectations">The expected values.</param>
    /// <param name="strict">
    /// true if the expected and actual sets must match in count as well; otherwise, false.
    /// </param>
    public static void HasUrlEncodedFormData
    (
        this HttpRequestMessage message,
        IReadOnlyDictionary<string, string> expectations,
        bool strict = false
    )
    {
        message.HasUrlEncodedFormData(out var data);

        if (strict)
        {
            expectations.Should().BeEquivalentTo(data);
        }
        else
        {
            expectations.Should().BeSubsetOf(data);
        }
    }

    /// <summary>
    /// Asserts that the message has URL-encoded query parameters.
    /// </summary>
    /// <param name="message">The message.</param>
    public static void HasQueryParameters(this HttpRequestMessage message)
    {
        message.HasQueryParameters(out _);
    }

    /// <summary>
    /// Asserts that the message has URL-encoded query parameters.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="data">The decoded form data.</param>
    public static void HasQueryParameters
    (
        this HttpRequestMessage message,
        out IReadOnlyDictionary<string, string> data
    )
    {
        message.RequestUri?.Query
            .Should().NotBeNull().And
            .NotBeEmpty();

        var collection = HttpUtility.ParseQueryString(message.RequestUri!.Query);
        data = collection.AllKeys.ToDictionary
        (
            key => key ?? throw new InvalidOperationException(),
            key => collection[key] ?? throw new InvalidOperationException()
        );
    }

    /// <summary>
    /// Asserts that the message has URL-encoded form data as its content.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="expectations">The expected values.</param>
    /// <param name="strict">
    /// true if the expected and actual sets must match in count as well; otherwise, false.
    /// </param>
    public static void HasQueryParameters
    (
        this HttpRequestMessage message,
        IReadOnlyDictionary<string, string> expectations,
        bool strict = false
    )
    {
        message.HasQueryParameters(out var data);

        if (strict)
        {
            expectations.Should().BeEquivalentTo(data);
        }
        else
        {
            expectations.Should().BeSubsetOf(data);
        }
    }
}
