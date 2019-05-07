using System;
using FluentAssertions;
using Xunit;

namespace OnionSeed.Types
{
	public class IntervalTests
	{
		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters(bool includeMin, bool includeMax)
		{
			// Arrange
			var min = includeMin ? "ABC" : null;
			var max = includeMax ? "DEF" : null;

			// Act
			Action action = () => new Interval<string>(min, max);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Theory]
		[InlineData(3, 3)]
		[InlineData(3, 4)]
		[InlineData(3, 7)]
		public void Constructor_ShouldCreateInstance(int min, int max)
		{
			// Act
			var result = new Interval<int>(min, max);

			// Assert
			result.MinIsIncluded.Should().BeTrue();
			result.Min.Should().Be(min);
			result.Max.Should().Be(max);
			result.MaxIsIncluded.Should().BeTrue();
		}

		[Theory]
		[InlineData(3, 2)]
		public void Constructor_ShouldThrowException_WhenMaxIsLessThanMin(int min, int max)
		{
			// Act
			Action action = () => new Interval<int>(min, max);

			// Assert
			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(false, true)]
		[InlineData(true, false)]
		public void Constructor_ShouldValidateParameters_WhenInclusionIsSpecified(bool includeMin, bool includeMax)
		{
			// Arrange
			var min = includeMin ? "ABC" : null;
			var max = includeMax ? "DEF" : null;

			// Act
			Action action = () => new Interval<string>(false, min, max, false);

			// Assert
			action.Should().Throw<ArgumentNullException>();
		}

		[Theory]
		[InlineData(false, 3, 3, false)]
		[InlineData(false, 3, 7, false)]
		[InlineData(false, 3, 3, true)]
		[InlineData(false, 3, 7, true)]
		[InlineData(true, 3, 3, false)]
		[InlineData(true, 3, 7, false)]
		[InlineData(true, 3, 3, true)]
		[InlineData(true, 3, 7, true)]
		public void Constructor_ShouldCreateInstance_WhenInclusionIsSpecified(bool includeMin, int min, int max, bool includeMax)
		{
			// Act
			var result = new Interval<int>(includeMin, min, max, includeMax);

			// Assert
			result.MinIsIncluded.Should().Be(includeMin);
			result.Min.Should().Be(min);
			result.Max.Should().Be(max);
			result.MaxIsIncluded.Should().Be(includeMax);
		}

		[Theory]
		[InlineData(false, 3, 2, false)]
		[InlineData(false, 3, 2, true)]
		[InlineData(true, 3, 2, false)]
		[InlineData(true, 3, 2, true)]
		public void Constructor_ShouldThrowException_WhenMaxIsLessThanMin_AndInclusionIsSpecified(bool includeMin, int min, int max, bool includeMax)
		{
			// Act
			Action action = () => new Interval<int>(includeMin, min, max, includeMax);

			// Assert
			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Theory]
		[InlineData(false, 3, 7, false, true)]
		[InlineData(false, 3, 7, true, false)]
		[InlineData(false, 3, 6, false, false)]
		[InlineData(false, 4, 7, false, false)]
		[InlineData(true, 3, 7, false, false)]
		public void EqualityOperator_ShouldReturnCorrectResult(bool includeMin, int min, int max, bool includeMax, bool expected)
		{
			// Arrange
			var left = new Interval<int>(false, 3, 7, false);
			var right = new Interval<int>(includeMin, min, max, includeMax);

			// Act
			var result = left == right;

			// Assert
			result.Should().Be(expected);
		}

		[Theory]
		[InlineData(false, 3, 7, false, false)]
		[InlineData(false, 3, 7, true, true)]
		[InlineData(false, 3, 6, false, true)]
		[InlineData(false, 4, 7, false, true)]
		[InlineData(true, 3, 7, false, true)]
		public void InequalityOperator_ShouldReturnCorrectResult(bool includeMin, int min, int max, bool includeMax, bool expected)
		{
			// Arrange
			var left = new Interval<int>(false, 3, 7, false);
			var right = new Interval<int>(includeMin, min, max, includeMax);

			// Act
			var result = left != right;

			// Assert
			result.Should().Be(expected);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("hello")]
		public void Equals_ShouldReturnFalse_WhenTypeIsWrong(string other)
		{
			// Arrange
			var interval = new Interval<int>(3, 7);

			// Act
			var result = interval.Equals(other);

			// Assert
			result.Should().BeFalse();
		}

		[Theory]
		[InlineData(false, 3, 7, false, true)]
		[InlineData(false, 3, 7, true, false)]
		[InlineData(false, 3, 6, false, false)]
		[InlineData(false, 4, 7, false, false)]
		[InlineData(true, 3, 7, false, false)]
		public void Equals_ShouldReturnCorrectResult_WhenTypeIsCorrect(bool includeMin, int min, int max, bool includeMax, bool expected)
		{
			// Arrange
			var interval = new Interval<int>(false, 3, 7, false);
			var other = new Interval<int>(includeMin, min, max, includeMax);

			// Act
			var result = interval.Equals(other);

			// Assert
			result.Should().Be(expected);
		}

		[Fact]
		public void GetHashCode_ShouldReturnUniqueValues()
		{
			// Arrange
			var subject1 = new Interval<int>(false, 3, 7, false);
			var subject2 = new Interval<int>(false, 3, 7, true);
			var subject3 = new Interval<int>(false, 3, 6, false);
			var subject4 = new Interval<int>(false, 4, 7, false);
			var subject5 = new Interval<int>(true, 3, 7, false);

			// Act
			var result1 = subject1.GetHashCode();
			var result2 = subject2.GetHashCode();
			var result3 = subject3.GetHashCode();
			var result4 = subject4.GetHashCode();
			var result5 = subject5.GetHashCode();

			// Assert
			result1.Should().NotBe(0);
			result2.Should().NotBe(0);
			result3.Should().NotBe(0);
			result4.Should().NotBe(0);
			result5.Should().NotBe(0);

			result1.Should().NotBe(result2);
			result1.Should().NotBe(result3);
			result1.Should().NotBe(result4);
			result1.Should().NotBe(result5);

			result2.Should().NotBe(result3);
			result2.Should().NotBe(result4);
			result2.Should().NotBe(result5);

			result3.Should().NotBe(result4);
			result3.Should().NotBe(result5);

			result4.Should().NotBe(result5);
		}

		[Theory]
		[InlineData(false, 3, 7, false, "(3, 7)")]
		[InlineData(false, 3, 7, true, "(3, 7]")]
		[InlineData(true, 3, 7, false, "[3, 7)")]
		[InlineData(true, 3, 7, true, "[3, 7]")]
		public void ToString_ShouldReturnCorrectRepresentation(bool includeMin, int min, int max, bool includeMax, string expected)
		{
			// Arrange
			var subject = new Interval<int>(includeMin, min, max, includeMax);

			// Act
			var result = subject.ToString();

			// Assert
			result.Should().Be(expected);
		}
	}
}
