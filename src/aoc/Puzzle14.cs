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
        public delegate void MaskParser(ReadOnlySpan<char> maskString, ref long mask1, ref long mask2);
        public delegate void MaskApplicator(Memory memory, long index, long val, long mask1, long mask2);

        private readonly ITestOutputHelper _output;

        public Puzzle14(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetMasks1()
        {
            yield return new object[] { GetToyExample1(), 165L };
            yield return new object[] { File.ReadAllLines("Data/14_masks.txt") };
        }

        public static IEnumerable<object[]> GetMasks2()
        {
            yield return new object[] { GetToyExample2(), 208L };
            yield return new object[] { File.ReadAllLines("Data/14_masks.txt") };
        }

        public static string[] GetToyExample1()
        {
            return @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0".Split("\r\n");
        }

        public static string[] GetToyExample2()
        {
            return @"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1".Split("\r\n");
        }

        [Theory]
        [MemberData(nameof(GetMasks1))]
        public void RunActualPrograms1(string[] program, long? expectedSum = default)
        {
            Memory memory = new();
            RunProgram1(program, memory);
            var sum = memory.AddressSpace.Values.Sum();
            _output.WriteLine($"{sum}");

            if (expectedSum.HasValue)
            {
                Assert.Equal(expectedSum, sum);
            }
        }

        [Theory]
        [MemberData(nameof(GetMasks2))]
        public void RunActualPrograms2(string[] program, long? expectedSum = default)
        {
            Memory memory = new();
            RunProgram2(program, memory);
            var sum = memory.AddressSpace.Values.Sum();
            _output.WriteLine($"{sum}");

            if (expectedSum.HasValue)
            {
                Assert.Equal(expectedSum, sum);
            }
        }

        public interface IStrategy
        {
            void ParseMask(ReadOnlySpan<char> maskString);
            void ApplyMask(Memory memory, long index, long val);
        }


        public class Strategy1 : IStrategy
        {
            private long _andMask;
            private long _orMask;
            
            public void ParseMask(ReadOnlySpan<char> mask)
            {
                _andMask = 0;
                _orMask = 0;
                var maskLength = mask.Length;
                for (int i = 0; i < maskLength; ++i)
                {
                    _andMask <<= 1;
                    _orMask <<= 1;

                    (_andMask, _orMask) = mask[i] switch
                    {
                        '1' => (_andMask + 1, _orMask + 1),
                        '0' => (_andMask, _orMask),
                        _ => (_andMask + 1, _orMask)
                    };
                }
            }

            public void ApplyMask(Memory memory, long index, long unmaskedValue)
            {
                memory.AddressSpace[index] = unmaskedValue & _andMask | _orMask;
            }
        }

        public class Strategy2 : IStrategy
        {
            private long _orMask;
            private readonly List<int> _indices = new List<int>();

            public void ApplyMask(Memory memory, long index, long val)
            {
                var allIndices = GetAllIndexCombinations(index);
                foreach (var i in allIndices)
                {
                    memory.AddressSpace[i] = val;
                }
            }

            private ICollection<long> GetAllIndexCombinations(long index)
            {
                var tempIndex = index | _orMask;

                return RecursivelyGetIndexCombinations(new HashSet<long> { tempIndex }, 0, _indices.Count - 1);
            }

            private ICollection<long> RecursivelyGetIndexCombinations(HashSet<long> existingCombinations, int currentIndex, int maxIndex)
            {
                if (currentIndex > maxIndex)
                {
                    return existingCombinations;
                }

                var orMask = 1L << _indices[currentIndex];
                var andMask = ~orMask;

                var newCombos = new List<long>();
                foreach (var combo in existingCombinations)
                {
                    newCombos.Add(combo | orMask);
                    newCombos.Add(combo & andMask);
                }

                existingCombinations.UnionWith(newCombos);

                return RecursivelyGetIndexCombinations(existingCombinations, currentIndex + 1, maxIndex);
            }

            public void ParseMask(ReadOnlySpan<char> mask)
            {
                _indices.Clear();
                _orMask = 0;
                var maxMaskIndex = mask.Length - 1;
                int? indexToAdd;
                for (int i = 0; i <= maxMaskIndex; ++i)
                {
                    _orMask <<= 1;

                    (indexToAdd, _orMask) = mask[i] switch
                    {
                        '1' => (null, _orMask + 1),
                        'X' => ((int?)(maxMaskIndex - i), _orMask),
                        _ => (null, _orMask),
                    };
                    
                    if (indexToAdd.HasValue)
                    {
                        _indices.Add(indexToAdd.GetValueOrDefault());
                    }
                }
            }
        }

        public class Memory
        {
            public Dictionary<long, long> AddressSpace = new Dictionary<long, long>();
        }
        

        private void RunCommand(ReadOnlySpan<char> command, Memory memory, IStrategy strategy)
        {
            if (command.StartsWith("mask"))
            {
                strategy.ParseMask(command[^36..^0]);
            }
            else
            {
                var preIndex = command.IndexOf('[') + 1;
                var postIndex = command.IndexOf(']');
                var equalIndex = command.IndexOf('=') + 2;

                var index = long.Parse(command[preIndex..postIndex]);
                var unmaskedValue = long.Parse(command[equalIndex..^0]);

                strategy.ApplyMask(memory, index, unmaskedValue);
            }
        }

        private void RunProgram1(string[] program, Memory memory)
        {
            var numCommands = program.Length;
            var strategy = new Strategy1();
            for (int i = 0; i < numCommands; ++i)
            {
                RunCommand(program[i].AsSpan(), memory, strategy);
            }
        }

        private void RunProgram2(string[] program, Memory memory)
        {
            var numCommands = program.Length;
            var strategy = new Strategy2();
            for (int i = 0; i < numCommands; ++i)
            {
                RunCommand(program[i].AsSpan(), memory, strategy);
            }
        }
    }
}
