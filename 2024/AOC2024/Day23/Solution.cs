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
        Assert.That(result, Is.EqualTo("co,de,ka,ta"));
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

        var computerNetworks = GenerateGraphDictionary(connections);

        var result = new HashSet<string>();

        foreach (var network in computerNetworks)
            FindAll3NodeLans(network.Key, [], computerNetworks, ref result);

        return result.Count;
    }

    string SolvePart2(string inputPath)
    {
        var connections = File.ReadAllLines(inputPath)
            .Select(x => x.Split('-'))
            .Select(x => (x[0], x[1]));

        var computerNetworks = GenerateGraphDictionary(connections);

        var result = new List<string>();

        foreach (var network in computerNetworks)
            FindBiggestLan(network.Key, [], computerNetworks, [], ref result);

        return result.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).Aggregate((x, y) => $"{x},{y}");
    }

    Dictionary<string, HashSet<string>> GenerateGraphDictionary(IEnumerable<(string, string)> connections)
    {
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

        return computerNetworks;
    }

    void FindAll3NodeLans(string currentNetworkKey, List<string> currentPath, Dictionary<string, HashSet<string>> allNetworks, ref HashSet<string> traversedPaths)
    {
        if (currentPath.Count is 3)
        {
            if (currentPath[0] == currentNetworkKey && currentPath.Any(x => x.StartsWith('t')))
                traversedPaths.Add(currentPath.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).Aggregate((x, y) => $"{x},{y}"));
            return;
        }

        foreach (var network in allNetworks[currentNetworkKey])
            FindAll3NodeLans(network, [.. currentPath, currentNetworkKey], allNetworks, ref traversedPaths);
    }

    void FindBiggestLan(string currentNetworkKey, List<string> currentPath, Dictionary<string, HashSet<string>> allNetworks, HashSet<int> traversedPathHashes, ref List<string> biggestLan)
    {
        if (currentPath.Contains(currentNetworkKey) || currentPath.Any(x => !allNetworks[x].Contains(currentNetworkKey)))
            return;

        currentPath = [.. currentPath, currentNetworkKey];

        var hash = GetOrderIndependentHashCode(currentPath);

        if (!traversedPathHashes.Add(hash))
            return;

        if (currentPath.Count > biggestLan.Count)
        {
            biggestLan = currentPath;
        }

        foreach (var network in allNetworks[currentNetworkKey])
            FindBiggestLan(network, currentPath, allNetworks, traversedPathHashes, ref biggestLan);
    }

    public static int GetOrderIndependentHashCode(IEnumerable<string> strings)
    {
        if (strings == null)
            return 0;

        int hash = 0;
        foreach (var str in strings)
        {
            hash ^= str.GetHashCode();
        }

        return hash;
    }
}
