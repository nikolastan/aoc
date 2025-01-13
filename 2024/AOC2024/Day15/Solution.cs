using NUnit.Framework;
using System.IO;
using Utility.Attributes;
using Utility.Enums;
using Utility.Extensions;

namespace Day15;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(10092));
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
		var (map, moves) = ReadInput(inputPath);

		var objects = ReadObjects(map);

		PerformAllMovements(ref objects, moves);

		return objects
			.Where(x => x.Type is ObjectType.Box)
			.Select(box => box.X * 100 + box.Y)
			.Sum();
	}

	int SolvePart2(string inputPath)
	{
		return 0;
	}

	static (char[][] Map, List<char> Moves) ReadInput(string inputPath)
	{
		var input = File.ReadAllLines(inputPath).ToList();

		var map = input[..input.IndexOf("")]
			.Select(x => x.ToArray())
			.ToArray();

		var moves = input[(input.IndexOf("") + 1)..]
			.SelectMany(x => x)
			.ToList();

		return (map, moves);
	}

	static List<Object> ReadObjects(char[][] map)
	{
		return map.SelectMany((row, i) =>
				row.Select((c, j) =>
				{
					var type = c switch
					{
						'#' => ObjectType.Wall,
						'O' => ObjectType.Box,
						'@' => ObjectType.Robot,
						'.' => (ObjectType)0,
						_ => throw new InvalidOperationException()
					};

					if (type is 0)
						return null;

					return new Object(i, j, type);
				}))
			.Where(x => x is not null)
			.ToList()!;
	}

	static void PerformAllMovements(ref List<Object> coordinates, List<char> robotMoves)
	{
		var robot = coordinates.Single(x => x.Type is ObjectType.Robot);

		while(robotMoves.Count > 0)
		{
			var nextMove = robotMoves.FirstOrDefault();
			robotMoves.Remove(nextMove);

			var direction = nextMove switch
			{
				'^' => Direction.North,
				'>' => Direction.East,
				'v' => Direction.South,
				'<' => Direction.West,
				_ => throw new InvalidOperationException(),
			};

			var att = direction.GetAttribute<VectorAttribute>();

			Move(ref coordinates, robot, (att.Di, att.Dj));
		}
	}

	static void Move(ref List<Object> allCoordinates, Object robot, (int dx, int dy) directionVector)
	{
		var inFrontOfRobot = allCoordinates.SingleOrDefault(
			pos => pos.X == robot.X + directionVector.dx
				&& pos.Y == robot.Y + directionVector.dy);

		if (inFrontOfRobot?.Type is ObjectType.Wall)
		{
			return;
		}

		if(inFrontOfRobot?.Type is ObjectType.Box)
		{
			var movables = new List<Object>() { inFrontOfRobot };

			var inFrontOfBox = allCoordinates.SingleOrDefault(
				pos => pos.X == inFrontOfRobot.X + directionVector.dx
					&& pos.Y == inFrontOfRobot.Y + directionVector.dy);

			while(inFrontOfBox?.Type is ObjectType.Box)
			{
				movables.Add(inFrontOfBox);
				inFrontOfBox = allCoordinates.SingleOrDefault(
				pos => pos.X == inFrontOfBox.X + directionVector.dx
					&& pos.Y == inFrontOfBox.Y + directionVector.dy);
			}

			if (inFrontOfBox?.Type is ObjectType.Wall)
				return;
			else
			{
				for(int i = 0; i < movables.Count; i++)
				{
					movables[i].X += directionVector.dx;
					movables[i].Y += directionVector.dy;
				}
			}	
		}

		robot.X += directionVector.dx;
		robot.Y += directionVector.dy;

		return;
	}

	class Object(int startX, int startY, ObjectType type)
	{
		public int X = startX;
		public int Y = startY;
		public ObjectType Type = type;
	}

	enum ObjectType
	{
		Wall = 1,
		Box,
		Robot
	}
}

