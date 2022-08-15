//
//  NonConventionalData.cs
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

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data class that serves as an implementation for <see cref="INonConventionalData"/>.
/// </summary>
public record NonConventionalData(string Value) : INonConventionalData;

/// <summary>
/// A data class that unconventionally alters the type of the interface's property.
/// </summary>
/// <param name="Value">Gets an arbitrary string.</param>
public record NonConventionalData2(string Value) : INonConventionalData
{
    /// <inheritdoc/>
    string INonConventionalData.Value { get; } = "this should be serialized";
}
