using NUnit.Framework;

namespace Day23;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(7));
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
        var connections = File.ReadAllLines(inputPath)
            .Select(x => x.Split('-'))
            .Select(x => (x[0], x[1]));

        var computerNetworks = new Dictionary<string, HashSet<string>>();

        foreach (var connection in connections)
        {
            if (computerNetworks.TryGetValue(connection.Item1, out HashSet<string>? value1))
                value1.Add(connection.Item2);
            else
                computerNetworks.Add(connection.Item1, [connection.Item2]);


            if (computerNetworks.TryGetValue(connection.Item2, out HashSet<string>? value2))
                value2.Add(connection.Item1);
            else
                computerNetworks.Add(connection.Item2, [connection.Item1]);
        }

        var result = new HashSet<string>();

        foreach (var network in computerNetworks)
            Traverse(network.Key, [], computerNetworks, ref result);

        return result.Count;
    }

    void Traverse(string currentNetworkKey, List<string> currentPath, Dictionary<string, HashSet<string>> allNetworks, ref HashSet<string> traversedPaths)
    {
        if (currentPath.Count is 3)
        {
            if (currentPath[0] == currentNetworkKey && currentPath.Any(x => x.StartsWith('t')))
                traversedPaths.Add(currentPath.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).Aggregate((x, y) => $"{x},{y}"));
            return;
        }

        foreach (var network in allNetworks[currentNetworkKey])
            Traverse(network, [.. currentPath, currentNetworkKey], allNetworks, ref traversedPaths);
    }

    int SolvePart2(string inputPath)
    {
        return 0;
    }
}
