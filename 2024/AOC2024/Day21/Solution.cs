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

    readonly (int Dx, int Dy)[] VectorOrder =
    {
        (0, -1), (1, 0), (-1, 0), (0, 1) //<, v, ^, >
	};

    List<string> BasicPatternMap = ["<vA", "<A", "A", ">>^A", "vA", "^A", "v>A", "<^A", ">A", "^>A", "v<<A", ">^A", "v<A"];

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
        var result = Solve("Inputs/example.txt", 25);
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

        var patternMemo = new Dictionary<string, string>();
        foreach (var code in codes)
        {
            GenerateFullPatternMap(code, 0, iterations + 5, patternMemo);

            var translation = patternMemo[code].Split(',')
                    .Where(x => x.Length > 0)
                    .Select(x => patternMemo[x])
                    .Aggregate((x, y) => $"{x},{y}");

            var translationMemo = new Dictionary<(int, int), long>();
            var translationLength = translation
                    .Split(',')
                    .Select(x => GetTranslationLength(int.Parse(x), 1, iterations - 1, patternMemo, translationMemo))
                    .Sum();

            var numericPart = int.Parse(code[0..3]);

            result += numericPart * translationLength;
        }

        return result;
    }

    long GetTranslationLength(int patternIndex, int iteration, int totalIterations, Dictionary<string, string> patternMemo, Dictionary<(int, int), long> translationMemo)
    {
        if (translationMemo.TryGetValue((patternIndex, iteration), out var result))
            return result;

        var pattern = BasicPatternMap[patternIndex];

        if (iteration == totalIterations)
            return pattern.Length;

        result = 0;

        if (patternMemo.TryGetValue(pattern, out string? value))
            foreach (var ind in value.Split(','))
                result += GetTranslationLength(int.Parse(ind), iteration + 1, totalIterations, patternMemo,
                    translationMemo);
        else
            throw new InvalidOperationException($"No {pattern} was found in pattern memo.");

        translationMemo[(patternIndex, iteration)] = result;

        return result;
    }

    void GenerateFullPatternMap(string code, int iteration, int totalTranslations, Dictionary<string, string> memo)
    {
        if (memo.ContainsKey(code))
            return;

        if (totalTranslations == iteration)
            return;

        var (Map, Start) = iteration is 0
            ? NumericKeyboard
            : DirectionalKeyboard;

        var currentTile = Start;

        foreach (var codeChar in code)
        {
            var endTile = GetTileCoordinates(Map, codeChar);

            var translation = EncodeDirections(Map, currentTile, endTile);
            GenerateFullPatternMap(translation, iteration + 1, totalTranslations, memo);

            if (!memo.TryGetValue(code, out string? value))
                memo[code] = "";
            else if (value.Last() != ',')
                return;

            if (iteration is 0)
            {
                memo[code] += translation;
            }
            else
                memo[code] += BasicPatternMap.IndexOf(translation);

            memo[code] += ',';

            currentTile = endTile;
        }

        memo[code] = memo[code][..^1];
    }

    string EncodeDirections(char[,] keyboard, (int X, int Y) startTile, (int X, int Y) endTile)
    {
        var memo = new Dictionary<(int X, int Y), List<(int X, int Y)>>();
        FindShortestRoute(keyboard, startTile, endTile, [], memo);

        var directionVectors = memo[endTile][..^1]
            .Zip(memo[endTile][1..])
            .Select(x => (x.Second.X - x.First.X, x.Second.Y - x.First.Y))
            .Cast<(int dX, int dY)>()
            .ToList();

        directionVectors = SortDirections(keyboard, startTile, directionVectors).ToList();

        return new string(TranslateDirectionsToCode(directionVectors).ToArray());
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
        else
            return;

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

        yield return 'A';
    }

    IEnumerable<(int Dx, int Dy)> SortDirections(char[,] keyboard, (int X, int Y) startTile, List<(int Dx, int Dy)> directionVectors)
    {
        var currentTile = startTile;
        var stash = new Stack<List<(int Dx, int Dy)>>();

        // Sort direction vectors based on their position in VectorOrder array
        directionVectors = [.. directionVectors.OrderBy(x => Array.IndexOf(VectorOrder, x))];

        int i = 0;
        while (i < directionVectors.Count)
        {
            var currentVector = directionVectors[i];
            var vectorGroup = new List<(int Dx, int Dy)> { currentVector };

            // Find all consecutive identical vectors
            int j = i + 1;
            while (j < directionVectors.Count &&
                   directionVectors[j].Dx == currentVector.Dx &&
                   directionVectors[j].Dy == currentVector.Dy)
            {
                vectorGroup.Add(directionVectors[j]);
                j++;
            }

            // Check if any vector in the group leads to an 'X' tile
            bool groupHasXTile = false;
            var tempTile = currentTile;
            foreach (var vec in vectorGroup)
            {
                if (keyboard[tempTile.X + vec.Dx, tempTile.Y + vec.Dy] is 'X')
                {
                    groupHasXTile = true;
                    break;
                }
                tempTile = (tempTile.X + vec.Dx, tempTile.Y + vec.Dy);
            }

            if (groupHasXTile)
            {
                // If any vector in the group leads to an 'X' tile, push the entire group onto the stack
                stash.Push(vectorGroup);
            }
            else
            {
                // Otherwise, yield all vectors in the group and update the current position
                foreach (var vec in vectorGroup)
                {
                    yield return vec;
                    currentTile = (currentTile.X + vec.Dx, currentTile.Y + vec.Dy);
                }
            }

            // Move to the next group of vectors
            i = j;
        }

        // After processing all direction vectors, yield those that lead to 'X' tiles in reverse order
        while (stash.Count > 0)
        {
            var group = stash.Pop();
            foreach (var vec in group)
            {
                yield return vec;
                // Note: We don't update currentTile here because these vectors lead to 'X' tiles
                // and we're just yielding them at the end
            }
        }
    }
}
