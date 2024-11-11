using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;
    
    void Start()
    {
       Destroy(gameObject, timeToDestroy); 
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponentInParent<EnemyHealth>())
        {
            var enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
            enemyHealth.TakeDamage(weapon.damage);
            
            if (enemyHealth.Health <= 0.0f && !enemyHealth.isDead)
            {
                var enemyController = other.gameObject.GetComponentInParent<EnemyController>();

                if (enemyController != null && (enemyController.CurrentState != enemyController.Run && enemyController.CurrentState != enemyController.Scream))
                {
                    Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                    rb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
                    enemyHealth.isDead = true;
                }
                
            }
            
        }
        Destroy(gameObject);
    }
}
