using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle7
    {
        private readonly ITestOutputHelper _output;

        private readonly Regex _definitionExtractor = new Regex(@"(?<bagDef>\w+ \w+) bags contain");
        private readonly Regex _extractor = new Regex(@"bags contain (?:((?<cardinality>\d+) (?<subBag>\w+ \w+) bag[s,]* ?)*)", RegexOptions.Compiled);

        public Puzzle7(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetBagDefinitions()
        {
            yield return new[] { File.ReadAllLines(@"Data/07_bags.txt") };
        }

        public static IEnumerable<object[]> GetKnownBagDefinitions()
        {
            yield return new object[]
            {
                "posh brown bags contain 5 dim coral bags, 1 plaid blue bag, 2 faded bronze bags, 2 light black bags.",
                "posh brown",
                new string[] { "dim coral", "plaid blue", "faded bronze", "light black" },
                new int[] { 5, 1, 2, 2 }
            };

            yield return new object[]
            {
                "posh brown bags contain no other bags.",
                "posh brown",
                new string[] { },
                new int[] { }
            };
        }

        public static IEnumerable<object[]> ToyBagDefinitions()
        {
            yield return new[] {
                @"shiny gold bags contain 2 dark red bags.
dark red bags contain 2 dark orange bags.
dark orange bags contain 2 dark yellow bags.
dark yellow bags contain 2 dark green bags.
dark green bags contain 2 dark blue bags.
dark blue bags contain 2 dark violet bags.
dark violet bags contain no other bags.".Split("\r\n")};
        }

        private (string, IDictionary<string, int>) DecomposeDefinition(string def)
        {
            var defMatch = _definitionExtractor.Match(def);
            if (!defMatch.Success)
            {
                throw new Exception("Unable to parse definition");
            }
            var bagDef = defMatch.Groups["bagDef"].Value;
            var ingredients = new Dictionary<string, int>();

            var m = _extractor.Match(def);
            if (m.Success)
            {
                ingredients = m.Groups["subBag"].Captures.Zip(m.Groups["cardinality"].Captures).ToDictionary(zipped => zipped.First.Value, zipped => int.Parse(zipped.Second.Value));
            }

            return (bagDef, ingredients);
        }

        [Theory]
        [MemberData(nameof(GetKnownBagDefinitions))]
        public void CanParseBagDefinitions(string definition, string expectedBagDef, string[] bagNames, int[] cardinality)
        {
            var (bagDef, ingredients) = DecomposeDefinition(definition);
            Assert.Equal(expectedBagDef, bagDef);
            for (int i = 0; i < bagNames.Length; ++i)
            {
                Assert.Equal(cardinality[i], ingredients[bagNames[i]]);
            }
        }

        [Theory]
        [MemberData(nameof(GetBagDefinitions))]
        public void CanFindAllShinyGoldBags(string[] definitions)
        {
            var seeking = new HashSet<string> { "shiny gold" };
            var all = definitions.Select(DecomposeDefinition).ToList();

            var startSize = 0;
            do
            {
                startSize = seeking.Count;
                foreach (var definition in all)
                {
                    if (seeking.Any(s => definition.Item2.ContainsKey(s)))
                    {
                        seeking.Add(definition.Item1);
                    }
                }
            } while (startSize != seeking.Count);

            _output.WriteLine($"{seeking.Count - 1}");
        }

        [Theory]
        [MemberData(nameof(GetBagDefinitions))]
        [MemberData(nameof(ToyBagDefinitions))]
        public void CanFindAllBagsInShinyGoldBags(string[] definitions)
        {
            var seeking = new Stack<(string, int)>(new[] { ("shiny gold", 1) });
            var all = definitions.Select(DecomposeDefinition).ToDictionary(elem => elem.Item1, elem => elem.Item2);

            long total = 0;

            while (seeking.Count > 0)
            {
                var (current, count) = seeking.Pop();
                total += count;

                if (all.TryGetValue(current, out var contents) && contents is { Count:>0 })
                {
                    foreach (var (def, num) in contents)
                    {
                        seeking.Push((def, num * count));
                    }
                }
            }

            _output.WriteLine($"{total - 1}");
        }
    }
}
