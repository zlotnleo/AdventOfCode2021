using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    class Program
    {
        private static int[][] ReadInput()
        {
            return File.ReadAllLines("input.txt")
                .Select(line => line.Select(c => c - '0').ToArray())
                .ToArray();
        }

        private static (int, int) Solve(int[][] octopuses)
        {
            const int stopCountingAtStep = 100;
            var countFlashes = 0;
            var stepWhenAllFlashed = -1;

            var allCoordinates = Enumerable.Range(0, octopuses.Length)
                .SelectMany(y => Enumerable.Range(0, octopuses[y].Length).Select(x => (x, y)))
                .ToArray();
            var dCoords = Enumerable.Range(-1, 3)
                .SelectMany(dx => Enumerable.Range(-1, 3).Select(dy => (dx, dy)))
                .Where(t => t.dx != 0 || t.dy != 0)
                .ToArray();

            for (var step = 1; step <= stopCountingAtStep || stepWhenAllFlashed == -1; step++)
            {
                var toFlash = new Queue<(int, int)>();
                foreach (var (x, y) in allCoordinates)
                {
                    octopuses[y][x]++;
                    if (octopuses[y][x] > 9)
                    {
                        toFlash.Enqueue((x, y));
                    }
                }

                var flashed = new HashSet<(int, int)>(toFlash);
                while (toFlash.Count > 0)
                {
                    var (x, y) = toFlash.Dequeue();
                    flashed.Add((x, y));
                    foreach (var (dx, dy) in dCoords)
                    {
                        var newX = x + dx;
                        var newY = y + dy;
                        if (newY >= 0 && newY < octopuses.Length
                            && newX >= 0 && newX < octopuses[y].Length
                            && !flashed.Contains((newX, newY)))
                        {
                            octopuses[newY][newX]++;
                            if (octopuses[newY][newX] > 9)
                            {
                                flashed.Add((newX, newY));
                                toFlash.Enqueue((newX, newY));
                            }
                        }
                    }
                }

                foreach (var (x, y) in flashed)
                {
                    octopuses[y][x] = 0;
                }

                if (step <= stopCountingAtStep)
                {
                    countFlashes += flashed.Count;
                }

                if (flashed.Count == octopuses.Length * octopuses[0].Length)
                {
                    stepWhenAllFlashed = step;
                }
            }

            return (countFlashes, stepWhenAllFlashed);
        }

        public static void Main()
        {
            var input = ReadInput();
            var (part1, part2) = Solve(input);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }
    }
}
