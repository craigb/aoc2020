using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle14
    {
        private readonly ITestOutputHelper _output;

        public Puzzle14(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetMasks()
        {
            yield return new object[] { GetToyExample(), 165L };
            yield return new object[] { File.ReadAllLines("Data/14_masks.txt") };
        }

        public static string[] GetToyExample()
        {
            return @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0".Split("\r\n");
        }

        [Theory]
        [MemberData(nameof(GetMasks))]
        public void RunActualPrograms(string[] program, long? expectedSum = default)
        {
            Memory memory = new();
            RunProgram(program, memory);
            var sum = memory.AddressSpace.Values.Sum();
            _output.WriteLine($"{sum}");

            if (expectedSum.HasValue)
            {
                Assert.Equal(expectedSum, sum);
            }
        }

        public class Memory
        {
            public Dictionary<long, long> AddressSpace = new Dictionary<long, long>();
        }
        
        public void ParseMask(ReadOnlySpan<char> mask, ref long andMask, ref long orMask)
        {
            andMask = 0;
            orMask = 0;
            var maskLength = mask.Length;
            for (int i = 0; i < maskLength; ++i)
            {
                andMask <<= 1;
                orMask <<= 1;

                (andMask, orMask) = mask[i] switch
                {
                    '1' => (andMask + 1, orMask + 1),
                    '0' => (andMask, orMask),
                    _ => (andMask + 1, orMask)
                };
            }
        }

         void RunCommand(ReadOnlySpan<char> command, Memory memory, ref long andMask, ref long orMask)
        {
            if (command.StartsWith("mask"))
            {
                ParseMask(command[^36..^0], ref andMask, ref orMask);
            }
            else
            {
                var preIndex = command.IndexOf('[') + 1;
                var postIndex = command.IndexOf(']');
                var equalIndex = command.IndexOf('=') + 2;

                var index = long.Parse(command[preIndex..postIndex]);
                var unmaskedValue = long.Parse(command[equalIndex..^0]);

                memory.AddressSpace[index] = unmaskedValue & andMask | orMask;
            }
        }

        public void RunProgram(string[] program, Memory memory)
        {
            var numCommands = program.Length;
            long andMask = 0;
            long orMask = 0;
            for (int i = 0; i < numCommands; ++i)
            {
                RunCommand(program[i].AsSpan(), memory, ref andMask, ref orMask);
            }
        }
    }
}
