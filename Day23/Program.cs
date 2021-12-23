using System.Text;

namespace Day23;

class Program
{
    private record Amphipod(char Type, bool BeenInHallway = false)
    {
        public readonly int TargetRoom = Type - 'A';

        private static readonly int[] MoveCosts = {1, 10, 100, 1000};
        public readonly int MoveCost = MoveCosts[Type - 'A'];

        public virtual bool Equals(Amphipod? other) => !ReferenceEquals(null, other) && (
            ReferenceEquals(this, other) || Type == other.Type
        );

        public override int GetHashCode() => Type.GetHashCode();
    }

    private record Room(IReadOnlyList<Amphipod?> Amphipods)
    {
        public virtual bool Equals(Room? other) => !ReferenceEquals(other, null) && (
            ReferenceEquals(this, other) || Amphipods.SequenceEqual(other.Amphipods)
        );

        public override int GetHashCode() => Amphipods.Aggregate(0, HashCode.Combine);
    }

    private record HallwayEnd(Amphipod? Outer, Amphipod? Inner);

    private record HallwayMiddle(Amphipod? Amphipod);

    private record State(IReadOnlyList<Room> Rooms, HallwayEnd Left, IReadOnlyList<HallwayMiddle> Middles, HallwayEnd Right)
    {
        public virtual bool Equals(State? other) => !ReferenceEquals(other, null) && (ReferenceEquals(this, other) || (
            Rooms.SequenceEqual(other.Rooms)
            && Left.Equals(other.Left)
            && Middles.SequenceEqual(other.Middles)
            && Right.Equals(other.Right)
        ));

        public override int GetHashCode() => HashCode.Combine(
            Rooms.Aggregate(0, HashCode.Combine),
            Left,
            Middles.Aggregate(0, HashCode.Combine),
            Right
        );

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("#############");
            sb.Append('#');
            sb.Append(Left.Outer?.Type ?? '.');
            sb.Append(Left.Inner?.Type ?? '.');
            sb.Append('.');
            foreach (var middle in Middles)
            {
                sb.Append(middle.Amphipod?.Type ?? '.');
                sb.Append('.');
            }
            sb.Append(Right.Inner?.Type ?? '.');
            sb.Append(Right.Outer?.Type ?? '.');
            sb.Append('#');
            sb.AppendLine();

            for (var i = 0; i < Rooms[0].Amphipods.Count; i++)
            {
                sb.Append(i == 0 ? "###" : "  #");
                foreach (var t in Rooms)
                {
                    sb.Append(t.Amphipods[i]?.Type ?? '.');
                    sb.Append('#');
                }
                sb.AppendLine(i == 0 ? "##" : "");
            }

