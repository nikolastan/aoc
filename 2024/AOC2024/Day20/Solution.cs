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
        var result = SolvePart1("Inputs/example.txt", 38);
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1("Inputs/input.txt", 100);
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
    int SolvePart1(string inputPath, int minPicoseconds)
    {
        var map = File.ReadLines(inputPath)
            .Select(x => x.ToCharArray())
            .ToArray();

        var route = GetRaceRoute(map);

        return CountCheats(map, route, minPicoseconds);
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

        while (currentTile != end)
        {
            route.Add(currentTile);

            foreach (var (dx, dy) in possibleDirections.Select(x => x.GetAttribute<VectorAttribute>()).Select(x => (x.Di, x.Dj)))
            {
                var nextTile = (currentTile.X + dx, currentTile.Y + dy);

                if (IsValidMove(map, route, nextTile))
                {
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
        return ArrayExtensions.IsValidTile(map, nextTile)
            && !route.Contains(nextTile)
            && map[nextTile.X][nextTile.Y] != '#';
    }

    int CountCheats(char[][] map, List<(int X, int Y)> route, int minPicoseconds)
    {
        var count = 0;
        var i = 0;
        foreach (var (X, Y) in route)
        {
            var nearbyTiles = route
                .Select((tile, i) => new { tile, i })
                .Where(x => (Math.Abs(X - x.tile.X) < 3 && Y == x.tile.Y)
                    || (Math.Abs(Y - x.tile.Y) < 3 && X == x.tile.X));

            count += nearbyTiles.Select(x => Math.Abs(x.i - i) - 2).Where(x => x >= minPicoseconds).Count();
            i++;
        }

        return count / 2;
    }
}
