using UnityEngine;

// HK - This script will control the elements of a grid
public class Cell
{
    public Vector3 worldPos;
    public Vector2Int gridIndex;
    public byte cost;
    public ushort bestCost;
    
    public Cell(Vector3 _worldPos, Vector2Int _gridIndex)
    {
        worldPos = _worldPos;
        gridIndex = _gridIndex;
        cost = 1;
        bestCost = ushort.MaxValue; // Set to the highest value, so that a lower value can overwrite it
    }

    public void IncreaseCost(int _amount)
    {
        if (cost == byte.MaxValue) { return; }
        if (_amount + cost >= byte.MaxValue) { cost = byte.MaxValue; }
        else { cost += (byte)_amount; }
    }
}
