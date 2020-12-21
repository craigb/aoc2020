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

        public IEnumerable<object[]> GetMasks(int toy = 0)
        {
            var args = toy switch
            {
                _ => new object[] { File.ReadAllLines("Data/14_masks.txt") },
            };

            yield return args;
        }

        class Memory
        {
            Dictionary<long, long> AddressSpace;
        }
        
        public void ParseMask(Span<char> mask, ref long andMask, ref long orMask)
        {
            andMask = 0;
            orMask = 0;
            for (int i = 0; i < 36; ++i)
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

        public void RunCommand(Span<char> command, Memory memory, ref long andMask, ref long orMask)
        {
            if (command.StartsWith("mask"))
            {
                ParseMask(command[^36..^0], ref andMask, ref orMask);
            }
            else
            {
                
            }
        }

        public void RunProgram(string[] program, Memory memory)
        {
            var numCommands = program.Length;
            for (int i = 0; i < numCommands; ++i)
            {
                var command = program[i].Split;

            }
        }
    }
}
