using NUnit.Framework;
using Utility.Attributes;
using Utility.Enums;
using Utility.Extensions;

namespace Day20;
public class Solution
{
    List<Direction> possibleDirections = [Direction.North, Direction.South, Direction.West, Direction.East];

    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt", 2, 38);
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt", 2, 100);
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart1("Inputs/example.txt", 20, 50);
        Assert.That(result, Is.EqualTo(285));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart1("Inputs/input.txt", 20, 100);
        Console.WriteLine(result);
    }
    int SolvePart1(string inputPath, int picosPerCheat, int minPicoseconds)
    {
        var map = File.ReadLines(inputPath)
            .Select(x => x.ToCharArray())
            .ToArray();

        var route = GetRaceRoute(map);

        return CountCheats(map, route, picosPerCheat, minPicoseconds);
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    List<(int X, int Y)> GetRaceRoute(char[][] map)
    {
        (int X, int Y)? start = null, end = null;

        for (int i = 0; i < map.Length; i++)
            for (int j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] is 'S')
                    start = (i, j);

                if (map[i][j] is 'E')
                    end = (i, j);
            }

        List<(int X, int Y)> route = [];
        var currentTile = start!.Value;
        var vectors = possibleDirections.Select(x => x.GetAttribute<VectorAttribute>()).Select(x => (x.Di, x.Dj));

        while (currentTile != end)
        {
            route.Add(currentTile);
            foreach (var (dx, dy) in vectors)
            {
                var nextTile = (currentTile.X + dx, currentTile.Y + dy);
                if (IsValidMove(map, route, nextTile))
                {
                    map[currentTile.X][currentTile.Y] = '#';
                    currentTile = nextTile;
                    break;
                }
            }
        }
        route.Add(end!.Value);
        return route;
    }

    bool IsValidMove(char[][] map, List<(int, int)> route, (int X, int Y) nextTile)
    {
        return map[nextTile.X][nextTile.Y] != '#';
    }

    int CountCheats(char[][] map, List<(int X, int Y)> route, int picosPerCheat, int minPicoseconds)
    {
        var count = 0;
        var i = 0;

        foreach (var (X, Y) in route[..^(minPicoseconds + 2)])
        {
            count += route[(minPicoseconds + i + 2)..]
                .Where(tile => (Math.Abs(X - tile.X) + Math.Abs(Y - tile.Y)) <= picosPerCheat)
                .Count();

            i++;
        }

        return count;
    }
}
