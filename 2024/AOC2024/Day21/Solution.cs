using NUnit.Framework;
using Utility.Extensions;

namespace Day21;
public class Solution
{
    readonly char[,] NumericKeyboard = new char[4, 3]
    {
        {'7', '8', '9' },
        {'4', '5', '6' },
        {'1', '2', '3' },
        {'X', '0', 'A' }
    };

    readonly char[,] DirectionalKeyboard = new char[2, 3]
    {
        {'X', '^', 'A' },
        {'<', 'v', '>' }
    };

    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(126384));
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
        var codes = File.ReadLines(inputPath);
        var result = 0;

        foreach (var code in codes)
        {
            var first = Translate(NumericKeyboard, (3, 2), code);
            var second = Translate(DirectionalKeyboard, (0, 2), first);
            var third = Translate(DirectionalKeyboard, (0, 2), second);

            var numericPart = int.Parse(code[0..3]);

            result += numericPart * third.Length;
        }

        return result;
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }

    string Translate(char[,] keyboard, (int X, int Y) start, string numericCode)
    {
        var result = "";
        (int X, int Y) currentTile = start;

        foreach (var codeChar in numericCode)
        {
            var goalCoordinates = GetTileCoordinates(keyboard, codeChar);
            var memo = new Dictionary<(int X, int Y), List<(int X, int Y)>>();
            FindShortestRoute(keyboard, currentTile, goalCoordinates, [], memo);

            var directionVectors = memo[goalCoordinates][..^1]
                .Zip(memo[goalCoordinates][1..])
                .Select(x => (x.Second.X - x.First.X, x.Second.Y - x.First.Y))
                .Cast<(int X, int Y)>()
                .ToList();

            var translatedCode = TranslateDirectionsToCode(directionVectors);

            result += string.Concat<char>([.. translatedCode, 'A']);
            currentTile = goalCoordinates;
        }

        return result;
    }

    void FindShortestRoute(
        char[,] map,
        (int X, int Y) currentTile,
        (int X, int Y) goal,
        List<(int X, int Y)> currentRoute,
        Dictionary<(int X, int Y), List<(int X, int Y)>> memo)
    {
        if (currentRoute.Contains(currentTile)
            || !ArrayExtensions.IsValidTile(map, currentTile)
            || map[currentTile.X, currentTile.Y] == 'X')
            return;

        if (!memo.TryGetValue(currentTile, out var currentMin) || currentMin.Count - 1 > currentRoute.Count)
            memo[currentTile] = [.. currentRoute, currentTile];

        if (currentTile == goal)
            return;

        FindShortestRoute(map, (currentTile.X + 1, currentTile.Y), goal, [.. currentRoute, currentTile], memo);
        FindShortestRoute(map, (currentTile.X, currentTile.Y + 1), goal, [.. currentRoute, currentTile], memo);
        FindShortestRoute(map, (currentTile.X - 1, currentTile.Y), goal, [.. currentRoute, currentTile], memo);
        FindShortestRoute(map, (currentTile.X, currentTile.Y - 1), goal, [.. currentRoute, currentTile], memo);
    }

    (int X, int Y) GetTileCoordinates(char[,] map, char tile)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
                if (map[i, j] == tile)
                    return (i, j);
        }

        throw new InvalidOperationException($"No {tile} tile has been found!");
    }

    IEnumerable<char> TranslateDirectionsToCode(List<(int dx, int dy)> dirVectors)
    {
        foreach (var dirVec in dirVectors)
        {
            yield return dirVec switch
            {
                (1, 0) => 'v',
                (0, 1) => '>',
                (-1, 0) => '^',
                (0, -1) => '<',
                _ => throw new InvalidOperationException($"Invalid vector {dirVec}.")
            };
        }
    }
}
