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
                if(x.Contains('E'))
                    end = new Point(i, x.IndexOf('E'));

                return x.ToArray();
            })
            .ToArray();

        var memo = new Dictionary<(Point, Direction), int>();

        var startMove = new Move(start);

        PerformMovement(map, startMove, 0, memo);

        return memo[(end, 0)]; 
    }

    static void PerformMovement(char[][] map, Move lastMove, int currentScore, Dictionary<(Point, Direction), int> memo)
    {
        var availableMoves = GetAvailableMoves(map, lastMove.Tile)
            .Where(move => move.Tile != lastMove.Tile);

        if (!availableMoves.Any())
        {
            return;
        }

        var finalMove = availableMoves.SingleOrDefault(x => map[x.Tile.X][x.Tile.Y] is 'E');
        if (finalMove is not null)
        {
            var totalScore = currentScore += finalMove.Direction == lastMove.Direction
                ? 1
                : 1001;

            if(!memo.TryGetValue((finalMove.Tile,0), out var score) || score > totalScore)
                memo[(finalMove.Tile, 0)] = totalScore;

            return;
        }

        foreach(var move in availableMoves)
        {
            var newScore = move.Direction == lastMove.Direction
                ? currentScore + 1
                : currentScore + 1001;

            if (memo.TryGetValue((move.Tile, move.Direction!.Value), out var score) && score < newScore)
                return;
            else
                memo[(move.Tile, move.Direction!.Value)] = newScore;


            PerformMovement(map, move, newScore, memo);
		}
        return;
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
