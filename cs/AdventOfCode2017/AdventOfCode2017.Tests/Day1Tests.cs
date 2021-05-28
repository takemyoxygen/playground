using AdventOfCode2017.App;
using NUnit.Framework;

namespace AdventOfCode2017.Tests
{
    public class Day1Tests
    {
        [TestCase("1122", 3)]
        [TestCase("1111", 4)]
        [TestCase("1234", 0)]
        [TestCase("91212129", 9)]
        public void Solve1Tests(string input, int expected)
        {
            var actual = Day1.Solve1(input);
            Assert.AreEqual(expected, actual);
        }
        
        [TestCase("1212", 6)]
        [TestCase("1221", 0)]
        [TestCase("123425", 4)]
        [TestCase("123123", 12)]
        [TestCase("12131415", 4)]
        public void Solve2Tests(string input, int expected)
        {
            var actual = Day1.Solve2(input);
            Assert.AreEqual(expected, actual);
        }
    }
}