            sb.AppendLine("  #########");
            return sb.ToString();
        }
    }

    private static (Room[], Amphipod, int)? GetNewRoomsAndTopAmphipod(IReadOnlyList<Room> rooms, int roomIndex)
    {
        var room = rooms[roomIndex];
        Amphipod? topRoomAmphipod = null;
        int steps;
        for (steps = 0; steps < room.Amphipods.Count; steps++)
        {
            if (room.Amphipods[steps] is {} a)
            {
                topRoomAmphipod = a;
                break;
            }
        }

        if (topRoomAmphipod is null || topRoomAmphipod.BeenInHallway)
        {
            return null;
        }

        topRoomAmphipod = topRoomAmphipod with {BeenInHallway = true};
        var newRoomAmphipods = room.Amphipods.ToArray();
        newRoomAmphipods[steps] = null;
        var newRooms = rooms.ToArray();
        newRooms[roomIndex] = new Room(newRoomAmphipods);
        return (newRooms, topRoomAmphipod, steps);
    }

    private static IEnumerable<(State, int)> GetMovesFromRoomToHallway(State state, int score, int roomIndex)
    {
        var (rooms, left, middles, right) = state;

        if (GetNewRoomsAndTopAmphipod(rooms, roomIndex) is not var (newRooms, amphipod, stepsToTopOfRoom))
        {
            yield break;
        }

        var reachableLeft = roomIndex - 1;
        var stepsInHallway = 2;
        while (reachableLeft >= 0 && middles[reachableLeft] is (null) _)
        {
            var newHallwayMiddles = middles.ToArray();
            newHallwayMiddles[reachableLeft] = new HallwayMiddle(amphipod);
            yield return (
                state with
                {
                    Rooms = newRooms,
                    Middles = newHallwayMiddles
                },
                score + (stepsToTopOfRoom + stepsInHallway) * amphipod.MoveCost
            );
            reachableLeft--;
            stepsInHallway += 2;
        }

        if (reachableLeft == -1)
        {
            if (left is (_, null))
            {
                yield return (
                    state with
                    {
                        Rooms = newRooms,
                        Left = left with {Inner = amphipod}
                    },
                    score + (stepsToTopOfRoom + stepsInHallway) * amphipod.MoveCost
                );
            }

            if (left is (null, null))
            {
                yield return (
                    state with
                    {
                        Rooms = newRooms,
                        Left = left with {Outer = amphipod}
                    },
                    score + (stepsToTopOfRoom + stepsInHallway + 1) * amphipod.MoveCost
                );
            }
        }

        var reachableRight = roomIndex;
        stepsInHallway = 2;
        while (reachableRight < middles.Count && middles[reachableRight] is (null) _)
        {
            var newHallwayMiddles = middles.ToArray();
            newHallwayMiddles[reachableRight] = new HallwayMiddle(amphipod);
            yield return (
                state with
                {
                    Rooms = newRooms,
                    Middles = newHallwayMiddles
                },
                score + (stepsToTopOfRoom + stepsInHallway) * amphipod.MoveCost
            );
            reachableRight++;
            stepsInHallway += 2;
        }

        if (reachableRight == middles.Count)
        {
            if (right is (_, null))
            {
                yield return (
                    state with
                    {
                        Rooms = newRooms,
                        Right = right with {Inner = amphipod}
                    },
                    score + (stepsToTopOfRoom + stepsInHallway) * amphipod.MoveCost
                );
            }

            if (right is (null, null))
            {
                yield return (
                    state with
                    {
                        Rooms = newRooms,
                        Right = right with {Outer = amphipod}
                    },
                    score + (stepsToTopOfRoom + stepsInHallway + 1) * amphipod.MoveCost
                );
            }
        }
    }

    private static (HallwayEnd, Amphipod, int)? GetNewHallwayAndClosestAmphipod(HallwayEnd hallwayEnd) =>
        hallwayEnd switch
        {
            (null, null) => null,
            (_, {} amphipod) => (hallwayEnd with {Inner = null}, amphipod, 0),
            ({} amphipod, null) => (hallwayEnd with {Outer = null}, amphipod, 1)
        };

    private static (Room[], int)? GetRoomsWithAmphipodMovedIn(IReadOnlyList<Room> rooms, Amphipod amphipod)
    {
        var roomIndex = amphipod.TargetRoom;
        var roomAmphipods = rooms[roomIndex].Amphipods;
        int steps;
        for (steps = 0; steps < roomAmphipods.Count; steps++)
        {
            if (roomAmphipods[steps] != null)
            {
                break;
            }
        }

        if (steps == 0)
        {
            return null;
        }

        for (var i = steps; i < roomAmphipods.Count; i++)
        {
            if (roomAmphipods[i]?.Type != amphipod.Type)
            {
                return null;
            }
        }

        steps--;
        var newAmphipods = roomAmphipods.ToArray();
        newAmphipods[steps] = amphipod;
        var newRooms = rooms.ToArray();
        newRooms[roomIndex] = new Room(newAmphipods);
        return (newRooms, steps);
    }

    private static bool IsPathClearGoingRight(IReadOnlyList<HallwayMiddle> middles, int fromIndex, int targetRoomIndex)
    {
        for (var i = fromIndex; i < targetRoomIndex; i++)
        {
            if (middles[i] is not (null) _)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPathClearGoingLeft(IReadOnlyList<HallwayMiddle> middles, int fromIndex, int targetRoomIndex)
    {
        for (var i = fromIndex; i >= targetRoomIndex; i--)
        {
            if (middles[i] is not (null) _)
            {
                return false;
            }
        }

        return true;
    }

    private static (State, int)? GetMoveFromLeftHallwayEndToRoom(State state, int score)
    {
        var (rooms, left, middles, _) = state;
        if (GetNewHallwayAndClosestAmphipod(left) is not var (newLeft, amphipod, stepsToInnerSideOfHallwayEnd))
        {
            return null;
        }

        if (GetRoomsWithAmphipodMovedIn(rooms, amphipod) is not var (newRooms, stepsIntoRoom))
        {
            return null;
        }

        var targetRoomIndex = amphipod.TargetRoom;
        if (!IsPathClearGoingRight(middles, 0, targetRoomIndex))
        {
            return null;
        }

        var totalSteps = stepsToInnerSideOfHallwayEnd + 2 * (targetRoomIndex + 1) + stepsIntoRoom;
        return (
            state with
            {
                Rooms = newRooms,
                Left = newLeft
            },
            score + totalSteps * amphipod.MoveCost
        );
    }

    private static (State, int)? GetMoveFromRightHallwayEndToRoom(State state, int score)
    {
        var (rooms, _, middles, right) = state;
        if (GetNewHallwayAndClosestAmphipod(right) is not var (newRight, amphipod, stepsToInnerSideOfHallwayEnd))
        {
            return null;
        }

        if (GetRoomsWithAmphipodMovedIn(rooms, amphipod) is not var (newRooms, stepsIntoRoom))
        {
            return null;
        }

        var targetRoomIndex = amphipod.TargetRoom;
        if (!IsPathClearGoingLeft(middles, middles.Count - 1, targetRoomIndex))
        {
            return null;
        }

        var totalSteps = stepsToInnerSideOfHallwayEnd + 2 * (rooms.Count - targetRoomIndex) + stepsIntoRoom;
        return (
            state with
            {
                Rooms = newRooms,
                Right = newRight,
            },
            score + totalSteps * amphipod.MoveCost
        );
    }

    private static (State, int)? GetMoveFromHallwayMiddleToRoom(State state, int score, int hallwayIndex)
    {
        var (rooms, _, middles, _) = state;
        var hallway = middles[hallwayIndex];
        if (hallway is not ({} amphipod) _)
        {
            return null;
        }

        if (GetRoomsWithAmphipodMovedIn(rooms, amphipod) is not var (newRooms, stepsIntoRoom))
        {
            return null;
        }

        var targetRoomIndex = amphipod.TargetRoom;
        var goingLeft = targetRoomIndex <= hallwayIndex;

        if (goingLeft && !IsPathClearGoingLeft(middles, hallwayIndex - 1, targetRoomIndex)
            || !goingLeft && !IsPathClearGoingRight(middles, hallwayIndex + 1, targetRoomIndex))
        {
            return null;
        }

        var newMiddles = middles.ToArray();
        newMiddles[hallwayIndex] = new HallwayMiddle(null);
        var totalSteps = 2 * (goingLeft ? hallwayIndex - targetRoomIndex + 1 : targetRoomIndex - hallwayIndex) + stepsIntoRoom;
        return (
            state with
            {
                Rooms = newRooms,
                Middles = newMiddles,
            },
            score + totalSteps * amphipod.MoveCost
        );
    }

    private static IEnumerable<(State, int)> GetAllNextStates(State state, int score)
    {
        var nextStates = Enumerable.Range(0, state.Rooms.Count)
            .SelectMany(roomIndex => GetMovesFromRoomToHallway(state, score, roomIndex))
            .ToList();
        nextStates.AddRange(
            Enumerable.Range(0, state.Middles.Count)
                .Select(hallwayMiddleIndex => GetMoveFromHallwayMiddleToRoom(state, score, hallwayMiddleIndex))
                .Where(s => s.HasValue)
                .Select(s => s!.Value)
        );
        if (GetMoveFromLeftHallwayEndToRoom(state, score) is {} leftHallwayEndMove)
        {
            nextStates.Add(leftHallwayEndMove);
        }

        if (GetMoveFromRightHallwayEndToRoom(state, score) is {} rightHallwayEndMove)
        {
            nextStates.Add(rightHallwayEndMove);
        }

        return nextStates;
    }

    private static bool IsFinalState(State s)
    {
        for (var i = 0; i < s.Rooms.Count; i++)
        {
            if (s.Rooms[i].Amphipods.Any(a => a == null || a.TargetRoom != i))
            {
                return false;
            }
        }

        return true;
    }

    private static (List<State>, int) Solve(State initialState)
    {
        var lowestScores = new Dictionary<State, (int score, State? from)> {{initialState, (0, null)}};
        var priorityQueue = new PriorityQueue<State, int>();
        priorityQueue.Enqueue(initialState, 0);
        while (priorityQueue.Count > 0)
        {
            var state = priorityQueue.Dequeue();
            var (score, from) = lowestScores[state];

            if (IsFinalState(state))
            {
                var path = new List<State> {state};
                while (from != null)
                {
                    path.Add(from);
                    from = lowestScores[from].from;
                }

                path.Reverse();
                return (path, score);
            }

            var nextStates = GetAllNextStates(state, score);
            foreach (var (nextState, nextStateScore) in nextStates)
            {
                if (!lowestScores.TryGetValue(nextState, out var existing) || existing.score > nextStateScore)
                {
                    lowestScores[nextState] = (nextStateScore, state);
                    priorityQueue.Enqueue(nextState, nextStateScore);
                }
            }
        }

        return (new List<State>(), -1);
    }

    private static readonly State InitialState = new(
        new[]
        {
            new Room(new Amphipod?[] {new('B'), new('D')}),
            new Room(new Amphipod?[] {new('A'), new('C')}),
            new Room(new Amphipod?[] {new('A'), new('B')}),
            new Room(new Amphipod?[] {new('D'), new('C')}),
        },
        new HallwayEnd(null, null),
        new[]
        {
            new HallwayMiddle(null),
            new HallwayMiddle(null),
            new HallwayMiddle(null)
        },
        new HallwayEnd(null, null)
    );

    private static State InsertMoreAmphipods(State state)
    {
        var (rooms, _, _, _) = state;

        var newRooms = new Room[]
        {
            new(new[] {rooms[0].Amphipods[0], new('D'), new('D'), rooms[0].Amphipods[1]}),
            new(new[] {rooms[1].Amphipods[0], new('C'), new('B'), rooms[1].Amphipods[1]}),
            new(new[] {rooms[2].Amphipods[0], new('B'), new('A'), rooms[2].Amphipods[1]}),
            new(new[] {rooms[3].Amphipods[0], new('A'), new('C'), rooms[3].Amphipods[1]})
        };

        return state with
        {
            Rooms = newRooms
        };
    }

    public static void Main()
    {
        var (_, part1) = Solve(InitialState);
        var (part2Path, part2) = Solve(InsertMoreAmphipods(InitialState));
        Console.WriteLine(part1);
        Console.WriteLine(part2);

        foreach (var state in part2Path)
        {
            Console.WriteLine(state);
        }
    }
}
