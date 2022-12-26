//
//  SPDX-FileName: OptionalData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Rest.Core;

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data record that contains an optional member.
/// </summary>
/// <param name="Value">An optional string.</param>
public record OptionalData(Optional<string> Value) : IOptionalData;
