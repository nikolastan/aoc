using NUnit.Framework;
using Utility.Extensions;

namespace Day8;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(14));
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
		Assert.That(result, Is.EqualTo(34));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	int SolvePart1(string inputPath)
	{
		var grid = File.ReadLines(inputPath)
			.Select(x => x.ToCharArray())
			.ToArray();

		var allAntennaGroups = grid
			.SelectMany((row, i) => row
				.Select((el, j) => new { Antenna = el, Coordinates = (i, j) })
				.Where(x => x.Antenna != '.'))
			.GroupBy(x => x.Antenna, x => x.Coordinates)
			.Where(group => group.Count() > 1)
			.ToDictionary(x => x.Key, x => x.ToList());

		var antinodes = new List<(int x, int y)>();

		foreach(var antennaGroup in allAntennaGroups)
		{
			foreach (var antenna in antennaGroup.Value)
			{
				var group = new List<(int i, int j)>(antennaGroup.Value);
				group.Remove(antenna);

				antinodes.AddRange(group
					.Select(a => 
							(antenna.i + (antenna.i - a.i),
							(antenna.j + (antenna.j - a.j)))
						)
					);
			}
		}

		return antinodes
			.Where(node => ArrayExtensions.IsValidTile(grid, node.x, node.y))
			.Distinct()
			.Count();
	}

	int SolvePart2(string inputPath)
	{
		var grid = File.ReadLines(inputPath)
			.Select(x => x.ToCharArray())
			.ToArray();

		var allAntennaGroups = grid
			.SelectMany((row, i) => row
				.Select((el, j) => new { Antenna = el, Coordinates = (i, j) })
				.Where(x => x.Antenna != '.'))
			.GroupBy(x => x.Antenna, x => x.Coordinates)
			.Where(group => group.Count() > 1)
			.ToDictionary(x => x.Key, x => x.ToList());

		var antinodes = new List<(int x, int y)>();

		foreach (var antennaGroup in allAntennaGroups)
		{
			foreach (var antenna in antennaGroup.Value)
			{
				var group = new List<(int i, int j)>(antennaGroup.Value);
				group.Remove(antenna);

				antinodes.AddRange(group
					.SelectMany(a =>
					{
						var di = (antenna.i - a.i);
						var dj = (antenna.j - a.j);

						(int currentX, int currentY) = (antenna.i - di, (antenna.j - dj));
						var results = new List<(int x, int y)>();

						while (ArrayExtensions.IsValidTile(grid, currentX, currentY))
						{
							results.Add((currentX, currentY));
							(currentX, currentY) = (currentX - di, (currentY - dj));
						}

						return results;
					})
				);
			}
		}

		return antinodes
			.Distinct()
			.Count();
	}
}

