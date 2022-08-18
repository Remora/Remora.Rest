//
//  SnakeCaseNamingPolicy.cs
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
using System.Buffers;
using System.Text.Json;
using JetBrains.Annotations;

namespace Remora.Rest.Json.Policies;

/// <summary>
/// Represents a snake_case naming policy.
/// </summary>
[PublicAPI]
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    private readonly bool _upperCase;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeCaseNamingPolicy"/> class.
    /// </summary>
    /// <param name="upperCase">Whether the converted names should be in all upper case.</param>
    public SnakeCaseNamingPolicy(bool upperCase = false)
    {
        _upperCase = upperCase;
    }

    /// <inheritdoc />
    public override string ConvertName(string input) => _upperCase
        ? FastSnakeCaser.Snake(input).ToUpperInvariant()
        : FastSnakeCaser.Snake(input);

    /// <summary>
    /// Converts C# identifiers to snake_case strings.
    /// </summary>
    /// <remarks>
    /// This code is partially based on JSON.NET's implementation, with additional safety features and flexibility.
    /// </remarks>
    private static class FastSnakeCaser
    {
        private const byte _stackAllocationCap = 128;

        /// <summary>
        /// Converts the input string to its equivalent snake_case representation.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The snake_case representation.</returns>
        public static string Snake(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var outputSize = input.Length + (input.Length / 2);
            return outputSize <= _stackAllocationCap
                ? SnakeStackAlloc(input, outputSize)
                : SnakeHeapAlloc(input, outputSize);
        }

        /// <summary>
        /// Converts the input string to its equivalent snake_case representation using stack-allocated buffers.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="outputSize">The maximum size of the output string.</param>
        /// <returns>The snake_case representation.</returns>
        private static string SnakeStackAlloc(string input, int outputSize)
        {
            Span<char> output = stackalloc char[outputSize];
            var length = SnakeCore(input, output);
            return new(output[..length]);
        }

        /// <summary>
        /// Converts the input string to its equivalent snake_case representation using rented heap-allocated arrays.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="outputSize">The maximum size of the output string.</param>
        /// <returns>The snake_case representation.</returns>
        private static string SnakeHeapAlloc(string input, int outputSize)
        {
            var output = ArrayPool<char>.Shared.Rent(outputSize);
            try
            {
                var length = SnakeCore(input, output);
                return new string(output[..length]);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(output);
            }
        }

        /// <summary>
        /// Converts the input span to its equivalent snake_case representation.
        /// </summary>
        /// <param name="input">The input span.</param>
        /// <param name="output">
        /// The output span. This span must be large enough to accommodate the resulting representation.
        /// </param>
        /// <returns>
        /// The length of the resulting representation. This value may be smaller than the size of the output buffer.
        /// </returns>
        private static int SnakeCore(ReadOnlySpan<char> input, Span<char> output)
        {
            var index = 0;
            var state = InputState.Start;

            for (var i = 0; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                {
                    switch (state)
                    {
                        case InputState.Upper:
                        {
                            var hasNext = i + 1 < input.Length;
                            if (i > 0 && hasNext)
                            {
                                var nextChar = input[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_')
                                {
                                    output[index] = '_';
                                    index++;
                                }
                            }

                            break;
                        }
                        case InputState.Lower:
                        {
                            output[index] = '_';
                            index++;
                            break;
                        }
                    }

                    var c = char.ToLowerInvariant(input[i]);

                    output[index] = c;
                    index++;

                    state = InputState.Upper;
                }
                else if (input[i] == '_')
                {
                    output[index] = '_';
                    index++;
                    state = InputState.Start;
                }
                else
                {
                    output[index] = input[i];
                    index++;
                    state = InputState.Lower;
                }
            }

            return index;
        }

        private enum InputState
        {
            Start,
            Lower,
            Upper
        }
    }
}
