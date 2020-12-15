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
    public class Puzzle12
    {
        private readonly ITestOutputHelper _output;

        public Puzzle12(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetInstructions(int toy = 0)
        {
            var args = toy switch
            {
                1 => new object[] { GetToyInstructions(), 25 },
                2 => new object[] { GetToyInstructions(), 286 },
                _ => new object[] { File.ReadAllLines("Data/12_instructions.txt") }
            };

            yield return args;
        }
        
        public static string[] GetToyInstructions()
        {
            return @"F10
N3
F7
R90
F11".Split("\r\n");
        }

        public static class Directions
        {
            public const char North = 'N';
            public const char South = 'S';
            public const char East = 'E';
            public const char West = 'W';
            public const char Left = 'L';
            public const char Right = 'R';
            public const char Forward = 'F';
        }

        public record Position
        {
            public double Latitude = 0;
            public double Longitude = 0;
            public (double Lat, double Long) Heading = (0,0);

            public Position(int latitude, int longitude, char heading)
            {
                Latitude = latitude;
                Longitude = longitude;
                Heading = heading switch
                {
                    Directions.North => (1, 0),
                    Directions.East => (0, 1),
                    Directions.South => (-1, 0),
                    Directions.West => (0, -1),
                    _ => throw new Exception($"Unrecognized heading: {heading}")
                };
            }

            public Position(double latitude, double longitude, (double, double) heading)
            {
                Latitude = latitude;
                Longitude = longitude;
                Heading = heading;
            }

            public Position Move(string operation)
            {
                var op = operation.AsSpan();
                var num = int.Parse(op.Slice(1));
                return (op[0], num) switch
                {
                    (Directions.North, int d) => new Position(Latitude+d, Longitude, Heading),
                    (Directions.South, int d) => new Position(Latitude-d, Longitude, Heading),
                    (Directions.East, int d) => new Position(Latitude, Longitude+d, Heading),
                    (Directions.West, int d) => new Position(Latitude, Longitude-d, Heading),
                    (Directions.Forward, int d) => new Position(Latitude+d*Heading.Lat, Longitude+d*Heading.Long, Heading),
                    (Directions.Right, int d) => new Position(Latitude, Longitude, RotateDegrees(-d, Heading)),
                    (Directions.Left, int d) => new Position(Latitude, Longitude, RotateDegrees(d, Heading)),
                    (char o, _) => throw new Exception($"Unexpected direction: {o}")
                };
            }

            public Position Move2(string operation)
            {
                var op = operation.AsSpan();
                var num = int.Parse(op.Slice(1));
                return (op[0], num) switch
                {
                    (Directions.North, int d) => new Position(Latitude, Longitude, (Heading.Lat+d, Heading.Long)),
                    (Directions.South, int d) => new Position(Latitude, Longitude, (Heading.Lat-d, Heading.Long)),
                    (Directions.East, int d) => new Position(Latitude, Longitude, (Heading.Lat, Heading.Long+d)),
                    (Directions.West, int d) => new Position(Latitude, Longitude, (Heading.Lat, Heading.Long-d)),
                    (Directions.Forward, int d) => new Position(Latitude+d*Heading.Lat, Longitude+d*Heading.Long, Heading),
                    (Directions.Right, int d) => new Position(Latitude, Longitude, RotateDegrees(-d, Heading)),
                    (Directions.Left, int d) => new Position(Latitude, Longitude, RotateDegrees(d, Heading)),
                    (char o, _) => throw new Exception($"Unexpected direction: {o}")
                };
            }

            public static (double, double) RotateDegrees(double degrees, (double lat, double lon)heading)
            {
                var radian = degrees / 360.0 * 2.0 * Math.PI;
                var sin = (int)Math.Sin(radian);
                var cos = (int)Math.Cos(radian);
                return (heading.lon * sin + heading.lat * cos, heading.lon * cos - heading.lat * sin);
            }
        }

        [Theory]
        [MemberData(nameof(GetInstructions), parameters: 1)]
        public void ValidateMovement(string[] operations, int expectedManhattanDistance)
        {
            var current = new Position(0, 0, Directions.East);
            foreach (var op in operations)
            {
                current = current.Move(op);
            }

            _output.WriteLine($"{current.Latitude}, {current.Longitude}");
            Assert.Equal(expectedManhattanDistance, Math.Abs(current.Latitude) + Math.Abs(current.Longitude));
        }

        [Theory]
        [MemberData(nameof(GetInstructions), parameters: 0)]
        public void GetDistance(string[] operations)
        {
            var current = new Position(0, 0, Directions.East);
            foreach (var op in operations)
            {
                current = current.Move(op);
            }

            _output.WriteLine($"{current.Latitude}, {current.Longitude}");
            _output.WriteLine($"{Math.Abs(current.Latitude) + Math.Abs(current.Longitude)}");
        }


        [Theory]
        [MemberData(nameof(GetInstructions), parameters: 2)]
        public void ValidateMovement2(string[] operations, int expectedManhattanDistance)
        {
            var current = new Position(0, 0, (1,10));
            foreach (var op in operations)
            {
                current = current.Move2(op);
            }

            _output.WriteLine($"{current.Latitude}, {current.Longitude}");
            Assert.Equal(expectedManhattanDistance, Math.Abs(current.Latitude) + Math.Abs(current.Longitude));
        }

        [Theory]
        [MemberData(nameof(GetInstructions), parameters: 0)]
        public void GetDistance2(string[] operations)
        {
            var current = new Position(0, 0, (1, 10));
            foreach (var op in operations)
            {
                current = current.Move2(op);
            }

            _output.WriteLine($"{current.Latitude}, {current.Longitude}");
            _output.WriteLine($"{Math.Abs(current.Latitude) + Math.Abs(current.Longitude)}");
        }
    }
}
