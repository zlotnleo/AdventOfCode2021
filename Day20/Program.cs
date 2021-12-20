namespace Day20;

class Program
{
    private record Image(HashSet<(int, int)> Pixels, int MinX, int MaxX, int MinY, int MaxY, bool Outside);

    private static (Image, bool[]) GetInput()
    {
        var lines = File.ReadAllLines("input.txt");
        var enhancement = lines.First().Select(c => c == '#').ToArray();

        var (maxX, maxY) = (0, 0);
        var pixels = lines.Skip(2).SelectMany((l, y) =>
        {
            maxY = Math.Max(maxY, y);
            return l.SelectMany((c, x) =>
            {
                maxX = Math.Max(maxX, x);
                return c == '#' ? Enumerable.Repeat((x, y), 1) : Enumerable.Empty<(int, int)>();
            });
        }).ToHashSet();
        return (new Image(pixels, 0, maxX, 0, maxY, false), enhancement);
    }

    private static readonly (int dx, int dy)[] Surrounding =
        Enumerable.Range(-1, 3).SelectMany(dy =>
            Enumerable.Range(-1, 3).Select(dx => (dx, dy))
        ).ToArray();

    private static int GetBinary(IEnumerable<bool> bits)
    {
        var number = 0;
        foreach (var bit in bits)
        {
            number <<= 1;
            number |= bit ? 1 : 0;
        }

        return number;
    }

    private static Image Enhance(Image image, bool[] enhancement)
    {
        var (pixels, minX, maxX, minY, maxY, outside) = image;
        var newPixels = new HashSet<(int, int)>();
        for (var x = minX - 1; x <= maxX + 1; x++)
        {
            for (var y = minY - 1; y <= maxY + 1; y++)
            {
                var enhancementIndex = GetBinary(
                    Surrounding.Select(n =>
                    {
                        var newX = x + n.dx;
                        var newY = y + n.dy;
                        return minX <= newX && newX <= maxX && minY <= newY && newY <= maxX
                            ? pixels.Contains((newX, newY))
                            : outside;
                    }).ToArray()
                );
                if (enhancement[enhancementIndex])
                {
                    newPixels.Add((x, y));
                }
            }
        }

        var newOutside = outside ? enhancement[^1] : enhancement[0];
        return new Image(newPixels, minX - 1, maxX + 1, minY - 1, maxY + 1, newOutside);
    }

    private static int Solve(Image image, bool[] enhancement, int times)
    {
        for (var i = 0; i < times; i++)
        {
            image = Enhance(image, enhancement);
        }

        return image.Pixels.Count;
    }

    public static void Main()
    {
        var (image, enhancement) = GetInput();
        Console.WriteLine(Solve(image, enhancement, 2));
        Console.WriteLine(Solve(image, enhancement, 50));
    }
}

