using NUnit.Framework;
using System.Data;

namespace Day7;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(3749));
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
		Assert.That(result, Is.EqualTo(11387));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	long SolvePart1(string inputPath)
	{
		var equations = ReadInput(inputPath);

		return equations
			.Where(eq => EvaluateAllPossibleEquations(eq.Numbers, ['+', '*']).Contains(eq.TestValue))
			.Sum(eq => eq.TestValue);
	}

	long SolvePart2(string inputPath)
	{
		var equations = ReadInput(inputPath);

		return equations
			.Where(eq => EvaluateAllPossibleEquations(eq.Numbers, ['+', '*', '|']).Contains(eq.TestValue))
			.Sum(eq => eq.TestValue);
	}

	static IEnumerable<Equation> ReadInput(string inputPath)
	{
		return File.ReadAllLines(inputPath)
			.Select(x => x.Split(':').Select(y => y.Trim()).ToArray())
			.Select(x => new Equation(
				long.Parse(x[0]),
				x[1].Split(' ').Select(int.Parse).ToArray())
			);
	}

	static IEnumerable<long> EvaluateAllPossibleEquations(int[] numbers, char[] operators)
	{
		int numOperators = numbers.Length - 1;
		var totalCombinations = (int)Math.Pow(operators.Length, numOperators);

		for(int i = 0; i < totalCombinations; i++)
		{
			var currentOperators = new Queue<char>(['+']);
			int temp = i;

			for(int j = 0; j < numOperators; j++)
			{
				currentOperators.Enqueue(operators[temp % operators.Length]);
				temp /= operators.Length;
			}

			yield return numbers
				.Aggregate(0L, (x, y) =>
				{
					var op = currentOperators.Dequeue();
					return op switch
					{
						'+' => x + y,
						'*' => x * y,
						'|' => long.Parse($"{x}{y}"),
						_ => throw new InvalidOperationException("Not supported!")
					};
				});
		}
	}

	struct Equation(long testValue, int[] numbers)
	{
		public long TestValue = testValue;
		public int[] Numbers = numbers;
	}
}

