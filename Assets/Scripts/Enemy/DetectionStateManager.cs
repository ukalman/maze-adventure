using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [SerializeField] private float lookDistance = 30.0f, fov = 120.0f;
    [SerializeField] private Transform enemyEyes;
    private Transform playerHead;
    
    // Start is called before the first frame update
    void Start()
    {
        playerHead = GameManager.Instance.playerHead;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (PlayerSeen())
        {
            Debug.Log("Player is seen: ");
        }
        else
        {
            Debug.Log("Where are you... ");
        }
    }

    public bool PlayerSeen()
    {
        Debug.DrawRay(enemyEyes.position, enemyEyes.forward, Color.red);
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance) return false;

        Vector3 dirToPlayer = (playerHead.position - enemyEyes.position).normalized;

        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, dirToPlayer);

        if (angleToPlayer > fov * 0.5f) return false; // 60 degrees for each side
        
        enemyEyes.LookAt(playerHead.position);

        RaycastHit hit;
        if (Physics.Raycast(enemyEyes.position, enemyEyes.forward, out hit, lookDistance))
        {
            if (hit.transform == null) return false;

            if (hit.transform.CompareTag(playerHead.tag))
            {
                Debug.Log("YESSSS");
                Debug.DrawRay(enemyEyes.position, hit.point, Color.green);
                return true;
            }
        }

        return false;

    }
}
