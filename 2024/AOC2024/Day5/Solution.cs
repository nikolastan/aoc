using NUnit.Framework;

namespace Day5;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(143));
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
        Assert.That(result, Is.EqualTo(123));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }
    int SolvePart1(string inputPath)
    {
        var rules = ReadRules(inputPath)
            .GroupBy(x => x.After, x => x.Before)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var updates = ReadUpdates(inputPath);

        return updates
            .Where(update => update
                .Select((page, i) => new { Page = page, Index = i })
                .All(x => !update[(x.Index + 1)..].Intersect(rules.GetValueOrDefault(x.Page) ?? []).Any())
                )
            .Select(x => x.ToArray())
            .Select(x => x.ElementAt(x.Length / 2))
            .Sum();
    }

    int SolvePart2(string inputPath)
    {
        var rules = ReadRules(inputPath)
            .GroupBy(x => x.After, x => x.Before)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var updates = ReadUpdates(inputPath);

        return updates
            .Where(update => update
                .Select((page, i) => new { Page = page, Index = i })
                .Any(x => update[(x.Index + 1)..].Intersect(rules.GetValueOrDefault(x.Page) ?? []).Any())
                )
            .Select(x => x.OrderBy(page => page, new PageComparer(rules)).ToArray())
            .Select(x => x.ElementAt(x.Length / 2))
            .Sum();
    }

    IEnumerable<Rule> ReadRules(string inputPath)
    {
        using var reader = new StreamReader(inputPath);

        var currentLine = reader.ReadLine();

        while (currentLine != string.Empty)
        {
            var nums = currentLine!.Split("|");
            yield return new Rule(int.Parse(nums[0]), int.Parse(nums[1]));
            currentLine = reader.ReadLine();
        }
    }

    IEnumerable<int[]> ReadUpdates(string inputPath)
    {
        using var reader = new StreamReader(inputPath);

        while (reader.ReadLine() != string.Empty)
        { }

        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine()!
                .Split(",")
                .Select(int.Parse)
                .ToArray();
        }
    }

    struct Rule(int before, int after)
    {
        public int Before = before;
        public int After = after;
    }

    class PageComparer(Dictionary<int, int[]> rules) : IComparer<int>
    {
        readonly Dictionary<int, int[]> Rules = rules;

        public int Compare(int x, int y)
        {
            var pageRules = Rules.GetValueOrDefault(x) ?? [];

            if (pageRules.Contains(y))
                return -1;

            return 1;
        }
    }
}
