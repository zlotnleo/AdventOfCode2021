using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day13
{
    class Program
    {
        private static (HashSet<(int x, int y)>, ICollection<(char, int)>) GetDotsAndFolds()
        {
            using var lines = ((IEnumerable<string>)File.ReadAllLines("input.txt")).GetEnumerator();
            var dots = new HashSet<(int x, int y)>();
            while (lines.MoveNext() && lines.Current != "")
            {
                var split = lines.Current.Split(',');
                dots.Add((int.Parse(split[0]), int.Parse(split[1])));
            }

            var folds = new List<(char, int)>();
            while (lines.MoveNext())
            {
                var split = lines.Current.Split('=');
                folds.Add((split[0][^1], int.Parse(split[1])));
            }

            return (dots, folds);
        }

        private static HashSet<(int x, int y)> MakeFold(IEnumerable<(int x, int y)> dots, (char axis, int coord) fold) =>
            dots.Select(dot => fold.axis switch
            {
                'y' when dot.y > fold.coord => (dot.x, fold.coord * 2 - dot.y),
                'x' when dot.x > fold.coord => (fold.coord * 2 - dot.x, dot.y),
                _ => dot
            }).ToHashSet();

        private static string GetPaper(ICollection<(int x, int y)> dots)
        {
            var height = dots.Select(dot => dot.y).Max();
            var width = dots.Select(dot => dot.x).Max();
            var builder = new StringBuilder();
            for (var y = 0; y <= height; y++)
            {
                for (var x = 0; x <= width; x++)
                {
                    builder.Append(dots.Contains((x, y)) ? '#' : ' ');
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static int Part1(HashSet<(int x, int y)> dots, (char, int) fold) =>
            MakeFold(dots, fold).Count;

        private static string Part2(HashSet<(int x, int y)> dots, IEnumerable<(char, int)> folds) =>
            GetPaper(folds.Aggregate(dots, MakeFold));

        public static void Main()
        {
            var (state, folds) = GetDotsAndFolds();
            Console.WriteLine(Part1(state, folds.First()));
            Console.WriteLine(Part2(state, folds));
        }
    }
}
