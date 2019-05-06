using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OnionSeed.Types
{
	/// <summary>
	/// Encapsulates a fluent way to combine multiple hash codes together.
	/// </summary>
	/// <remarks>This code is taken and modified from Muhammad Rehan Saeed's blog
	/// (https://rehansaeed.com/gethashcode-made-easy/). He's a Microsoft developer.
	/// <para>This could be replaced by the new HashCode class introduced in .NET Standard 2.1.</para></remarks>
	[SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "This type is not meant to be compared to other insatances.")]
	[SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "This type is only meant to be returned from the GetHashCode() method.")]
	public struct HashCode
	{
		private readonly int value;

		private HashCode(int value) => this.value = value;

		/// <summary>
		/// Converts the given <see cref="HashCode"/> to an <see cref="int"/>.
		/// </summary>
		/// <param name="hashCode">The <see cref="HashCode"/> to be converted.</param>
		public static implicit operator int(HashCode hashCode) => hashCode.value;

		/// <summary>
		/// Calculates the hash code of the given item.
		/// </summary>
		/// <typeparam name="T">The data type of the item.</typeparam>
		/// <param name="item">The item for which a hash code should be calculated.</param>
		/// <returns>The hash code of the given item.</returns>
		public static HashCode Of<T>(T item) => new HashCode(GetHashCode(item));

		/// <summary>
		/// Calculates the hash code of the given set of items.
		/// </summary>
		/// <typeparam name="T">The data type of the items in the set.</typeparam>
		/// <param name="items">The set for which a hash code should be calculated.</param>
		/// <returns>The hash code of the given set of items.</returns>
		public static HashCode OfEach<T>(IEnumerable<T> items) => new HashCode(GetHashCode(items, 0));

		/// <summary>
		/// Adds the hash code of the given item to the current instance.
		/// </summary>
		/// <typeparam name="T">The data type of the item.</typeparam>
		/// <param name="item">The item whose hash code should be added to the current instance.</param>
		/// <returns>The hash code of the given item combined with the current instance.</returns>
		public HashCode And<T>(T item) => new HashCode(CombineHashCodes(value, GetHashCode(item)));

		/// <summary>
		/// Adds the hash code of the given set of items to the current instance.
		/// </summary>
		/// <typeparam name="T">The data type of the items in the set.</typeparam>
		/// <param name="items">>The set whose hash code should be added to the current instance.</param>
		/// <returns>The hash code of the given set of items combined with the current instance.</returns>
		public HashCode AndEach<T>(IEnumerable<T> items)
		{
			if (items == null)
				return new HashCode(value);

			return new HashCode(GetHashCode(items, value));
		}

		private static int GetHashCode<T>(IEnumerable<T> items, int startHashCode)
		{
			var temp = startHashCode;
			foreach (var item in items)
			{
				temp = CombineHashCodes(temp, GetHashCode(item));
			}

			return temp;
		}

		private static int GetHashCode<T>(T item) => item == null ? 0 : item.GetHashCode();

		private static int CombineHashCodes(int h1, int h2)
		{
			unchecked
			{
				// Code copied from System.Tuple so it must be the best way to combine hash codes or at least a good one.
				return ((h1 << 5) + h1) ^ h2;
			}
		}
	}
}
