using NUnit.Framework;

namespace Day19;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(6));
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
        Assert.That(result, Is.EqualTo(16));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    int SolvePart1(string inputPath)
    {
        var designs = ReadInput(inputPath, out var patterns);

        return designs
            .Where(x => IsPossible(x, patterns))
            .Count();
    }

    long SolvePart2(string inputPath)
    {
        var designs = ReadInput(inputPath, out var patterns);

        var memo = new Dictionary<string, long>();
        return designs
            .Sum(x => CountPossibleWays(x, patterns, memo));
    }

    IEnumerable<string> ReadInput(string inputPath, out List<string> patterns)
    {
        using (var reader = new StreamReader(inputPath))
        {
            patterns = [.. reader.ReadLine()!.Split(", ")];

            reader.ReadLine(); //blank line

            return reader.ReadToEnd()
                .Split("\r\n");
        }
    }

    bool IsPossible(string design, List<string> patterns)
    {
        if (patterns.Contains(design))
            return true;

        var matches = patterns.Where(x => design.IndexOf(x) is 0);

        return matches
            .Any(match => IsPossible(design[match.Length..], patterns));
    }

    long CountPossibleWays(string design, List<string> patterns, Dictionary<string, long> memo)
    {
        if (memo.TryGetValue(design, out var count))
            return count;

        var matches = patterns.Where(x => design.IndexOf(x) is 0).ToList();

        if (design == "" || (matches.Count is 1 && matches[0] == design))
            return 1;

        if (matches.Count is 0)
            return 0;

        var result = matches.Sum(match => CountPossibleWays(design[match.Length..], patterns, memo));

        memo[design] = result;
        return result;
    }
}
