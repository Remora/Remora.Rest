//
//  MockedRequestExtensions.cs
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
        Func<AuthenticationHeaderValue, bool>? headerPredicate = null
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
}
