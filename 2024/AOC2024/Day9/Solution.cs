using NUnit.Framework;
using System;

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
		Assert.That(result, Is.EqualTo(2858));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	long SolvePart1(string inputPath)
	{
		var diskMap = ReadDiskMap(inputPath);

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
				lastFragment.Size -= firstAvailableSpace.Size;
				diskMap =
				[
					.. diskMap[..index],
					new Fragment(Type.File, firstAvailableSpace.Size, lastFragment.ID),
					.. diskMap[(index + 1)..],
				];
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

		return CalculateChecksum(diskMap);
	}

	long SolvePart2(string inputPath)
	{
		var diskMap = ReadDiskMap(inputPath);

		while (diskMap.Any(x => x.Type is Type.File && !x.Checked))
		{
			var lastFragment = diskMap.Last(x => x.Type is Type.File && !x.Checked);
			var indexOfLastFragment = diskMap.IndexOf(lastFragment);

			var firstAvailableSpace = diskMap[..indexOfLastFragment]
				.FirstOrDefault(x => x.Type is Type.FreeSpace && x.Size >= lastFragment.Size);
			
			if(firstAvailableSpace is not null)
			{
				var index = diskMap.IndexOf(firstAvailableSpace);

				firstAvailableSpace.Size -= lastFragment.Size;
				diskMap =
				[
					.. diskMap[..index],
					new Fragment(Type.File, lastFragment.Size, lastFragment.ID) { Checked = true },
					.. diskMap[index..],
				];

				lastFragment.Type = Type.FreeSpace;
				lastFragment.ID = null;

				if (firstAvailableSpace.Size is 0)
					diskMap.Remove(firstAvailableSpace);
			}

			lastFragment.Checked = true;
		}

		return CalculateChecksum(diskMap);
	}

	static List<Fragment> ReadDiskMap(string inputPath)
	{
		return File.ReadLines(inputPath).First()
			.Select((x, i) => new Fragment(
				type: (Type)(i % 2),
				id: i % 2 is 0 ? i / 2 : null,
				size: x - '0')
			)
			.Where(x => x.Size is not 0)
			.ToList();
	}

	static long CalculateChecksum(List<Fragment> diskMap)
	{
		return diskMap
			.SelectMany(x => Enumerable.Range(0, x.Size).Select(_ => x.ID ?? 0))
			.Select((x, i) => (long)x * i)
			.Sum();
	}

	class Fragment(Type type, int size, int? id = null)
	{
		public Type Type = type;
		public int? ID = id;
		public int Size = size;

		public bool Checked = false;
	}

	enum Type
	{
		File,
		FreeSpace
	}
}

