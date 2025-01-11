using NUnit.Framework;

namespace Day13;
public partial class Solution
{
	[Test]
	public void Part1_Example()
	{
		var result = SolvePart1("Inputs/example.txt");
		Assert.That(result, Is.EqualTo(480));
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
		Console.WriteLine(result);
		//Assert.That(result, Is.EqualTo(0));
	}

	[Test]
	public void Part2_Input()
	{
		var result = SolvePart2("Inputs/input.txt");
		Console.WriteLine(result);
	}

	long SolvePart1(string inputPath)
	{
		var machineConfigs = ReadConfigs(inputPath, 0)
			.ToList();

		return machineConfigs
			.Select(x => new { Sln = FindSolution(x), Config = x})
			.Where(x => x.Config.AX * x.Sln.a + x.Config.BX * x.Sln.b == x.Config.XPrize
				&& x.Config.AY * x.Sln.a + x.Config.BY * x.Sln.b == x.Config.YPrize)
			.Select(x => x.Sln.a * 3 + x.Sln.b)
			.Sum();
	}

	long SolvePart2(string inputPath)
	{
		var machineConfigs = ReadConfigs(inputPath, 10000000000000)
			.ToList();

		return machineConfigs
			.Select(x => new { Sln = FindSolution(x), Config = x })
			.Where(x => x.Config.AX * x.Sln.a + x.Config.BX * x.Sln.b == x.Config.XPrize
				&& x.Config.AY * x.Sln.a + x.Config.BY * x.Sln.b == x.Config.YPrize)
			.Select(x => x.Sln.a * 3 + x.Sln.b)
			.Sum();
	}

	static IEnumerable<MachineConfig> ReadConfigs(string inputPath, long prizeCorrection)
	{
		using var reader = new StreamReader(inputPath);

		while(!reader.EndOfStream)
		{
			var btnA = reader.ReadLine();
			var btnB = reader.ReadLine();
			var prize = reader.ReadLine();

			_ = reader.ReadLine();

			var aConfig = btnA![10..].Split(", ").Select(x => x[2..]).Select(int.Parse);
			var bConfig = btnB![10..].Split(", ").Select(x => x[2..]).Select(int.Parse);
			var prizeConfig = prize![7..].Split(", ").Select(x => x[2..]).Select(int.Parse);

			yield return 
				new MachineConfig(
					aConfig.First(), 
					aConfig.Last(),
					bConfig.First(),
					bConfig.Last(),
					prizeConfig.First() + prizeCorrection, 
					prizeConfig.Last() + prizeCorrection);
		}
	}

	static (long a, long b) FindSolution(MachineConfig config)
	{
		var potentialB = (config.YPrize * config.AX - config.XPrize * config.AY) / (config.BY * config.AX - config.BX * config.AY);
		var potentialA = (config.XPrize - potentialB * config.BX) / config.AX;

		return (potentialA, potentialB);
	}

	struct MachineConfig(int aX, int aY, int bX, int bY, long xPrize, long yPrize)
	{
		public int AX = aX;
		public int AY = aY;
		public int BX = bX;
		public int BY = bY;
		public long XPrize = xPrize;
		public long YPrize = yPrize;
	}
}

