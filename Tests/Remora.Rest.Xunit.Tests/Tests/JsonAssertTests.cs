//
//  JsonAssertTests.cs
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

using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Sdk;

namespace Remora.Rest.Xunit.Tests;

/// <summary>
/// Tests the <see cref="JsonAssert"/> class.
/// </summary>
public static class JsonAssertTests
{
    /// <summary>
    /// Tests the <see cref="JsonAssert.Equivalent(JsonDocument, JsonDocument, JsonAssertOptions)"/> method and its
    /// overloads.
    /// </summary>
    public static class Equivalent
    {
        /// <summary>
        /// Tests whether the assertion passes for equivalent JSON documents.
        /// </summary>
        /// <param name="expected">The expected document.</param>
        /// <param name="actual">The actual document.</param>
        [ClassData(typeof(EquivalentJsonDocumentData))]
        [Theory]
        public static void PassesForEquivalentDocuments(JsonDocument expected, JsonDocument actual)
        {
            JsonAssert.Equivalent(expected, actual);
        }

        /// <summary>
        /// Tests whether the assertion passes for equivalent JSON documents.
        /// </summary>
        /// <param name="expected">The expected document.</param>
        /// <param name="actual">The actual document.</param>
        [ClassData(typeof(NonEquivalentJsonDocumentData))]
        [Theory]
        public static void AssertsForNonEquivalentDocuments(JsonDocument expected, JsonDocument actual)
        {
            Assert.ThrowsAny<XunitException>(() => JsonAssert.Equivalent(expected, actual));
        }

        /// <summary>
        /// Tests whether the assertion passes when a property is missing, but its name has been exempted from the
        /// missing property check.
        /// </summary>
        [Fact]
        public static void PassesOnMissingPropertyWhenNameIsExempted()
        {
            var options = new JsonAssertOptions
            {
                AllowMissing = new[] { "other" }
            };

            var expected = JsonDocument.Parse
            (
                "{ \"property\": 1, \"other\": true }"
            );

            var actual = JsonDocument.Parse
            (
                "{ \"property\": 1 }"
            );

            JsonAssert.Equivalent(expected, actual, options);
        }

        /// <summary>
        /// Tests whether the assertion passes when a property is missing, but its name has been exempted from the
        /// missing property check by a function.
        /// </summary>
        [Fact]
        public static void PassesOnMissingPropertyWhenPatternIsExempted()
        {
            var options = new JsonAssertOptions
            {
                AllowMissingBy = p => p.NameEquals("other")
            };

            var expected = JsonDocument.Parse
            (
                "{ \"property\": 1, \"other\": true }"
            );

            var actual = JsonDocument.Parse
            (
                "{ \"property\": 1 }"
            );

            JsonAssert.Equivalent(expected, actual, options);
        }
    }

    private class EquivalentJsonDocumentData : TheoryData<JsonDocument, JsonDocument>
    {
        public EquivalentJsonDocumentData()
        {
            // pure values
            Add(JsonDocument.Parse("\"ooga\""), JsonDocument.Parse("\"ooga\""));
            Add(JsonDocument.Parse("0"), JsonDocument.Parse("0"));
            Add(JsonDocument.Parse("0.0"), JsonDocument.Parse("0.0"));
            Add(JsonDocument.Parse("true"), JsonDocument.Parse("true"));
            Add(JsonDocument.Parse("false"), JsonDocument.Parse("false"));
            Add(JsonDocument.Parse("null"), JsonDocument.Parse("null"));

            // empty arrays and objects
            Add(JsonDocument.Parse("[ ]"), JsonDocument.Parse("[ ]"));
            Add(JsonDocument.Parse("{ }"), JsonDocument.Parse("{ }"));

            // exactly equivalent arrays and objects
            Add(JsonDocument.Parse("[ 1 ]"), JsonDocument.Parse("[ 1 ]"));
            Add(JsonDocument.Parse("{ \"property\": 1 }"), JsonDocument.Parse("{ \"property\": 1 }"));
            Add
            (
                JsonDocument.Parse
                (
                    "{ \"property\": 1, \"other\": true }"
                ),
                JsonDocument.Parse
                (
                    "{ \"property\": 1, \"other\": true }"
                )
            );

            // reordered objects
            Add
            (
                JsonDocument.Parse
                (
                    "{ \"property\": 1, \"other\": true }"
                ),
                JsonDocument.Parse
                (
                    "{ \"other\": true, \"property\": 1 }"
                )
            );

            // objects with more properties
            Add
            (
                JsonDocument.Parse
                (
                    "{ \"property\": 1 }"
                ),
                JsonDocument.Parse
                (
                    "{ \"property\": 1, \"other\": true }"
                )
            );
        }
    }

    private class NonEquivalentJsonDocumentData : TheoryData<JsonDocument, JsonDocument>
    {
        public NonEquivalentJsonDocumentData()
        {
            // value differences
            Add(JsonDocument.Parse("\"ooga\""), JsonDocument.Parse("\"boga\""));
            Add(JsonDocument.Parse("0"), JsonDocument.Parse("1"));
            Add(JsonDocument.Parse("0.0"), JsonDocument.Parse("0.1"));
            Add(JsonDocument.Parse("true"), JsonDocument.Parse("false"));

            // type differences
            var values = new[]
            {
                "\"ooga\"",
                "0",
                "true",
                "null"
            };

            foreach (var value in values)
            {
                var otherValues = values.Except(new[] { value });
                foreach (var otherValue in otherValues)
                {
                    Add(JsonDocument.Parse(value), JsonDocument.Parse(otherValue));
                }
            }

            // arrays and objects with value differences
            Add(JsonDocument.Parse("[ 1 ]"), JsonDocument.Parse("[ 2 ]"));
            Add(JsonDocument.Parse("{ \"property\": 1 }"), JsonDocument.Parse("{ \"property\": 2 }"));

            // arrays with count differences
            Add(JsonDocument.Parse("[ 1 ]"), JsonDocument.Parse("[ 1, 2 ]"));

            // reordered arrays
            Add(JsonDocument.Parse("[ 1, 2 ]"), JsonDocument.Parse("[ 2, 1 ]"));

            // objects with missing properties
            Add
            (
                JsonDocument.Parse
                (
                    "{ \"property\": 1, \"other\": true }"
                ),
                JsonDocument.Parse
                (
                    "{ \"property\": 1 }"
                )
            );
        }
    }
}
