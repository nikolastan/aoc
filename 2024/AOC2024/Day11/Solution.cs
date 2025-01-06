using NUnit.Framework;

namespace Day11;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(55312));
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
        Console.WriteLine(result);
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    long SolvePart1(string inputPath)
    {
        var stones = ReadStones(inputPath);

        var memo = new Dictionary<(long, int), long>();

        return stones
            .Select(stone => Blink(stone, 25, memo))
            .Sum();
    }

    long SolvePart2(string inputPath)
    {
        var stones = ReadStones(inputPath);

        var memo = new Dictionary<(long, int), long>();

        return stones
            .Select(stone => Blink(stone, 75, memo))
            .Sum();
    }

    static List<long> ReadStones(string inputPath)
    {
        return File.ReadLines(inputPath)
            .First()
            .Split(' ')
            .Select(long.Parse)
            .ToList();
    }

    static long Blink(long stone, int blinksLeft, Dictionary<(long, int), long> memo)
    {
        if (blinksLeft is 0)
        {
            return 1;
        }

        if (memo.ContainsKey((stone, blinksLeft)))
        {
            return memo.GetValueOrDefault((stone, blinksLeft))!;
        }

        var newStones = 0L;

        if (stone is 0)
        {
            newStones = Blink(1, blinksLeft - 1, memo);
        }
        else if (stone.ToString().Length % 2 is 0)
        {
            var rawStone = stone.ToString();
            newStones += Blink(long.Parse(rawStone[..(rawStone.Length / 2)]), blinksLeft - 1, memo);
            newStones += Blink(long.Parse(rawStone[(rawStone.Length / 2)..]), blinksLeft - 1, memo);
        }
        else
            newStones = Blink(stone * 2024, blinksLeft - 1, memo);

        memo.TryAdd((stone, blinksLeft), newStones);

        return newStones;
    }
}
