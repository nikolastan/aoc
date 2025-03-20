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

        List<(int X, int Y)> blockingBytes = [];
        FindBlockingBytes(map, new Queue<(int X, int Y)>(fallingBytes), (0, 0), (mapDim - 1, mapDim - 1), [], blockingBytes);

        var lastByte = blockingBytes
            .Distinct()
            .Select(@byte => new { @byte.X, @byte.Y, Tile = map[@byte.X][@byte.Y] })
            .OrderByDescending(x => x.Tile.Value)
            .First();

        return $"{lastByte.X},{lastByte.Y}";
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
                if (upBytes.Contains((i, j)))
                    return new Tile(TileType.Falling, upBytes.IndexOf((i, j)));

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

    static void FindBlockingBytes(
        Tile[][] map,
        Queue<(int X, int Y)> fallingBytes,
        (int X, int Y) currentPos,
        (int X, int Y) end,
        List<(int, int)> currentRoute,
		List<(int, int)> totalBlockingBytes,
		(int, int)? currentBlockingByte = null)
    {
        if (map[currentPos.X][currentPos.Y].Type is TileType.Corrupt)
            return;

        if (currentRoute.Contains(currentPos))
            return;
        else
            currentRoute.Add(currentPos);

		if (map[currentPos.X][currentPos.Y].Type is TileType.Falling)
		{
			if (totalBlockingBytes.Contains(currentPos))
				return;

			var nextBlock = fallingBytes.Peek();

			if (nextBlock == currentPos)
            {
                currentBlockingByte = nextBlock;
                fallingBytes.Dequeue();
            }
        }
        else if (currentPos == end && currentBlockingByte is not null)
        {
            totalBlockingBytes.Add(currentBlockingByte.Value);
            return;
        }

        var nextPos = (currentPos.X + 1, currentPos.Y);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindBlockingBytes(map, fallingBytes, nextPos, end, new List<(int, int)>(currentRoute), totalBlockingBytes, currentBlockingByte);

        nextPos = (currentPos.X, currentPos.Y + 1);
		if (ArrayExtensions.IsValidTile(map, nextPos))
			FindBlockingBytes(map, fallingBytes, nextPos, end, new List<(int, int)>(currentRoute), totalBlockingBytes, currentBlockingByte);

		nextPos = (currentPos.X - 1, currentPos.Y);
		if (ArrayExtensions.IsValidTile(map, nextPos))
			FindBlockingBytes(map, fallingBytes, nextPos, end, new List<(int, int)>(currentRoute), totalBlockingBytes, currentBlockingByte);

		nextPos = (currentPos.X, currentPos.Y - 1);
		if (ArrayExtensions.IsValidTile(map, nextPos))
			FindBlockingBytes(map, fallingBytes, nextPos, end, new List<(int, int)>(currentRoute), totalBlockingBytes, currentBlockingByte);
	}

    class Tile(TileType type, int? value = null)
    {
        public TileType Type = type;
        public int? Value =  value;
    }

    enum TileType
    {
        Free,
        Corrupt,
        Falling
    }
}