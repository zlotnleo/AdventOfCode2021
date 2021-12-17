namespace Day17;

class Program
{
    private static (int, int) Solve(int xMin, int xMax, int yMin, int yMax)
    {
        var maxHeightReached = 0;
        var count = 0;
        for (var initialXVelocity = 0; initialXVelocity <= xMax; initialXVelocity++)
        {
            // if thrown up, it will always pass y=0 with dy = -initialYVelocity on the way down
            // so it will overshoot if initialYVelocity > yMin
            for (var initialYVelocity = yMin; initialYVelocity <= -yMin; initialYVelocity++)
            {
                var (x, y) = (0, 0);
                var maxHeightThisThrow = 0;
                var (dx, dy) = (initialXVelocity, initialYVelocity);
                var hitTarget = false;
                for (var t = 0;; t++)
                {
                    if (xMin <= x && x <= xMax && yMin <= y && y <= yMax)
                    {
                        hitTarget = true;
                        break;
                    }

                    if (x > xMax || y < yMin)
                    {
                        break;
                    }

                    x += dx;
                    y += dy;
                    maxHeightThisThrow = Math.Max(maxHeightThisThrow, y);

                    dy--;
                    if (dx > 0)
                    {
                        dx--;
                    }
                }

                if (hitTarget)
                {
                    maxHeightReached = Math.Max(maxHeightReached, maxHeightThisThrow);
                    count++;
                }
            }
        }

        return (maxHeightReached, count);
    }

    public static void Main()
    {
        const int xMin = 94;
        const int xMax = 151;
        const int yMin = -156;
        const int yMax = -103;
        var (part1, part2) = Solve(xMin, xMax, yMin, yMax);
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}
