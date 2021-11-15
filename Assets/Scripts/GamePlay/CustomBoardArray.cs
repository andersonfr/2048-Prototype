using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomBoardArray<T>
{
    public static T[] GetColumn(T[,] matrix, int column) 
    {
        return Enumerable.Range(0, matrix.GetLength(0)).Select(x => matrix[x, column]).ToArray();
    }

    public static T[] GetRow(T[,] matrix, int row) 
    {
        return Enumerable.Range(0, matrix.GetLength(1)).Select(x => matrix[row, x]).ToArray();
    }

    public static void SetRow(T[,] matrix, int row, T[] array) 
    {
        for (int i = 0; i < matrix.GetLength(1); i++)
            matrix[row, i] = array[i];
    }

    public static void SetColumn(T[,] matrix, int column, T[] array)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
            matrix[i, column] = array[i];
    }
}
