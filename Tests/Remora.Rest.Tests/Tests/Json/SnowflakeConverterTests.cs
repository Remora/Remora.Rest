//
//  SnowflakeConverterTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
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
using System.Text;
using System.Text.Json;
using Remora.Rest.Core;
using Remora.Rest.Json;
using Xunit;

namespace Remora.Rest.Tests.Json;

/// <summary>
/// Tests the <see cref="SnowflakeConverter"/> class.
/// </summary>
public class SnowflakeConverterTests
{
    /// <summary>
    /// Tests the <see cref="SnowflakeConverter.Read"/> method.
    /// </summary>
    public class Read
    {
        /// <summary>
        /// Tests whether the converter successfully reads well-formatted snowflakes.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("\"999999999999999999\"")]
        [InlineData("99999999999999999")]
        public void SuccessfullyReadsSnowflakes(string json)
        {
            var data = Encoding.UTF8.GetBytes(json);
            var reader = new Utf8JsonReader(data);
            reader.Read();

            var converter = new SnowflakeConverter(0);
            converter.Read(ref reader, typeof(Snowflake), new JsonSerializerOptions());
        }

        /// <summary>
        /// Tests whether the converter asserts if the data is not a string or a number.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("true")]
        [InlineData("false")]
        public void AssertsIfValueIsNotStringOrNumber(string json)
        {
            var converter = new SnowflakeConverter(0);
            Assert.Throws<JsonException>(() =>
            {
                var data = Encoding.UTF8.GetBytes(json);
                var reader = new Utf8JsonReader(data);
                reader.Read();
                return converter.Read(ref reader, typeof(Snowflake), new JsonSerializerOptions());
            });
        }

        /// <summary>
        /// Tests whether the converter asserts if the data is unrepresentable as a snowflake.
        /// </summary>
        /// <param name="json">The JSON to test.</param>
        [Theory]
        [InlineData("-1")]
        [InlineData("\"-1\"")]
        [InlineData("18446744073709551616")]
        [InlineData("\"18446744073709551616\"")]
        public void AssertsIfValueIsUnrepresentableAsSnowflake(string json)
        {
            var converter = new SnowflakeConverter(0);
            Assert.ThrowsAny<Exception>(() =>
            {
                var data = Encoding.UTF8.GetBytes(json);
                var reader = new Utf8JsonReader(data);
                reader.Read();
                return converter.Read(ref reader, typeof(Snowflake), new JsonSerializerOptions());
            });
        }
    }

    /// <summary>
    /// Tests the <see cref="SnowflakeConverter.Write"/> method.
    /// </summary>
    public class Write
    {
        /// <summary>
        /// Tests whether the method correctly formats an output snowflake.
        /// </summary>
        [Fact]
        public void WritesCorrectlyFormattedData()
        {
            using var stream = new MemoryStream();

            using (var writer = new Utf8JsonWriter(stream))
            {
                var snowflake = new Snowflake(999999999999999999);
                var converter = new SnowflakeConverter(0);
                converter.Write(writer, snowflake, new JsonSerializerOptions());
            }

            // Rewind and read
            stream.Seek(0, SeekOrigin.Begin);
            var document = JsonDocument.Parse(stream);

            var actual = document.RootElement.GetRawText();
            Assert.Equal("\"999999999999999999\"", actual);
        }
    }
}
