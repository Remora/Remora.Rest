//
//  JsonSerializerOptionsExtensions.cs
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

using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Remora.Rest.Json;

namespace Remora.Rest.Extensions;

/// <summary>
/// Defines extension methods for the <see cref="JsonSerializerOptions"/> class.
/// </summary>
[PublicAPI]
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds a data object converter to the given json options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <typeparam name="TActual">The actual type.</typeparam>
    /// <returns>The added converter.</returns>
    public static DataObjectConverter<TInterface, TActual> AddDataObjectConverter<TInterface, TActual>
    (
        this JsonSerializerOptions options
    ) where TActual : TInterface
    {
        var converter = new DataObjectConverter<TInterface, TActual>();
        options.Converters.Insert(0, converter);

        return converter;
    }

    /// <summary>
    /// Adds a JSON converter to the given json options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <typeparam name="TConverter">The converter type.</typeparam>
    /// <returns>The added converter.</returns>
    public static JsonSerializerOptions AddConverter<TConverter>(this JsonSerializerOptions options)
        where TConverter : JsonConverter, new()
    {
        options.Converters.Insert(0, new TConverter());

        return options;
    }
}
