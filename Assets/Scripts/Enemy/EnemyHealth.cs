using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health;
    
    public void TakeDamage(float damage)
    {
        if (health > 0.0f)
        {
            health -= damage;
            if (health <= 0.0f) EnemyDeath();
            Debug.Log("Hit!");   
        }
    }

    private void EnemyDeath()
    {
        Debug.Log("Enemy died!");
    }
    
}
