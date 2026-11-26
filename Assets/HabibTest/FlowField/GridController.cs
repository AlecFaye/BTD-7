using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GridController : MonoBehaviour
{
    [Header("Test Targets")]
    [SerializeField] private Transform destinationTarget;
    [SerializeField] private GridDebug gridDebug;
    
    [Header("Flowfield Grid Variables")]
    public Transform CreationCorner;
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public Flowfield curFlowfield;
    
    

    private void InitializeFlowField()
    {
        curFlowfield = new Flowfield(cellRadius, gridSize);

        Vector2 cornerTarget;
        cornerTarget.x = CreationCorner.position.x;
        cornerTarget.y = CreationCorner.position.y;
        
        curFlowfield.CreateGrid(cornerTarget);
        
        gridDebug.SetFlowField(curFlowfield);
    }

    public void TestCreateFlowfield()
    {
        InitializeFlowField();
        curFlowfield.CreateCostField();

        Vector2 target;
        target.x = destinationTarget.position.x - CreationCorner.position.x;
        target.y = destinationTarget.position.y - CreationCorner.position.y;

        // Vector2 cornerOffset;
        // cornerOffset.x =;
        // cornerOffset.y = CreationCorner.position.y;
        
        var destinationCell = curFlowfield.GetCellFromWorldPos(target);
        
        curFlowfield.CreateIntegrationField(destinationCell);
        
        curFlowfield.CreateFlowfield();
        gridDebug.DrawFlowField();
    }
}
