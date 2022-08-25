//
//  DTOPropertyInfo.cs
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

using System.Reflection;
using System.Text.Json;
using Remora.Rest.Core;
using Remora.Rest.Json.Reflection;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// Bundles data needed for properties in <see cref="BoundDataObjectConverter{TInterface, TImplementation}"/>.
/// </summary>
/// <param name="Property">The CLR property of the implementation type.</param>
/// <param name="ReadNames">The names this property allows when reading.</param>
/// <param name="WriteName">The name used when writing this property.</param>
/// <param name="Writer">A delegate that writes the property.</param>
/// <param name="AllowsNull">Whether this property allows null.</param>
/// <param name="DefaultValue">null, if this property is required, otherwise its default value (= default of <see cref="Optional{TValue}"/>).</param>
/// <param name="Options">The serializer options used when serializing this property.</param>
/// <param name="ReadIndex">The index of this property in the _readProperties array. Has no meaning when writing the object to JSON.</param>
internal sealed record DTOPropertyInfo
(
    PropertyInfo Property,
    string[] ReadNames,
    string WriteName,
    DTOPropertyWriter Writer,
    bool AllowsNull,
    object? DefaultValue,
    JsonSerializerOptions Options,
    int ReadIndex
);
