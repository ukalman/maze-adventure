using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBloom : MonoBehaviour
{
    [SerializeField] private float defaultBloomAngle = 3.0f;
    [SerializeField] private float walkBloomMultiplier = 1.5f;
    [SerializeField] private float crouchBloomMultiplier = 0.5f;
    [SerializeField] private float sprintBloomMultiplier = 2.0f;
    [SerializeField] private float aimBloomMultiplier = 0.5f;

    private MovementStateManager movement;
    private AimStateManager aiming;

    private float currentBloom;
    
    void Start()
    {
        movement = GetComponentInParent<MovementStateManager>();
        aiming = GetComponentInParent<AimStateManager>();
    }

    public Vector3 BloomAngle(Transform barrelPos)
    {
        if (movement.CurrentState == movement.Idle) currentBloom = defaultBloomAngle;
        else if (movement.CurrentState == movement.Walk) currentBloom = defaultBloomAngle * walkBloomMultiplier;
        else if (movement.CurrentState == movement.Walk) currentBloom = defaultBloomAngle * sprintBloomMultiplier;
        else if (movement.CurrentState == movement.Crouch)
        {
            if (movement.dir.magnitude == 0.0f) currentBloom = defaultBloomAngle * crouchBloomMultiplier;
            else currentBloom = defaultBloomAngle * crouchBloomMultiplier * walkBloomMultiplier;
        }

        if (aiming.CurrentState == aiming.Aim) currentBloom *= aimBloomMultiplier;

        float randX = Random.Range(-currentBloom, currentBloom);
        float randY = Random.Range(-currentBloom, currentBloom);
        float randZ = Random.Range(-currentBloom, currentBloom);

        Vector3 randomRotation = new Vector3(randX, randY, randZ);
        return barrelPos.localEulerAngles + randomRotation;

    }
    
   
   
}
