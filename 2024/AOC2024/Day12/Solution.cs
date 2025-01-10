using NUnit.Framework;
using Utility.Extensions;

namespace Day12;
public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(1930));
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
        Assert.That(result, Is.EqualTo(1206));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }

    int SolvePart1(string inputPath)
    {
        var gardenMap = File.ReadAllLines(inputPath)
            .Select(x => x.Select(c => new Crop(c)).ToArray())
            .ToArray();

        var gardenPlots = GetGardenPlots(gardenMap);

        return gardenPlots
            .Select(plot => CalculatePerimeter(gardenMap, plot) * plot.Count)
            .Sum();
    }

    int SolvePart2(string inputPath)
    {
        var gardenMap = File.ReadAllLines(inputPath)
            .Select(x => x.Select(c => new Crop(c)).ToArray())
            .ToArray();

        gardenMap = ExpandMap(gardenMap, 3);

        var gardenPlots = GetGardenPlots(gardenMap);

        return gardenPlots
            .Select(plot => CalculateNumOfSides(gardenMap, plot) * plot.Count / 9)
            .Sum();
    }

    static Crop[][] ExpandMap(Crop[][] map, int times)
    {
        var expandedMap = new Crop[map.Length * times][];

        for (int i = 0; i < expandedMap.Length; i++)
        {
            expandedMap[i] = new Crop[map[i / times].Length * times];

            for (int j = 0; j < expandedMap[i].Length; j++)
            {
                expandedMap[i][j] = map[i / times][j / times];
            }
        }

        return expandedMap;
    }

    static IEnumerable<List<(int X, int Y)>> GetGardenPlots(Crop[][] gardenMap)
    {
        while (gardenMap.Any(row => row.Any(x => !x.Traversed)))
        {
            var currentCrop = gardenMap
                .SelectMany((row, i) => row
                    .Select((x, j) => new { X = i, Y = j, Crop = x })
                    .Where(x => !x.Crop.Traversed))
                .Select(crop => (crop.X, crop.Y))
                .First();

            var plantsToCheck = new List<(int X, int Y)>() { currentCrop };
            var gardenPlot = new List<(int X, int Y)>() { currentCrop };

            while (plantsToCheck.Count is not 0)
            {
                currentCrop = plantsToCheck.First();
                plantsToCheck.Remove(currentCrop);

                var surroundingPlants = GetSurroundingPlantsIfSame(gardenMap, currentCrop.X, currentCrop.Y)
                    .Except(gardenPlot)
                    .ToList();

                plantsToCheck.AddRange(surroundingPlants);
                gardenPlot.AddRange(surroundingPlants);

                gardenMap[currentCrop.X][currentCrop.Y].Traversed = true;
            }

            yield return gardenPlot;
        }
    }

    static IEnumerable<(int X, int Y)> GetSurroundingPlantsIfSame(Crop[][] map, int x, int y)
    {
        if (ArrayExtensions.IsValidTile(map, x - 1, y) && map[x - 1][y].Plant == map[x][y].Plant)
            yield return (x - 1, y);

        if (ArrayExtensions.IsValidTile(map, x, y - 1) && map[x][y - 1].Plant == map[x][y].Plant)
            yield return (x, y - 1);

        if (ArrayExtensions.IsValidTile(map, x + 1, y) && map[x + 1][y].Plant == map[x][y].Plant)
            yield return (x + 1, y);

        if (ArrayExtensions.IsValidTile(map, x, y + 1) && map[x][y + 1].Plant == map[x][y].Plant)
            yield return (x, y + 1);
    }

    static int GetNumberOfEdges(Crop[][] map, int x, int y)
    {
        var result = 0;

        if (!ArrayExtensions.IsValidTile(map, x - 1, y) || map[x - 1][y].Plant != map[x][y].Plant)
            result++;

        if (!ArrayExtensions.IsValidTile(map, x, y - 1) || map[x][y - 1].Plant != map[x][y].Plant)
            result++;

        if (!ArrayExtensions.IsValidTile(map, x + 1, y) || map[x + 1][y].Plant != map[x][y].Plant)
            result++;

        if (!ArrayExtensions.IsValidTile(map, x, y + 1) || map[x][y + 1].Plant != map[x][y].Plant)
            result++;

        return result;
    }

    static int CalculatePerimeter(Crop[][] map, List<(int X, int Y)> area)
    {
        var result = area
            .Select(crop => GetNumberOfEdges(map, crop.X, crop.Y))
            .Sum();

        return result;
    }

    static int CalculateNumOfSides(Crop[][] map, List<(int X, int Y)> area)
    {
        if (area.Count is 1)
            return 4;

        var edgePoints = area.Where(tile => IsOnEdge(map, tile.X, tile.Y)).ToList();

        var sides = GetNumOfSides(map, edgePoints, area);

        return sides;
    }

    static bool IsOnEdge(Crop[][] map, int x, int y)
    {
        if (!ArrayExtensions.IsValidTile(map, x - 1, y) || map[x - 1][y].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x, y - 1) || map[x][y - 1].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x + 1, y) || map[x + 1][y].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x, y + 1) || map[x][y + 1].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x - 1, y - 1) || map[x - 1][y - 1].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x + 1, y + 1) || map[x + 1][y + 1].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x - 1, y + 1) || map[x - 1][y + 1].Plant != map[x][y].Plant)
            return true;

        if (!ArrayExtensions.IsValidTile(map, x + 1, y - 1) || map[x + 1][y - 1].Plant != map[x][y].Plant)
            return true;

        return false;
    }

    static int GetNumOfSides(Crop[][] map, List<(int X, int Y)> edgePoints, List<(int X, int Y)> area)
    {
        var sides = 0;

        foreach (var (X, Y) in edgePoints)
        {
            sides += CountInsideCorners(area, X, Y);

            sides += CountOutsideCorners(area, X, Y);
        }

        return sides;
    }

    static int CountInsideCorners(List<(int X, int Y)> area, int X, int Y)
    {
        var corners = 0;

        if (area.Contains((X - 1, Y)) && area.Contains((X, Y - 1)) && !area.Contains((X - 1, Y - 1)))
            corners++;

        if (area.Contains((X, Y - 1)) && area.Contains((X + 1, Y)) && !area.Contains((X + 1, Y - 1)))
            corners++;

        if (area.Contains((X + 1, Y)) && area.Contains((X, Y + 1)) && !area.Contains((X + 1, Y + 1)))
            corners++;

        if (area.Contains((X, Y + 1)) && area.Contains((X - 1, Y)) && !area.Contains((X - 1, Y + 1)))
            corners++;

        return corners;

    }

    static int CountOutsideCorners(List<(int X, int Y)> area, int X, int Y)
    {
        var sides = 0;

        if (area.Contains((X - 1, Y)) && area.Contains((X, Y - 1)) && !area.Contains((X + 1, Y)) && !area.Contains((X, Y + 1)))
            sides++;

		if (area.Contains((X, Y - 1)) && area.Contains((X + 1, Y)) && !area.Contains((X - 1, Y)) && !area.Contains((X, Y + 1)))
			sides++;

		if (area.Contains((X + 1, Y)) && area.Contains((X, Y + 1)) && !area.Contains((X - 1, Y)) && !area.Contains((X, Y - 1)))
			sides++;

		if (area.Contains((X, Y + 1)) && area.Contains((X - 1, Y)) && !area.Contains((X, Y - 1)) && !area.Contains((X + 1, Y)))
			sides++;

		return sides;
    }

    struct Crop(char plant)
    {
        public char Plant = plant;
        public bool Traversed = false;
    }
}
