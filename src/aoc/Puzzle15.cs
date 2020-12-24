using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle15
    {
        private readonly ITestOutputHelper _output;

        public Puzzle15(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        private int GetNthNumber(int[] seed, int n)
        {
            Dictionary<int, int> valueToIndexMapping = new();

            for (int i = 0; i < seed.Length; ++i)
            {
                valueToIndexMapping[seed[i]] = i + 1;
            }

            var result = seed[^1];
            for (int i = seed.Length+1; i <= n; ++i)
            {
                result = MakeNumber(valueToIndexMapping, result, i);
            }

            return result;
        }

        private int MakeNumber(Dictionary<int, int> valueToIndexMapping, int lastNumber, int index)
        {
            int newNumber = 0;
            if (valueToIndexMapping.TryGetValue(lastNumber, out var lastIndex))
            {
                newNumber = index - 1 - lastIndex;
            }

            valueToIndexMapping[lastNumber] = index - 1;
            return newNumber;
        }

        [Theory]
        [InlineData("0,3,6", 4, 0)]
        [InlineData("0,3,6", 5, 3)]
        [InlineData("0,3,6", 6, 3)]
        [InlineData("0,3,6", 7, 1)]
        [InlineData("0,3,6", 8, 0)]
        [InlineData("0,3,6", 9, 4)]
        [InlineData("0,3,6", 10, 0)]
        [InlineData("1,3,2", 2020, 1)]
        [InlineData("2,1,3", 2020, 10)]
        [InlineData("1,2,3", 2020, 27)]
        [InlineData("2,3,1", 2020, 78)]
        [InlineData("3,2,1", 2020, 438)]
        [InlineData("3,1,2", 2020, 1836)]
        [InlineData("1,2,16,19,18,0", 2020)]
        [InlineData("1,2,16,19,18,0", 30000000)]
        public void TestNumber(string numList, int iteration, int? expectedOutput = default)
        {
            var numbers = numList.Split(',').Select(int.Parse).ToArray();
            var number = GetNthNumber(numbers, iteration);
            if (expectedOutput.HasValue)
            {
                Assert.Equal(expectedOutput, number);
            }
            else
            {
                _output.WriteLine($"{number}");
            }
        }
    }
}
