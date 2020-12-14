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
    public class Puzzle10
    {
        private readonly ITestOutputHelper _output;

        public Puzzle10(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetAdapters(int toy = 0)
        {
            var inputs = toy switch
            {
                1 => new object[] { GetToyAdapters().ParseInts(), (7, 5) },
                2 => new object[] { GetToyAdapters2().ParseInts(), (22, 10) },
                _ => new[] { File.ReadAllLines("Data/10_adapters.txt").ParseInts() }
            };

            yield return inputs;
        }

        public static IEnumerable<object[]> GetAdaptersForArrangement(int toy = 0)
        {
            var inputs = toy switch
            {
                1 => new object[] { GetToyAdapters().ParseInts(), 8UL },
                2 => new object[] { GetToyAdapters2().ParseInts(), 19208UL },
                _ => new[] { File.ReadAllLines("Data/10_adapters.txt").ParseInts() }
            };

            yield return inputs;
        }

        public (int single, int @double, int triple) GetDifferences(int[] adapters)
        {
            var sortedAdapters = adapters.OrderBy(a => a).ToList();
            int last = 0;
            int single = 0;
            int @double = 0;
            int triple = 1;

            foreach (var adapter in sortedAdapters)
            {
                var difference = adapter - last;
                switch (difference)
                {
                    case 3:
                        ++triple;
                        break;
                    case 2:
                        ++@double;
                        break;
                    case 1:
                        ++single;
                        break;
                    default:
                        throw new Exception($"Unexpected adapter difference: {difference}");
                }

                last = adapter;
            }

            return (single, @double, triple);
        }

        public ulong GetArrangements(int[] adapters)
        {
            var sortedAdapters = adapters.OrderBy(a => a).ToList();
            var arrangements = new ulong[adapters.Max()+1];

            arrangements[0] = 1;
            foreach (var adapter in sortedAdapters)
            {
                foreach (var point in arrangements[Math.Max(0, adapter - 3)..adapter])
                {
                    arrangements[adapter] += point;
                }
            }

            return arrangements[^1];
        }

        [Theory]
        [MemberData(nameof(GetAdaptersForArrangement), parameters: 1)]
        [MemberData(nameof(GetAdaptersForArrangement), parameters: 2)]
        public void ValidateArrangements(int[] adapters, ulong expectedArrangements)
        {
            var arrangements = GetArrangements(adapters);
            Assert.Equal(expectedArrangements, arrangements);
        }

        [Theory]
        [MemberData(nameof(GetAdapters), parameters: 1)]
        [MemberData(nameof(GetAdapters), parameters: 2)]
        public void ValidateAlgo(int[] adapters, (int single, int triple)expectedDifferences)
        {
            var differences = GetDifferences(adapters);
            Assert.Equal(expectedDifferences.single, differences.single);
            Assert.Equal(expectedDifferences.triple, differences.triple);
        }

        [Theory]
        [MemberData(nameof(GetAdapters), parameters: 0)]
        public void GetAnswer1(int[] adapters)
        {
            var differences = GetDifferences(adapters);
            _output.WriteLine($"{differences.single} * {differences.triple} = {differences.single * differences.triple}");
        }

        [Theory]
        [MemberData(nameof(GetAdapters), parameters: 0)]
        public void GetAnswer2(int[] adapters)
        {
            var arrangements = GetArrangements(adapters);
            _output.WriteLine($"{arrangements}");
        }

        public static string[] GetToyAdapters()
        {
            return @"16
10
15
5
1
11
7
19
6
12
4".Split("\r\n");
        }

        public static string[] GetToyAdapters2()
        {
            return @"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3".Split("\r\n");
        }
    }
}
