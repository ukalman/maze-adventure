using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public Transform recoilFollowPos;
    [SerializeField] private float kickBackAmount = -1.0f;
    [SerializeField] private float kickBackSpeed = 10.0f, returnSpeed = 20.0f;
    private float currentRecoilPosition, finalRecoilPosition;
    
    void Update()
    {
        currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0.0f, returnSpeed * Time.deltaTime);
        finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickBackSpeed * Time.deltaTime);
        recoilFollowPos.localPosition = new Vector3(0.0f, 0.0f, finalRecoilPosition);
    }

    public void TriggerRecoil() => currentRecoilPosition += kickBackAmount;
}
