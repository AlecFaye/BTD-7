using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public Flowfield CurFlowfield;

    private void InitializeFlowField()
    {
        CurFlowfield = new Flowfield(cellRadius, gridSize);
        CurFlowfield.CreateGrid();
    }

    private void Start()
    {
        InitializeFlowField();
        CurFlowfield.CreateCostField();
    }
    
    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     InitializeFlowField();
        // }
    }
}
