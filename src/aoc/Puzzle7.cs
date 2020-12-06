using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Data
{
    public class Puzzle7
    {
        private readonly ITestOutputHelper _output;
        public Puzzle7(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetAnswers()
        {
            yield return new[] { File.ReadAllLines(@"Data/") };
        }
    }
}
