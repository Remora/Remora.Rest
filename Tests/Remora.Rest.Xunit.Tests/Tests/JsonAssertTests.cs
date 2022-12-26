//
//  SPDX-FileName: JsonAssertTests.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
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
        /// <param name="expectedError">A substring which is expected in the final error message.</param>
        [ClassData(typeof(NonEquivalentJsonDocumentData))]
        [Theory]
        public static void AssertsForNonEquivalentDocuments
        (
            JsonDocument expected,
            JsonDocument actual,
            string expectedError
        )
        {
            var exception = Assert.ThrowsAny<XunitException>(() => JsonAssert.Equivalent(expected, actual));
            Assert.Contains(expectedError, exception.Message);
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

    private class NonEquivalentJsonDocumentData : TheoryData<JsonDocument, JsonDocument, string>
    {
        public NonEquivalentJsonDocumentData()
        {
            // value differences
            Add(JsonDocument.Parse("\"ooga\""), JsonDocument.Parse("\"boga\""), "be equivalent to \"ooga\"");
            Add(JsonDocument.Parse("0"), JsonDocument.Parse("1"), "to be 0.0");
            Add(JsonDocument.Parse("0.0"), JsonDocument.Parse("0.1"), "to be 0.0");
            Add(JsonDocument.Parse("true"), JsonDocument.Parse("false"), "actual.ValueKind to be JsonValueKind.True");

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
                    Add(JsonDocument.Parse(value), JsonDocument.Parse(otherValue), "actual.ValueKind to be");
                }
            }

            // arrays and objects with value differences
            Add(JsonDocument.Parse("[ 1 ]"), JsonDocument.Parse("[ 2 ]"), "to be 1.0");
            Add(JsonDocument.Parse("{ \"property\": 1 }"), JsonDocument.Parse("{ \"property\": 2 }"), "to be 1.0");

            // arrays with count differences
            Add(JsonDocument.Parse("[ 1 ]"), JsonDocument.Parse("[ 1, 2 ]"), "to have 1 item");

            // reordered arrays
            Add(JsonDocument.Parse("[ 1, 2 ]"), JsonDocument.Parse("[ 2, 1 ]"), "to be 1.0");

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
                ),
                "item matching e.NameEquals(\"other\")"
            );
        }
    }
}
