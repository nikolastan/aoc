using NUnit.Framework;
using Utility.Enums;

namespace Day4;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("Inputs/example.txt");
        Assert.That(result, Is.EqualTo(18));
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
        Assert.That(result, Is.EqualTo(9));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2("Inputs/input.txt");
        Console.WriteLine(result);
    }
    int SolvePart1(string inputPath)
    {
        var grid = File.ReadAllLines(inputPath)
            .Select(x => x.ToCharArray())
            .ToArray();

        var result = 0;
        var word = "XMAS";
        var directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                var numOfWordsFound = directions
                    .Count(direction =>
                        IsWordInDirection(grid, i, j, word, direction)
                        || IsWordInDirection(grid, i, j, word.Reverse().ToString(), direction));

                result += numOfWordsFound;
            }
        }

        return result;
    }

    int SolvePart2(string inputPath)
    {
        var grid = File.ReadAllLines(inputPath)
            .Select(x => x.ToCharArray())
            .ToArray();

        var result = 0;
        var word = "MAS";

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                var foundWord = IsWordInDirection(grid, i, j, word, Direction.Southeast) 
                    || IsWordInDirection(grid, i, j, new string(word.Reverse().ToArray()), Direction.Southeast);

                if (foundWord)
					foundWord = IsWordInDirection(grid, i, j + 2, word, Direction.Southwest)
					|| IsWordInDirection(grid, i, j + 2, new string(word.Reverse().ToArray()), Direction.Southwest);

				if (foundWord)
                    result++;
            }
        }

        return result;
    }

    bool IsWordInDirection(char[][] grid, int i, int j, string word, Direction searchDirection)
    {
        if (!CanPlaceWord(grid, i, j, word.Length, searchDirection))
            return false;

        int di = 0, dj = 0;
        switch (searchDirection)
        {
            case Direction.North: di = -1; break;
            case Direction.Northeast: di = -1; dj = 1; break;
            case Direction.Northwest: di = -1; dj = -1; break;
            case Direction.East: dj = 1; break;
            case Direction.Southeast: di = 1; dj = 1; break;
            case Direction.Southwest: di = 1; dj = -1; break;
            case Direction.South: di = 1; break;
            case Direction.West: dj = -1; break;
        }

        for (int k = 0; k < word.Length; k++)
        {
            int ni = i + k * di;
            int nj = j + k * dj;
            if (grid[ni][nj] != word[k])
                return false;
        }

        return true;
    }

    bool CanPlaceWord(char[][] grid, int i, int j, int wordLength, Direction searchDirection)
    {
        if (i < 0 || j < 0 || i >= grid.Length || j >= grid[i].Length)
            return false;

        return searchDirection switch
        {
            Direction.North => i - wordLength + 1 >= 0,
            Direction.Northeast => i - wordLength + 1 >= 0 && j + wordLength - 1 < grid[i].Length,
            Direction.Northwest => i - wordLength + 1 >= 0 && j - wordLength + 1 >= 0,
            Direction.East => j + wordLength - 1 < grid[i].Length,
            Direction.Southeast => i + wordLength - 1 < grid.Length && j + wordLength - 1 < grid[i].Length,
            Direction.South => i + wordLength - 1 < grid.Length,
            Direction.Southwest => i + wordLength - 1 < grid.Length && j - wordLength + 1 >= 0,
            Direction.West => j - wordLength + 1 >= 0,
            _ => false
        };
    }
}
