using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Day3;

public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(161));
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
		Assert.That(result, Is.EqualTo(48));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	int SolvePart2(string inputPath)
	{
		var rgx = MultCommandRegex();

		var input = File.ReadAllText(inputPath);

		var matches = rgx.Matches(input)
			.Select(x => x.Value);

		var mulEnabled = true;
		var result = 0;

		foreach(var match in matches)
		{
			if(match == "don't()")
			{
				mulEnabled = false;
				continue;
			}

			if (match == "do()")
			{
				mulEnabled = true;
				continue;
			}

			if (mulEnabled)
				result += match[4..^1]
					.Split(",")
					.Select(int.Parse)
					.Aggregate((x, y) => x * y);
			else
				continue;
		}

		return result;
	}

	int SolvePart1(string inputPath)
	{
		var rgx = MultCommandRegex();

		var input = File.ReadAllText(inputPath);

		var result = rgx.Matches(input)
			.Select(x => x.Value)
			.Select(x => x[4..^1])
			.Select(x => x.Split(','))
			.Select(x => int.Parse(x[0]) * int.Parse(x[1]))
			.Sum();

		return result;
	}

	[GeneratedRegex(@"(mul\(\d{1,3},\d{1,3}\)|do\(\)|don't\(\))")]
	private static partial Regex MultCommandRegex();
}
