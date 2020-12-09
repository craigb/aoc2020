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
    public class Puzzle05
    {
        private readonly ITestOutputHelper _output;
        public Puzzle05(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetSeats()
        {
            yield return new[] { File.ReadAllLines(@"Data/05_seats.txt") };
        }

        private static int ConvertSeatToId(string seat)
        {
            var span = seat.AsSpan();
            int id = 0;
            while (span.Length > 0)
            {
                id <<= 1;
                if (span[0] is 'B' or 'R')
                {
                    id += 1;
                }
                span = span.Slice(1);
            }

            return id;
        }

        [Theory]
        [InlineData("BFFFBBFRRR", 567)]
        [InlineData("BFFFBBF", 70)]
        public void ValidateConversion(string seat, int id)
        {
            Assert.Equal(id, ConvertSeatToId(seat));
        }

        [Theory]
        [MemberData(nameof(GetSeats))]
        public void HighestSeatNumber(string[] seats)
        {
            var max = seats.Select(seat => ConvertSeatToId(seat)).Max();

            _output.WriteLine($"{max}");
        }

        [Theory]
        [MemberData(nameof(GetSeats))]
        public void MySeatNumber(string[] seats)
        {
            var allIds = seats.Select(seat => ConvertSeatToId(seat)).OrderBy(id => id);
            var min = allIds.First();
            var afterMine = allIds.Where((id, i) => id != i + min).First();
            var mine = afterMine - 1;

            _output.WriteLine($"{mine}");
            Assert.DoesNotContain(mine, allIds);
        }
    }
}
