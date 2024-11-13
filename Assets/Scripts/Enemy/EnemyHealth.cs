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

    public ParticleSystem bloodSplatterPrefab;
    
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
            isDead = true;
        }
        // gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        
        int deadEnemyLayer = LayerMask.NameToLayer("DeadEnemy");
        foreach (Transform child in transform)
        {
            GameManager.Instance.SetLayerRecursively(child.gameObject, deadEnemyLayer);
        }
        // isDead is set to true in Bullet script
    }
    
}
