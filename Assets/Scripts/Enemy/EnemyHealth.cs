using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health { get; private set; }
    private RagdollManager ragdollManager;
    [HideInInspector] public bool isDead;

    private void Start()
    {
        Health = 100.0f;
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
        ragdollManager.TriggerRagdoll();
        // isDead is set to true in Bullet script
        Debug.Log("Enemy died!");
    }
    
}
