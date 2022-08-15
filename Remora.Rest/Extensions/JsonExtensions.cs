//
//  JsonExtensions.cs
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

using System.Buffers;
using System.Text.Json;

namespace Remora.Rest.Extensions;

/// <summary>
/// Provides extension classes for deserializing <see cref="JsonElement"/>s and <see cref="JsonDocument"/>.
/// </summary>
/// <remarks>
/// As per the <see href="https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0#serialize-to-utf-8">docs</see>,
/// serializing to UTF-8 is about 5-10% faster than using string-based methods. The difference is because bytes (as UTF-8) don't need to be converted to strings (UTF-16).
/// </remarks>
public static class JsonExtensions
{
    /// <summary>
    /// Deserializes the current <see cref="JsonElement"/> to the specified <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type to return.</typeparam>
    /// <param name="element">The <see cref="JsonElement"/> to deserialize.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/>.</param>
    /// <returns>The converted <typeparamref name="TEntity"/> object or null.</returns>
    public static TEntity? ToObject<TEntity>(this JsonElement element, JsonSerializerOptions? options = null)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(bufferWriter);
        element.WriteTo(writer);
        return JsonSerializer.Deserialize<TEntity>(bufferWriter.WrittenSpan, options);
    }

    /// <summary>
    /// Deserializes the current <see cref="JsonDocument"/> to the specified <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type to return.</typeparam>
    /// <param name="document">The <see cref="JsonDocument"/> to deserialize.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/>.</param>
    /// <returns>The converted <typeparamref name="TEntity"/> object or null.</returns>
    public static TEntity? ToObject<TEntity>(this JsonDocument document, JsonSerializerOptions? options = null)
        => document.RootElement.ToObject<TEntity>(options);
}
