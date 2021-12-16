using System.Collections.Immutable;

namespace Day16;

class Program
{
    private interface IPacket
    {
    }

    private enum OpType
    {
        Sum,
        Product,
        Min,
        Max,
        Greater,
        Less,
        Equal
    }

    private record LiteralPacket(int Version, long Number) : IPacket;

    private record OperatorPacket(int Version, OpType OpType, ImmutableQueue<IPacket> SubPackets) : IPacket;

    private static IEnumerable<byte> ConvertHex(char hexChar)
    {
        return (hexChar switch
            {
                >= '0' and <= '9' => Convert.ToString(hexChar - '0', 2),
                >= 'A' and <= 'F' => Convert.ToString(0xA + hexChar - 'A', 2)
            })
            .PadLeft(4, '0')
            .Select(c => c == '0' ? (byte)0 : (byte)1);
    }

    private static void ReadNBits(IEnumerator<(byte value, int)> input, int n, ref long into)
    {
        for (var i = 0; i < n; i++)
        {
            input.MoveNext();
            into = (into << 1) | input.Current.value;
        }
    }

    private static long ReadNBits(IEnumerator<(byte, int)> input, int n)
    {
        var value = 0L;
        ReadNBits(input, n, ref value);
        return value;
    }

    private static IPacket ParsePacket(IEnumerator<(byte, int index)> input)
    {
        var version = ReadNBits(input, 3);
        var typeId = ReadNBits(input, 3);

        if (typeId == 4)
        {
            var value = 0L;
            var isLastGroup = false;
            while (!isLastGroup)
            {
                if (ReadNBits(input, 1) == 0)
                {
                    isLastGroup = true;
                }

                ReadNBits(input, 4, ref value);
            }

            return new LiteralPacket((int)version, value);
        }

        var packets = ImmutableQueue<IPacket>.Empty;
        var lengthTypeId = ReadNBits(input, 1);
        if (lengthTypeId == 0)
        {
            var numberOfBits = ReadNBits(input, 15);
            var endIndex = input.Current.index + numberOfBits;
            while (input.Current.index < endIndex)
            {
                packets = packets.Enqueue(ParsePacket(input));
            }
        }
        else
        {
            var numberOfPackets = ReadNBits(input, 11);
            for (var i = 0; i < numberOfPackets; i++)
            {
                packets = packets.Enqueue(ParsePacket(input));
            }
        }

        var opType = typeId switch
        {
            0 => OpType.Sum,
            1 => OpType.Product,
            2 => OpType.Min,
            3 => OpType.Max,
            5 => OpType.Greater,
            6 => OpType.Less,
            7 => OpType.Equal
        };
        return new OperatorPacket((int)version, opType, packets);
    }

    private static IPacket GetInput()
    {
        using var input = File.ReadAllText("input.txt").Trim()
            .SelectMany(ConvertHex)
            .Select((b, i) => (b, i))
            .GetEnumerator();

        return ParsePacket(input);
    }

    private static int SumVersions(IPacket packet) =>
        packet switch
        {
            LiteralPacket(var version, _) => version,
            OperatorPacket(var version, _, var subPackets) =>
                version + subPackets.Select(SumVersions).Sum(),
        };

    private static long Eval(IPacket packet) =>
        packet switch
        {
            LiteralPacket(_, var value) => value,
            OperatorPacket(_, OpType.Sum, var subPackets) =>
                subPackets.Select(Eval).Sum(),
            OperatorPacket(_, OpType.Product, var subPackets) =>
                subPackets.Select(Eval).Aggregate((acc, val) => acc * val),
            OperatorPacket(_, OpType.Min, var subPackets) =>
                subPackets.Select(Eval).Min(),
            OperatorPacket(_, OpType.Max, var subPackets) =>
                subPackets.Select(Eval).Max(),
            OperatorPacket(_, OpType.Greater, var (p1, (p2, _))) =>
                Eval(p1) > Eval(p2) ? 1 : 0,
            OperatorPacket(_, OpType.Less, var (p1, (p2, _))) =>
                Eval(p1) < Eval(p2) ? 1 : 0,
            OperatorPacket(_, OpType.Equal, var (p1, (p2, _))) =>
                Eval(p1) == Eval(p2) ? 1 : 0
        };

    public static void Main()
    {
        var packet = GetInput();
        Console.WriteLine(SumVersions(packet));
        Console.WriteLine(Eval(packet));
    }
}

public static class Extensions
{
    public static void Deconstruct<T>(this ImmutableQueue<T> queue, out T top, out ImmutableQueue<T> rest) =>
        rest = queue.Dequeue(out top);
}
