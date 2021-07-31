using System;
using System.Linq;

namespace AdventOfCode2017.App
{

    public record Level(int Index, int[] Corners);
        
    public static class Day3
    {
        private const int Input = 368078;
        
        private static Level NextLevel(Level current)
        {
            var index = current.Index + 1;
            var step = 2 * index;
            var currentLevelEnd = current.Corners[3];
            return new Level(index, Enumerable.Range(1, 4).Select(x => x * step + currentLevelEnd).ToArray());
        }
        
        public static Level FindLevel(int number)
        {
            var level = new Level(0, new[] {1, 1, 1, 1});

            while (level.Corners[3] < number)
            {
                level = NextLevel(level);
            }

            return level;
        }
        
        public static int Solve1(int input = Input)
        {
            var level = FindLevel(input);
            var edgeEnd = level.Corners.First(c => input <= c);
            var midEdge = edgeEnd - level.Index;
            return Math.Abs(midEdge - input) + level.Index;
        }
    }
}