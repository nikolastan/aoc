using NUnit.Framework;
using System.Drawing;
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
        Assert.That(result, Is.EqualTo(9021));
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

        var objects = ReadObjects(map, (1, 1));

        PerformAllMovements(ref objects, moves);

        return objects
            .Where(x => x.Type is ObjectType.Box)
            .Select(box => box.Start.X * 100 + box.Start.Y)
            .Sum();
    }

    int SolvePart2(string inputPath)
    {
        var (map, moves) = ReadInput(inputPath);

        var objects = ReadObjects(map, (1, 2));

        PerformAllMovements(ref objects, moves);

        return objects
            .Where(x => x.Type is ObjectType.Box)
            .Select(box => box.Start.X * 100 + box.Start.Y)
            .Sum();
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

    static List<Object> ReadObjects(char[][] map, (int X, int Y) scale)
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

                    if (type is ObjectType.Robot)
                    {
                        return new Object(
                        (new Point(i * scale.X, j * scale.Y),
                        new Point(i * scale.X, j * scale.Y)),
                        type);
                    }

                    return new Object(
                        (new Point(i * scale.X, j * scale.Y),
                        new Point(i * scale.X + scale.X - 1, j * scale.Y + scale.Y - 1)),
                        type);
                }))
            .Where(x => x is not null)
            .ToList()!;
    }

    static void PerformAllMovements(ref List<Object> coordinates, List<char> robotMoves)
    {
        var robot = coordinates.Single(x => x.Type is ObjectType.Robot);

        while (robotMoves.Count > 0)
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

            Move(coordinates, robot, (att.Di, att.Dj));
        }
    }

    static void Move(List<Object> allCoordinates, Object robot, (int dx, int dy) directionVector)
    {
        var inFrontOfRobot = allCoordinates.SingleOrDefault(tile => AreIntersected(tile, robot, directionVector));

        if (inFrontOfRobot?.Type is ObjectType.Wall)
        {
            return;
        }

        if (inFrontOfRobot?.Type is ObjectType.Box)
        {
            var movables = new List<Object>() { inFrontOfRobot };

            var inFrontOfBox = allCoordinates
                .Where(tile => tile.Start != inFrontOfRobot.Start && tile.End != inFrontOfRobot.End)
                .Where(tile => AreIntersected(tile, inFrontOfRobot, directionVector))
                .ToList();

            while (inFrontOfBox.Count > 0 && inFrontOfBox.All(x => x.Type is ObjectType.Box))
            {
                movables.AddRange(inFrontOfBox);
                inFrontOfBox = allCoordinates
                    .Where(tile => inFrontOfBox.All(x => x.Start != tile.Start || x.End != tile.End))
                    .Where(tile => inFrontOfBox.Any(x => AreIntersected(tile, x, directionVector)))
                    .ToList();
            }

            if (inFrontOfBox.Any(x => x.Type is ObjectType.Wall))
                return;
            else
            {
                for (int i = 0; i < movables.Count; i++)
                {
                    movables[i].Start = new Point(
                        movables[i].Start.X += directionVector.dx,
                        movables[i].Start.Y += directionVector.dy);
                    movables[i].End = new Point(
                        movables[i].End.X += directionVector.dx,
                        movables[i].End.Y += directionVector.dy);
                }
            }
        }

        robot.Start = new Point(
            robot.Start.X += directionVector.dx,
            robot.Start.Y += directionVector.dy);
        robot.End = new Point(
            robot.End.X += directionVector.dx,
            robot.End.Y += directionVector.dy);

        return;
    }

    class Object((Point Start, Point End) coordinates, ObjectType type)
    {
        public Point Start = coordinates.Start;
        public Point End = coordinates.End;
        public ObjectType Type = type;
    }

    static bool AreIntersected(
        Object object1,
        Object object2,
        (int dx, int dy) directionVector)
    {
        return ((object1.Start.X <= object2.Start.X + directionVector.dx
            && object1.End.X >= object2.Start.X + directionVector.dx)
            || (object1.Start.X <= object2.End.X + directionVector.dx
            && object1.End.X >= object2.End.X + directionVector.dx))

            &&

            ((object1.Start.Y <= object2.Start.Y + directionVector.dy
            && object1.End.Y >= object2.Start.Y + directionVector.dy)
            || (object1.Start.Y <= object2.End.Y + directionVector.dy
            && object1.End.Y >= object2.End.Y + directionVector.dy));
    }

    enum ObjectType
    {
        Wall = 1,
        Box,
        Robot
    }
}

