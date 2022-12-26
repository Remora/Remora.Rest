//
//  SPDX-FileName: JsonSerializerOptionsExtensions.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
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
