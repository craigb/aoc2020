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
    public class Puzzle06
    {
        private readonly ITestOutputHelper _output;
        public Puzzle06(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetAnswers()
        {
            yield return new[] { File.ReadAllLines(@"Data/06_answers.txt") };
        }

        public static IEnumerable<string> CollectGroups(string[] answers)
        {
            StringBuilder group = new();
            foreach (var answer in answers)
            {
                if (string.IsNullOrWhiteSpace(answer))
                {
                    yield return group.ToString();
                    group.Clear();
                }
                else
                {
                    group.Append(answer.Trim());
                }
            }

            if (group.Length > 0)
            {
                yield return group.ToString();
            }
        }

        public static IEnumerable<string> CollectIntersectingGroups(string[] answers)
        {
            HashSet<char> group = null;
            foreach (var answer in answers)
            {
                if (string.IsNullOrWhiteSpace(answer))
                {
                    yield return new string(group.ToArray());
                    group = null;
                }
                else
                {
                    if (group is null)
                    {
                        group = new(answer.Trim());
                    }
                    else
                    {
                        group.IntersectWith(answer.Trim());
                    }
                }
            }

            if (group is not null && group.Any())
            {
                yield return new string(group.ToArray());
            }
        }

        public int CountAnswers(string[] answers)
        {
            int total = 0;
            var groups = CollectGroups(answers);
            foreach (var group in groups)
            {
                total += group.GroupBy(ch => ch).Count();
            }

            return total;
        }

        public int CountCommonAnswers(string[] answers)
        {
            int total = 0;
            var groups = CollectIntersectingGroups(answers);
            foreach (var group in groups)
            {
                total += group.GroupBy(ch => ch).Count();
            }

            return total;
        }

        [Theory]
        [InlineData(@"abc

a
b
c

ab
ac

a
a
a
a

b", 11)]
        public void TestAnswerCount(string answers, int count)
        {
            var total = CountAnswers(answers.Split("\n"));

            Assert.Equal(count, total);
        }

        [Theory]
        [InlineData(@"abc

a
b
c

ab
ac

a
a
a
a

b", 6)]
        public void TestAnswerCount2(string answers, int count)
        {
            var total = CountCommonAnswers(answers.Split("\n"));

            Assert.Equal(count, total);
        }

        [Theory]
        [MemberData(nameof(GetAnswers))]
        public void CommonAnswerCount(string[] answers)
        {
            var total = CountCommonAnswers(answers);
            _output.WriteLine($"{total}");
        }



        [Theory]
        [MemberData(nameof(GetAnswers))]
        public void AnswerCount(string[] answers)
        {
            int total = CountAnswers(answers);

            _output.WriteLine($"{total}");
        }

    }
}
