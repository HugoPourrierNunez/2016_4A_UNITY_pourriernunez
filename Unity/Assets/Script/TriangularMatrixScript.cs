using UnityEngine;
using System.Collections;

public class TriangularMatrixScript<T>
{
    private readonly T[] hiddenStructure;
    private readonly int[] rowIndex;

    public TriangularMatrixScript(int row, int columns)
    {
        rowIndex = new int[row];
        var rowDecal = 0;

        for (var r = 0; r < row; r++)
        {
            rowDecal += columns - r - 1;
        }

        rowIndex[0] = - 1;
        for (var r = 1; r < row; r++)
        {
            rowIndex[r] = columns - r + rowIndex[r - 1];
        }

        hiddenStructure = new T[rowDecal];
    }

    public T Get(int i, int j)
    {
        return hiddenStructure[rowIndex[i] - i + j];
    }

    public void Set(int i, int j, T val)
    {
        hiddenStructure[rowIndex[i] - i + j] = val;
    }

    public int GetHiddenSize()
    {
        return hiddenStructure.Length;
    }
}