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
    public class Puzzle09
    {
        private readonly ITestOutputHelper _output;

        public Puzzle09(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public static IEnumerable<object[]> GetCipher()
        {
            return ConvertToLongs(GetRawCipher);
        }

        public static IEnumerable<object[]> GetToyCipher()
        {
            return ConvertToLongs(GetRawToyCipher);
        }

        public static IEnumerable<object[]> GetRawCipher()
        {
            yield return new object[] { File.ReadAllLines("Data/09_cipher.txt"), 25 };
        }

        public static IEnumerable<object[]> GetRawToyCipher()
        {
            yield return new object[] { @"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576".Split("\r\n"), 5 };
        }

        public static IEnumerable<object[]> ConvertToLongs(Func<IEnumerable<object[]>> getInputs)
        {
            foreach (var input in getInputs())
            {
                if (input[0] is string[] strings)
                {
                    input[0] = strings.Select(long.Parse).ToArray();
                    yield return input;
                }
            }
        }

        private long FindOutlier(long[] cipher, int lookback)
        {
            var cipherSpan = cipher.AsSpan();
            var window = new long[lookback];

            var valuesToCheck = cipher.Length - lookback;
            for (int i = lookback; i < valuesToCheck; ++i)
            {
                var cipherWindow = cipherSpan.Slice(i - lookback, lookback);
                cipherWindow.CopyTo(window);
                var current = cipher[i];
                int j = 0, k = 0;
                for (j = 0; j < lookback; ++j)
                {
                    window[j] = current - window[j];
                }
                for (j = 0; j < lookback; ++j)
                {
                    for (k = 0; k < lookback; ++k)
                    {
                        if (window[j] == cipherWindow[k] && cipherWindow[j] != cipherWindow[k])
                        {
                            break;
                        }
                    }
                    if (k < lookback)
                    {
                        break;
                    }
                }
                if (j < lookback && k < lookback)
                {
                    continue;
                }

                return current;
            }

            return -1;
        }

        [Theory]
        [MemberData(nameof(GetToyCipher))]
        [MemberData(nameof(GetCipher))]
        public void CanFindOutlier(long[] cipher, int lookback)
        {
            var outlier = FindOutlier(cipher, lookback);
            _output.WriteLine($"{outlier}");
            Assert.True(outlier > 0, "Unable to find outlier");
        }

        private (int min, int max) SubarraySum(long[] cipher, long target)
        {
            int i = 0, j = 0;
            long sum = cipher[0];
            for (; i < cipher.Length && j < cipher.Length;)
            {
                if (sum == target)
                {
                    return (i, j);
                }
                if (sum < target)
                {
                    j++;
                    if (j < cipher.Length)
                    {
                        sum += cipher[j];
                    }
                }
                else
                {
                    sum -= cipher[i];
                    i++;
                }
            }
            return (-1, -1);
        }

        [Theory]
        [MemberData(nameof(GetToyCipher))]
        [MemberData(nameof(GetCipher))]
        public void FindSubArray(long[] cipher, int lookback)
        {
            var outlier = FindOutlier(cipher, lookback);
            var (min, max) = SubarraySum(cipher, outlier) switch
            {
                (-1, -1) => throw new Exception("Unable to find target"),
                (int s, int e) => (s, e),
            };


            var span = cipher.AsSpan(min..(max+1));
            long? lowest = null;
            long? highest = null;
            for (int i = 0; i < span.Length; ++i)
            {
                if (lowest is null || span[i] < lowest)
                {
                    lowest = span[i];
                }
                if (highest is null || span[i] > highest)
                {
                    highest = span[i];
                }
            }

            _output.WriteLine($"({min} .. {max})");
            _output.WriteLine($"{lowest + highest}");
        }
    }
}
