using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day5
{
    class Program
    {
        private static List<(int, int, int, int)> GetLines()
        {
            var lineRegex = new Regex("(\\d+),(\\d+) -> (\\d+),(\\d+)", RegexOptions.Compiled);
            return File.ReadAllLines("input.txt")
                .Select(line => lineRegex.Match(line))
                .Where(match => match.Success)
                .Select(match => (
                    int.Parse(match.Groups[1].Value),
                    int.Parse(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value),
                    int.Parse(match.Groups[4].Value)
                )).ToList();
        }

        private static int Solve(IEnumerable<(int, int, int, int)> lines, bool includeDiagonals)
        {
            var linesCount = new Dictionary<(int, int), int>();

            foreach (var (x1, y1, x2, y2) in lines)
            {
                if (includeDiagonals || x1 == x2 || y1 == y2)
                {
                    var dx = Math.Sign(x2 - x1);
                    var dy = Math.Sign(y2 - y1);
                    int x, y;
                    for (x = x1, y = y1; x != x2 + dx || y != y2 + dy; x += dx, y += dy)
                    {
                        linesCount[(x, y)] = linesCount.GetValueOrDefault((x, y), 0) + 1;
                    }
                }
            }

            return linesCount.Count(entry => entry.Value >= 2);
        }

        public static void Main()
        {
            var lines = GetLines();
            Console.WriteLine(Solve(lines, false));
            Console.WriteLine(Solve(lines, true));
        }
    }
}
