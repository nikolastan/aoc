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

        var memo = new Dictionary<Point, int>();

        var startMove = new Move(start);

        PerformMovement(map, startMove, 0, ref memo);

        return memo[end]; 
    }

    static void PerformMovement(char[][] map, Move lastMove, int currentScore, ref Dictionary<Point, int> memo)
    {
        var availableMoves = GetAvailableMoves(map, lastMove).ToList();

        if (availableMoves.Count is 0)
        {
            return;
        }

        var finalMove = availableMoves.SingleOrDefault(x => map[x.Tile.X][x.Tile.Y] is 'E');
        if (finalMove is not null)
        {
            var totalScore = currentScore += finalMove.Direction == lastMove.Direction
                ? 1
                : 1001;

            if(!memo.TryGetValue(finalMove.Tile, out var score) || score > totalScore)
                memo[finalMove.Tile] = totalScore;

            return;
        }

		while (availableMoves.Count is 1)
		{
            var nextMove = availableMoves.First();

            currentScore += nextMove.Direction == lastMove.Direction
                ? 1
                : 1001;

            availableMoves = GetAvailableMoves(map, nextMove).ToList();

            if(availableMoves.Any(x => map[x.Tile.X][x.Tile.Y] is 'E'))
            {
                PerformMovement(map, nextMove, currentScore, ref memo);
                return;
            }

            lastMove = nextMove;
		}

        if(availableMoves.Count > 1)
		{
			if (memo.TryGetValue(lastMove.Tile, out var minScore) && minScore < currentScore)
				return;
			else
				memo[lastMove.Tile] = currentScore;
		}

		foreach (var move in availableMoves)
        {
            var newScore = move.Direction == lastMove.Direction
                ? currentScore + 1
                : currentScore + 1001;

            PerformMovement(map, move, newScore, ref memo);
		}
        return;
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    static IEnumerable<Move> GetAvailableMoves(char[][] map, Move lastMove)
    {
        if (ArrayExtensions.IsValidTile(map, lastMove.Tile.X - 1, lastMove.Tile.Y) 
            && map[lastMove.Tile.X - 1][lastMove.Tile.Y] is not '#'
            && lastMove.Direction is not Direction.South)
            yield return new Move(new Point(lastMove.Tile.X - 1, lastMove.Tile.Y), Direction.North);

        if (ArrayExtensions.IsValidTile(map, lastMove.Tile.X, lastMove.Tile.Y + 1) 
            && map[lastMove.Tile.X][lastMove.Tile.Y + 1] is not '#'
            && lastMove.Direction is not Direction.West)
            yield return new Move(new Point(lastMove.Tile.X, lastMove.Tile.Y + 1), Direction.East);

        if (ArrayExtensions.IsValidTile(map, lastMove.Tile.X + 1, lastMove.Tile.Y) 
            && map[lastMove.Tile.X + 1][lastMove.Tile.Y] is not '#'
            && lastMove.Direction is not Direction.North)
            yield return new Move(new Point(lastMove.Tile.X + 1, lastMove.Tile.Y), Direction.South);

        if (ArrayExtensions.IsValidTile(map, lastMove.Tile.X, lastMove.Tile.Y - 1) 
            && map[lastMove.Tile.X][lastMove.Tile.Y - 1] is not '#'
            && lastMove.Direction is not Direction.East)
            yield return new Move(new Point(lastMove.Tile.X, lastMove.Tile.Y - 1), Direction.West);
    }

    class Move(Point tile, Direction? direction = null)
    {
        public Point Tile = tile;
        public Direction? Direction = direction;
    }
}
