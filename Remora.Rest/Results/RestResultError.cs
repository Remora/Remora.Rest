//
//  RestResultError.cs
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

using JetBrains.Annotations;
using Remora.Results;

#pragma warning disable CS1591

namespace Remora.Rest.Results;

/// <summary>
/// Represents an error returned by the REST API.
/// </summary>
/// <typeparam name="TError">A type which represents an error payload returned by the API.</typeparam>
[PublicAPI]
public record RestResultError<TError>(TError Error)
    : ResultError("REST request failed. See inner error object for details.");
