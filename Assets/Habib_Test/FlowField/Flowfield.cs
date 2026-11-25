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
                var worldPos = new Vector2(x * cellDiameter + cellRadius, y * cellDiameter + cellRadius);
                
                grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    } 
}
