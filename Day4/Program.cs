using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day4
{
    class Program
    {
        private record Cell(int Number, bool Marked)
        {
            public bool Marked { get; set; } = Marked;
        }


        private static (int[] numbers, List<List<Cell[]>> boards) ParseInput()
        {
            var lines = File.ReadAllLines("input.txt");
            var numbers = lines[0].Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
            var boards = new List<List<Cell[]>>();
            foreach (var line in lines[1..])
            {
                if (line.Length == 0)
                {
                    boards.Add(new List<Cell[]>());
                    continue;
                }

                boards[^1].Add(
                    line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => new Cell(int.Parse(s), false))
                        .ToArray()
                );
            }

            return (numbers, boards);
        }

        private static int? MarkNumberOnBoard(List<Cell[]> board, int number)
        {
            foreach (var row in board)
            {
                foreach (var cell in row)
                {
                    if (cell.Number == number)
                    {
                        cell.Marked = true;
                    }
                }
            }

            var win = board.Any(row => row.All(cell => cell.Marked))
                      || Enumerable.Range(0, board[0].Length).Select(i => board.Select(row => row[i]))
                          .Any(column => column.All(cell => cell.Marked));
            return win
                ? number * board.SelectMany(row => row)
                    .Select(cell => cell.Marked ? 0 : cell.Number)
                    .Sum()
                : null;
        }

        private static (int, int) Solve(IEnumerable<int> numbers, IList<List<Cell[]>> boards)
        {
            int? firstWinScore = null;
            var lastWinScore = -1;

            foreach (var number in numbers)
            {
                for (var i = boards.Count - 1; i >= 0; i--)
                {
                    var winScore = MarkNumberOnBoard(boards[i], number);
                    if (winScore.HasValue)
                    {
                        firstWinScore ??= winScore.Value;
                        lastWinScore = winScore.Value;
                        boards.RemoveAt(i);
                    }
                }
            }

            return (firstWinScore ?? -1, lastWinScore);
        }

        public static void Main()
        {
            var (numbers, boards) = ParseInput();
            var (part1, part2) = Solve(numbers, boards);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }
    }
}
