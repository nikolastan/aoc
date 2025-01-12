using NUnit.Framework;

namespace Day14;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt", (7, 11));
		Assert.That(result, Is.EqualTo(12));
	}

	[Test]
	public void Part1_Input()
	{
		var result = SolvePart1("Inputs/input.txt", (103, 101));
		Console.WriteLine(result);
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt", (103, 101));
		Console.WriteLine(result);
	}

	int SolvePart1(string inputPath, (int X, int Y) mapSize)
	{
		var robotConfigs = ReadRobotConfigs(inputPath);

		for(int i = 0; i < 100; i++)
		{
			for (int j = 0; j < robotConfigs.Count; j++)
			{
				var xVal = (robotConfigs[j].posX + robotConfigs[j].velocityX) % mapSize.X;
				var yVal = (robotConfigs[j].posY + robotConfigs[j].velocityY) % mapSize.Y;

				robotConfigs[j].posX = xVal < 0 ? mapSize.X + xVal : xVal;
				robotConfigs[j].posY = yVal < 0 ? mapSize.Y + yVal : yVal;
			}
		}

		return robotConfigs
			.Select(config =>
			{
				var quadrant = (config.posX, config.posY) switch
				{
					(int X, int Y) when X < mapSize.X / 2 && Y < mapSize.Y / 2 => 1,
					(int X, int Y) when X > mapSize.X / 2 && Y < mapSize.Y / 2 => 2,
					(int X, int Y) when X < mapSize.X / 2 && Y > mapSize.Y / 2 => 3,
					(int X, int Y) when X > mapSize.X / 2 && Y > mapSize.Y / 2 => 4,
					_ => -1,
				};
				return new { Config = config, Quadrant = quadrant };
			})
			.GroupBy(x => x.Quadrant)
			.Where(x => x.Key != -1)
			.Select(x => x.Count())
			.Aggregate((x, y) => x * y);
	}
	
	int SolvePart2(string inputPath, (int X, int Y) mapSize)
	{
		var robotConfigs = ReadRobotConfigs(inputPath);

		for (int i = 0; i < 10500; i++)
		{
			for (int j = 0; j < robotConfigs.Count; j++)
			{
				var xVal = (robotConfigs[j].posX + robotConfigs[j].velocityX) % mapSize.X;
				var yVal = (robotConfigs[j].posY + robotConfigs[j].velocityY) % mapSize.Y;

				robotConfigs[j].posX = xVal < 0 ? mapSize.X + xVal : xVal;
				robotConfigs[j].posY = yVal < 0 ? mapSize.Y + yVal : yVal;
			}

			if (TryPrintTree(mapSize, robotConfigs.Select(x => (x.posX, x.posY)).ToList(), IsTree))
				return i + 1;
		}

		return 0;
	}

	static List<Robot> ReadRobotConfigs(string inputPath)
	{
		return File.ReadAllLines(inputPath)
			.Select(x => x.Split(' '))
			.Select(x => x.Select(y => y[2..]).ToList())
			.Select(x =>
			{
				var pos = x[0].Split(',');
				var vel = x[1].Split(',');

				return new Robot(int.Parse(pos[1]), int.Parse(pos[0]), int.Parse(vel[1]), int.Parse(vel[0]));
			})
			.ToList();
	}

	static bool TryPrintTree((int X, int Y) mapSize, List<(int X, int Y)> robotCoordinates, Func<(int X, int Y), List<(int X, int Y)>, bool> filter)
	{
		if (!filter.Invoke(mapSize, robotCoordinates))
			return false;

		for (int i = 0; i < mapSize.X; i++)
		{
			for(int j = 0; j < mapSize.Y; j++)
			{
				var count = robotCoordinates.Count(x => x == (i, j));
				if (count > 0)
					TestContext.Out.Write(count);
				else
					TestContext.Out.Write('.');
			}

			TestContext.Out.Write('\n');
		}

		return true;
	}

	bool IsTree((int X, int Y) mapSize, List<(int X, int Y)> coordinates)
	{
		return coordinates.Any(x => HasDiagonalNeighbours(x, coordinates, 10));
	}

	static bool HasDiagonalNeighbours((int X, int Y) robot, List<(int X, int Y)> robotCoordinates, int num)
	{
		var neighbourCoordinates = new List<(int X, int Y)>();

		for(int i = 1; i <= num; i++)
		{
			neighbourCoordinates.Add((robot.X - i, robot.Y + i));
		}

		return neighbourCoordinates.All(robotCoordinates.Contains);
	}

	class Robot(int startX, int startY, int vX, int vY)
	{
		public int posX = startX;
		public int posY = startY;
		public int velocityX = vX;
		public int velocityY = vY;
	}
}

