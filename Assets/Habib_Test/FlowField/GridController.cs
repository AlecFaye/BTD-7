using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GridController : MonoBehaviour
{
    [Header("Test Targets")]
    [SerializeField] private Transform destinationTarget;
    
    [Header("Flowfield Grid Variables")]
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public Flowfield CurFlowfield;

    private void InitializeFlowField()
    {
        CurFlowfield = new Flowfield(cellRadius, gridSize);
        CurFlowfield.CreateGrid();
    }

    private void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     InitializeFlowField();
        // }
    }

    public void TestCreateFlowfield()
    {
        InitializeFlowField();
        CurFlowfield.CreateCostField();

        var target = new Vector2(destinationTarget.position.x, destinationTarget.position.y); // TODO: Adjust this to be less jank
        
        var destinationCell = CurFlowfield.GetCellFromWorldPos(target);
        CurFlowfield.CreateIntegrationField(destinationCell);
    }
}
