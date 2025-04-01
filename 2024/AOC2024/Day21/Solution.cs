using NUnit.Framework;
using Utility.Extensions;

namespace Day21;
public class Solution
{
    readonly (char[,] Map, (int X, int Y) Start) NumericKeyboard = (new char[4, 3]
    {
        {'7', '8', '9' },
        {'4', '5', '6' },
        {'1', '2', '3' },
        {'X', '0', 'A' }
    }, (3, 2));

    readonly (char[,] Map, (int X, int Y) Start) DirectionalKeyboard = (new char[2, 3]
    {
        {'X', '^', 'A' },
        {'<', 'v', '>' }
    }, (0, 2));

    [Test]
    public void Part1_Example()
    {
        var result = Solve("Inputs/example.txt", 3);
        Assert.That((int)result, Is.EqualTo(126384));
    }

    [Test]
    public void Part1_Input()
    {
        var result = Solve("Inputs/input.txt", 3);
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Example()
    {
        var result = Solve("Inputs/example.txt", 10);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void Part2_Input()
    {
        var result = Solve("Inputs/input.txt", 25);
        Console.WriteLine(result);
    }

    Int128 Solve(string inputPath, int iterations)
    {
        var codes = File.ReadLines(inputPath);
        Int128 result = 0;

        foreach (var code in codes)
        {
            var translation = Translate(code, 0, iterations, []);

            var numericPart = int.Parse(code[0..3]);

            result += numericPart * translation.Length;
        }

        return result;
    }

    string Translate(string code, int iteration, int totalTranslations, Dictionary<(string, int), string> memo)
    {
        if (totalTranslations == iteration)
            return code;

        if (memo.TryGetValue((code, iteration), out string? currentMin))
            return currentMin;

        var (Map, Start) = iteration is 0
            ? NumericKeyboard
            : DirectionalKeyboard;

        var currentTile = Start;
        var result = "";

        foreach (var codeChar in code)
        {
            var endTile = GetTileCoordinates(Map, codeChar);

            var translations = EncodeDirections(Map, currentTile, endTile);
            var shortestTranslation = translations
                .Select(x => Translate(x, iteration + 1, totalTranslations, memo))
                .OrderBy(x => x.Length)
                .First();

            result += shortestTranslation;
            currentTile = endTile;
        }

        memo[(code, iteration)] = result;

        return result;
    }

    List<string> EncodeDirections(char[,] keyboard, (int X, int Y) startTile, (int X, int Y) endTile)
    {
        var memo = new Dictionary<(int X, int Y), List<(int X, int Y)[]>>();
        FindAllShortestRoutes(keyboard, startTile, endTile, [], memo);

        var directionVectors = memo[endTile]
            .Select(x => x[..^1]
            .Zip(x[1..])
            .Select(x => (x.Second.X - x.First.X, x.Second.Y - x.First.Y))
            .Cast<(int X, int Y)>()
            .ToList());

        return directionVectors
            .Select(x => new string(TranslateDirectionsToCode(x).ToArray()))
            .ToList();
    }

    void FindAllShortestRoutes(
        char[,] map,
        (int X, int Y) currentTile,
        (int X, int Y) goal,
        List<(int X, int Y)> currentRoute,
        Dictionary<(int X, int Y), List<(int X, int Y)[]>> memo)
    {
        if (currentRoute.Contains(currentTile)
            || !ArrayExtensions.IsValidTile(map, currentTile)
            || map[currentTile.X, currentTile.Y] == 'X')
            return;

        if (!memo.TryGetValue(currentTile, out var currentMin) || currentMin.First().Length - 1 > currentRoute.Count)
            memo[currentTile] = [[.. currentRoute, currentTile]];
        else if (currentMin.Count > 0 && currentMin.First().Length - 1 == currentRoute.Count)
            memo[currentTile].Add([.. currentRoute, currentTile]);

        if (currentTile == goal)
            return;

        FindAllShortestRoutes(map, (currentTile.X + 1, currentTile.Y), goal, [.. currentRoute, currentTile], memo);
        FindAllShortestRoutes(map, (currentTile.X, currentTile.Y + 1), goal, [.. currentRoute, currentTile], memo);
        FindAllShortestRoutes(map, (currentTile.X - 1, currentTile.Y), goal, [.. currentRoute, currentTile], memo);
        FindAllShortestRoutes(map, (currentTile.X, currentTile.Y - 1), goal, [.. currentRoute, currentTile], memo);
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

        yield return 'A';
    }
}
