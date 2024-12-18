using NUnit.Framework;

namespace Day2;

public class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(2));
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
		Assert.That(result, Is.EqualTo(4));
	}

	[Test]
	public void Part2_EdgeCases()
	{
		var result = SolvePart2("Inputs/edgeCases.txt");
		Assert.That(result, Is.EqualTo(1));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	int SolvePart1(string inputPath)
	{
		var reports = File.ReadAllLines(inputPath)
			.Select(x => x.Split(' '))
			.Select(x => x.Select(int.Parse).ToList());

		var shiftedReports = reports
			.Select(x => x[1..])
			.ToArray();

		var result = reports
			.Select(x => x[..^1])
			.Select((report, i) => report.Select((c, j) => c - shiftedReports[i][j]).ToArray())
			.Select(x => x.All(y => Math.Sign(y) == Math.Sign(x[0]) && Math.Abs(y) <= 3 && Math.Abs(y) >= 1))
			.Where(x => x is true)
			.Count();

		return result;
	}

	int SolvePart2(string inputPath)
	{
		var reports = File.ReadAllLines(inputPath)
			.Select(x => x.Split(' '))
			.Select(x => x.Select(int.Parse).ToList());

		var shiftedReports = reports
			.Select(x => x[1..])
			.ToArray();

		var test123 = reports
			.Select(x => x[..^1])
			.Select((report, i) => report.Select((c, j) => c - shiftedReports[i][j]).ToArray())
			.Select(x => Enumerable.Range(0, x.Length - 1)
					.Select(i => x.Take(i).Concat([x[i] + x[i + 1]]).Concat(x.Skip(i + 2)))
					.Concat([x[1..], x[..^1]]))
			.First();

		var result = reports
			.Select(x => x[..^1])
			.Select((report, i) => report.Select((c, j) => c - shiftedReports[i][j]).ToArray())
			.Select(report => report.All(level => Math.Sign(level) == Math.Sign(report[0]) && Math.Abs(level) <= 3 && Math.Abs(level) >= 1)
				|| Enumerable.Range(0, report.Length - 1)
					.Select(i => report.Take(i).Concat([report[i] + report[i+1]]).Concat(report.Skip(i+2)))
					.Concat([report[1..], report[..^1]])
					.Select(x => x.ToArray())
					.Any(combination => combination.All(level => Math.Sign(level) == Math.Sign(combination[0]) && Math.Abs(level) <= 3 && Math.Abs(level) >= 1)))
			.Where(x => x is true)
			.Count();

		return result;
	}
}
