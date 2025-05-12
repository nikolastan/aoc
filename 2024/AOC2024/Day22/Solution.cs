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
        var result = SolvePart2("Inputs/example2.txt");
        Assert.That(result, Is.EqualTo(23));
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
            initialNumbers = initialNumbers
                    .Select(GenerateNextSecret)
                    .ToList();

        return initialNumbers.Sum();
    }

    int SolvePart2(string inputPath)
    {
        var currentSecrets = File.ReadAllLines(inputPath)
            .Select(long.Parse)
            .ToList();

        var globalMemo = new Dictionary<string, int>();

        foreach (var vendorIndex in Enumerable.Range(0, currentSecrets.Count))
        {
            var vendorMemo = new HashSet<string>();
            var diffs = new List<char>();

            foreach (var secretIndex in Enumerable.Range(0, 2000))
            {
                var nextSecret = GenerateNextSecret(currentSecrets[vendorIndex]);
                int lastPrice = (int)nextSecret % 10;
                diffs.Add((char)(lastPrice - (currentSecrets[vendorIndex] % 10)));
                currentSecrets[vendorIndex] = nextSecret;

                if (secretIndex >= 4) //We start from 5th price because monkey cannot sell before that
                {
                    var diffSequence = new string(diffs.TakeLast(4).ToArray());
                    if (!vendorMemo.TryGetValue(diffSequence, out _))
                    {
                        vendorMemo.Add(diffSequence);

                        if (globalMemo.TryGetValue(diffSequence, out _))
                            globalMemo[diffSequence] += lastPrice;
                        else
                            globalMemo.Add(diffSequence, lastPrice);
                    }
                }
            }
        }

        return globalMemo.Max(x => x.Value);
    }

    long GenerateNextSecret(long currentSecret)
    {
        currentSecret = ((currentSecret * 64) ^ currentSecret) % 16777216;
        currentSecret = ((currentSecret / 32) ^ currentSecret) % 16777216;
        currentSecret = ((currentSecret * 2048) ^ currentSecret) % 16777216;

        return currentSecret;
    }
}
