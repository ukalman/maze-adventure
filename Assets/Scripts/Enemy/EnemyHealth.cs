using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health { get; private set; }
    private EnemyController enemyController;
    [HideInInspector] public bool isDead;

    private RagdollManager ragdollManager;
    
    private void Start()
    {
        Health = 100.0f;
        enemyController = GetComponent<EnemyController>();
        ragdollManager = GetComponent<RagdollManager>();
    }

    public void TakeDamage(float damage)
    {
        if (Health > 0.0f)
        {
            Health -= damage;
            if (Health <= 0.0f) EnemyDeath();

            else Debug.Log("Hit!");   
        }
    }

    private void EnemyDeath()
    {
        //enemyController.isDead = true;

        if (enemyController.CurrentState != enemyController.Run && enemyController.CurrentState != enemyController.Scream)
        {
            Destroy(enemyController.anim);
            ragdollManager.TriggerRagdoll();
        }

        else
        {
            Debug.Log("Ow yeah, he is running!");
            isDead = true;
        }
        
        // isDead is set to true in Bullet script
        Debug.Log("Enemy died!");
    }
    
}
