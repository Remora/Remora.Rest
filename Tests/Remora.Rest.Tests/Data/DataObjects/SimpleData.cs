//
//  SPDX-FileName: SimpleData.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

namespace Remora.Rest.Tests.Data.DataObjects;

/// <summary>
/// A data record that does not contain any complex members.
/// </summary>
/// <param name="Value">An arbitrary string.</param>
public record SimpleData(string Value) : ISimpleData;
