﻿using System;
using OnionSeed.Helpers.Comparable;

namespace OnionSeed.Types
{
	/// <summary>
	/// Represents a mathematical interval. This can be useful for things like date ranges.
	/// </summary>
	/// <typeparam name="T">The type of values in the interval.</typeparam>
	/// <remarks>Details on mathematical intervals can be found here: https://en.wikipedia.org/wiki/Interval_(mathematics) .</remarks>
	public struct Interval<T> : IEquatable<Interval<T>>
		where T : IComparable<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Interval{T}"/> struct.
		/// </summary>
		/// <param name="min">The minimum (left) endpoint in the interval.</param>
		/// <param name="max">The maximum (right) endpoint in the interval.</param>
		/// <remarks>By default, the interval will be closed (inclusive).</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="min"/> is <c>null</c>.
		/// -or- <paramref name="max"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="max"/> is less than <paramref name="min"/>.</exception>
		public Interval(T min, T max)
			: this(true, min, max, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Interval{T}"/> struct.
		/// </summary>
		/// <param name="minIsIncluded">A value indicating whether the <see cref="Min"/> endpoint is included in the interval.</param>
		/// <param name="min">The minimum (left) endpoint in the interval.</param>
		/// <param name="max">The maximum (right) endpoint in the interval.</param>
		/// <param name="maxIsIncluded">A value indicating whether the <see cref="Max"/> endpoint is included in the interval.</param>
		/// <exception cref="ArgumentNullException"><paramref name="min"/> is <c>null</c>.
		/// -or- <paramref name="max"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="max"/> is less than <paramref name="min"/>.</exception>
		public Interval(bool minIsIncluded, T min, T max, bool maxIsIncluded)
		{
			if (min == null)
				throw new ArgumentNullException(nameof(min));
			if (max == null)
				throw new ArgumentNullException(nameof(max));
			if (max.IsLessThan(min))
				throw new ArgumentOutOfRangeException(nameof(max), "The Max endpoint cannot be less than the Min endpoint.");

			MinIsIncluded = minIsIncluded;
			Min = min;
			Max = max;
			MaxIsIncluded = maxIsIncluded;
		}

		/// <summary>
		/// Gets the minimum (left) endpoint in the interval.
		/// </summary>
		public T Min { get; }

		/// <summary>
		/// Gets the maximum (right) endpoint in the interval.
		/// </summary>
		public T Max { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="Min"/> endpoint is included in the interval.
		/// </summary>
		public bool MinIsIncluded { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="Max"/> endpoint is included in the interval.
		/// </summary>
		public bool MaxIsIncluded { get; }

		/// <summary>
		/// Returns <c>true</c> if the operands are equal, otherwise <c>false</c>.
		/// </summary>
		/// <param name="left">The first operand.</param>
		/// <param name="right">The second operand.</param>
		/// <returns><c>true</c> if the operands are equal, otherwise <c>false</c>.</returns>
		public static bool operator ==(Interval<T> left, Interval<T> right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Returns <c>false</c> if the operands are equal, otherwise <c>true</c>.
		/// </summary>
		/// <param name="left">The first operand.</param>
		/// <param name="right">The second operand.</param>
		/// <returns><c>false</c> if the operands are equal, otherwise <c>true</c>.</returns>
		public static bool operator !=(Interval<T> left, Interval<T> right)
		{
			return !left.Equals(right);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			var other = obj as Interval<T>?;
			return other.HasValue
				? Equals(other.Value)
				: false;
		}

		/// <inheritdoc />
		public bool Equals(Interval<T> other)
		{
			return Min.IsEqualTo(other.Min)
				&& Max.IsEqualTo(other.Max)
				&& MinIsIncluded == other.MinIsIncluded
				&& MaxIsIncluded == other.MaxIsIncluded;
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return HashCode
				.Of(MinIsIncluded)
				.And(Min)
				.And(Max)
				.And(MaxIsIncluded);
		}

		/// <summary>
		/// Returns the string representation of this instance.
		/// </summary>
		/// <returns>The string representation of this instance.</returns>
		public override string ToString()
		{
			var start = MinIsIncluded ? '[' : '(';
			var end = MaxIsIncluded ? ']' : ')';
			return $"{start}{Min}, {Max}{end}";
		}
	}
}