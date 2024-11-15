using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Rigidbody[] rbs;
    
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
    }

    public void TriggerRagdoll()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }

    public void DeactivateRagdoll()
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
        }
    }
    
}
