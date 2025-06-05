using NUnit.Framework;

namespace Day24;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(2024));
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
        ReadInput(inputPath, out var startValues, out var operations);

        var laterLookup = new List<Operation>();
        while (operations.Count > 0 || laterLookup.Count > 0)
        {
            if (operations.Count == 0)
            {
                operations.AddRange(laterLookup);
                laterLookup.Clear();
            }

            var operation = operations.First();
            operations.RemoveAt(0);

            if (!startValues.ContainsKey(operation.Value1) || !startValues.ContainsKey(operation.Value2))
            {
                laterLookup.Add(operation);
                continue;
            }

            var value = operation.Type switch
            {
                OperationType.AND => startValues[operation.Value1] & startValues[operation.Value2],
                OperationType.OR => startValues[operation.Value1] | startValues[operation.Value2],
                OperationType.XOR => startValues[operation.Value1] ^ startValues[operation.Value2],
                _ => throw new NotImplementedException()
            };

            if (!startValues.TryAdd(operation.Result, value))
                startValues[operation.Result] = value;
        }

        return Convert.ToInt64(startValues
            .Where(x => x.Key.StartsWith('z'))
            .OrderByDescending(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Value.ToString())
            .Aggregate((x, y) => x + y), 2);
    }

    int SolvePart2(string inputPath)
    {
        // Reduce problem space by setting x and y bits to random(?) numbers and testing the actual sum of those against resulting z bits (finding differentiating bits)
        // Try swapping around outputs of gates that are used for generating problematic z bits (and testing the results)
        return 0;
    }

    void ReadInput(string inputPath, out Dictionary<string, int?> startValues, out List<Operation> operations)
    {
        using var reader = new StreamReader(inputPath);

        startValues = [];
        operations = [];

        var nextLine = reader.ReadLine();
        while (nextLine != "")
        {
            startValues.Add(nextLine![..3], nextLine[^1] - '0');
            nextLine = reader.ReadLine();
        }

        while (!reader.EndOfStream)
        {
            nextLine = reader.ReadLine();

            var rawValues = nextLine!
                .Split(' ')
                .Where(x => x is not "->")
                .ToList();

            operations.Add(
                new Operation(rawValues[0], rawValues[2], (OperationType)Enum.Parse(typeof(OperationType), rawValues[1]), rawValues[3]));
        }
    }

    struct Operation(string value1, string value2, OperationType type, string result)
    {
        public string Value1 = value1;
        public string Value2 = value2;
        public OperationType Type = type;
        public string Result = result;
    }

    enum OperationType
    {
        AND,
        OR,
        XOR
    }
}
