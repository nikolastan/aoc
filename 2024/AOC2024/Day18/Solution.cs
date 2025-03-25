using NUnit.Framework;
using Utility.Extensions;

namespace Day18;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt", 7, 12);
        Assert.That(result, Is.EqualTo(22));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt", 71, 1024);
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2("Inputs/example.txt", 7, 12);
        Assert.That(result, Is.EqualTo("6,1"));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt", 71, 1024);
        Console.WriteLine(result);
    }

    static int SolvePart1(string inputPath, int mapDim, int numOfBytesFallen)
    {
        var map = ReadMap(inputPath, mapDim, numOfBytesFallen);
        Dictionary<(int, int), int> memo = [];
        FindMinSteps(map, (0, 0), (mapDim - 1, mapDim - 1), 0, memo);

        return memo[(mapDim - 1, mapDim - 1)];
    }

    static string SolvePart2(string inputPath, int mapDim, int numOfBytesFallen)
    {
        var map = ReadMap(inputPath, mapDim, numOfBytesFallen);

        var fallingBytes = ReadFallingBytes(inputPath, numOfBytesFallen);

        foreach(var @byte in fallingBytes)
        {
            map[@byte.X][@byte.Y].Type = TileType.Corrupt;

            if(!IsReachable(map, (0, 0), (mapDim - 1, mapDim - 1), []))
                return $"{@byte.X},{@byte.Y}";
		}

        throw new InvalidOperationException("Byte that prevents exit was not found!");
    }

    static Tile[][] ReadMap(string inputPath, int mapDim, int numOfBytesFallen)
    {
        var parsedBytes = ParseByteCoordinates(File.ReadAllLines(inputPath));

        var fallenBytes = parsedBytes
            .Take(numOfBytesFallen)
            .ToList();

        var upBytes = parsedBytes
            .Skip(numOfBytesFallen)
            .ToList();

        return Enumerable.Range(0, mapDim)
            .Select(x => new Tile[mapDim])
            .Select((row, i) => row.Select((_, j) =>
            {
                if (fallenBytes.Contains((i, j)))
                    return new Tile(TileType.Corrupt);

                return new Tile(TileType.Free);
            }).ToArray())
            .ToArray();
    }

    static IEnumerable<(int X, int Y)> ReadFallingBytes(string inputPath, int skipBytes)
    {
        return ParseByteCoordinates(File.ReadAllLines(inputPath))
            .Skip(skipBytes);
    }

    static IEnumerable<(int, int)> ParseByteCoordinates(string[] rawCoordinates)
    {
        return rawCoordinates.Select(x => x.Split(','))
            .Select(x => (int.Parse(x[0]), int.Parse(x[1])));
    }

    static void FindMinSteps(
        Tile[][] map,
        (int X, int Y) currentPos,
        (int X, int Y) end,
        int currentSteps,
        Dictionary<(int X, int Y), int> memo)
    {
        if (map[currentPos.X][currentPos.Y].Type is TileType.Corrupt)
            return;

        if (memo.TryGetValue(currentPos, out var currentMin) && currentSteps >= currentMin)
            return;
        else
            memo[currentPos] = currentSteps;

        if (currentPos == end)
            return;

        var nextPos = (currentPos.X + 1, currentPos.Y);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindMinSteps(map, nextPos, end, currentSteps + 1, memo);

        nextPos = (currentPos.X, currentPos.Y + 1);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindMinSteps(map, nextPos, end, currentSteps + 1, memo);

        nextPos = (currentPos.X - 1, currentPos.Y);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindMinSteps(map, nextPos, end, currentSteps + 1, memo);

        nextPos = (currentPos.X, currentPos.Y - 1);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindMinSteps(map, nextPos, end, currentSteps + 1, memo);
    }

    static bool IsReachable(Tile[][] map, (int X, int Y) currentPos, (int X, int Y) end, List<(int, int)> tilesVisited)
    {
        if (!ArrayExtensions.IsValidTile(map, currentPos)
            || map[currentPos.X][currentPos.Y].Type is TileType.Corrupt
            || tilesVisited.Contains(currentPos))
            return false;

        if (currentPos == end)
            return true;

        tilesVisited.Add(currentPos);

        var flood = new List<Func<bool>>()
        {
            () => IsReachable(map, (currentPos.X + 1, currentPos.Y), end, tilesVisited),
            () => IsReachable(map, (currentPos.X, currentPos.Y + 1), end, tilesVisited),
            () => IsReachable(map, (currentPos.X - 1, currentPos.Y), end, tilesVisited),
            () => IsReachable(map, (currentPos.X, currentPos.Y - 1), end, tilesVisited),
        };

        return flood.Any(x => x.Invoke());
	}

    class Tile(TileType type)
    {
        public TileType Type = type;
        public bool Traversed = false;
    }

    enum TileType
    {
        Free,
        Corrupt
    }
}