
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float CurrentHealth { get; private set; }

    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private float healAmount = 25.0f;
    
    [HideInInspector] public bool isDead;

    private RagdollManager ragdollManager;

    [SerializeField] private PlayerHealthbar healthBar;

    [SerializeField] private DamageEffect damageEffect;
    
    private void Start()
    {
        CurrentHealth = maxHealth;
        ragdollManager = GetComponent<RagdollManager>();
        damageEffect = GameManager.Instance.transform.GetComponent<DamageEffect>();
        EventManager.Instance.OnFirstAidUsed += Heal;
        healthBar.SetMaxHealth(CurrentHealth);
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnFirstAidUsed -= Heal;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth > 0.0f)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0.0f) PlayerDeath();
            Debug.Log($"Player took {damage} damage, New health is: {CurrentHealth}");
            healthBar.SetHealth(CurrentHealth);
            damageEffect.TriggerDamageEffect(CurrentHealth / maxHealth);
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

