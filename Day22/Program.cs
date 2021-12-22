namespace Day22;

class Program
{
    private static readonly string[] Separators = {" ", ",", "=", ".."};

    private static List<(bool, (int, int, int, int, int, int))> GetRebootSteps() =>
        File.ReadAllLines("input.txt")
            .Select(line => line.Split(Separators, StringSplitOptions.None))
            .Select(split => (
                split[0] == "on",
                (
                    int.Parse(split[2]), int.Parse(split[3]),
                    int.Parse(split[5]), int.Parse(split[6]),
                    int.Parse(split[8]), int.Parse(split[9])
                )
            )).ToList();

    private static readonly (int left, int right)[] InclusionOffsets = {
        (0, -1), // include cuboid min, exclude overlap min
        (0, 0), // include overlap min, include overlap max
        (1, 0) // exclude overlap max, include cuboid max
    };

    private static IEnumerable<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)> RemoveOverlap(
        (int minX, int maxX, int minY, int maxY, int minZ, int maxZ) cuboid,
        (int minX, int maxX, int minY, int maxY, int minZ, int maxZ) overlap
    )
    {
        var xBorders = new[] {cuboid.minX, overlap.minX, overlap.maxX, cuboid.maxX};
        var yBorders = new[] {cuboid.minY, overlap.minY, overlap.maxY, cuboid.maxY};
        var zBorders = new[] {cuboid.minZ, overlap.minZ, overlap.maxZ, cuboid.maxZ};

        for (var xi = 1; xi < 4; xi++)
        {
            var minX = xBorders[xi - 1] + InclusionOffsets[xi - 1].left;
            var maxX = xBorders[xi] + InclusionOffsets[xi - 1].right;
            if (minX > maxX)
            {
                continue;
            }

            for (var yi = 1; yi < 4; yi++)
            {
                var minY = yBorders[yi - 1] + InclusionOffsets[yi - 1].left;
                var maxY = yBorders[yi] + InclusionOffsets[yi - 1].right;
                if (minY > maxY)
                {
                    continue;
                }

                for (var zi = 1; zi < 4; zi++)
                {
                    var minZ = zBorders[zi - 1] + InclusionOffsets[zi - 1].left;
                    var maxZ = zBorders[zi] + InclusionOffsets[zi - 1].right;
                    if (minZ > maxZ || xi == 2 && yi == 2 && zi == 2)
                    {
                        continue;
                    }

                    yield return (minX, maxX, minY, maxY, minZ, maxZ);
                }
            }
        }
    }

    private static long Solve(IEnumerable<(bool, (int, int, int, int, int, int))> rebootSteps)
    {
        var cuboids = new LinkedList<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)>();
        foreach (var (on, rebootStepCuboid) in rebootSteps)
        {
            var rebootStepCuboids = new LinkedList<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)>();
            rebootStepCuboids.AddFirst(rebootStepCuboid);

            LinkedListNode<(int, int, int, int, int, int)>? nextCurrentCuboidNode;
            for (var currentCuboidNode = cuboids.First; currentCuboidNode != null; currentCuboidNode = nextCurrentCuboidNode)
            {
                var currentCuboid = currentCuboidNode.Value;
                nextCurrentCuboidNode = currentCuboidNode.Next;
                LinkedListNode<(int, int, int, int, int, int)>? nextRebootStepCuboidNode;
                for (var rebootStepCuboidNode = rebootStepCuboids.First; rebootStepCuboidNode != null; rebootStepCuboidNode = nextRebootStepCuboidNode)
                {
                    var currentRebootStepCuboid = rebootStepCuboidNode.Value;
                    nextRebootStepCuboidNode = rebootStepCuboidNode.Next;

                    var overlapMinX = Math.Max(currentRebootStepCuboid.minX, currentCuboid.minX);
                    var overlapMaxX = Math.Min(currentRebootStepCuboid.maxX, currentCuboid.maxX);
                    if (overlapMinX > overlapMaxX)
                    {
                        continue;
                    }

                    var overlapMinY = Math.Max(currentRebootStepCuboid.minY, currentCuboid.minY);
                    var overlapMaxY = Math.Min(currentRebootStepCuboid.maxY, currentCuboid.maxY);
                    if (overlapMinY > overlapMaxY)
                    {
                        continue;
                    }

                    var overlapMinZ = Math.Max(currentRebootStepCuboid.minZ, currentCuboid.minZ);
                    var overlapMaxZ = Math.Min(currentRebootStepCuboid.maxZ, currentCuboid.maxZ);
                    if (overlapMinZ > overlapMaxZ)
                    {
                        continue;
                    }

                    var overlap = (
                        overlapMinX, overlapMaxX,
                        overlapMinY, overlapMaxY,
                        overlapMinZ, overlapMaxZ
                    );

                    if (on)
                    {
                        foreach (var subCuboid in RemoveOverlap(currentRebootStepCuboid, overlap))
                        {
                            rebootStepCuboids.AddBefore(rebootStepCuboidNode, subCuboid);
                        }

                        rebootStepCuboids.Remove(rebootStepCuboidNode);
                    }
                    else
                    {
                        foreach (var subCuboid in RemoveOverlap(currentCuboid, overlap))
                        {
                            cuboids.AddBefore(currentCuboidNode, subCuboid);
                        }

                        cuboids.Remove(currentCuboidNode);
                    }
                }
            }

            if (on)
            {
                foreach (var leftoverRebootStepCuboid in rebootStepCuboids)
                {
                    cuboids.AddLast(leftoverRebootStepCuboid);
                }
            }
        }

        var total = 0L;
        foreach (var (minX, maxX, minY, maxY, minZ, maxZ) in cuboids)
        {
            total += ((long)maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1);
        }

        return total;
    }

    private static long Part1(IEnumerable<(bool, (int minX, int maxX, int minY, int maxY, int minZ, int maxZ) cuboid)> rebootSteps) =>
        Solve(rebootSteps.Where(step =>
            step.cuboid.minX >= -50
            && step.cuboid.maxX <= 50
            && step.cuboid.minY >= -50
            && step.cuboid.maxY <= 50
            && step.cuboid.minZ >= -50
            && step.cuboid.maxZ <= 50
        ));

    private static long Part2(IEnumerable<(bool, (int, int, int, int, int, int))> rebootSteps) =>
        Solve(rebootSteps);

    public static void Main()
    {
        var rebootSteps = GetRebootSteps();
        Console.WriteLine(Part1(rebootSteps));
        Console.WriteLine(Part2(rebootSteps));
    }
}
