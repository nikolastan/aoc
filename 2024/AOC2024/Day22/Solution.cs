using NUnit.Framework;

namespace Day22;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(37327623));
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

    long SolvePart1(string inputPath)
    {
        var initialNumbers = File.ReadAllLines(inputPath)
            .Select(long.Parse)
            .ToList();

        foreach (var _ in Enumerable.Range(0, 2000))
        {
            initialNumbers = initialNumbers
                .Select((x, i) => ((x * 64) ^ initialNumbers[i]) % 16777216)
                .ToList();

            initialNumbers = initialNumbers
                .Select((x, i) => ((x / 32) ^ initialNumbers[i]) % 16777216)
                .ToList();

            initialNumbers = initialNumbers
                .Select((x, i) => ((x * 2048) ^ initialNumbers[i]) % 16777216)
                .ToList();
        }

        return initialNumbers.Sum();
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }
}
