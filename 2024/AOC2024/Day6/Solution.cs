using NUnit.Framework;
using System.Diagnostics;
using Utility.Enums;
using Utility.Extensions;

namespace Day6;
public partial class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(41));
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
        Assert.That(result, Is.EqualTo(6));
    }

    [Test]
    public void Part2_Example2()
    {
        var result = SolvePart2("Inputs/example2.txt");
        Assert.That(result, Is.EqualTo(19));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
	}

	[Test]
	public void Part2v2_Example2()
	{
		var result = SolvePart2V2("Inputs/example2.txt");
		Assert.That(result, Is.EqualTo(19));
	}

	[Test]
	public void Part2v2_Input()
	{
		var result = SolvePart2V2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	int SolvePart1(string inputPath)
    {
        var grid = ReadGrid(inputPath, out (int, int) guardPos);
        var result = 0;

        var currentDirection = Direction.North;

        while (IsValidTile(grid, guardPos))
        {
            if (!grid[guardPos.Item1][guardPos.Item2].Traversed)
            {
                grid[guardPos.Item1][guardPos.Item2].Traversed = true;
                result++;
            }

            var inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);

            if (IsValidTile(grid, inFrontOfGuard))
            {
                if (grid[inFrontOfGuard.Item1][inFrontOfGuard.Item2].Char is '#')
                    currentDirection = CalculateNextDirection(currentDirection);

                guardPos = CalculateNextMove(guardPos, currentDirection);
            }
            else
                guardPos = inFrontOfGuard;
        }

        return result;
    }

    List<(int, int)> GetAllTraversedTiles(Tile[][] grid, (int, int) startPos, Direction startDirection)
    {
        var guardPos = startPos;
        var currentDirection = startDirection;

        var result = new List<(int, int)>();

        while (IsValidTile(grid, guardPos))
        {
            if (!grid[guardPos.Item1][guardPos.Item2].Traversed)
            {
                grid[guardPos.Item1][guardPos.Item2].Traversed = true;
                result.Add(guardPos);
            }

            var inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);

            if (IsValidTile(grid, inFrontOfGuard))
            {
                while (grid[inFrontOfGuard.Item1][inFrontOfGuard.Item2].Char is '#')
                {
                    currentDirection = CalculateNextDirection(currentDirection);
                    inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);
                }

                guardPos = CalculateNextMove(guardPos, currentDirection);
            }
            else
                guardPos = inFrontOfGuard;
        }

        return result;
    }

    int SolvePart2(string inputPath)
    {
        //Bugged? But can't find edge case...
        var grid = ReadGrid(inputPath, out (int, int) guardPos);
        var obstacles = new List<(int, int)>();

        var startPos = guardPos;
        var startDirection = Direction.North;

        var currentDirection = startDirection;

        while (IsValidTile(grid, guardPos))
        {
            var inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);

            grid[guardPos.Item1][guardPos.Item2].Traversed = true;

            if (IsValidTile(grid, inFrontOfGuard))
            {
                var currentTile = grid[inFrontOfGuard.Item1][inFrontOfGuard.Item2];

                if (inFrontOfGuard != startPos
                    //&& !currentTile.Traversed
                    && currentTile.Char is not '#'
                    && IsLoop(ArrayExtensions.DeepCopy(grid), startPos, inFrontOfGuard, startDirection)
                    && !obstacles.Contains(inFrontOfGuard))
                {
                    obstacles.Add(inFrontOfGuard);
                }

                while (grid[inFrontOfGuard.Item1][inFrontOfGuard.Item2].Char is '#')
                {
                    currentDirection = CalculateNextDirection(currentDirection);

                    inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);
                }

                guardPos = CalculateNextMove(guardPos, currentDirection);
            }
            else
                break;
        }

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (obstacles.Contains((i, j)))
                    Debug.Write('O');
                else
                    Debug.Write(grid[i][j].Char);
            }

            Debug.Write('\n');
        }


        return obstacles.Count;
    }

    int SolvePart2V2(string inputPath)
    {
        var grid = ReadGrid(inputPath, out (int, int) guardPos);
        var obstacles = new List<(int, int)>();

        var startPos = guardPos;
        var startDirection = Direction.North;

        var currentDirection = startDirection;

        var traversedTiles = GetAllTraversedTiles(grid, startPos, startDirection);

        var result = traversedTiles
            .Where(tilePos => IsLoop(ArrayExtensions.DeepCopy(grid), startPos, tilePos, startDirection))
            .Count();

        return result;
	}

    static Tile[][] ReadGrid(string inputPath, out (int, int) start)
    {
        (int, int) current = (0, 0);
        var grid = File.ReadAllLines(inputPath)
            .Select((x, i) =>
            {

                if (x.Contains('^'))
                    current = (i, x.IndexOf('^'));

                return x.Select(c => new Tile(c)).ToArray();
            })
            .ToArray();

        start = current;
        return grid;
    }

    static Direction CalculateNextDirection(Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new InvalidOperationException("Only NESW directions are possible!")
        };
    }

    static (int, int) CalculateNextMove((int, int) currentPos, Direction direction)
    {
        return direction switch
        {
            Direction.North => (currentPos.Item1 - 1, currentPos.Item2),
            Direction.South => (currentPos.Item1 + 1, currentPos.Item2),
            Direction.East => (currentPos.Item1, currentPos.Item2 + 1),
            Direction.West => (currentPos.Item1, currentPos.Item2 - 1),
            _ => throw new InvalidOperationException("Only NESW directions are possible!")
        };
    }

    static bool IsValidTile(Tile[][] grid, (int, int) move)
    {
        return move.Item1 >= 0
            && move.Item1 < grid.Length
            && move.Item2 >= 0
            && move.Item2 < grid[0].Length;
    }

    static bool IsLoop(Tile[][] grid, (int, int) currentPos, (int, int) potentialObstaclePos, Direction beamDirection)
    {
        //We set potential obstacle before checking for loop
        grid[potentialObstaclePos.Item1][potentialObstaclePos.Item2].Char = '#';

        var traversedObstacles = new List<(int, int, Direction)>();

        while (IsValidTile(grid, currentPos))
        {
            var inFrontOfBeam = CalculateNextMove(currentPos, beamDirection);

            if (!IsValidTile(grid, inFrontOfBeam))
                return false;

            while (grid[inFrontOfBeam.Item1][inFrontOfBeam.Item2].Char is '#')
            {
                if (traversedObstacles.Contains((inFrontOfBeam.Item1, inFrontOfBeam.Item2, beamDirection)))
                    return true;

                traversedObstacles.Add((inFrontOfBeam.Item1, inFrontOfBeam.Item2, beamDirection));

                beamDirection = CalculateNextDirection(beamDirection);

                inFrontOfBeam = CalculateNextMove(currentPos, beamDirection);
            }

            currentPos = CalculateNextMove(currentPos, beamDirection);
        }

        return false;
    }

    struct Tile(char c)
    {
        public char Char = c;
        public bool Traversed = false;
    }
}

