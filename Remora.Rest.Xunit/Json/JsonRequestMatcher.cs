//
//  JsonRequestMatcher.cs
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

using System.Net.Http;
using System.Text.Json;
using JetBrains.Annotations;
using RichardSzalay.MockHttp;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Json
{
    /// <inheritdoc />
    [PublicAPI]
    public class JsonRequestMatcher : IMockedRequestMatcher
    {
        private readonly JsonElementMatcher _elementMatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRequestMatcher"/> class.
        /// </summary>
        /// <param name="elementMatcher">The underlying element matcher.</param>
        public JsonRequestMatcher(JsonElementMatcher elementMatcher)
        {
            _elementMatcher = elementMatcher;
        }

        /// <inheritdoc />
        public bool Matches(HttpRequestMessage message)
        {
            Assert.NotNull(message.Content);

            var content = message.Content!.ReadAsStreamAsync().GetAwaiter().GetResult();
            try
            {
                using var json = JsonDocument.Parse(content);
                return _elementMatcher.Matches(json.RootElement);
            }
            catch (JsonException)
            {
                throw new IsTypeException("JSON", "Unknown");
            }
        }
    }
}
