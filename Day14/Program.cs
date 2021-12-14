using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Day14
{
    class Program
    {
        private static (string, List<(char, char, char)>) GetTemplateAndInsertions()
        {
            var lines = File.ReadAllLines("input.txt");
            var template = lines[0];
            var insertionRules = lines.Skip(2)
                .Select(line => line.Split(" -> "))
                .Select(split => (split[0][0], split[0][1], split[1][0]))
                .ToList();

            return (template, insertionRules);
        }

        private static long Solve(string template, List<(char, char, char)> insertions, int iterations)
        {
            var pairCounts = Enumerable.Range(0, template.Length - 1)
                .GroupBy(i => (left: template[i], right: template[i + 1]))
                .ToDictionary(g => g.Key, g => (long) g.Count());

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                var newPairCounts = new Dictionary<(char, char), long>(pairCounts);
                foreach (var (left, right, inserted) in insertions)
                {
                    if (pairCounts.TryGetValue((left, right), out var count) && count != 0)
                    {
                        newPairCounts[(left, right)] -= count;
                        newPairCounts[(left, inserted)] = count + newPairCounts.GetValueOrDefault((left, inserted), 0);
                        newPairCounts[(inserted, right)] = count + newPairCounts.GetValueOrDefault((inserted, right), 0);
                    }
                }

                pairCounts = newPairCounts;
            }

            var letterCounts = pairCounts.Select(pairCount => (letter: pairCount.Key.right, count: pairCount.Value))
                .Append((letter: template[0], count: 1)) // first letter never changes
                .GroupBy(t => t.letter)
                .Select(g => g.Select(t => t.count).Sum())
                .ToList();
            return letterCounts.Max() - letterCounts.Min();
        }

        public static void Main()
        {
            var (template, insertions) = GetTemplateAndInsertions();
            Console.WriteLine(Solve(template, insertions, 10));
            Console.WriteLine(Solve(template, insertions, 40));
        }
    }
}
