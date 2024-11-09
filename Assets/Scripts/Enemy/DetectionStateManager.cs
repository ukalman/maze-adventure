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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool PlayerSeen()
    {
        return false;
    }
}
