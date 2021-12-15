using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    class Program
    {
        private static int[][] GetRiskLevels()
        {
            return File.ReadAllLines("input.txt")
                .Select(line => line.Select(c => c - '0').ToArray())
                .ToArray();
        }

        private static int[][] ConvertGrid(int[][] tile)
        {
            var height = tile.Length;
            var width = tile[0].Length;
            return Enumerable.Range(0, 5 * height).Select(i =>
                Enumerable.Range(0, 5 * width).Select(j =>
                    (tile[i % height][j % width] + i / height + j / width - 1) % 9 + 1
                ).ToArray()
            ).ToArray();
        }

        private static int? Solve(int[][] riskLevels)
        {
            var dCoord = new[] {(-1, 0), (1, 0), (0, -1), (0, 1)};

            var currentTotalRisks = Enumerable.Range(0, riskLevels.Length).Select(_ =>
                Enumerable.Range(0, riskLevels[0].Length).Select(_ => null as int?).ToArray()
            ).ToArray();
            var priorityQueue = new PriorityQueue<(int, int), int>(
                riskLevels.Length * riskLevels[0].Length
            );
            priorityQueue.Enqueue((0, 0), 0);

            while (priorityQueue.TryDequeue(out var coord, out var totalRisk))
            {
                if (coord == (riskLevels[^1].Length - 1, riskLevels.Length - 1))
                {
                    return totalRisk;
                }

                var (x, y) = coord;
                foreach (var (dx, dy) in dCoord)
                {
                    var newX = x + dx;
                    var newY = y + dy;
                    if (newX >= 0 && newX < riskLevels[0].Length && newY >= 0 && newY < riskLevels.Length)
                    {
                        var newRisk = totalRisk + riskLevels[newY][newX];
                        if (currentTotalRisks[newY][newX] == null || newRisk < currentTotalRisks[newY][newX])
                        {
                            currentTotalRisks[newY][newX] = newRisk;
                            priorityQueue.Enqueue((newX, newY), newRisk);
                        }
                    }
                }
            }

            return null;
        }

        public static void Main()
        {
            var riskLevels = GetRiskLevels();
            Console.WriteLine(Solve(riskLevels));
            var converted = ConvertGrid(riskLevels);
            Console.WriteLine(Solve(converted));
        }
    }
}
