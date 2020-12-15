using System;
using System.Linq;

namespace AdventOfCode
{
    public static class Extensions
    {
        public static T[] Parse<T>(this string[] inputs, Func<string, T> transform)
        {
            return inputs.Select(transform).ToArray();
        }

        public static int[] ParseInts(this string[] inputs)
        {
            return inputs.Parse(int.Parse);
        }

        public static int[] ParseInts(this string[] inputs, int @default)
        {
            return inputs.Parse(i => int.TryParse(i, out var val) ? val : @default);
        }

        public static long[] ParseLongs(this string[] inputs)
        {
            return inputs.Parse(long.Parse);
        }
    }
}
