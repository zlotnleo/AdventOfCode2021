using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
{
    class Program
    {
        private static ICollection<int> GetFishTimers()
        {
            return File.ReadAllText("input.txt")
                .Split(',')
                .Select(int.Parse)
                .ToList();
        }

        private static long Solve(IEnumerable<int> initialTimers, int days)
        {
            var timerCounts = Enumerable.Repeat(0L, 9).ToArray();
            foreach (var timer in initialTimers)
            {
                timerCounts[timer]++;
            }

            for (var day = 0; day < days; day++)
            {
                var zerosAtStartOfDay = timerCounts[0];
                for (var i = 0; i < timerCounts.Length - 1; i++)
                {
                    timerCounts[i] = timerCounts[i + 1];
                }

                timerCounts[6] += zerosAtStartOfDay;
                timerCounts[8] = zerosAtStartOfDay;
            }

            return timerCounts.Sum();
        }

        public static void Main()
        {
            var timers = GetFishTimers();
            Console.WriteLine(Solve(timers, 80));
            Console.WriteLine(Solve(timers, 256));
        }
    }
}
