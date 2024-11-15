using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] private float lookDistance = 15.0f, fov = 120.0f;
    [SerializeField] private Transform enemyEyes;
    private Transform playerHead;
    private PlayerHealth playerHealth;
    
    private Transform originalEnemyEyes;
    private EnemyController enemyController;
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameManager.Instance.playerHead;
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (PlayerSeen())
        {
            enemyController.distToPlayer = Vector3.Distance(transform.position, playerHead.position);
            enemyController.playerSeen = true;
            //Debug.Log("Player is seen: ");
        }
        else
        {
            enemyController.playerSeen = false;
            //Debug.Log("Where are you... ");
        }
        
        // player'ı önceden görmüşse ama şimdi görmüyorsa birkaç saniye halen görmesini, veya son gördüğü yeri kaydetmesini sağla
    }

    public bool PlayerSeen()
    {
        if (playerHealth != null && playerHealth.isDead) return false;
        
        enemyEyes.LookAt(playerHead.position);
        Debug.DrawRay(enemyEyes.position, enemyEyes.forward, Color.red);
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance)
        {
            return false;
        }

        Vector3 dirToPlayer = (playerHead.position - enemyEyes.position).normalized;

        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, dirToPlayer);

        

        if (enemyController.CurrentState != enemyController.Attack)
        {
            if (angleToPlayer > fov * 0.5f)
            {
                return false; // 60 degrees for each side
            }
        }
        

        RaycastHit hit;
        if (Physics.Raycast(enemyEyes.position, enemyEyes.forward, out hit, lookDistance))
        {
            if (hit.transform == null)
            {
                return false;
            }

            if (hit.transform.CompareTag(playerHead.tag))
            {
                Debug.DrawRay(enemyEyes.position, hit.point, Color.green);
                return true;
            }
            
        }

        return false;

    }
}
