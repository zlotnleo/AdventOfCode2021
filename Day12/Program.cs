using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        private static Dictionary<string, List<string>> GetCaves()
        {
            var caves = new Dictionary<string, List<string>>();
            foreach (var line in File.ReadAllLines("input.txt"))
            {
                var split = line.Split('-');
                if (!caves.TryGetValue(split[0], out var connections))
                {
                    connections = caves[split[0]] = new List<string>();
                }
                connections.Add(split[1]);

                if (!caves.TryGetValue(split[1], out connections))
                {
                    connections = caves[split[1]] = new List<string>();
                }
                connections.Add(split[0]);
            }

            return caves;
        }

        private static (int, int) Solve(Dictionary<string, List<string>> caves)
        {
            var smallCaves = caves.Keys
                .Where(cave => cave.All(c => c is >= 'a' and <= 'z'))
                .ToHashSet();

            var visitingSmallCavesOncePathCount = 0;
            var visitingOneSmallCaveTwicePathCount = 0;
            var stack = new Stack<(string, ImmutableHashSet<string>, string)>();
            stack.Push(("start", ImmutableHashSet<string>.Empty, null));

            while (stack.Count > 0)
            {
                var (cave, visited, doubleVisited) = stack.Pop();
                if (cave == "end")
                {
                    if (doubleVisited == null)
                    {
                        visitingSmallCavesOncePathCount++;
                    }

                    if(doubleVisited == null || visited.Contains(doubleVisited))
                    {
                        visitingOneSmallCaveTwicePathCount++;
                    }

                    continue;
                }

                var newVisited = visited;
                var isSmallCave = smallCaves.Contains(cave);
                if (isSmallCave)
                {
                    if (visited.Contains(cave))
                    {
                        continue;
                    }

                    newVisited = visited.Add(cave);
                }

                foreach (var otherCave in caves[cave])
                {
                    if (doubleVisited == null && isSmallCave && cave != "start")
                    {
                        stack.Push((otherCave, visited, cave));
                    }

                    stack.Push((otherCave, newVisited, doubleVisited));
                }
            }

            return (visitingSmallCavesOncePathCount, visitingOneSmallCaveTwicePathCount);
        }

        public static void Main()
        {
            var caves = GetCaves();
            var (part1, part2) = Solve(caves);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }
    }
}
