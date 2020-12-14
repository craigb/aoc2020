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
    public class Puzzle11
    {
        private readonly ITestOutputHelper _output;

        public Puzzle11(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetSeats(int toy = 0)
        {
            var args = toy switch
            {
                1 => new object[] { ReadGrid(GetToySeating()), 37 },
                2 => new object[] { ReadGrid(GetToySeating()), 26 },
                _ => new object[] { ReadGrid(File.ReadAllLines("Data/11_seats.txt")) },
            };

            yield return args;
        }

        public static string[] GetToySeating()
        {
            return @"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL".Split("\r\n");
        }

        private static char[][] ReadGrid(string[] rows)
        {
            var numColumns = rows[0].Length;
            var grid = new char[rows.Length][];
            var numRows = rows.Length;
            for (int y = 0; y < numRows; ++y)
            {
                grid[y] = rows[y].ToCharArray();
            }

            return grid;
        }

        public static class SeatStatus
        {
            public const char Floor = '.';
            public const char Occupied = '#';
            public const char Empty = 'L';
        }

        private int DirectlySurrounding(char[][] plane, int row, int col)
        {
            var count = 0;
            var upper = row - 1;
            var lower = row + 1;
            var left = col - 1;
            var right = col + 1;
            var rightMost = plane[0].Length - 1;
            var lowest = plane.Length - 1;
            if (upper >= 0)
            {
                if (plane[upper][col] == SeatStatus.Occupied)
                {
                    ++count;
                }
                if (left >= 0 && plane[upper][left] == SeatStatus.Occupied)
                {
                    ++count;
                }
                if (right <= rightMost && plane[upper][right] == SeatStatus.Occupied)
                {
                    ++count;
                }
            }
            if (left >= 0 && plane[row][left] == SeatStatus.Occupied)
            {
                ++count;
            }
            if (right <= rightMost && plane[row][right] == SeatStatus.Occupied)
            {
                ++count;
            }
            if (lower <= lowest)
            {
                if (plane[lower][col] == SeatStatus.Occupied)
                {
                    ++count;
                }
                if (left >= 0 && plane[lower][left] == SeatStatus.Occupied)
                {
                    ++count;
                }
                if (right <= rightMost && plane[lower][right] == SeatStatus.Occupied)
                {
                    ++count;
                }
            }
            return count;
        }

        private char ApplySeatingRule(char[][] plane, int row, int col, Func<char[][], int, int, int> getSurroundingSeats, int limitOccupied)
        {
            var surrounded = getSurroundingSeats(plane, row, col);
            var current = plane[row][col];
            return (current, surrounded) switch
            {
                (SeatStatus.Floor, _) => SeatStatus.Floor,
                (SeatStatus.Empty, 0) => SeatStatus.Occupied,
                (SeatStatus.Occupied, int x) when x >= limitOccupied => SeatStatus.Empty,
                _ => current
            };
        }

        private void IterateSeating(ref char[][] plane, Func<char[][], int, int, int> getSurroundingSeats, int limitOccupied)
        {
            var numRows = plane.Length;
            var numCols = plane[0].Length;
            var buffer = new char[numRows][];

            for (int row = 0; row < numRows; ++row)
            {
                buffer[row] = new char[numCols];
            }

            var change = true;
            while (change)
            {
                change = false;

                for (int row = 0; row < numRows; ++row)
                {
                    for (int col = 0; col < numCols; ++col)
                    {
                        var buffSeat = ApplySeatingRule(plane, row, col, getSurroundingSeats, limitOccupied);
                        change |= buffSeat != plane[row][col];
                        buffer[row][col] = buffSeat;
                    }
                }

                (plane, buffer) = (buffer, plane);
            }
        }

        private (int occupied, int empty, int floor) CountSeats(char[][] plane)
        {
            var numRows = plane.Length;
            var numCols = plane[0].Length;

            int occupied = 0;
            int empty = 0;
            int floor = 0;
            for (int row = 0; row < numRows; ++row)
            {
                for (int col = 0; col < numCols; ++col)
                {
                    (occupied, empty, floor) = plane[row][col] switch
                    {
                        SeatStatus.Occupied => (occupied + 1, empty, floor),
                        SeatStatus.Empty => (occupied, empty+1, floor),
                        _ => (occupied, empty, floor+1)
                    };
                }
            }

            return (occupied, empty, floor);
        }

        private string PrintPlane(char[][] plane)
        {
            StringBuilder sb = new();
            var numRows = plane.Length;
            var numCols = plane[0].Length;

            for (int row = 0; row < numRows; ++row)
            {
                for (int col = 0; col < numCols; ++col)
                {
                    sb.Append(plane[row][col]);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private int SearchVisibly(char[][] plane, int startRow, int startCol, Func<int, int, bool>predicate, Func<int, int, (int row, int col)> update)
        {
            var (row, col) = (startRow, startCol);
            while (predicate(row, col) && plane[row][col] == SeatStatus.Floor)
            {
                (row, col) = update(row, col);
            }
            if (predicate(row, col) && plane[row][col] == SeatStatus.Occupied)
            {
                return 1;
            }
            return 0;
        }

        private int VisiblySurrounding(char[][] plane, int row, int col)
        {
            var count = 0;
            var upper = row - 1;
            var lower = row + 1;
            var left = col - 1;
            var right = col + 1;
            var rightMost = plane[0].Length - 1;
            var lowest = plane.Length - 1;

            count += SearchVisibly(plane, upper, left,  (r, c) => r >= 0 && c >= 0, (r, c) => (r - 1, c - 1));
            count += SearchVisibly(plane, upper, col,   (r, c) => r >= 0, (r, c) => (r - 1, c));
            count += SearchVisibly(plane, upper, right, (r, c) => r >= 0 && c <= rightMost, (r, c) => (r - 1, c + 1));

            count += SearchVisibly(plane, row, left,  (r, c) => c >= 0, (r, c) => (r, c - 1));
            count += SearchVisibly(plane, row, right, (r, c) => c <= rightMost, (r, c) => (r, c + 1));

            count += SearchVisibly(plane, lower, left,  (r, c) => r <= lowest && c >= 0, (r, c) => (r + 1, c - 1));
            count += SearchVisibly(plane, lower, col,   (r, c) => r <= lowest, (r, c) => (r + 1, c));
            count += SearchVisibly(plane, lower, right, (r, c) => r <= lowest && c <= rightMost, (r, c) => (r + 1, c + 1));

            return count;
        }

        [Theory]
        [MemberData(nameof(GetSeats), parameters:1)]
        public void ValidateOccupiedSeats1(char[][] plane, int expectedOccupied)
        {
            IterateSeating(ref plane, DirectlySurrounding, 4);
            var (occupied, _, _) = CountSeats(plane);
            Assert.Equal(expectedOccupied, occupied);
        }

        [Theory]
        [MemberData(nameof(GetSeats), parameters:0)]
        public void GetOccupiedSeats(char[][] plane)
        {
            IterateSeating(ref plane, DirectlySurrounding, 4);
            var (occupied, _, _) = CountSeats(plane);
            _output.WriteLine($"{occupied}");
        }

        [Theory]
        [MemberData(nameof(GetSeats), parameters:2)]
        public void ValidateOccupiedSeats2(char[][] plane, int expectedOccupied)
        {
            IterateSeating(ref plane, VisiblySurrounding, 5);
            var (occupied, _, _) = CountSeats(plane);
            Assert.Equal(expectedOccupied, occupied);
        }

        [Theory]
        [MemberData(nameof(GetSeats), parameters:0)]
        public void GetOccupiedSeats2(char[][] plane)
        {
            IterateSeating(ref plane, VisiblySurrounding, 5);
            var (occupied, _, _) = CountSeats(plane);
            _output.WriteLine($"{occupied}");
        }
    }
}
