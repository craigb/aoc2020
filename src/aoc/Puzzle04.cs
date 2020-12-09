using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode
{
    public class Puzzle04
    {
        private readonly ITestOutputHelper _output;
        public Puzzle04(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        public enum DataType
        {
            Valid,
            Invalid,
            Unknown
        }

        public static IEnumerable<object[]> GetPassports(DataType type = DataType.Unknown)
        {
            switch (type)
            {
                case DataType.Invalid:
                    {
                        var passports = MakePassports(@"eyr:1972 cid:100
                        hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

                        iyr:2019
                        hcl:#602927 eyr:1967 hgt:170cm
                        ecl:grn pid:012533040 byr:1946

                        hcl:dab227 iyr:2012
                        ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

                        hgt:59cm ecl:zzz
                        eyr:2038 hcl:74454a iyr:2023
                        pid:3556412378 byr:2007".Split("\n"));
                        yield return new object[] { passports, 0 };
                        break;
                    }
                case DataType.Valid:
                    {
                        var passports = MakePassports(@"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
                        hcl:#623a2f

                        eyr:2029 ecl:blu cid:129 byr:1989
                        iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

                        hcl:#888785
                        hgt:164cm byr:2001 iyr:2015 cid:88
                        pid:545766238 ecl:hzl
                        eyr:2022

                        iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719".Split("\n"));
                        yield return new object[] { passports, 4 };
                        break;
                    }
                default:
                    {
                        yield return new object[] { MakePassports(File.ReadAllLines(@"Data/04_passports.txt")), null };
                        break;
                    }
            }
        }

        public static string[] MakePassports(IEnumerable<string> lines)
        {
            StringBuilder passport = new();
            List<string> passports = new();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    passports.Add(FormatPassport(passport.ToString()));
                    passport.Clear();
                }

                passport.Append(line.Trim() + " ");
            }

            if (passport.Length > 0)
            {
                passports.Add(FormatPassport(passport.ToString()));
            }

            return passports.ToArray();
        }

        public static string FormatPassport(string unformatted)
        {
            var stringBuilder = new StringBuilder();
            var tokens = unformatted.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var tok in tokens.Where(t => !t.StartsWith("cid:") && !t.StartsWith("hgt:")).OrderBy(t => t))
            {
                stringBuilder.Append(tok + " ");
            }

            foreach (var tok in tokens.Where(t => t.StartsWith("cid:") || t.StartsWith("hgt:")).OrderByDescending(t => t))
            {
                stringBuilder.Append(tok + " ");
            }

            return stringBuilder.ToString();
        }

        private static HashSet<string> expectedCodes = new()
        {
            "byr:",
            "iyr:",
            "eyr:",
            "hgt:",
            "hcl:",
            "ecl:",
            "pid:",
            //"cid:",
        };

        public class Rule
        {
            private readonly Regex _extractor;
            private readonly Func<Match, bool> _moreValidation;

            public Rule(string extractionRegex, Func<Match, bool> moreValidation = default)
            {
                _extractor = new Regex(extractionRegex, RegexOptions.Compiled);
                _moreValidation = moreValidation ?? (m => m.Success);
            }

            public bool Evaluate(string passport)
            {
                var match = _extractor.Match(passport);
                var valid = _moreValidation(match);
                return valid;
            }
        }

        public static Rule[] validationRules = new[]
        {
            new Rule(
                @"byr:(?<year>\d{4})",
                m => m.Success && m.Groups["year"].Value is string yearStr
                && int.TryParse(yearStr, out var year)
                && year >= 1920
                && year <= 2002),

            new Rule(
                @"iyr:(?<year>\d{4})",
                m => m.Success && m.Groups["year"].Value is string yearStr
                && int.TryParse(yearStr, out var year)
                && year >= 2010
                && year <= 2020),

            new Rule(
                @"eyr:(?<year>\d{4})",
                m => m.Success && m.Groups["year"].Value is string yearStr
                && int.TryParse(yearStr, out var year)
                && year >= 2020
                && year <= 2030),

            new Rule(
                @"hgt:(?<value>\d+)(?<unit>in|cm)",
                m => m.Success && m.Groups["value"].Value is string valStr
                && int.TryParse(valStr, out var height)
                && m.Groups["unit"].Value is string unit
                && (
                    (unit == "in" && height >= 59 && height <= 76)
                    ||
                    (unit == "cm" && height >= 150 && height <= 193)
                )),

            new Rule(@"hcl:#[0-9a-f]{6}"),

            new Rule(@"ecl:(amb|blu|brn|gry|grn|hzl|oth)"),

            new Rule(@"pid:\d{9}\b"),

        };

        [Theory]
        [MemberData(nameof(GetPassports), parameters: DataType.Unknown)]
        public void TestPassports(string[] passports, int? expected)
        {
            int valid = 0;
            foreach (var p in passports)
            {
                if (expectedCodes.All(code => p.Contains(code)))
                {
                    valid++;
                }
            }

            _output.WriteLine($"{valid}");
            if (expected is int ex)
            {
                Assert.Equal(ex, valid);
            }
        }

        [Theory]
        [MemberData(nameof(GetPassports), parameters: DataType.Invalid)]
        [MemberData(nameof(GetPassports), parameters: DataType.Valid)]
        [MemberData(nameof(GetPassports), parameters: DataType.Unknown)]
        public void TestPassports2(string[] passports, int? expected)
        {
            IEnumerable<string> potentiallyValidPassports = passports;
            foreach (var rule in validationRules)
            {
                potentiallyValidPassports = potentiallyValidPassports.Where(passport => rule.Evaluate(passport));
            }

            var valid = potentiallyValidPassports.ToList();

            _output.WriteLine($"{valid.Count}");
            if (expected is int ex)
            {
                Assert.Equal(ex, valid.Count);
            }
        }
    }
}
