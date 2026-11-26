using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GridController gridController;
    public GameObject unitPrefab;
    public int numUnitsPerSpawn;
    public float moveSpeed;
 
    private List<GameObject> unitsInGame;
 
    private void Awake()
    {
        unitsInGame = new List<GameObject>();
    }
 
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1))
    //     {
    //         SpawnUnits();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.Alpha2))
    //     {
    //         DestroyUnits();
    //     }
    // }
 
    private void FixedUpdate()
    {
        if (gridController.curFlowfield == null) { return; }
        foreach (GameObject unit in unitsInGame)
        {
            var cellBelow = gridController.curFlowfield.GetCellFromWorldPos(unit.transform.position);
            var moveDirection = new Vector2(cellBelow.bestDirection.x, cellBelow.bestDirection.y);
            var unitRB = unit.GetComponent<Rigidbody2D>();
            unitRB.linearVelocity = moveDirection * moveSpeed;
        }
    }

    public void Spawn()
    {
        SpawnUnits();
    }
    
    public void Despawn()
    {
        DestroyUnits();
    }
    
    private void SpawnUnits()
    {
        var gridSize = gridController.gridSize;
        var nodeRadius = gridController.cellRadius;
        var maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius - 10, gridSize.y * nodeRadius * 2 + nodeRadius - 5);
        var colMask = LayerMask.GetMask("Impassible", "Units");
        Vector2 newPos;
        for (var i = 0; i < numUnitsPerSpawn; i++)
        {
            var newUnit = Instantiate(unitPrefab);
            newUnit.transform.parent = transform;
            unitsInGame.Add(newUnit);
            do
            {
                newPos = new Vector2(Random.Range(0, maxSpawnPos.x), Random.Range(0, maxSpawnPos.y));
                newUnit.transform.position = newPos;
            }
            while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
        }
    }
 
    private void DestroyUnits()
    {
        foreach (var go in unitsInGame)
        {
            Destroy(go);
        }
        unitsInGame.Clear();
    }
}
