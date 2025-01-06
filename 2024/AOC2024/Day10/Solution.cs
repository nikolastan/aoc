using NUnit.Framework;
using Utility.Extensions;

namespace Day10;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(36));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt");
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(81));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    int SolvePart1(string inputPath)
    {
        var trailheads = new Queue<(int, int)>();

        var map = File.ReadAllLines(inputPath)
            .Select((x, i) =>
                x.Select((c, j) =>
                    {
                        var value = c - '0';
                        if (value is 0)
                            trailheads.Enqueue((i, j));

                        return value;
                    }
                ).ToArray())
            .ToArray();

        var result = 0;

        while (trailheads.Count > 0)
        {
            var trailheadStart = trailheads.Dequeue();
            var tops = FindAllTopsReachableFrom(map, trailheadStart);
            result += tops.Distinct().Count();
        }

        return result;
    }

    int SolvePart2(string inputPath)
    {
        var trailheads = new Queue<(int, int)>();

        var map = File.ReadAllLines(inputPath)
            .Select((x, i) =>
                x.Select((c, j) =>
                {
                    var value = c - '0';
                    if (value is 0)
                        trailheads.Enqueue((i, j));

                    return value;
                }
                ).ToArray())
            .ToArray();

        var result = 0;

        while (trailheads.Count > 0)
        {
            var trailheadStart = trailheads.Dequeue();
            var tops = FindAllTopsReachableFrom(map, trailheadStart);
            result += tops.Count;
        }

        return result;
    }

    IEnumerable<(int, int)> LookAroundForTrails(int[][] map, int x, int y)
    {
        if (ArrayExtensions.IsValidTile(map, x - 1, y) && map[x - 1][y] - 1 == map[x][y])
            yield return (x - 1, y);

        if (ArrayExtensions.IsValidTile(map, x, y - 1) && map[x][y - 1] - 1 == map[x][y])
            yield return (x, y - 1);

        if (ArrayExtensions.IsValidTile(map, x + 1, y) && map[x + 1][y] - 1 == map[x][y])
            yield return (x + 1, y);

        if (ArrayExtensions.IsValidTile(map, x, y + 1) && map[x][y + 1] - 1 == map[x][y])
            yield return (x, y + 1);
    }

    List<(int, int)> FindAllTopsReachableFrom(int[][] map, (int, int) trailheadStart)
    {
        var tops = new List<(int, int)>();
        var trailQueue = new Queue<(int, int)>();

        foreach (var trail in LookAroundForTrails(map, trailheadStart.Item1, trailheadStart.Item2))
            trailQueue.Enqueue(trail);

        while (trailQueue.Count > 0)
        {
            var currentTrail = trailQueue.Dequeue();

            foreach (var trail in LookAroundForTrails(map, currentTrail.Item1, currentTrail.Item2))
            {
                if (map[trail.Item1][trail.Item2] is 9)
                    tops.Add(trail);
                else
                    trailQueue.Enqueue(trail);
            }
        }

        return tops;
    }
}
