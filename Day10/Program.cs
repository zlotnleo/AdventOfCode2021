using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    class Program
    {
        private static (long, long) Solve(IEnumerable<string> lines)
        {
            var syntaxErrorScore = 0;
            var autoCompleteScores = new List<long>();
            foreach (var line in lines)
            {
                var stack = new Stack<char>();
                var incomplete = true;
                for (var i = 0; i < line.Length && incomplete; i++)
                {
                    switch (line[i])
                    {
                        case '(' or '{' or '[' or '<':
                            stack.Push(line[i]);
                            break;
                        case ')' or '}' or ']' or '>':
                            incomplete = (stack.Pop(), line[i]) is ('(', ')') or ('[', ']') or ('{', '}') or ('<', '>');
                            if (!incomplete)
                            {
                                syntaxErrorScore += line[i] switch
                                {
                                    ')' => 3,
                                    ']' => 57,
                                    '}' => 1197,
                                    '>' => 25137
                                };
                            }

                            break;
                    }
                }

                if (incomplete)
                {
                    autoCompleteScores.Add(
                        stack.Select(b => b switch
                            {
                                '(' => 1,
                                '[' => 2,
                                '{' => 3,
                                '<' => 4
                            }
                        ).Aggregate(0L, (acc, s) => acc * 5 + s)
                    );
                }
            }

            autoCompleteScores.Sort();
            return (syntaxErrorScore, autoCompleteScores[autoCompleteScores.Count / 2]);
        }

        public static void Main()
        {
            var lines = File.ReadAllLines("input.txt");
            var (part1, part2) = Solve(lines);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }
    }
}
