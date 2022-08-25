//
//  BoundDataObjectConverterOptions.cs
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

using Remora.Rest.Json.Reflection;

namespace Remora.Rest.Json.Internal;

/// <summary>
/// Encapsulates all data needed to construct a <see cref="BoundDataObjectConverter{TInterface, TImplementation}"/>.
/// </summary>
/// <typeparam name="TInterface">The interface that is seen in the objects for the converter.</typeparam>
/// <param name="DTOFactory">The DTO factory.</param>
/// <param name="AllowExtraProperties">Whether extra undefined properties should be allowed.</param>
/// <param name="WriteProperties">Properties relevant when writing the DTO to JSON.</param>
/// <param name="ReadProperties">Properties relevant when reading the DTO from JSON.</param>
internal record BoundDataObjectConverterOptions<TInterface>
(
    ObjectFactory<TInterface> DTOFactory,
    bool AllowExtraProperties,
    DTOPropertyInfo[] WriteProperties,
    DTOPropertyInfo[] ReadProperties
);
