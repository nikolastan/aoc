using NUnit.Framework;
using Utility.Extensions;

namespace Day18;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt", 6);
        Assert.That(result, Is.EqualTo(22));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt", 70);
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }
    int SolvePart1(string inputPath, int mapDim)
    {
        var map = ReadMap(inputPath, mapDim);
        Dictionary<(int, int), int> memo = [];
        FindMinSteps(map, (0, 0), (mapDim - 1, mapDim - 1), 0, memo);

        return memo[(mapDim - 1, mapDim - 1)];
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static char[][] ReadMap(string inputPath, int mapDim)
    {
        var byteCoordinates = File.ReadAllLines(inputPath)
            .Select(x => x.Split(','))
            .Select(x => (int.Parse(x[0]), int.Parse(x[1])))
            .ToList();

        return Enumerable.Range(0, mapDim)
            .Select(x => new char[mapDim])
            .Select((row, i) => row.Select((col, j) => byteCoordinates.Contains((i, j))
                    ? '#'
                    : '.')
                .ToArray())
            .ToArray();
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

        if (currentPos == end)
        {
            if (memo.TryGetValue(end, out var currentMinSteps) && currentMinSteps > currentSteps)
                memo[end] = currentSteps;

            return;
        }

        if (memo.TryGetValue(currentPos, out var currentMin) && currentSteps > currentMin)
            return;
        else
            memo[currentPos] = currentSteps;

        if (ArrayExtensions.IsValidTile(map, currentPos.X + 1, currentPos.Y))
            FindMinSteps(map, (currentPos.X + 1, currentPos.Y), end, ++currentSteps, memo);

        if (ArrayExtensions.IsValidTile(map, currentPos.X, currentPos.Y + 1))
            FindMinSteps(map, (currentPos.X, currentPos.Y + 1), end, ++currentSteps, memo);

        if (ArrayExtensions.IsValidTile(map, currentPos.X - 1, currentPos.Y))
            FindMinSteps(map, (currentPos.X - 1, currentPos.Y), end, ++currentSteps, memo);

        if (ArrayExtensions.IsValidTile(map, currentPos.X, currentPos.Y - 1))
            FindMinSteps(map, (currentPos.X, currentPos.Y - 1), end, ++currentSteps, memo);
    }
}
