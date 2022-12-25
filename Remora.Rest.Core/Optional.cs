//
//  Optional.cs
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

#pragma warning disable SA1402

namespace Remora.Rest.Core;

/// <summary>
/// Contains utility methods for <see cref="Optional{TValue}"/>.
/// </summary>
[PublicAPI]
public static class Optional
{
    /// <summary>
    /// Gets a new <see cref="Optional{TValue}"/> of the specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the optional to create.</typeparam>
    /// <returns>The newly created optional value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> Empty<T>() => default;

    /// <summary>
    /// Gets a new <see cref="Optional{TValue}"/> with the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">The type of the optional to create.</typeparam>
    /// <returns>The newly created optional value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<T> From<T>(T value) => new(value);
}

/// <summary>
/// Represents an optional value. This is mainly used for JSON de/serializalization where a value can be either
/// present, null, or completely missing.
///
/// While a <see cref="Nullable"/> allows for a value to be logically present but contain a null value,
/// <see cref="Optional{TValue}"/> allows for a value to be logically missing. For example, an optional without a
/// value would never be serialized, but a nullable with a null value would (albeit as "null").
/// </summary>
/// <typeparam name="TValue">The inner type.</typeparam>
[PublicAPI]
[DebuggerDisplay("HasValue = {HasValue}, Value = {_value}")]
public readonly struct Optional<TValue> : IOptional
{
    private readonly TValue _value;

    /// <summary>
    /// Gets the value contained in the optional.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the optional does not contain a value.</exception>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public TValue Value
    {
        get
        {
            if (this.HasValue)
            {
                return _value;
            }

            throw new InvalidOperationException("The optional did not contain a valid value.");
        }
    }

    /// <inheritdoc />
    public bool HasValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Optional{TValue}"/> struct.
    /// </summary>
    /// <param name="value">The contained value.</param>
    public Optional(TValue value)
    {
        _value = value;
        this.HasValue = true;
    }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(true, nameof(_value))]
    public bool IsDefined() => this.HasValue && _value is not null;

    /// <summary>
    /// Determines whether the option has a defined value; that is, whether it both has a value and that value is
    /// non-null.
    /// </summary>
    /// <param name="value">The defined value.</param>
    /// <returns>true if the optional has a value and that value is non-null; otherwise, false.</returns>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsDefined([NotNullWhen(true)] out TValue? value)
    {
        value = default;

        if (!IsDefined())
        {
            return false;
        }

        value = _value;
        return true;
    }

    /// <summary>
    /// Implicitly converts actual values into an optional.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The created optional.</returns>
    public static implicit operator Optional<TValue>(TValue value)
    {
        return new(value);
    }

    /// <summary>
    /// Compares two optionals, for equality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>true if the operands are equal, false otherwise.</returns>
    public static bool operator ==(Optional<TValue> left, Optional<TValue> right)
        => left.Equals(right);

    /// <summary>
    /// Compares two optionals, for inequality.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>false if the operands are equal, true otherwise.</returns>
    public static bool operator !=(Optional<TValue> left, Optional<TValue> right)
        => !left.Equals(right);

    /// <summary>
    /// Compares this instance for equality with another instance.
    /// </summary>
    /// <param name="other">The other instance.</param>
    /// <returns>true if the instances are considered equal; otherwise, false.</returns>
    public bool Equals(Optional<TValue> other)
    {
        return EqualityComparer<TValue>.Default.Equals(_value, other._value) && this.HasValue == other.HasValue;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Optional<TValue> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(_value, this.HasValue);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.HasValue
            ? $"{{{_value?.ToString() ?? "null"}}}"
            : "Empty";
    }

    /// <summary>
    /// Returns the value of the current <see cref="Optional{TValue}"/>, or <c>default</c> if the current
    /// <see cref="Optional{TValue}"/> is empty.
    /// </summary>
    /// <returns>The value of <c>this</c> or <c>default</c> if none is present.</returns>
    public TValue? OrDefault()
    {
        return TryGet(out var value) ? value : default;
    }

    /// <summary>
    /// Returns the value of the current <see cref="Optional{TValue}"/>, or <paramref name="defaultValue"/> if
    /// <see cref="Optional{TValue}"/> is <b>null or empty</b>.
    /// </summary>
    /// <param name="defaultValue">The default value to fallback to if the optional is empty.</param>
    /// <returns>
    /// The value of <see cref="Optional{TValue}"/> or <paramref name="defaultValue"/> if none is present.
    /// </returns>
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public TValue? OrDefault(TValue? defaultValue)
    {
        return IsDefined(out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Returns the value of the current <see cref="Optional{TValue}"/>, or throws the exception in
    /// <paramref name="func"/> if empty.
    /// </summary>
    /// <param name="func">The function generating an <see cref="Exception"/>.</param>
    /// <returns>The value of <see cref="Optional{TValue}"/>.</returns>
    /// <exception cref="Exception">If <see cref="Optional{TValue}"/> is empty.</exception>
    public TValue OrThrow([RequireStaticDelegate] Func<Exception> func)
    {
        return TryGet(out var value) ? value : throw func();
    }

    /// <summary>
    /// Casts the current <see cref="Optional{TValue}"/> to a nullable <typeparamref name="TValue"/>?.
    /// </summary>
    /// <returns>
    /// An <see cref="Optional{TValue}"/> with the type pararameter changed to <typeparamref name="TValue"/>?.
    /// </returns>
    public Optional<TValue?> Nullable()
    {
        return TryGet(out var value) ? value : default(Optional<TValue?>);
    }

    /// <summary>
    /// Gets the underlying value of a <see cref="Optional{TValue}"/> if it has one.
    /// </summary>
    /// <param name="value">Set to the value of <see cref="Optional{TValue}"/>, or <c>default</c> if it has none.</param>
    /// <returns><c>true</c> if the <see cref="Optional{TValue}"/> has a value, even when it's <c>null</c>.</returns>
    /// <seealso cref="IsDefined(out TValue?)"/>
    public bool TryGet([MaybeNullWhen(false)] out TValue value)
    {
        if (this.HasValue)
        {
            value = _value;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Applies a mapping function to the value of the <see cref="Optional{TValue}"/> if it has one; otherwise, returns
    /// a new empty <see cref="Optional{TValue}"/> of the resulting type.
    /// </summary>
    /// <param name="mappingFunc">The mapping function.</param>
    /// <typeparam name="TResult">The value type for the output of the mapping result.</typeparam>
    /// <returns>
    /// A new optional with the mapping result if <see cref="Optional{TValue}"/> is non-empty; an empty opitional
    /// otherwise.
    /// </returns>
    public Optional<TResult> Map<TResult>(Func<TValue, TResult> mappingFunc)
    {
        return this.HasValue
            ? mappingFunc(_value)
            : default(Optional<TResult>);
    }
}
