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
    public void Part1_Example2()
    {
        var result = SolvePart1("Inputs/example2.txt");
        Assert.That(result, Is.EqualTo(11048));
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
        Assert.That(result, Is.EqualTo(45));
    }

    [Test]
    public void Part2_Example2()
    {
        var result = SolvePart2("Inputs/example2.txt");
        Assert.That(result, Is.EqualTo(64));
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
        Point end = new(0, 0);

        var map = File.ReadAllLines(inputPath)
            .Select((x, i) =>
            {
                if (x.Contains('S'))
                    start = new Point(i, x.IndexOf('S'));
                if (x.Contains('E'))
                    end = new Point(i, x.IndexOf('E'));

                return x.ToArray();
            })
            .ToArray();

        var memo = new Dictionary<(Point, Direction?), (int Score, List<Point>? Traversed)>();
        var traversed = new List<Point>();
        var startMove = new Move(start);

        PerformMovement(map, startMove, end, 0, traversed, memo);

        return memo[(end, null)].Score;
    }

    int SolvePart2(string inputPath)
    {
        Point start = new(0, 0);
        Point end = new(0, 0);

        var map = File.ReadAllLines(inputPath)
            .Select((x, i) =>
            {
                if (x.Contains('S'))
                    start = new Point(i, x.IndexOf('S'));
                if (x.Contains('E'))
                    end = new Point(i, x.IndexOf('E'));

                return x.ToArray();
            })
            .ToArray();

        var memo = new Dictionary<(Point, Direction?), (int Score, List<Point>? Traversed)>();
        var traversed = new List<Point>();
        var startMove = new Move(start);

        PerformMovement(map, startMove, end, 0, traversed, memo);

        return memo[(end, null)].Traversed!.Count;

    }
    static void PerformMovement(char[][] map, Move lastMove, Point endTile, int currentScore, List<Point> traversed, Dictionary<(Point, Direction?), (int Score, List<Point>? Traversed)> memo)
    {
        var availableMoves = GetAvailableMoves(map, lastMove).ToList();

        traversed.Add(lastMove.Tile);

        if (IsFinalMove(map, availableMoves, lastMove, traversed, memo, currentScore))
            return;

        while (availableMoves.Count is 1)
        {
            var nextMove = availableMoves.First();

            currentScore = IncrementScore(currentScore, lastMove.Direction, nextMove.Direction);
            availableMoves = GetAvailableMoves(map, nextMove).ToList();
            traversed.Add(nextMove.Tile);

            if (IsFinalMove(map, availableMoves, nextMove, traversed, memo, currentScore))
                return;

            lastMove = nextMove;
        }

        if ((memo.TryGetValue((lastMove.Tile, lastMove.Direction), out var currentMin) && currentMin.Score < currentScore)
            || (memo.TryGetValue((endTile, null), out currentMin) && currentMin.Score <= currentScore)
            || BetterScoreInAnyDirection(memo, lastMove.Tile, currentScore))
            return;
        else
        {
            memo[(lastMove.Tile, lastMove.Direction)] = (currentScore, null);
        }

        foreach (var move in availableMoves)
        {
            var newScore = IncrementScore(currentScore, lastMove.Direction, move.Direction);
            PerformMovement(map, move, endTile, newScore, [.. traversed], memo);
        }
        return;
    }

    static bool BetterScoreInAnyDirection(Dictionary<(Point, Direction?), (int Score, List<Point>? Traversed)> memo, Point from, int currentScore)
    {
        if (memo.TryGetValue((from, Direction.North), out var currentMin) && currentMin.Score < currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.East), out currentMin) && currentMin.Score < currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.West), out currentMin) && currentMin.Score < currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.South), out currentMin) && currentMin.Score < currentScore - 1000)
            return true;


        return false;
    }

    static IEnumerable<Move> GetAvailableMoves(char[][] map, Move lastMove)
    {
        if (IsValidMove(map, lastMove.Tile.X - 1, lastMove.Tile.Y)
            && lastMove.Direction is not Direction.South)
            yield return new Move(new Point(lastMove.Tile.X - 1, lastMove.Tile.Y), Direction.North);

        if (IsValidMove(map, lastMove.Tile.X, lastMove.Tile.Y + 1)
            && lastMove.Direction is not Direction.West)
            yield return new Move(new Point(lastMove.Tile.X, lastMove.Tile.Y + 1), Direction.East);

        if (IsValidMove(map, lastMove.Tile.X + 1, lastMove.Tile.Y)
            && lastMove.Direction is not Direction.North)
            yield return new Move(new Point(lastMove.Tile.X + 1, lastMove.Tile.Y), Direction.South);

        if (IsValidMove(map, lastMove.Tile.X, lastMove.Tile.Y - 1)
            && lastMove.Direction is not Direction.East)
            yield return new Move(new Point(lastMove.Tile.X, lastMove.Tile.Y - 1), Direction.West);
    }

    static bool IsValidMove(char[][] map, int x, int y)
    {
        return ArrayExtensions.IsValidTile(map, x, y) && map[x][y] is not '#';
    }

    static int IncrementScore(int score, Direction? lastDirection, Direction? nextDirection)
    {
        return lastDirection == nextDirection
                ? score + 1
                : score + 1001;
    }

    static bool IsFinalMove(char[][] map, List<Move> availableMoves, Move lastMove, List<Point> traversed, Dictionary<(Point, Direction?), (int Score, List<Point>? Traversed)> memo, int currentScore)
    {
        if (availableMoves.SingleOrDefault(x => map[x.Tile.X][x.Tile.Y] is 'E') is Move finalMove)
        {
            var totalScore = IncrementScore(currentScore, lastMove.Direction, finalMove.Direction);
            traversed.Add(finalMove.Tile);

            if (!memo.TryGetValue((finalMove.Tile, null), out var currentMin) || currentMin.Score > totalScore)
                memo[(finalMove.Tile, null)] = (totalScore, traversed);
            else if (memo.TryGetValue((finalMove.Tile, null), out currentMin) && currentMin.Score == totalScore)
                memo[(finalMove.Tile, null)] = (totalScore, memo[(finalMove.Tile, null)].Traversed!.Union(traversed).ToList());

            return true;
        }

        return false;
    }

    class Move(Point tile, Direction? direction = null)
    {
        public Point Tile = tile;
        public Direction? Direction = direction;
    }
}
