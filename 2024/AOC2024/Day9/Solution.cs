using NUnit.Framework;

namespace Day9;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(1928));
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
		var diskMap = File.ReadLines(inputPath).First()
			.Select((x, i) => new Fragment(
				type: (Type)(i % 2),
				id: i % 2 is 0 ? i / 2 : null,
				size: x - '0')
			)
			.Where(x => x.Size is not 0)
			.ToList();

		while (diskMap.Any(x => x.Type is Type.FreeSpace))
		{
			var lastFragment = diskMap.Last();

			if (lastFragment.Type is Type.FreeSpace || lastFragment.Size is 0)
			{
				diskMap.Remove(lastFragment);
				continue;
			}

			var firstAvailableSpace = diskMap.First(x => x.Type is Type.FreeSpace);
			var index = diskMap.IndexOf(firstAvailableSpace);

			if (firstAvailableSpace.Size <= lastFragment.Size)
			{
				diskMap =
				[
					.. diskMap[..index],
					new Fragment(Type.File, firstAvailableSpace.Size, lastFragment.ID),
					.. diskMap[(index + 1)..],
				];
				lastFragment.Size -= firstAvailableSpace.Size;
			}
			else
			{
				diskMap.Remove(lastFragment);
				firstAvailableSpace.Size -= lastFragment.Size;
				diskMap =
				[
					.. diskMap[..index],
					new Fragment(Type.File, lastFragment.Size, lastFragment.ID),
					.. diskMap[index..],
				];
			}
		}

		return diskMap
			.SelectMany(x => Enumerable.Range(0, x.Size).Select(_ => x.ID!.Value))
			.Select((x, i) => (long)x * i)
			.Sum();
	}

	int SolvePart2(string inputPath)
	{
		return 0;
	}

	class Fragment(Type type, int size, int? id = null)
	{
		public Type Type = type;
		public int? ID = id;
		public int Size = size;
	}

	enum Type
	{
		File,
		FreeSpace
	}
}

