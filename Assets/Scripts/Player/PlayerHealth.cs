
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float CurrentHealth { get; private set; }

    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float healAmount = 25.0f;
    
    [HideInInspector] public bool isDead;

    private RagdollManager ragdollManager;

    [SerializeField] private PlayerHealthbar healthBar;
    
    private void Start()
    {
        CurrentHealth = maxHealth;
        ragdollManager = GetComponent<RagdollManager>();
        EventManager.Instance.OnFirstAidUsed += Heal;
        healthBar.SetMaxHealth(CurrentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth > 0.0f)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0.0f) PlayerDeath();
            Debug.Log($"Player took {damage} damage, New health is: {CurrentHealth}");
            healthBar.SetHealth(CurrentHealth);
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
        CurrentHealth += healAmount;
        Debug.Log($"Healed! New health is {CurrentHealth}");
        healthBar.SetHealth(CurrentHealth);
    }
    

}

