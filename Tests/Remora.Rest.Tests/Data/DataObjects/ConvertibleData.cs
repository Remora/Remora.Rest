//
//  SPDX-FileName: ConvertibleData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data record that contains a member that needs a custom converter.
/// </summary>
/// <param name="Value">An enumeration value.</param>
public record ConvertibleData(StringifiedEnum Value) : IConvertibleData;
