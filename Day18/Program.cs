using System.Diagnostics;

namespace Day18;

class Program
{
    private interface ISnailfishNumber
    {
        public Pair? Parent { get; set; }
        public bool IsParentsLeft { get; set; }
        public int Depth { get; set; }

        public ISnailfishNumber Clone(Pair? parent = null, bool isLeft = false);
    }

    private class Value : ISnailfishNumber
    {
        public Pair? Parent { get; set; }
        public bool IsParentsLeft { get; set; }
        public int Depth { get; set; }
        public int X;

        public void Deconstruct(out Pair? parent, out bool isParentsLeft, out int depth, out int x)
        {
            parent = Parent;
            isParentsLeft = IsParentsLeft;
            depth = Depth;
            x = X;
        }

        public ISnailfishNumber Clone(Pair? parent, bool isLeft) =>
            new Value
            {
                Parent = parent,
                IsParentsLeft = isLeft,
                Depth = Depth,
                X = X
            };
    }

    private class Pair : ISnailfishNumber
    {
        public Pair? Parent { get; set; }
        public bool IsParentsLeft { get; set; }
        public int Depth { get; set; }
        public ISnailfishNumber Left;
        public ISnailfishNumber Right;

        public void Deconstruct(out Pair? parent, out bool isParentsLeft, out int depth, out ISnailfishNumber left, out ISnailfishNumber right)
        {
            parent = Parent;
            isParentsLeft = IsParentsLeft;
            depth = Depth;
            left = Left;
            right = Right;
        }

        public void Insert(ISnailfishNumber number, bool isLeft)
        {
            (isLeft ? ref Left : ref Right) = number;
            number.Parent = this;
            number.IsParentsLeft = isLeft;
            number.Depth = Depth + 1;
        }

        public ISnailfishNumber Clone(Pair? parent, bool isLeft)
        {
            var clone = new Pair
            {
                Parent = parent,
                IsParentsLeft = isLeft,
                Depth = Depth
            };
            clone.Left = Left.Clone(clone, true);
            clone.Right = Right.Clone(clone, false);
            return clone;
        }
    }

    private static ISnailfishNumber ParseSnailfishNumber(IEnumerator<char> input, Pair? parent, bool isLeft, int depth)
    {
        input.MoveNext();
        if (input.Current is >= '0' and <= '9')
        {
            return new Value
            {
                Parent = parent,
                IsParentsLeft = isLeft,
                Depth = depth,
                X = input.Current - '0'
            };
        }

        var number = new Pair
        {
            Parent = parent,
            IsParentsLeft = isLeft,
            Depth = depth
        };
        Debug.Assert(input.Current == '[');
        number.Left = ParseSnailfishNumber(input, number, true, depth + 1);
        input.MoveNext();
        Debug.Assert(input.Current == ',');
        number.Right = ParseSnailfishNumber(input, number, false, depth + 1);
        input.MoveNext();
        Debug.Assert(input.Current == ']');
        return number;
    }

    private static ISnailfishNumber[] ReadInput() =>
        File.ReadAllLines("input.txt")
            .Select(line => ParseSnailfishNumber(line.GetEnumerator(), null, false, 0))
            .ToArray();

    private static List<Pair> GetAllPairsToExplode(ISnailfishNumber number)
    {
        var toExplode = new List<Pair>();

        void Traverse(ISnailfishNumber n)
        {
            switch (n)
            {
                case Value:
                    break;
                case Pair(_, _, 4, Value, Value) p:
                    toExplode.Add(p);
                    break;
                case Pair(_, _, _, var left, var right):
                    Traverse(left);
                    Traverse(right);
                    break;
            }
        }

        Traverse(number);
        return toExplode;
    }

    private static Value? GetValueToSplit(ISnailfishNumber number) =>
        number switch
        {
            Value(_, _, _, >= 10) v => v,
            Value => null,
            Pair p => GetValueToSplit(p.Left) ?? GetValueToSplit(p.Right)
        };

    private static Value? GetLeftValue(ISnailfishNumber? number)
    {
        while (number is {IsParentsLeft: true})
        {
            number = number.Parent;
        }

        if (number?.Parent is null)
        {
            return null;
        }

        number = number.Parent.Left;
        while (true)
        {
            switch (number)
            {
                case Value v:
                    return v;
                case Pair {Right: var right}:
                    number = right;
                    break;
            }
        }
    }

    private static Value? GetRightValue(ISnailfishNumber? number)
    {
        while (number is {IsParentsLeft: false})
        {
            number = number.Parent;
        }

        if (number?.Parent is null)
        {
            return null;
        }

        number = number.Parent.Right;
        while (true)
        {
            switch (number)
            {
                case Value v:
                    return v;
                case Pair {Left: var left}:
                    number = left;
                    break;
            }
        }
    }

    private static void Explode(Pair pair)
    {
        var (parent, isParentsLeft, _, vl, vr) = pair;
        var l = ((Value)vl).X;
        var r = ((Value)vr).X;

        Debug.Assert(parent != null);
        var left = GetLeftValue(pair);
        if (left != null)
        {
            left.X += l;
        }

        var right = GetRightValue(pair);
        if (right != null)
        {
            right.X += r;
        }

        parent.Insert(new Value{X = 0}, isParentsLeft);
    }

    private static Pair? Split(Value number)
    {
        var (parent, isParentsLeft, depth, x) = number;
        Debug.Assert(parent != null);
        var halfRoundDown = x / 2;
        var pair = new Pair();
        parent.Insert(pair, isParentsLeft);
        pair.Insert(new Value{X = halfRoundDown}, true);
        pair.Insert(new Value{X = x - halfRoundDown}, false);
        return depth == 4 ? pair : null;
    }

    private static void IncrementDepths(ISnailfishNumber number)
    {
        number.Depth++;
        if (number is Pair p)
        {
            IncrementDepths(p.Left);
            IncrementDepths(p.Right);
        }
    }

    private static ISnailfishNumber Add(ISnailfishNumber n1, ISnailfishNumber n2)
    {
        var number = new Pair
        {
            Parent = null,
            Depth = 0
        };
        IncrementDepths(n1);
        IncrementDepths(n2);
        number.Insert(n1, true);
        number.Insert(n2, false);

        var initialPairsToExplode = GetAllPairsToExplode(number);
        var changed = initialPairsToExplode.Any();
        foreach (var pair in initialPairsToExplode)
        {
            Explode(pair);
        }

        while (changed)
        {
            changed = false;
            Pair? pairToExplode = null;

            var valueToSplit = GetValueToSplit(number);
            if(valueToSplit != null)
            {
                changed = true;
                pairToExplode = Split(valueToSplit);
            }

            if (pairToExplode != null)
            {
                changed = true;
                Explode(pairToExplode);
            }
        }

        return number;
    }

    private static int GetMagnitude(ISnailfishNumber number) =>
        number switch
        {
            Value v => v.X,
            Pair p => 3 * GetMagnitude(p.Left) + 2 * GetMagnitude(p.Right)
        };

    private static int Part1(IEnumerable<ISnailfishNumber> numbers) =>
        GetMagnitude(numbers.Select(n => n.Clone()).Aggregate(Add));

    private static int Part2(ISnailfishNumber[] numbers) =>
        numbers.SelectMany(n1 => numbers.Select(n2 => (n1, n2)))
            .Where(t => t.n1 != t.n2)
            .Select(t => GetMagnitude(Add(t.n1.Clone(), t.n2.Clone())))
            .Max();

    public static void Main()
    {
        var input = ReadInput();
        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }
}
