using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit;
using System.IO;

namespace AdventOfCode
{
    public class Puzzle08
    {
        private readonly ITestOutputHelper _output;

        public Puzzle08(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetInstructions()
        {
            yield return new object[] { File.ReadAllLines("Data/08_instructions.txt"), null };
        }

        public static IEnumerable<object[]> GetExampleInstructions()
        {
            yield return new object[]
            {
                @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6".Split("\r\n"), 5
            };
        }

        public static (string Instruction, int Number) ParseInstruction(string instruction)
        {
            var split = instruction.Trim().Split(" ");
            return (split[0], int.Parse(split[1].TrimStart('+')));
        }

        private static bool TryRunCommands((string Instruction, int Number)[] parsedInstructions, out int acc)
        {
            acc = 0;
            int i = 0;
            var totalInstructions = parsedInstructions.Length;
            var visited = Enumerable.Repeat(false, totalInstructions).ToArray();

            while (i < totalInstructions && visited[i] == false)
            {
                visited[i] = true;
                var (add, offset) = parsedInstructions[i] switch
                {
                    ("acc", int a) => (a, 1),
                    ("jmp", int o) => (0, o),
                    _ => (0, 1)
                };

                acc += add;
                i += offset;
            }

            return i >= totalInstructions;
        }

        [Theory]
        [MemberData(nameof(GetExampleInstructions))]
        [MemberData(nameof(GetInstructions))]
        public void RunToFirstLoop(string[] instructions, int? accumulator)
        {
            var parsedInstructions = instructions.Select(ParseInstruction).ToArray();
            TryRunCommands(parsedInstructions, out var acc);

            if (accumulator.HasValue)
            {
                Assert.Equal(accumulator, acc);
            }
            _output.WriteLine($"{acc}");
        }

        [Theory]
        [MemberData(nameof(GetExampleInstructions))]
        [MemberData(nameof(GetInstructions))]
        public void FindRightMutation(string[] instructions, int? accumulator)
        {
            var parsedInstructions = instructions.Select(ParseInstruction).ToArray();
            var totalInstructions = parsedInstructions.Length;
            for (int i = 0; i < totalInstructions; ++i)
            {
                string tempInstruction = parsedInstructions[i].Instruction;
                if (tempInstruction == "acc")
                {
                    continue;
                }
                else if(tempInstruction == "jmp")
                {
                    parsedInstructions[i].Instruction = "nop";
                }
                else
                {
                    parsedInstructions[i].Instruction = "jmp";
                }

                if (TryRunCommands(parsedInstructions, out int acc))
                {
                    _output.WriteLine($"{acc}");
                    return;
                }

                parsedInstructions[i].Instruction = tempInstruction;
            }

        }
    }
}
