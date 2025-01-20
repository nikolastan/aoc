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

        var memo = new Dictionary<(Point, Direction?), int>();

        var startMove = new Move(start);

        PerformMovement(map, startMove, 0, ref memo);

        return memo[(end, null)];
    }

    static void PerformMovement(char[][] map, Move lastMove, int currentScore, ref Dictionary<(Point, Direction?), int> memo)
    {
        var availableMoves = GetAvailableMoves(map, lastMove).ToList();

        if (IsFinalMove(map, availableMoves, lastMove, ref memo, currentScore))
            return;

        while (availableMoves.Count is 1)
        {
            var nextMove = availableMoves.First();

            currentScore = IncrementScore(currentScore, lastMove.Direction, nextMove.Direction);
            availableMoves = GetAvailableMoves(map, nextMove).ToList();

            if (IsFinalMove(map, availableMoves, lastMove, ref memo, currentScore))
                return;

            lastMove = nextMove;
        }

        if (memo.TryGetValue((lastMove.Tile, lastMove.Direction), out var minScore) && minScore <= currentScore)
            return;
        else
        {
            if (BetterScoreInAnyDirection(ref memo, lastMove.Tile, currentScore))
                return;

            memo[(lastMove.Tile, lastMove.Direction)] = currentScore;
        }

        foreach (var move in availableMoves)
        {
            var newScore = IncrementScore(currentScore, lastMove.Direction, move.Direction);
            PerformMovement(map, move, newScore, ref memo);
        }
        return;
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static bool BetterScoreInAnyDirection(ref Dictionary<(Point, Direction?), int> memo, Point from, int currentScore)
    {
        if (memo.TryGetValue((from, Direction.North), out var minScore) && minScore <= currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.East), out minScore) && minScore <= currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.West), out minScore) && minScore <= currentScore - 1000)
            return true;
        if (memo.TryGetValue((from, Direction.South), out minScore) && minScore <= currentScore - 1000)
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

    static bool IsFinalMove(char[][] map, List<Move> availableMoves, Move lastMove, ref Dictionary<(Point, Direction?), int> memo, int currentScore)
    {
        if (availableMoves.SingleOrDefault(x => map[x.Tile.X][x.Tile.Y] is 'E') is Move finalMove)
        {
            var totalScore = IncrementScore(currentScore, lastMove.Direction, finalMove.Direction);
            if (!memo.TryGetValue((finalMove.Tile, null), out var score) || score > totalScore)
                memo[(finalMove.Tile, null)] = totalScore;

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
