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
    public class Puzzle13
    {
        private readonly ITestOutputHelper _output;

        public Puzzle13(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetNotes(int toy = 0)
        {
            var inputUnparsed = toy switch
            {
                1 => GetToyData1(),
                _ => File.ReadAllLines("Data/13_notes.txt"),
            };

            var input = ParseInput(inputUnparsed);
            var args = toy switch
            {
                1 => new object[] { input.arrival, input.buses, 295 },
                _ => new object[] { input.arrival, input.buses },
            };

            yield return args;
        }

        public static IEnumerable<object[]> GetNotes2()
        {
            var data = GetToyData2();
            foreach (var datum in data)
            {
                var pieces = datum.Split(" ");
                yield return new object[] { pieces[0].Split(',').ParseInts(0), long.Parse(pieces[1]) };
            }
        }

        public static string[] GetToyData1()
        {
            return @"939
7,13,x,x,59,x,31,19".Split("\r\n");
        }

        public static string[] GetToyData2()
        {
            return @"17,x,13,19 3417
67,7,59,61 754018
67,x,7,59,61 779210
67,7,x,59,61 1261476
1789,37,47,1889 1202161486
7,13,x,x,59,x,31,19 1068781".Split("\r\n");
        }

        private static (int arrival, int[] buses) ParseInput(string[] lines)
        {
            var arrival = int.Parse(lines[0]);
            var buses = lines[1]
                .Split(',')
                .Select(b => int.TryParse(b, out int bus) ? bus : 0)
                .ToArray();

            return (arrival, buses);
        }

        public (int bus, int wait) GetNearestBusAfterArrival(int arrival, int[] buses)
        {
            return buses
                .Where (b => b > 0)
                .Select(b => (bus:b, wait:(-arrival % b) + b ))
                .OrderBy(a => a.wait)
                .First();
        }

        public long GetMagicalTime(int[] buses)
        {
            var time = 0L;
            var multiplier = 1L;
            for (int i = 0; i < buses.Length; ++i)
            {
                var bus = buses[i];
                if (bus == 0)
                {
                    continue;
                }

                var k = Unmod(time, multiplier, bus, bus-i);

                time += k * multiplier;
                multiplier *= bus;
            }

            return time;
        }

        public int Unmod(long total, long multiplier, int modulo, int result)
        {
            var add = (int)(multiplier % modulo);
            int count = 0;
            int value = (int)(total % modulo);
            result %= modulo;
            if (result < 0)
            {
                result += modulo;
            }
            for (; value != result; ++count)
            {
                value += add;
                if (value > modulo)
                {
                    value -= modulo;
                }
            }

            return count;
        }

        [Theory]
        [InlineData(0, 17*13, 19, 5)]
        [InlineData(119, 17*13, 19, 3)]
        public void RunUnmod(long total, long multiplier, int modulo, int result)
        {
            Assert.Equal(0, Unmod(total, multiplier, modulo, result));
        }

        [Theory]
        [MemberData(nameof(GetNotes), parameters: 1)]
        public void ValidateAlgo1(int arrival, int[] buses, int expectedProduct)
        {
            var (bus, wait) = GetNearestBusAfterArrival(arrival, buses);
            Assert.Equal(expectedProduct, bus * wait);
        }

        [Theory]
        [MemberData(nameof(GetNotes), parameters: 0)]
        public void GetProduct(int arrival, int[] buses)
        {
            var (bus, wait) = GetNearestBusAfterArrival(arrival, buses);
            _output.WriteLine($"{bus * wait}");
        }

        [Theory]
        [MemberData(nameof(GetNotes2))]
        public void ValidateAlgo2(int[] buses, long expectedTime)
        {
            var time = GetMagicalTime(buses);
            Assert.Equal(expectedTime, time);
        }

        [Theory]
        [MemberData(nameof(GetNotes), parameters: 0)]
        public void GetTime(int arrival, int[] buses)
        {
            var time = GetMagicalTime(buses);
            _output.WriteLine($"{time}");
        }
    }
}
