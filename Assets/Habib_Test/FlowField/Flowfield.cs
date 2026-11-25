using System;
using System.Collections.Generic;
using UnityEngine;


// HK - This script will control the Flowfield
// Requires - Cell.cs
public class Flowfield
{
    public Cell[,] grid { get; private set; }
    public Vector2Int gridSize { get; private set; }
    public float cellRadius { get; private set; }
    public Cell destinationCell;
    private float cellDiameter;

    public Flowfield(float _cellRadius, Vector2Int _gridSize)
    {
        cellRadius = _cellRadius;
        cellDiameter = _cellRadius * 2;
        gridSize = _gridSize;
    }

    public void CreateGrid()
    {
        grid = new Cell[gridSize.x, gridSize.y];

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var worldPos = new Vector2(x * cellDiameter + cellRadius - 10, y * cellDiameter + cellRadius - 5); // TODO: Make it so that the moving is automatically calculated, Maybe use the transform as the anchor

                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

    // This method will create set the cost of moving into each cell.
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
                else if (obstacle.gameObject.layer == 10) // TODO: Implement a system to have path cost less when going through
                {
                    // Debug.Log("Path");
                }
            }
        }
    }

    public void CreateIntegrationField(Cell _destinationCell)
    {
        destinationCell = _destinationCell;

        destinationCell.cost = 0;
        destinationCell.bestCost = 0;
        
        var cellsToCheck = new Queue<Cell>();
        
        cellsToCheck.Enqueue(_destinationCell);
        
        while (cellsToCheck.Count > 0)
        {
            var curCell = cellsToCheck.Dequeue();
            var curNeighbors = GetNeighborCells(curCell.gridIndex, GridDirection.CardinalDirections);

            foreach (var curNeighbor in curNeighbors)
            {
                if (curNeighbor.cost == byte.MaxValue) { continue; } // If the neighboring cell is impassable, ignore it

                if (curNeighbor.cost + curCell.cost < curNeighbor.bestCost)
                {
                    curCell.bestCost = (ushort)(curNeighbor.cost + curCell.cost);
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    private List<Cell> GetNeighborCells(Vector2Int _gridIndex, List<GridDirection> _directions)
    {
        var neighborCells = new List<Cell>();

        foreach (Vector2Int curDirection in _directions)
        {
            var newNeighbor = GetCellAtRelativePos(_gridIndex, curDirection);

            if (newNeighbor != null)
            {
                neighborCells.Add(newNeighbor);
            }
        }
        
        return neighborCells;
    }

    private Cell GetCellAtRelativePos(Vector2Int _originPos, Vector2Int _relativePos)
    {
        var cellPos = _originPos + _relativePos;

        if (cellPos.x < 0 || cellPos.x >= gridSize.x || cellPos.y < 0 || cellPos.y >= gridSize.y)
        {
            return null;
        }
        
        return grid[cellPos.x, cellPos.y];
    }

    public Cell GetCellFromWorldPos(Vector2 _worldPos)
    {
        var percentX = _worldPos.x / (gridSize.x * cellDiameter);
        var percentY = _worldPos.y / (gridSize.y * cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        
        var x = Mathf.Clamp(Mathf.FloorToInt(percentX * gridSize.x), 0, gridSize.x - 1);
        var y = Mathf.Clamp(Mathf.FloorToInt(percentY * gridSize.y), 0, gridSize.y - 1);
        
        return grid[x, y];
    }
}
