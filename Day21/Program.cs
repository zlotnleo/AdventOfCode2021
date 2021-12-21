namespace Day21;

class Program
{
    private static int Part1(int player1, int player2)
    {
        var currentPlayer = 0;
        var positions = new[] {player1, player2};
        var scores = new[] {0, 0};
        int rollCount;
        for (rollCount = 3;; rollCount += 3)
        {
            var roll = (rollCount - 3) % 100 + (rollCount - 2) % 100 + (rollCount - 1) % 100 + 3;
            positions[currentPlayer] = (positions[currentPlayer] + roll - 1) % 10 + 1;
            scores[currentPlayer] += positions[currentPlayer];
            if (scores[currentPlayer] >= 1000)
            {
                break;
            }

            currentPlayer = 1 - currentPlayer;
        }

        return scores[1 - currentPlayer] * rollCount;
    }

    private static long[,,] GetUniverseCounts(int initialPosition, int[] rollOutcomeCounts, int maxScore, int maxNumberOfMoves, int scoreToWin)
    {
        // count universe for each [score, moves, position]
        var countsPerPosition = new long[maxScore + 1, maxNumberOfMoves + 1, 11];
        countsPerPosition[0, 0, initialPosition] = 1;

        for (var score = 1; score <= maxScore; score++)
        {
            for (var numMoves = 1; numMoves <= maxNumberOfMoves; numMoves++)
            {
                for (var position = 1; position <= 10; position++)
                {
                    var prevScore = score - position;
                    // move only valid if previous score didn't finish the game
                    if (prevScore >= 0 && prevScore < scoreToWin)
                    {
                        for (var rollOutcome = 3; rollOutcome <= 9; rollOutcome++)
                        {
                            var prevPos = (position - rollOutcome + 10 - 1) % 10 + 1;
                            countsPerPosition[score, numMoves, position] +=
                                countsPerPosition[prevScore, numMoves - 1, prevPos] * rollOutcomeCounts[rollOutcome - 3];
                        }
                    }
                }
            }
        }

        return countsPerPosition;
    }

    private static long Part2(int player1, int player2)
    {
        var rollOutcomeCounts = new int[7];
        for (var roll1 = 1; roll1 <= 3; roll1++)
        {
            for (var roll2 = 1; roll2 <= 3; roll2++)
            {
                for (var roll3 = 1; roll3 <= 3; roll3++)
                {
                    rollOutcomeCounts[roll1 + roll2 + roll3 - 3]++;
                }
            }
        }

        const int scoreToWin = 21;
        const int maxScore = scoreToWin + 10 - 1;
        const int maxNumberOfMoves = maxScore / 3;
        var universeCounts1 = GetUniverseCounts(player1, rollOutcomeCounts, maxScore, maxNumberOfMoves, scoreToWin);
        var universeCounts2 = GetUniverseCounts(player2, rollOutcomeCounts, maxScore, maxNumberOfMoves, scoreToWin);

        var countPlayer1Wins = 0L;
        var countPlayer2Wins = 0L;
        for (var winningScore = scoreToWin; winningScore <= maxScore; winningScore++)
        {
            for (var move = 1; move <= maxNumberOfMoves; move++)
            {
                var countPlayer1WinsNow = 0L;
                var countPlayer2WinsNow = 0L;
                var countPlayer2LosesIn1FewerMoves = 0L;
                var countPlayer1LosesInSameMoves = 0L;

                for (var position = 1; position <= 10; position++)
                {
                    countPlayer1WinsNow += universeCounts1[winningScore, move, position];
                    countPlayer2WinsNow += universeCounts2[winningScore, move, position];

                    for (var losingScore = 0; losingScore < scoreToWin; losingScore++)
                    {
                        countPlayer2LosesIn1FewerMoves += universeCounts2[losingScore, move - 1, position];
                        countPlayer1LosesInSameMoves += universeCounts1[losingScore, move, position];
                    }
                }

                countPlayer1Wins += countPlayer1WinsNow * countPlayer2LosesIn1FewerMoves;
                countPlayer2Wins += countPlayer2WinsNow * countPlayer1LosesInSameMoves;
            }
        }

        return Math.Max(countPlayer1Wins, countPlayer2Wins);
    }

    public static void Main()
    {
        const int player1Position = 10;
        const int player2Position = 2;
        Console.WriteLine(Part1(player1Position, player2Position));
        Console.WriteLine(Part2(player1Position, player2Position));
    }
}
