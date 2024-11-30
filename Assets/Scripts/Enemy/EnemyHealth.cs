using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float Health { get; private set; }
    private EnemyController controller;
    private DetectionStateManager detectionStateManager;
    [HideInInspector] public bool isDead;

    private RagdollManager ragdollManager;
    
    public ParticleSystem bloodSplatterPrefab;

    [SerializeField] private GameObject minimapTile;
    
    private void Start()
    {
        bloodSplatterPrefab.Play();
        bloodSplatterPrefab.Stop();
        controller = GetComponent<EnemyController>();
        detectionStateManager = GetComponent<DetectionStateManager>();
        ragdollManager = GetComponent<RagdollManager>();
        ResetHealth();
        
    }

    private void ResetHealth()
    {
        switch (LevelManager.Instance.GetGameDifficulty())
        {
            case GameDifficulty.EASY:
                Health = 100.0f;
                break;
            case GameDifficulty.MODERATE:
                Health = 120.0f;
                break;
            case GameDifficulty.HARD:
                Health = 150.0f;
                break;
        }
    }
    
    public void ResetModule()
    {
        isDead = false;
        ResetHealth();
        int aliveEnemyLayer = LayerMask.NameToLayer("AliveEnemy");
        foreach (Transform child in transform)
        {
            if (child.CompareTag("MinimapTile")) child.gameObject.layer = LayerMask.NameToLayer("MinimapTile");
            Extensions.SetLayerRecursively(child.gameObject, aliveEnemyLayer);
        }
        
    }
    
    public void TakeDamage(float damage)
    {
        if (Health > 0.0f)
        {
            detectionStateManager.playerHeardRecently = true;
            if (controller.CurrentState == controller.Idle) damage *= 1.2f;
            Health -= damage;
            if (Health <= 0.0f) EnemyDeath();
        }
    }

    private void EnemyDeath()
    {
        //controller.isDead = true;

        minimapTile.SetActive(false);
        
        if (controller.CurrentState != controller.Run && controller.CurrentState != controller.Scream)
        {
            controller.anim.enabled = false;
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
            Extensions.SetLayerRecursively(child.gameObject, deadEnemyLayer);
        }
        // isDead is set to true in Bullet script
    }
    
}
