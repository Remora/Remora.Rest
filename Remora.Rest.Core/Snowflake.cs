//
//  SPDX-FileName: Snowflake.cs
//  SPDX-FileCopyrightText: Copyright (c) Jarl Gullberg
//  SPDX-License-Identifier: LGPL-3.0-or-later
//

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remora.Rest.Core;

/// <summary>
/// Represents a snowflake, used as a unique ID.
/// </summary>
/// <remarks>
/// See <a href="https://en.wikipedia.org/wiki/Snowflake_ID"/> for more information.
/// </remarks>
[PublicAPI]
public readonly struct Snowflake : IEquatable<Snowflake>, IComparable<Snowflake>, IEquatable<ulong>, IComparable<ulong>
{
    /// <summary>
    /// Holds the epoch of the snowflake.
    /// </summary>
    private readonly ulong _epoch;

    /// <summary>
    /// Gets the internal value representation of the snowflake.
    /// </summary>
    public ulong Value { get; }

    /// <summary>
    /// Gets the timestamp embedded in the snowflake.
    /// </summary>
    public DateTimeOffset Timestamp
        => DateTimeOffset.FromUnixTimeMilliseconds((long)((this.Value >> 22) + _epoch)).UtcDateTime;

    /// <summary>
    /// Gets the internal worker ID used by the generating service.
    /// </summary>
    public byte InternalWorkerID => (byte)((this.Value & 0x3E0000) >> 17);

    /// <summary>
    /// Gets the internal process ID used by generating service.
    /// </summary>
    public byte InternalProcessID => (byte)((this.Value & 0x1F000) >> 12);

    /// <summary>
    /// Gets a per-process increment. This number is incremented every time a new ID is generated on the process
    /// referred to by <see cref="InternalProcessID"/>.
    /// </summary>
    public ushort Increment => (ushort)(this.Value & 0xFFF);

    /// <summary>
    /// Initializes a new instance of the <see cref="Snowflake"/> struct.
    /// </summary>
    /// <param name="value">The binary representation of the snowflake.</param>
    /// <param name="epoch">The time epoch used for the embedded timestamp.</param>
    public Snowflake(ulong value, ulong epoch = 0)
    {
        _epoch = epoch;
        this.Value = value;
    }

    /// <summary>
    /// Creates a new snowflake from a timestamp. This is generally used for pagination in API endpoints.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <param name="epoch">The epoch used for the embedded timestamp.</param>
    /// <returns>The snowflake.</returns>
    public static Snowflake CreateTimestampSnowflake(DateTimeOffset? timestamp = null, ulong epoch = 0)
    {
        timestamp ??= DateTimeOffset.UtcNow;

        var value = (ulong)((timestamp.Value.ToUnixTimeMilliseconds() - (long)epoch) << 22);
        return new Snowflake(value);
    }

    /// <summary>
    /// Attempts to parse a snowflake value from the given string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="result">The result.</param>
    /// <param name="epoch">The time epoch used for the embedded timestamp.</param>
    /// <returns>true if a snowflake was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string value, [NotNullWhen(true)] out Snowflake? result, ulong epoch = 0)
    {
        result = null;

        if (!ulong.TryParse(value, out var binary))
        {
            return false;
        }

        result = new Snowflake(binary, epoch);

        return true;
    }

    /// <inheritdoc/>
    public bool Equals(Snowflake other) => this.Value == other.Value;

    /// <inheritdoc/>
    public int CompareTo(Snowflake other) => this.Value.CompareTo(other.Value);

    /// <inheritdoc />
    public bool Equals(ulong other) => this.Value == other;

    /// <inheritdoc />
    public int CompareTo(ulong other) => this.Value.CompareTo(other);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Snowflake other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => this.Value.GetHashCode();

    /// <inheritdoc/>
    public override string ToString() => this.Value.ToString();

    /// <summary>
    /// Compares two snowflakes for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the operands are equal, false otherwise.</returns>
    public static bool operator ==(Snowflake left, Snowflake right) => left.Equals(right);

    /// <summary>
    /// Compares two snowflakes for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>false if the operands are equal, true otherwise.</returns>
    public static bool operator !=(Snowflake left, Snowflake right) => !left.Equals(right);

    /// <summary>
    /// Compares two snowflakes, determining whether the left operand is considered less than the right operand.
    /// This is generally based on time. An earlier snowflake will compare as less than another, and a snowflake
    /// with a higher increment will compare as more than another (provided they are from the same worker and same
    /// time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <(Snowflake left, Snowflake right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Compares two snowflakes, determining whether the left operand is considered greater than the right operand.
    /// This is generally based on time. An earlier snowflake will compare as less than another, and a snowflake
    /// with a higher increment will compare as more than another (provided they are from the same worker and same
    /// time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >(Snowflake left, Snowflake right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Compares two snowflakes, determining whether the left operand is considered less than or equal to the right
    /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker
    /// and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <=(Snowflake left, Snowflake right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Compares two snowflakes, determining whether the left operand is considered greater than or equal to the
    /// right operand. This is generally based on time. An earlier snowflake will compare as less than another, and
    /// a snowflake with a higher increment will compare as more than another (provided they are from the same
    /// worker and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >=(Snowflake left, Snowflake right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Compares a snowflake with a ulong for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the operands are equal, false otherwise.</returns>
    public static bool operator ==(Snowflake left, ulong right) => left.Equals(right);

    /// <summary>
    /// Compares a snowflake with a ulong for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>false if the operands are equal, true otherwise.</returns>
    public static bool operator !=(Snowflake left, ulong right) => !left.Equals(right);

    /// <summary>
    /// Compares a snowflake with a ulong, determining whether the left operand is considered less than the right
    /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker and
    /// same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <(Snowflake left, ulong right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Compares a snowflake with a ulong, determining whether the left operand is considered greater than the right
    /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker and
    /// same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >(Snowflake left, ulong right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Compares a snowflake with a ulong, determining whether the left operand is considered less than or equal to the
    /// right operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker
    /// and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <=(Snowflake left, ulong right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Compares a snowflake with a ulong, determining whether the left operand is considered greater than or equal to
    /// the right operand. This is generally based on time. An earlier snowflake will compare as less than another, and
    /// a snowflake with a higher increment will compare as more than another (provided they are from the same
    /// worker and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >=(Snowflake left, ulong right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Compares a ulong with a snowflake for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the operands are equal, false otherwise.</returns>
    public static bool operator ==(ulong left, Snowflake right) => left.Equals(right.Value);

    /// <summary>
    /// Compares a ulong with a snowflake for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>false if the operands are equal, true otherwise.</returns>
    public static bool operator !=(ulong left, Snowflake right) => !left.Equals(right.Value);

    /// <summary>
    /// Compares a ulong with a snowflake, determining whether the left operand is considered less than the right
    /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker and
    /// same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <(ulong left, Snowflake right) => left.CompareTo(right.Value) < 0;

    /// <summary>
    /// Compares a ulong with a snowflake, determining whether the left operand is considered greater than the right
    /// operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker and
    /// same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >(ulong left, Snowflake right) => left.CompareTo(right.Value) > 0;

    /// <summary>
    /// Compares a ulong with a snowflake, determining whether the left operand is considered less than or equal to the
    /// right operand. This is generally based on time. An earlier snowflake will compare as less than another, and a
    /// snowflake with a higher increment will compare as more than another (provided they are from the same worker
    /// and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator <=(ulong left, Snowflake right) => left.CompareTo(right.Value) <= 0;

    /// <summary>
    /// Compares a ulong with a snowflake, determining whether the left operand is considered greater than or equal to
    /// the right operand. This is generally based on time. An earlier snowflake will compare as less than another, and
    /// a snowflake with a higher increment will compare as more than another (provided they are from the same
    /// worker and same time).
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the relationship holds; otherwise, false.</returns>
    public static bool operator >=(ulong left, Snowflake right) => left.CompareTo(right.Value) >= 0;
}
