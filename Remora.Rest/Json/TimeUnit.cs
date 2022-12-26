//
//  SPDX-FileName: TimeUnit.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using JetBrains.Annotations;

namespace Remora.Rest.Json;

/// <summary>
/// Enumerates various units of time.
/// </summary>
[PublicAPI]
public enum TimeUnit
{
    /// <summary>
    /// A unit of days.
    /// </summary>
    Days,

    /// <summary>
    /// A unit of hours.
    /// </summary>
    Hours,

    /// <summary>
    /// A unit of minutes.
    /// </summary>
    Minutes,

    /// <summary>
    /// A unit of seconds.
    /// </summary>
    Seconds,

    /// <summary>
    /// A unit of milliseconds.
    /// </summary>
    Milliseconds
}
