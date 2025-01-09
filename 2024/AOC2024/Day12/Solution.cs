using NUnit.Framework;
using Utility.Enums;
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

        var gardenPlots = GetGardenPlots(gardenMap);

        return gardenPlots
            .Select(plot => CalculateNumOfSides(gardenMap, plot) * plot.Count)
            .Sum();
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
        var edgePoints = area.Where(tile => IsOnEdge(map, tile.X, tile.Y)).ToList();

        var sides = GetNumOfSides(map, edgePoints);

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

        return false;
    }

    static int GetNumOfSides(Crop[][] map, List<(int X, int Y)> edgePoints)
    {

    }

    static IEnumerable<Direction> GetSideDirections(Crop[][] map, int x, int y)
    {
        if (ArrayExtensions.IsValidTile(map, x - 1, y) && map[x - 1][y].Plant == map[x][y].Plant)
            yield return Direction.North;

        if (ArrayExtensions.IsValidTile(map, x, y - 1) && map[x][y - 1].Plant == map[x][y].Plant)
            yield return Direction.West;

        if (ArrayExtensions.IsValidTile(map, x + 1, y) && map[x + 1][y].Plant == map[x][y].Plant)
            yield return Direction.South;

        if (ArrayExtensions.IsValidTile(map, x, y + 1) && map[x][y + 1].Plant == map[x][y].Plant)
            yield return Direction.East;
    }

    struct Crop(char plant)
    {
        public char Plant = plant;
        public bool Traversed = false;
    }
}
