//
//  IComplexOneOfData.cs
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

using System.Collections.Generic;
using OneOf;

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data interface that contains a OneOf member.
/// </summary>
public interface IComplexOneOfData
{
    /// <summary>
    /// Gets an optional string.
    /// </summary>
    OneOf<ApplicationCommandData, MessageComponentData> Value { get; }
}

/// <summary>
/// A sister object for the <see cref="IComplexOneOfData"/> type.
/// </summary>
/// <param name="ID">An ID value.</param>
/// <param name="Name">A name value.</param>
/// <param name="Type">A type value.</param>
/// <param name="Options">A list of option values.</param>
public record ApplicationCommandData
(
    ulong ID,
    string Name,
    int Type,
    IReadOnlyList<string> Options
);

/// <summary>
/// A sister object for the <see cref="IComplexOneOfData"/> type.
/// </summary>
/// <param name="CustomID">An ID value.</param>
/// <param name="ComponentType">A type value.</param>
public record MessageComponentData
(
    string CustomID,
    int ComponentType
);
