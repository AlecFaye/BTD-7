using System;
using UnityEngine;

public class TempEnemyDestroyer : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) 
        { 
            Debug.Log(collision.gameObject.name);
        } 
    }
}
