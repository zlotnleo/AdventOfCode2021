using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        private static ICollection<(string[], string[])> ReadInput() =>
            File.ReadAllLines("input.txt")
                .Select(line => line.Trim().Split('|'))
                .Select(parts => (
                    parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => string.Concat(s.OrderBy(c => c)))
                        .ToArray(),
                    parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => string.Concat(s.OrderBy(c => c)))
                        .ToArray()
                ))
                .ToArray();

        private static int Part1(IEnumerable<(string[], string[] outputs)> input) =>
            input
                .SelectMany(t => t.outputs)
                .Count(output => output.Length is 2 or 4 or 3 or 7);

        private static int Part2(IEnumerable<(string[], string[])> input)
        {
            const string allSegments = "abcdefg";
            var total = 0;
            foreach (var (patterns, outputPatterns) in input)
            {
                var patternsByLength = patterns.GroupBy(p => p.Length).ToDictionary(g => g.Key, g => g.ToArray());

                var segmentsIn1 = patternsByLength[2].Single();
                var segmentsIn4 = patternsByLength[4].Single();
                var segmentsIn7 = patternsByLength[3].Single();
                var segmentsIn8 = patternsByLength[7].Single();

                var segmentsIn3 = patternsByLength[5].Single(p => segmentsIn1.All(p.Contains));
                var segmentsIn6 = patternsByLength[6].Single(p => !segmentsIn1.All(p.Contains));

                var topRightSegment = allSegments.Except(segmentsIn6).Single();
                var segmentsIn2 = patternsByLength[5].Single(p => p != segmentsIn3 && p.Contains(topRightSegment));
                var segmentsIn5 = patternsByLength[5].Single(p => p != segmentsIn3 && p != segmentsIn2);

                var topLeftSegment = allSegments.Except(segmentsIn2).Single(c => !segmentsIn1.Contains(c));
                var middleSegment = segmentsIn4.Except(segmentsIn1.Append(topLeftSegment)).Single();
                var segmentsIn0 = patternsByLength[6].Single(p => !p.Contains(middleSegment));
                var segmentsIn9 = patternsByLength[6].Single(p => p != segmentsIn0 && p != segmentsIn6);

                var digitsBySegments = new Dictionary<string, int>
                {
                    [segmentsIn0] = 0,
                    [segmentsIn1] = 1,
                    [segmentsIn2] = 2,
                    [segmentsIn3] = 3,
                    [segmentsIn4] = 4,
                    [segmentsIn5] = 5,
                    [segmentsIn6] = 6,
                    [segmentsIn7] = 7,
                    [segmentsIn8] = 8,
                    [segmentsIn9] = 9
                };

                var output = 0;
                foreach (var outputSegments in outputPatterns)
                {
                    output *= 10;
                    output += digitsBySegments[outputSegments];
                }

                total += output;
            }

            return total;
        }

        public static void Main()
        {
            var input = ReadInput();
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }
    }
}
