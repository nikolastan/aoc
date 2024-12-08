using NUnit.Framework;
using System.Collections;
using System.Linq;

namespace Day1;

public class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(11));
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
		Assert.That(result, Is.EqualTo(31));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	int SolvePart1(string inputPath)
	{
		(List<int> numList1, List<int> numList2) = ReadInput(inputPath);

		numList1.Sort();
		numList2.Sort();

		return numList1
			.Select((x, i) => Math.Abs(x - numList2[i]))
			.Sum();
	}

	int SolvePart2(string inputPath)
	{
		(List<int> numList1, List<int> numList2) = ReadInput(inputPath);

		var hashMap = numList1
			.Distinct()
			.ToDictionary(num => num, num => 0);

		foreach (var num in numList2)
		{
			if (hashMap.TryGetValue(num, out int value))
				hashMap[num] = ++value;
		}

		return numList1
			.Select(x => x * hashMap[x])
			.Sum();
	}

	(List<int>, List<int>) ReadInput(string inputPath)
	{
		List<int> numList1 = [];
		List<int> numList2 = [];

		var numPairs = File.ReadAllLines(inputPath)
			.Select(x => x.Split(' '))
			.Select(x => x.Where(s => !string.IsNullOrEmpty(s)));

		foreach (var pair in numPairs)
		{
			numList1.Add(int.Parse(pair.First()));
			numList2.Add(int.Parse(pair.Last()));
		}

		return (numList1, numList2);
	}
}
