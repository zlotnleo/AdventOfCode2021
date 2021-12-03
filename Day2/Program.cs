using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        private static ICollection<(string, int)> GetInput() =>
            File.ReadAllLines("input.txt")
                .Select(l => l.Split(' '))
                .Select(l => (l[0], int.Parse(l[1])))
                .ToArray();

        private static int Part1(IEnumerable<(string, int)> instructions)
        {
            var horizPos = 0;
            var depth = 0;
            foreach (var (direction, distance) in instructions)
            {
                switch (direction)
                {
                    case "forward":
                        horizPos += distance;
                        break;
                    case "down":
                        depth += distance;
                        break;
                    case "up":
                        depth -= distance;
                        break;
                }
            }

            return horizPos * depth;
        }

        private static int Part2(IEnumerable<(string, int)> instructions)
        {
            var horizPos = 0;
            var depth = 0;
            var aim = 0;

            foreach (var (direction, distance) in instructions)
            {
                switch (direction)
                {
                    case "forward":
                        horizPos += distance;
                        depth += distance * aim;
                        break;
                    case "down":
                        aim += distance;
                        break;
                    case "up":
                        aim -= distance;
                        break;
                }
            }

            return horizPos * depth;
        }

        public static void Main()
        {
            var instructions = GetInput();
            Console.WriteLine(Part1(instructions));
            Console.WriteLine(Part2(instructions));
        }
    }
}
