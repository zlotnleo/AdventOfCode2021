namespace Day19;

class Program
{
    private readonly record struct Coord(int X, int Y, int Z)
    {
        public readonly int X = X;
        public readonly int Y = Y;
        public readonly int Z = Z;

        public static Coord operator -(Coord c) => new(-c.X, -c.Y, -c.Z);

        public static Coord operator +(Coord c1, Coord c2) =>
            new(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);

        public static Coord operator -(Coord c1, Coord c2) => c1 + -c2;

        public int DotProduct(Coord c) =>
            X * c.X + Y * c.Y + Z * c.Z;

        public Coord CrossProduct(Coord c) => new(
            Y * c.Z - Z * c.Y,
            Z * c.X - X * c.Z,
            X * c.Y - Y * c.X
        );
    }

    private record OrientationMatrix(Coord A, Coord B, Coord C)
    {
        public static readonly OrientationMatrix I = new(
            new Coord(1, 0, 0),
            new Coord(0, 1, 0),
            new Coord(0, 0, 1)
        );

        public static Coord operator *(OrientationMatrix m, Coord c) => new(
            m.A.X * c.X + m.A.Y * c.Y + m.A.Z * c.Z,
            m.B.X * c.X + m.B.Y * c.Y + m.B.Z * c.Z,
            m.C.X * c.X + m.C.Y * c.Y + m.C.Z * c.Z
        );

        public static OrientationMatrix operator *(OrientationMatrix m1, OrientationMatrix m2)
        {
            var (m2Col1, m2Col2, m2Col3) = m2.Transpose();
            return new OrientationMatrix(
                m1 * m2Col1,
                m1 * m2Col2,
                m1 * m2Col3
            ).Transpose();
        }

        public OrientationMatrix Transpose() => new(
            new Coord(A.X, B.X, C.X),
            new Coord(A.Y, B.Y, C.Y),
            new Coord(A.Z, B.Z, C.Z)
        );
    }

    private static readonly IReadOnlyList<Coord> UnitVectors = new Coord[] {new(1, 0, 0), new(0, 1, 0), new(0, 0, 1)};

    private static readonly IReadOnlyList<OrientationMatrix> Orientations =
        UnitVectors.SelectMany(v1 =>
                UnitVectors.SelectMany(v2 =>
                    v1.DotProduct(v2) == 0
                        ? new[] {(v1, v2), (v1, -v2), (-v1, v2), (-v1, -v2)}
                        : Enumerable.Empty<(Coord a, Coord b)>()
                )
            ).Select(t => new OrientationMatrix(t.a, t.b, t.a.CrossProduct(t.b)))
            .ToList();

    private static List<Dictionary<OrientationMatrix, HashSet<Coord>>> GetScanners()
    {
        var scanners = new List<List<Coord>>();
        var lines = File.ReadAllLines("input.txt");
        foreach (var line in lines)
        {
            if (line == "")
            {
                continue;
            }

            if (line.StartsWith("---"))
            {
                scanners.Add(new List<Coord>());
                continue;
            }

            var split = line.Split(',');
            scanners[^1].Add(new Coord(
                int.Parse(split[0]),
                int.Parse(split[1]),
                int.Parse(split[2])
            ));
        }

        return scanners.Select(scanner =>
            Orientations.ToDictionary(
                orientation => orientation,
                orientation =>
                    scanner.Select(coord => orientation * coord).ToHashSet()
            )
        ).ToList();
    }

    private static (OrientationMatrix, Coord)? GetOverlaps(
        HashSet<Coord> scanner1Coords,
        Dictionary<OrientationMatrix, HashSet<Coord>> scanner2Orientations)
    {
        foreach (var (scanner2Orientation, scanner2Coords) in scanner2Orientations)
        {
            foreach (var scanner1Coord in scanner1Coords)
            {
                foreach (var scanner2Coord in scanner2Coords)
                {
                    var relativePosition = scanner1Coord - scanner2Coord;
                    if (scanner2Coords.Count(c2 => scanner1Coords.Contains(relativePosition + c2)) >= 12)
                    {
                        return (scanner2Orientation, relativePosition);
                    }
                }
            }
        }

        return null;
    }

    private static (int, int) Solve(List<Dictionary<OrientationMatrix, HashSet<Coord>>> scanners)
    {
        var relativeScanners = Enumerable.Range(0, scanners.Count)
            .ToDictionary(i => i, _ => new Dictionary<int, (Coord pos, OrientationMatrix orientation)>());
        for (var i = 0; i < scanners.Count - 1; i++)
        {
            for (var j = i + 1; j < scanners.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var scanner1 = scanners[i];
                var scanner2 = scanners[j];
                if (GetOverlaps(scanner1[OrientationMatrix.I], scanner2) is var (relativeOrientation, relativeOffset))
                {
                    relativeScanners[i][j] = (relativeOffset, relativeOrientation);

                    var inverseRelativeOrientation = relativeOrientation.Transpose();
                    relativeScanners[j][i] = (inverseRelativeOrientation * -relativeOffset, inverseRelativeOrientation);
                }
            }
        }

        var absoluteScanners = new Dictionary<int, (Coord pos, OrientationMatrix orientation)>
        {
            {0, (new Coord(0, 0, 0), OrientationMatrix.I)}
        };
        var queue = new Queue<int>();
        queue.Enqueue(0);
        while (queue.Count > 0)
        {
            var scannerIndex = queue.Dequeue();
            var (absolutePosition, absoluteOrientation) = absoluteScanners[scannerIndex];
            foreach (var (otherScanner, (relativePosition, relativeOrientation)) in relativeScanners[scannerIndex])
            {
                if (absoluteScanners.ContainsKey(otherScanner))
                {
                    continue;
                }

                absoluteScanners[otherScanner] = (
                    absolutePosition + absoluteOrientation * relativePosition,
                    absoluteOrientation * relativeOrientation
                );
                queue.Enqueue(otherScanner);
            }
        }

        var absoluteBeacons = new HashSet<Coord>();
        for (var i = 0; i < scanners.Count; i++)
        {
            var (pos, orientation) = absoluteScanners[i];
            absoluteBeacons.UnionWith(
                scanners[i][orientation].Select(c => pos + c)
            );
        }

        var maxDistanceBetweenScanners = absoluteScanners.SelectMany(scanner1 =>
                absoluteScanners.Select(scanner2 => scanner1.Value.pos - scanner2.Value.pos)
            ).Select(offset => Math.Abs(offset.X) + Math.Abs(offset.Y) + Math.Abs(offset.Z))
            .Max();

        return (absoluteBeacons.Count, maxDistanceBetweenScanners);
    }

    public static void Main()
    {
        var scanners = GetScanners();
        var (part1, part2) = Solve(scanners);
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}
