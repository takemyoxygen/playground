using AdventOfCode2017.App;
using NUnit.Framework;

namespace AdventOfCode2017.Tests
{
    public class Day3Tests
    {
        private void AssertLevel(int expectedIndex, int[] expectedCorners, Level actualLevel)
        {
            Assert.AreEqual(expectedIndex, actualLevel.Index);
            CollectionAssert.AreEqual(expectedCorners, actualLevel.Corners);
        }
        
        [Test]
        public void FindLevelTest()
        {
            AssertLevel(2, new []{13, 17, 21, 25}, Day3.FindLevel(16));
            AssertLevel(2, new []{13, 17, 21, 25}, Day3.FindLevel(10));
            AssertLevel(2, new []{13, 17, 21, 25}, Day3.FindLevel(25));
            
            AssertLevel(1, new []{3, 5, 7, 9}, Day3.FindLevel(4));
        }

        [TestCase(1, 0)]
        [TestCase(12, 3)]
        [TestCase(23, 2)]
        [TestCase(1024, 31)]
        public void Solve1Tests(int input, int expected)
        {
            var actual = Day3.Solve1(input);
            Assert.AreEqual(expected, actual);
        }
    }
}