using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        private static int Solve(IReadOnlyList<int> depths, int offset) =>
            Enumerable.Range(0, depths.Count - offset)
                .Count(i => depths[i + offset] > depths[i]);

        public static void Main()
        {
            var depths = File.ReadAllLines("input.txt").Select(int.Parse).ToArray();
            Console.WriteLine(Solve(depths, 1));
            Console.WriteLine(Solve(depths, 3));
        }
    }
}
