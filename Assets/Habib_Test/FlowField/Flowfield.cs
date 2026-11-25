using System;
using UnityEngine;


// HK - This script will control the Flowfield
// Requires - Cell.cs
public class Flowfield
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    
    private float cellDiameter;

    public Flowfield(float cellRadius, Vector2Int gridSize)
    {
        this.cellRadius = cellRadius;
        this.cellDiameter = cellRadius * 2;
        this.gridSize = gridSize;
    }

    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var worldPos = new Vector2(x * cellDiameter + cellRadius, y * cellDiameter + cellRadius); // TODO: Move the creation start point to the bottom left of the screen
                
                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

    public void CreateCostField()
    {
        var cellHalfExtents = Vector3.one * cellRadius;
        var terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain", "MainPath"); // 8, 9, 10

        foreach (var cell in grid)
        {
            var obstacles = Physics.OverlapBox(cell.worldPos, cellHalfExtents, Quaternion.identity, terrainMask);
            var hasIncreasedCost = false;
            foreach (var obstacle in obstacles)
            {
                if (obstacle.gameObject.layer == 8) // If the obstacle is impassable, deny any movement
                {
                    cell.IncreaseCost(byte.MaxValue);
                }
                else if (!hasIncreasedCost && obstacle.gameObject.layer == 9) // If the obstacle is Rough Terrain, add a cost to movement
                {
                    cell.IncreaseCost(10); // costs 10 more to move through rough terrain
                    hasIncreasedCost = true; // Added here so the cost doesn't get added again from another piece of overlapping terrain
                }
                else if (obstacle.gameObject.layer == 10)
                {
                    // Debug.Log("Path");
                }
            }
        }
    }
}
