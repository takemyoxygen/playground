using AdventOfCode2017.App;
using NUnit.Framework;

namespace AdventOfCode2017.Tests
{
    public class Day2Tests
    {
        [TestCase(@"
5 1 9 5
7 5 3
2 4 6 8", 18)]
        public void Solve1Tests(string input, int expected)
        {
            var actual = Day2.Solve1(input);
            Assert.AreEqual(expected, actual);
        }
        
        [TestCase(@"
5 9 2 8
9 4 7 3
3 8 6 5", 9)]
        public void Solve2Tests(string input, int expected)
        {
            var actual = Day2.Solve2(input);
            Assert.AreEqual(expected, actual);
        }

    }
}