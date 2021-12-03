using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    class Program
    {
        private static (int zeros, int ones) CountBitsAtPosition(IEnumerable<string> report, int i)
        {
            var counts = new int[2];
            foreach (var line in report)
            {
                counts[line[i] - '0']++;
            }

            return (counts[0], counts[1]);
        }

        private static int Part1(string[] report)
        {
            var bitCounts = Enumerable.Range(0, report[0].Length)
                .Select(i => CountBitsAtPosition(report, i))
                .ToArray();

            var gamma = 0;
            foreach (var (zeros, ones) in bitCounts)
            {
                gamma <<= 1;

                if (ones > zeros)
                {
                    gamma |= 1;
                }
            }

            // epsilon is last bitCounts.Length bits of gamma inverted
            return gamma * (~gamma & ((1 << bitCounts.Length) - 1));
        }

        private static int Part2(string[] report)
        {
            var o2RatingCandidates = new List<string>(report);
            for (var i = 0; i < report[0].Length && o2RatingCandidates.Count > 1; i++)
            {
                var (zeros, ones) = CountBitsAtPosition(o2RatingCandidates, i);
                var mostCommon = ones >= zeros ? '1' : '0';
                o2RatingCandidates = o2RatingCandidates
                    .Where(candidate => candidate[i] == mostCommon)
                    .ToList();
            }

            var co2RatingCandidates = new List<string>(report);
            for (var i = 0; i < report[0].Length && co2RatingCandidates.Count > 1; i++)
            {
                var (zeros, ones) = CountBitsAtPosition(co2RatingCandidates, i);
                var leastCommon = zeros <= ones ? '0' : '1';
                co2RatingCandidates = co2RatingCandidates
                    .Where(candidate => candidate[i] == leastCommon)
                    .ToList();
            }

            var o2Rating = Convert.ToInt32(o2RatingCandidates.Single(), 2);
            var co2Rating = Convert.ToInt32(co2RatingCandidates.Single(), 2);
            return o2Rating * co2Rating;
        }

        public static void Main()
        {
            var report = File.ReadAllLines("input.txt");
            Console.WriteLine(Part1(report));
            Console.WriteLine(Part2(report));
        }
    }
}
