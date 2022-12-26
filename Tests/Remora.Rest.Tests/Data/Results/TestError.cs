//
//  SPDX-FileName: TestError.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using Remora.Results;

namespace Remora.Rest.Tests.Data;

/// <summary>
/// Represents a fake API error, returned from unit tests.
/// </summary>
public record TestError() : ResultError("An error occurred.");
