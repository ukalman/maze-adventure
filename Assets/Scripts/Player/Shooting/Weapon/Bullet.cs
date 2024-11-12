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
        var enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            Vector3 hitPoint = other.contacts[0].point; // Get the exact point of collision
            Vector3 burstDirection = -transform.forward;

            if (enemyHealth.bloodSplatterPrefab != null)
            {
                ParticleSystem bloodSplatter = Instantiate(enemyHealth.bloodSplatterPrefab, hitPoint, Quaternion.identity);
                bloodSplatter.transform.rotation = Quaternion.LookRotation(burstDirection);
                bloodSplatter.Play();
                Destroy(bloodSplatter.gameObject, bloodSplatter.main.duration);
            }
            
            if (!enemyHealth.isDead)
            {
                enemyHealth.TakeDamage(weapon.damage);
                
                if (enemyHealth.Health <= 0.0f)
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
            else // might not be of use, but still reassurance
            {
                // The enemy is already dead: stop the bullet's velocity to avoid affecting the ragdoll
                Rigidbody bulletRb = GetComponent<Rigidbody>();
                bulletRb.velocity = Vector3.zero;
                bulletRb.isKinematic = true;
            }
            
            Destroy(gameObject);
        }
    }


}
