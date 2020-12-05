using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle3
    {
        private readonly ITestOutputHelper _output;
        public Puzzle3(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetTrees()
        {
            yield return new[] { File.ReadAllLines(@"Data/03_trees.txt") };
        }

        [Theory]
        [MemberData(nameof(GetTrees))]
        public void TobogganRun(string[] trees)
        {
            var hits = HitTrees(trees, 3, 1);
            _output.WriteLine($"{hits}");
        }

        [Theory]
        [MemberData(nameof(GetTrees))]
        public void TobogganRun2(string[] trees)
        {
            var first = HitTrees(trees, 1, 1);
            var second = HitTrees(trees, 3, 1);
            var third = HitTrees(trees, 5, 1);
            var fourth = HitTrees(trees, 7, 1);
            var fifth = HitTrees(trees, 1, 2);
            _output.WriteLine($"{first * second * third * fourth * fifth}");
        }

        private uint HitTrees(string[] trees, int right, int down)
        {
            int width = trees[0].Length;
            int lastRow = trees.Length - down;
            uint hits = 0;
            for (int currentX = 0, currentY = 0; currentY < lastRow;)
            {
                currentX = (currentX + right) % width;
                currentY += down;

                if (trees[currentY][currentX] == '#')
                {
                    hits++;
                }
            }

            return hits;
        }
    }
}
