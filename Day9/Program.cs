using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        private static int[][] GetHeightmap() =>
            File.ReadAllLines("input.txt")
                .Select(line => line.Select(c => c - '0').ToArray())
                .ToArray();

        private static readonly IReadOnlyList<(int dx, int dy)> Neighbours =
            new[] {(-1, 0), (1, 0), (0, -1), (0, 1)};

        private static bool IsValid(int[][] heightmap, int x, int y) =>
            y >= 0 && y < heightmap.Length && x >= 0 && x < heightmap[0].Length;

        private static ICollection<(int, int)> GetLowestPoints(int[][] heightmap)
        {
            var lowestPoints = new List<(int, int)>();
            for (var y = 0; y < heightmap.Length; y++)
            {
                for (var x = 0; x < heightmap[y].Length; x++)
                {
                    var isLowest = Neighbours.All(d =>
                        !IsValid(heightmap, x + d.dx, y + d.dy) || heightmap[y][x] < heightmap[y + d.dy][x + d.dx]
                    );
                    if (isLowest)
                    {
                        lowestPoints.Add((x, y));
                    }
                }
            }

            return lowestPoints;
        }

        private static int Part1(int[][] heightmap, IEnumerable<(int x, int y)> lowestPoints) =>
            lowestPoints.Select(p => heightmap[p.y][p.x] + 1).Sum();

        private static int Part2(int[][] heightmap, IEnumerable<(int, int)> lowestPoints)
        {
            var basinSizes = new List<int>();
            foreach (var lowestPoint in lowestPoints)
            {
                var basin = new HashSet<(int, int)>();
                var queue = new Queue<(int, int)>();
                queue.Enqueue(lowestPoint);
                while (queue.Count > 0)
                {
                    var (x, y) = queue.Dequeue();
                    if (basin.Contains((x, y)))
                    {
                        continue;
                    }

                    basin.Add((x, y));
                    foreach (var (dx, dy) in Neighbours)
                    {
                        if (IsValid(heightmap, x + dx, y + dy)
                            && heightmap[y + dy][x + dx] > heightmap[y][x]
                            && heightmap[y + dy][x + dx] != 9)
                        {
                            queue.Enqueue((x + dx, y + dy));
                        }
                    }
                }

                basinSizes.Add(basin.Count);
            }

            return basinSizes.OrderByDescending(s => s)
                .Take(3)
                .Aggregate((acc, x) => acc * x);
        }

        public static void Main()
        {
            var heightmap = GetHeightmap();
            var lowestPoints = GetLowestPoints(heightmap);
            Console.WriteLine(Part1(heightmap, lowestPoints));
            Console.WriteLine(Part2(heightmap, lowestPoints));
        }
    }
}
