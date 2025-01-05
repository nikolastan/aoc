﻿namespace Utility.Extensions;
public static class ArrayExtensions
{
    public static T[][] DeepCopy<T>(T[][] source)
    {
        T[][] copy = new T[source.Length][];

        for (int i = 0; i < source.Length; i++)
        {
            copy[i] = new T[source[i].Length];
            System.Array.Copy(source[i], copy[i], source[i].Length);
        }

        return copy;
    }

    public static bool IsValidTile<T>(T[][] grid, int x, int y)
    {
        return x >= 0
            && x < grid.Length
            && y >= 0
            && y < grid[x].Length;
    }
}
