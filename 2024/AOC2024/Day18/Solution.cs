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
        FindBlockingBytes(map, (0, 0), (mapDim - 1, mapDim - 1), [], [], blockingBytes);

        (int X, int Y)? lastByte = null;
        foreach (var @byte in fallingBytes)
        {
            if (blockingBytes.Contains(@byte))
                lastByte = @byte;
            else
                break;
        }

        if (lastByte is null)
            throw new InvalidOperationException("Byte that prevents exit was not found!");

        return $"{lastByte.Value.X},{lastByte.Value.Y}";
    }

    static char[][] ReadMap(string inputPath, int mapDim, int numOfBytesFallen)
    {
        var parsedBytes = ParseByteCoordinates(File.ReadAllLines(inputPath));

        var fallenBytes = parsedBytes
            .Take(numOfBytesFallen)
            .ToList();

        var upBytes = parsedBytes
            .Skip(numOfBytesFallen)
            .ToList();

        return Enumerable.Range(0, mapDim)
            .Select(x => new char[mapDim])
            .Select((row, i) => row.Select((_, j) =>
            {
                if (fallenBytes.Contains((i, j)))
                    return '#';
                if (upBytes.Contains((i, j)))
                    return '?';

                return '.';
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
        char[][] map,
        (int X, int Y) currentPos,
        (int X, int Y) end,
        int currentSteps,
        Dictionary<(int X, int Y), int> memo)
    {
        if (map[currentPos.X][currentPos.Y] is '#')
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
        char[][] map,
        (int X, int Y) currentPos,
        (int X, int Y) end,
        List<(int, int)> currentRoute,
        List<(int, int)> currentBlockingBytes,
        List<(int, int)> totalBlockingBytes)
    {
        if (map[currentPos.X][currentPos.Y] is '#')
            return;

        if (currentRoute.Contains(currentPos))
            return;
        else
            currentRoute.Add(currentPos);

        if (map[currentPos.X][currentPos.Y] is '?' && !currentBlockingBytes.Contains(currentPos))
        {
            currentBlockingBytes.Add(currentPos);
        }

        if (currentPos == end)
        {
            totalBlockingBytes.AddRange(currentBlockingBytes);
            return;
        }

        var nextPos = (currentPos.X + 1, currentPos.Y);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindBlockingBytes(map, nextPos, end,
                new List<(int, int)>(currentRoute),
                new List<(int, int)>(currentBlockingBytes), totalBlockingBytes);

        nextPos = (currentPos.X, currentPos.Y + 1);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindBlockingBytes(map, nextPos, end,
                new List<(int, int)>(currentRoute),
                new List<(int, int)>(currentBlockingBytes),
                totalBlockingBytes);

        nextPos = (currentPos.X - 1, currentPos.Y);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindBlockingBytes(map, nextPos, end,
                new List<(int, int)>(currentRoute),
                new List<(int, int)>(currentBlockingBytes),
                totalBlockingBytes);

        nextPos = (currentPos.X, currentPos.Y - 1);
        if (ArrayExtensions.IsValidTile(map, nextPos))
            FindBlockingBytes(map, nextPos, end,
                new List<(int, int)>(currentRoute),
                new List<(int, int)>(currentBlockingBytes),
                totalBlockingBytes);
    }
}