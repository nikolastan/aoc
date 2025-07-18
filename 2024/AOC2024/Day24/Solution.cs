﻿using NUnit.Framework;

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

        return PerformOperations(startValues, operations);
    }

    int SolvePart2(string inputPath)
    {
        // Reduce problem space by setting x and y bits to random(?) numbers and testing the actual sum of those against resulting z bits (finding differentiating bits)
        // Try swapping around outputs of gates that are used for generating problematic z bits (and testing the results)

        ReadInput(inputPath, out var startValues, out var operations);

        startValues = SetBits(startValues, [('x', 1, 1)]);

        var xNum = ConvertBitsToBinary(startValues.Where(x => x.Key.StartsWith('x')));
        var yNum = ConvertBitsToBinary(startValues.Where(x => x.Key.StartsWith('y')));

        var zNum = Convert.ToInt64(xNum, 2) + Convert.ToInt64(yNum, 2);

        var actualZNum = PerformOperations(startValues, operations);

        var bitDiff = Convert.ToString(zNum ^ actualZNum, 2);

        return 0;
    }

    static string ConvertBitsToBinary(IEnumerable<KeyValuePair<string, int?>> bits)
    {
        return bits
            .OrderByDescending(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Value.ToString())
            .Aggregate((x, y) => x + y)!;
    }

    long PerformOperations(Dictionary<string, int?> startValues, List<Operation> operations)
    {
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

        return Convert.ToInt64(ConvertBitsToBinary(startValues.Where(x => x.Key.StartsWith('z'))), 2);
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

    private Dictionary<string, int?> SetBits(IDictionary<string, int?> startBitValues, IList<(char Reg, int BitNum, int Value)> bits)
    {
        var result = new Dictionary<string, int?>();

        foreach (var startingValue in startBitValues)
        {
            var (Reg, BitNum, NewBitValue) = bits.FirstOrDefault(setBit => startingValue.Key.StartsWith(setBit.Reg) && int.Parse(startingValue.Key[^2..]) == setBit.BitNum);

            if (Reg != default)
                result.Add(startingValue.Key, NewBitValue);
            else
                result.Add(startingValue.Key, 0);
        }

        return result;
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
