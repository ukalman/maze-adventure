
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float Health { get; private set; }
    [SerializeField] private float healAmount = 25.0f;
    
    [HideInInspector] public bool isDead;

    private RagdollManager ragdollManager;
    
    private void Start()
    {
        Health = 100.0f;
        ragdollManager = GetComponent<RagdollManager>();
        EventManager.Instance.OnFirstAidUsed += Heal;
    }

    public void TakeDamage(float damage)
    {
        if (Health > 0.0f)
        {
            Health -= damage;
            if (Health <= 0.0f) PlayerDeath();
            Debug.Log($"Player took {damage} damage, New health is: {Health}");
        }
    }

    private void PlayerDeath()
    {
        isDead = true;
        Debug.Log("PLAYER DIED!");
        transform.GetComponent<Animator>().enabled = false;
        Destroy(transform.GetComponent<MovementStateManager>());
        Destroy(transform.GetComponent<ActionStateManager>());
        Destroy(transform.GetComponent<AimStateManager>());
        Destroy(transform.GetComponent<WeaponClassManager>());
        Destroy(transform.GetComponent<CharacterController>());
        ragdollManager.TriggerRagdoll();
    }

    private void Heal()
    {
        Health += healAmount;
        Debug.Log($"Healed! New health is {Health}");
    }
    

}

