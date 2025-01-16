using NUnit.Framework;
using System.Drawing;
using Utility.Enums;
using Utility.Extensions;

namespace Day16;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(7036));
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
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    int SolvePart1(string inputPath)
    {
        Point start = new(0, 0);

        var map = File.ReadAllLines(inputPath)
            .Select((x, i) =>
            {
                if (x.Contains('S'))
                    start = new Point(i, x.IndexOf('S'));

                return x.ToArray();
            })
            .ToArray();

        var memo = new Dictionary<Point, int>();

        var startMove = new Move(start);
        return PerformMovement(map, [startMove], 0, memo) - 1;
    }

    static int PerformMovement(char[][] map, List<Move> performedMoves, int currentScore, Dictionary<Point, int> memo)
    {
        var lastMove = performedMoves.Last();

        if (memo.TryGetValue(lastMove.Tile, out var score) && score < currentScore)
        {
            return score;
        }

        var availableMoves = GetAvailableMoves(map, lastMove.Tile)
            .Where(x => !performedMoves.Select(x => x.Tile).Contains(x.Tile));

        if (availableMoves.Count() is 0)
            return int.MaxValue;

        var finalMove = availableMoves.SingleOrDefault(x => map[x.Tile.X][x.Tile.Y] is 'E');

        if (finalMove is not null)
        {
            var totalScore = currentScore += finalMove.Direction != lastMove.Direction
                ? 1001
                : 1;

            if (!memo.TryGetValue(lastMove.Tile, out score) || totalScore < score)
                memo[finalMove.Tile] = totalScore;

            return totalScore;
        }

        return availableMoves
            .Select(x => x.Direction == lastMove.Direction
                ? PerformMovement(map, [.. performedMoves, x], ++currentScore, memo)
                : PerformMovement(map, [.. performedMoves, x], currentScore + 1001, memo))
            .Min();
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static IEnumerable<Move> GetAvailableMoves(char[][] map, Point currentPos)
    {
        if (ArrayExtensions.IsValidTile(map, currentPos.X - 1, currentPos.Y) && map[currentPos.X - 1][currentPos.Y] is not '#')
            yield return new Move(new Point(currentPos.X - 1, currentPos.Y), Direction.North);

        if (ArrayExtensions.IsValidTile(map, currentPos.X, currentPos.Y + 1) && map[currentPos.X][currentPos.Y + 1] is not '#')
            yield return new Move(new Point(currentPos.X, currentPos.Y + 1), Direction.East);

        if (ArrayExtensions.IsValidTile(map, currentPos.X + 1, currentPos.Y) && map[currentPos.X + 1][currentPos.Y] is not '#')
            yield return new Move(new Point(currentPos.X + 1, currentPos.Y), Direction.South);

        if (ArrayExtensions.IsValidTile(map, currentPos.X, currentPos.Y - 1) && map[currentPos.X][currentPos.Y - 1] is not '#')
            yield return new Move(new Point(currentPos.X, currentPos.Y - 1), Direction.West);
    }

    class Move(Point tile, Direction? direction = null)
    {
        public Point Tile = tile;
        public Direction? Direction = direction;
    }
}
