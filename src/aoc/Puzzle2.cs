using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle2
    {
        private readonly ITestOutputHelper _output;

        public Puzzle2(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        private readonly Regex _validatorExtractor = new Regex(@"(?<min>\d+)-(?<max>\d+) (?<character>\w): (?<password>\w+)", RegexOptions.Compiled);

        public static IEnumerable<object[]> GetPasswords()
        {
            var allLines = File.ReadAllText(@"Data/02_passwords.txt");
            yield return new[] { allLines.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()) };
        }

        [Theory]
        [MemberData(nameof(GetPasswords))]
        public void PasswordValidator(string[] passwords)
        {
            Func<int, int, char, string, bool> validator = (min, max, letter, password) =>
            {
                if ((password.GroupBy(c => c).FirstOrDefault(g => g.Key == letter)?.Count() ?? 0) is int count)
                {
                    return count >= min && count <= max;
                }

                throw new Exception("Failed to validate");
            };

            var count = 0;
            foreach (var p in passwords)
            {
                if (IsValid(p, validator))
                {
                    count++;
                }
            }

            _output.WriteLine($"{count}");
            Assert.False(count == 0);
        }

        [Theory]
        [MemberData(nameof(GetPasswords))]
        public void PasswordValidator2(string[] passwords)
        {
            Func<int, int, char, string, bool> validator = (min, max, letter, password) =>
            {
                return password.Length > max - 1 && (password[min - 1] == letter) ^ (password[max - 1] == letter);
            };

            var count = 0;
            foreach (var p in passwords)
            {
                if (IsValid(p, validator))
                {
                    count++;
                }
            }

            _output.WriteLine($"{count}");
            Assert.False(count == 0);
        }


        private bool IsValid(string validatablePassword, Func<int, int, char, string, bool> isValid)
        {
            var extractorMatch = _validatorExtractor.Match(validatablePassword);
            if (extractorMatch.Groups["min"]?.Value is string minStr && int.TryParse(minStr, out var min)
                && extractorMatch.Groups["max"]?.Value is string maxStr && int.TryParse(maxStr, out var max)
                && extractorMatch.Groups["character"]?.Value is string letterStr and { Length: > 0 } && letterStr[0] is char letter
                && extractorMatch.Groups["password"]?.Value is string password and { Length: > 0 })
            {
                return isValid(min, max, letter, password);
            }

            throw new Exception("Parsing failed");
        }
    }
}
