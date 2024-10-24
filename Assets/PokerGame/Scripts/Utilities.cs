////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Utilities.cs
//FileType: C# class file
//Description : This is a static class contains shuffle functionality extension for list
////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;


public static class Utilities
{
    private static System.Random rng = new System.Random();
    /// <summary>
    /// Shuffle given list of items
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
