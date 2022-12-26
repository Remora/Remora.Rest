//
//  SPDX-FileName: MockedRequestExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using JetBrains.Annotations;
using Remora.Rest.Xunit.Json;
using RichardSzalay.MockHttp;

namespace Remora.Rest.Xunit.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="MockHttpMessageHandler"/> class.
/// </summary>
[PublicAPI]
public static class MockedRequestExtensions
{
    /// <summary>
    /// Adds a requirement that the request has no content.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <returns>The request; with the new requirement.</returns>
    public static MockedRequest WithNoContent(this MockedRequest request)
    {
        return request.With(m =>
        {
            m.HasNoContent();
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has an authorization header.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="headerPredicate">The predicate check.</param>
    /// <returns>The request; with the new requirement.</returns>
    public static MockedRequest WithAuthentication
    (
        this MockedRequest request,
        Expression<Func<AuthenticationHeaderValue, bool>>? headerPredicate = null
    )
    {
        headerPredicate ??= _ => true;

        return request.With(m =>
        {
            m.HasAuthentication(headerPredicate);
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has a Json body.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the Json body.</param>
    /// <returns>The request; with the new requirements.</returns>
    public static MockedRequest WithJson
    (
        this MockedRequest request,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        return request.With(m =>
        {
            m.HasJson(elementMatcher.Build());
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the multipart request has a JSON payload.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="elementMatcherBuilder">The additional requirements on the JSON payload.</param>
    /// <returns>The request; with the new requirements.</returns>
    public static MockedRequest WithMultipartJsonPayload
    (
        this MockedRequest request,
        Action<JsonElementMatcherBuilder>? elementMatcherBuilder = null
    )
    {
        var elementMatcher = new JsonElementMatcherBuilder();
        elementMatcherBuilder?.Invoke(elementMatcher);

        return request.With(m =>
        {
            m.HasMultipartJsonPayload(elementMatcher.Build());
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has multipart form data with the given string field.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="name">The name of the form field.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithMultipartFormData
    (
        this MockedRequest request,
        string name,
        string value
    )
    {
        return request.With(m =>
        {
            m.HasMultipartFormData(name, value);
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has multipart form data with the given file-type stream field.
    /// </summary>
    /// <param name="request">The mocked request.</param>
    /// <param name="name">The name of the form field.</param>
    /// <param name="fileName">The filename of the field.</param>
    /// <param name="value">The value of the field.</param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithMultipartFormData
    (
        this MockedRequest request,
        string name,
        string fileName,
        Stream value
    )
    {
        return request.With(m =>
        {
            m.HasMultipartFormData(name, fileName, value);
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has URL-encoded form data.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithUrlEncodedFormData(this MockedRequest request)
    {
        return request.With(m =>
        {
            m.HasUrlEncodedFormData();
            return true;
        });
    }

    /// <summary>
    /// Adds a requirement that the request has URL-encoded form data that contains the given data.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="expectations">The expected values.</param>
    /// <param name="strict">
    /// true if the expected and actual sets must match in count as well; otherwise, false.
    /// </param>
    /// <returns>The request, with the new requirements.</returns>
    public static MockedRequest WithUrlEncodedFormData
    (
        this MockedRequest request,
        IReadOnlyDictionary<string, string> expectations,
        bool strict = false
    )
    {
        return request.With(m =>
        {
            m.HasUrlEncodedFormData(expectations, strict);
            return true;
        });
    }
}
