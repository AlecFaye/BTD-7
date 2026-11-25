using UnityEngine;

// HK - This script will control the elements of a grid
public class Cell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public byte cost;

    public Cell(Vector3 worldPos, Vector2Int gridIndex)
    {
        this.worldPos = worldPos;
        this.gridIndex = gridIndex;
    }

    public void IncreaseCost(int amount)
    {
        if (cost == byte.MaxValue) { return; }
        if (amount + cost >= byte.MaxValue) { cost = byte.MaxValue; }
        else { cost += (byte)amount; }
    }
}
