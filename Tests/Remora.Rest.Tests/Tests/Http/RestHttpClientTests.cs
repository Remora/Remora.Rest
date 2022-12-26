//
//  SPDX-FileName: RestHttpClientTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System.Net.Http;
using Remora.Rest.Tests.TestBases;

namespace Remora.Rest.Tests;

/// <summary>
/// Tests the <see cref="RestHttpClient{TError}"/> class.
/// </summary>
public class RestHttpClientTests
{
    /// <summary>
    /// Tests the <see cref="HttpMethod.Get"/> method.
    /// </summary>
    public class Get : RestHttpClientRequestTestBase
    {
        /// <inheritdoc />
        protected override HttpMethod RequestMethod => HttpMethod.Get;
    }

    /// <summary>
    /// Tests the <see cref="HttpMethod.Post"/> method.
    /// </summary>
    public class Post : RestHttpClientRequestTestBase
    {
        /// <inheritdoc />
        protected override HttpMethod RequestMethod => HttpMethod.Post;
    }

    /// <summary>
    /// Tests the <see cref="HttpMethod.Patch"/> method.
    /// </summary>
    public class Patch : RestHttpClientRequestTestBase
    {
        /// <inheritdoc />
        protected override HttpMethod RequestMethod => HttpMethod.Patch;
    }

    /// <summary>
    /// Tests the <see cref="HttpMethod.Delete"/> method.
    /// </summary>
    public class Delete : RestHttpClientRequestTestBase
    {
        /// <inheritdoc />
        protected override HttpMethod RequestMethod => HttpMethod.Delete;
    }

    /// <summary>
    /// Tests the <see cref="HttpMethod.Put"/> method.
    /// </summary>
    public class Put : RestHttpClientRequestTestBase
    {
        /// <inheritdoc />
        protected override HttpMethod RequestMethod => HttpMethod.Put;
    }
}
