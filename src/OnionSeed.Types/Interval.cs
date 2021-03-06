﻿using System;
using System.Collections.Generic;
using OnionSeed.Helpers.Comparable;

namespace OnionSeed.Types
{
	/// <summary>
	/// Represents a mathematical interval. The interval can be iterated at varying levels of granularity,
	/// without needing to store every value in the interval.
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
		public Interval(bool minIsIncluded, T min, T max, bool maxIsIncluded)
		{
			if (min == null)
				throw new ArgumentNullException(nameof(min));
			if (max == null)
				throw new ArgumentNullException(nameof(max));

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
		/// Gets a value indicating whether the interval does not include any of its endpoints.
		/// </summary>
		public bool IsOpen => !MinIsIncluded && !MaxIsIncluded;

		/// <summary>
		/// Gets a value indicating whether the interval includes exactly one of its endpoints.
		/// </summary>
		public bool IsHalfOpen => MinIsIncluded ^ MaxIsIncluded;

		/// <summary>
		/// Gets a value indicating whether the interval includes both of its endpoints.
		/// </summary>
		public bool IsClosed => MinIsIncluded && MaxIsIncluded;

		/// <summary>
		/// Gets a value indicating whether the interval contains zero elements.
		/// </summary>
		public bool IsEmpty =>
			Min.IsGreaterThan(Max) ||
			(Min.IsEqualTo(Max) && !IsClosed);

		/// <summary>
		/// Gets a value indicating whether the interval contains exactly one element.
		/// </summary>
		public bool IsDegenerate => Min.IsEqualTo(Max) && IsClosed;

		/// <summary>
		/// Gets a value indicating whether the interval is neither empty nor degenerate
		/// (i.e. it has potentially infinite elements).
		/// </summary>
		public bool IsProper => Min.IsLessThan(Max);

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
		public bool Equals(Interval<T> other) =>
			Min.IsEqualTo(other.Min) &&
			Max.IsEqualTo(other.Max) &&
			MinIsIncluded == other.MinIsIncluded &&
			MaxIsIncluded == other.MaxIsIncluded;

		/// <inheritdoc />
		public override int GetHashCode() => HashCode
			.Of(MinIsIncluded)
			.And(Min)
			.And(Max)
			.And(MaxIsIncluded);

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

		/// <summary>
		/// Enumerates the values that fall within the interval, in ascending order (from <see cref="Min"/> to <see cref="Max"/>).
		/// </summary>
		/// <param name="increment">The method that determines how to increment from one value to the next.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains all the values in the interval,
		/// in ascending order, as determined by the <paramref name="increment"/> algorithm.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="increment"/> is <b>null</b>.</exception>
		/// <remarks>If <paramref name="increment"/> does not move the current value from <see cref="Min"/> towards <see cref="Max"/>,
		/// then this enumeration will result in an infinite loop.</remarks>
		public IEnumerable<T> Ascending(Func<T, T> increment)
		{
			if (increment == null)
				throw new ArgumentNullException(nameof(increment));

			var start = MinIsIncluded ? Min : increment(Min);
			var end = Max;
			Predicate<T> predicate = MaxIsIncluded
				? (T c) => c.IsLessThanOrEqualTo(end)
				: (Predicate<T>)((T c) => c.IsLessThan(end));

			return Enumerate(start, increment, predicate);
		}

		/// <summary>
		/// Enumerates the values that fall within the interval, in descending order (from <see cref="Max"/> to <see cref="Min"/>).
		/// </summary>
		/// <param name="decrement">The method that determines how to decrement from one value to the next.</param>
		/// <returns>An <see cref="IEnumerable{T}"/> that contains all the values in the interval,
		/// in descending order, as determined by the <paramref name="decrement"/> algorithm.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="decrement"/> is <b>null</b>.</exception>
		/// <remarks>If <paramref name="decrement"/> does not move the current value from <see cref="Max"/> towards <see cref="Min"/>,
		/// then this enumeration will result in an infinite loop.</remarks>
		public IEnumerable<T> Descending(Func<T, T> decrement)
		{
			if (decrement == null)
				throw new ArgumentNullException(nameof(decrement));

			var start = MaxIsIncluded ? Max : decrement(Max);
			var end = Min;
			Predicate<T> predicate = MinIsIncluded
				? (T c) => c.IsGreaterThanOrEqualTo(end)
				: (Predicate<T>)((T c) => c.IsGreaterThan(end));

			return Enumerate(start, decrement, predicate);
		}

		private static IEnumerable<T> Enumerate(T current, Func<T, T> step, Predicate<T> shouldContinue)
		{
			while (shouldContinue(current))
			{
				yield return current;
				current = step(current);
			}
		}
	}
}
