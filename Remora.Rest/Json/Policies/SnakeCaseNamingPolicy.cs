//
//  SnakeCaseNamingPolicy.cs
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

// This code uses modified portion of MIT licensed Newtonsoft.Json source code
//
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using System.Text;
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
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var sb = new StringBuilder(name.Length + (name.Length / 2));
        var state = SeparatedCaseState.Start;

        for (var i = 0; i < name.Length; i++)
        {
            if (name[i] == ' ')
            {
                if (state != SeparatedCaseState.Start)
                {
                    state = SeparatedCaseState.NewWord;
                }
            }
            else if (char.IsUpper(name[i]))
            {
                switch (state)
                {
                    case SeparatedCaseState.Upper:
                        var hasNext = i + 1 < name.Length;
                        if (i > 0 && hasNext)
                        {
                            var nextChar = name[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != '_')
                            {
                                sb.Append('_');
                            }
                        }

                        break;
                    case SeparatedCaseState.Lower:
                    case SeparatedCaseState.NewWord:
                        sb.Append('_');
                        break;
                }

                var c = !_upperCase ? char.ToLowerInvariant(name[i]) : char.ToUpperInvariant(name[i]);

                sb.Append(c);

                state = SeparatedCaseState.Upper;
            }
            else if (name[i] == '_')
            {
                sb.Append('_');
                state = SeparatedCaseState.Start;
            }
            else
            {
                if (state == SeparatedCaseState.NewWord)
                {
                    sb.Append('_');
                }

                sb.Append(!_upperCase ? name[i] : char.ToUpperInvariant(name[i]));
                state = SeparatedCaseState.Lower;
            }
        }

        return sb.ToString();
    }

    private enum SeparatedCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }
}
