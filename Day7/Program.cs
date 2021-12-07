using System;
using System.IO;
using System.Linq;

namespace Day7
{
    class Program
    {
        private static int Solve(int[] crabs, Func<int, int> fuelForDistance)
        {
            var min = crabs.Min();
            var max = crabs.Max();
            var minTotal = int.MaxValue;
            for (var pos = min; pos <= max; pos++)
            {
                var total = crabs.Select(c => fuelForDistance(Math.Abs(c - pos))).Sum();
                minTotal = Math.Min(minTotal, total);
            }

            return minTotal;
        }

        public static void Main()
        {
            var crabs = File.ReadAllText("input.txt").Split(',').Select(int.Parse).ToArray();
            Console.WriteLine(Solve(crabs, d => d));
            Console.WriteLine(Solve(crabs, d => d * (d + 1) / 2));
        }
    }
}
