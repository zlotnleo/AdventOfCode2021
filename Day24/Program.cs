namespace Day24;

class Program
{
    public static long Decompiled(long[] input)
    {
        var (w, x, y, z) = (0L, 0L, 0L, 0L);

        // z in base 26

        w = input[0];
        x = z % 26 + 10;
        z /= 1;
        if (x != w) // always true, x >= 10, 1 <= w <= 9
        {
            z = z * 26 + w + 13; // 26-digits of z: [input[0] + 13] (length 1)
        }


        w = input[1];
        x = z % 26 + 13;
        z /= 1;
        if (x != w) // always true, x >= 13, 1 <= w <= 9
        {
            z = z * 26 + w + 10; // 26-digits of z: [input[0] + 13][input[1] + 10] (length 2)
        }

        w = input[2];
        x = z % 26 + 13;
        z /= 1;
        if (x != w) // always true, x >= 13, 1 <= w <= 9
        {
            z = z * 26 + w + 3; // 26-digits of z: [input[0] + 13][input[1] + 10][input[2] + 3] (length 3)
        }

        w = input[3];
        x = z % 26 + -11; // x is input[2] + 3 - 11 = input[2] - 8
        z /= 26; // 26-digits of z: [input[0] + 13][input[1] + 10]
        if (x != w) // if input[3] != input[2] - 8
        {
            z = z * 26 + w + 1; // 26-digits of z: [input[0] + 13][input[1] + 10][input[3] + 1]
        }

        // if input[3] == input[2] - 8: z is [input[0] + 13][input[1] + 10] (length 2)
        // else: z is [input[0] + 13][input[1] + 10][input[3] + 1] (length 3)

        w = input[4];
        x = z % 26 + 11;
        z /= 1;
        if (x != w) // always true, x >= 11, 1 <= w <= 9
        {
            z = z * 26 + w + 9; // z is length 3 or 4 in base-26, last digit [input[4] + 9]
        }

        w = input[5];
        x = z % 26 + -4; // x is input[4] + 9 - 4 = input[4] + 5
        z /= 26; // z is length 2 or 3
        if (x != w) // if input[5] != input[4] + 5
        {
            z = z * 26 + w + 3;
        }
        // z is length 2, 3 or 4

        w = input[6];
        x = z % 26 + 12;
        z /= 1;
        if (x != w) // always true, x >= 12, 1 <= w <= 9
        {
            z = z * 26 + w + 5; // z is length 3, 4 or 5, last digit [input[6] + 5]
        }

        w = input[7];
        x = z % 26 + 12;
        z /= 1;
        if (x != w) // always true, x >= 12, 1 <= w <= 9
        {
            z = z * 26 + w + 1; // z is length 4, 5 or 6, last digit [input[7] + 1]
        }

        w = input[8];
        x = z % 26 + 15;
        z /= 1;
        if (x != w) // always true, x >= 15, 1 <= w <= 9
        {
            z = z * 26 + w + 0; // z is length 5, 6 or 7, last digit [input[8]]
        }

        // Next we do 5 divisions by 26, so it has to be 5 digits long
        // and the program mustn't go into any conditionals
        // There is only 1 way to get here with z being 5 base-26 digits:
        // input[3] == input[2] - 8
        // input[5] == input[4] + 5
        // so z must be [input[0] + 13][input[1] + 10][input[6] + 5][input[7] + 1][input[8]]

        w = input[9];
        x = z % 26 + -2; // input[8] - 2
        z /= 26;
        if (x != w) // input[9] != input[8] - 2
        {
            z = z * 26 + w + 13;
        }
        // without going into conditional, z is [input[0] + 13][input[1] + 10][input[6] + 5][input[7] + 1]

        w = input[10];
        x = z % 26 + -5; // input[7] + 1 - 5 = input[7] - 4
        z /= 26;
        if (x != w) // input[10] != input[7] - 4
        {
            z = z * 26 + w + 7;
        }
        // without going into conditional, z is [input[0] + 13][input[1] + 10][input[6] + 5]

        w = input[11];
        x = z % 26 + -11; // input[6] + 5 - 11 = input[6] - 6
        z /= 26;
        if (x != w) // input[11] != input[6] - 6
        {
            z = z * 26 + w + 15;
        }
        // without going into conditional, z is [input[0] + 13][input[1] + 10]

        w = input[12];
        x = z % 26 + -13; // input[1] + 10 - 13 = input[1] - 3
        z /= 26;
        if (x != w) // input[12] != input[1] - 3
        {
            z = z * 26 + w + 12;
        }
        // without going into conditional, z is [input[0] + 13]

        w = input[13];
        x = z % 26 + -10; // input[0] + 13 - 10 = input[0] + 3
        z /= 26;
        if (x != w) // input[13] != input[0] + 3
        {
            z = z * 26 + w + 8;
        }
        // without going into conditional, z is 0

        // For z to be 0, the following is true:
        // input[3] == input[2] - 8
        // input[5] == input[4] + 5
        // input[9] == input[8] - 2
        // input[10] == input[7] - 4
        // input[11] == input[6] - 6
        // input[12] == input[1] - 3
        // input[13] == input[0] + 3
        return z;
    }

    private static long Part1()
    {
        // input index: 0 1 2 3 4 5 6 7 8 9 0 1 2 3
        //      digits: 6 9 9 1 4 9 9 9 9 7 5 3 6 9
        return 69914999975369L;
    }

    private static long Part2()
    {
        // input index: 0 1 2 3 4 5 6 7 8 9 0 1 2 3
        //      digits: 1 4 9 1 1 6 7 5 3 1 1 1 1 4
        return 14911675311114L;
    }

    public static void Main()
    {
        Console.WriteLine(Part1());
        Console.WriteLine(Part2());
    }
}
