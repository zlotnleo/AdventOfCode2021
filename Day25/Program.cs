class Program
{
    private static char[][] GetSeafloor() =>
        File.ReadAllLines("input.txt")
            .Select(line => line.ToArray())
            .ToArray();

    private static bool MoveEast(char[][] seaFloor)
    {
        var moved = false;
        for (var y = 0; y < seaFloor.Length; y++)
        {
            var toMove = new List<int>();

            for (var x = 0; x < seaFloor[0].Length; x++)
            {
                var newX = (x + 1) % seaFloor[0].Length;
                if (seaFloor[y][x] == '>' && seaFloor[y][newX] == '.')
                {
                    toMove.Add(x);
                }
            }

            foreach(var x in toMove){
                moved = true;
                var newX = (x + 1) % seaFloor[0].Length;
                (seaFloor[y][x], seaFloor[y][newX]) = (seaFloor[y][newX], seaFloor[y][x]);
            }
        }

        return moved;
    }

    private static bool MoveSouth(char[][] seaFloor)
    {
        var moved = false;
        for (var x = 0; x < seaFloor[0].Length; x++)
        {
            var toMove = new List<int>();

            for (var y = 0; y < seaFloor.Length; y++)
            {
                var newY = (y + 1) % seaFloor.Length;
                if (seaFloor[y][x] == 'v' && seaFloor[newY][x] == '.')
                {
                    toMove.Add(y);
                }
            }

            foreach(var y in toMove){
                moved = true;
                var newY = (y + 1) % seaFloor.Length;
                (seaFloor[y][x], seaFloor[newY][x]) = (seaFloor[newY][x], seaFloor[y][x]);
            }
        }

        return moved;
    }

    private static int Solve(char[][] seafloor)
    {
        for(var step = 1;; step++) {
            var moved = MoveEast(seafloor);
            moved |= MoveSouth(seafloor);

            if(!moved){
                return step;
            }
        }
    }

    public static void Main()
    {
        var seafloor = GetSeafloor();
        Console.WriteLine(Solve(seafloor));
    }
}
