using NUnit.Framework;
using Utility.Enums;

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
		var result = SolvePart2("Inputs/example2.txt");
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
		var grid = ReadGrid(inputPath, out (int, int) guardPos);
		var result = 0;

		var currentDirection = Direction.North;

		while (guardPos.Item1 >= 0 
			&& guardPos.Item1 < grid.Length 
			&& guardPos.Item2 >= 0 
			&& guardPos.Item2 < grid[0].Length)
		{
			if (!grid[guardPos.Item1][guardPos.Item2].Traversed)
			{
				grid[guardPos.Item1][guardPos.Item2].Traversed = true;
				result++;
			}

			var inFrontOfGuard = CalculateNextMove(guardPos, currentDirection);

			if(IsValidMove(grid, inFrontOfGuard))
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

	int SolvePart2(string inputPath)
	{
		//Implement with checking for passed obstacle on right side (with vectors)
		return 0;
	}

	static Tile[][] ReadGrid(string inputPath, out (int, int) start)
	{
		(int, int) current = (0, 0);
		var grid = File.ReadAllLines(inputPath)
			.Select((x, i) => {

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

	static bool IsValidMove(Tile[][] grid, (int, int) move)
	{
		return move.Item1 >= 0
			&& move.Item1 < grid.Length
			&& move.Item2 >= 0
			&& move.Item2 < grid[0].Length;
	}

	struct Tile(char c)
	{
		public char Char = c;
		public bool Traversed = false;
	}
}

