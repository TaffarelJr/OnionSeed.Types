using System.Linq;
using FluentAssertions;
using Xunit;

namespace OnionSeed.Types
{
	public class HashCodeTests
	{
		[Fact]
		public void Of_ShouldCalculateHash_FromGivenValue()
		{
			// Act
			int result1 = HashCode.Of('a');
			int result2 = HashCode.Of('b');
			int result3 = HashCode.Of('c');

			// Assert
			result1.Should().NotBe(0);
			result2.Should().NotBe(0);
			result3.Should().NotBe(0);

			result1.Should().NotBe(result2);
			result1.Should().NotBe(result3);

			result2.Should().NotBe(result3);
		}

		[Fact]
		public void OfEach_ShouldCalculateHash_FromCombinedValues()
		{
			// Arrange
			var data = new[] { 'a', 'b', 'c' };

			// Act
			int result1 = HashCode.OfEach(data.Take(1));
			int result2 = HashCode.OfEach(data.Take(2));
			int result3 = HashCode.OfEach(data.Take(3));

			// Assert
			result1.Should().NotBe(0);
			result2.Should().NotBe(0);
			result3.Should().NotBe(0);

			result1.Should().NotBe(result2);
			result1.Should().NotBe(result3);

			result2.Should().NotBe(result3);
		}

		[Fact]
		public void And_ShouldCombineHashes()
		{
			// Arrange
			var subject = HashCode.Of('a');

			// Act
			var result1 = subject.And('b');
			var result2 = result1.And('c');
			var result3 = result2.And('d');

			// Assert
			result1.Should().NotBe(0);
			result2.Should().NotBe(0);
			result3.Should().NotBe(0);

			subject.Should().NotBe(result1);
			subject.Should().NotBe(result2);
			subject.Should().NotBe(result3);

			result1.Should().NotBe(result2);
			result1.Should().NotBe(result3);

			result2.Should().NotBe(result3);
		}

		[Fact]
		public void AndEach_ShouldCombineHashes()
		{
			// Arrange
			var data = new[] { 'b', 'c', 'd' };
			var subject = HashCode.Of('a');

			// Act
			int result1 = subject.AndEach(data.Take(1));
			int result2 = subject.AndEach(data.Take(2));
			int result3 = subject.AndEach(data.Take(3));

			// Assert
			result1.Should().NotBe(0);
			result2.Should().NotBe(0);
			result3.Should().NotBe(0);

			subject.Should().NotBe(result1);
			subject.Should().NotBe(result2);
			subject.Should().NotBe(result3);

			result1.Should().NotBe(result2);
			result1.Should().NotBe(result3);

			result2.Should().NotBe(result3);
		}
	}
}
