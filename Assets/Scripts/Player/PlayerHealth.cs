
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
        //damageEffect = GameManager.Instance.transform.GetComponent<DamageEffect>();
        EventManager.Instance.OnFirstAidUsed += Heal;
        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;
        healthBar.SetMaxHealth(CurrentHealth);
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnFirstAidUsed -= Heal;
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth > 0.0f)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0.0f)
            {
                AudioManager.Instance.OnKilled();
                PlayerDeath();
            } else AudioManager.Instance.OnDamageTaken();
            healthBar.SetHealth(CurrentHealth);
            damageEffect.TriggerDamageEffect();
        }
    }

    private void PlayerDeath()
    {
        isDead = true;
        transform.GetComponent<Animator>().enabled = false;
        transform.GetComponent<MovementStateManager>().enabled = false;
        transform.GetComponent<ActionStateManager>().enabled = false;
        transform.GetComponent<AimStateManager>().enabled = false;
        transform.GetComponent<WeaponClassManager>().enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
        /*
        Destroy(transform.GetComponent<MovementStateManager>());
        Destroy(transform.GetComponent<ActionStateManager>());
        Destroy(transform.GetComponent<AimStateManager>());
        Destroy(transform.GetComponent<WeaponClassManager>());
        Destroy(transform.GetComponent<CharacterController>());
        */
        ragdollManager.TriggerRagdoll();
        
        EventManager.Instance.InvokeOnPlayerDied();
    }

    private void OnCountdownEnded()
    {
        isDead = true;
        transform.GetComponent<Animator>().enabled = false;
        transform.GetComponent<MovementStateManager>().enabled = false;
        transform.GetComponent<ActionStateManager>().enabled = false;
        transform.GetComponent<AimStateManager>().enabled = false;
        transform.GetComponent<WeaponClassManager>().enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
    }
    
    private void Heal()
    {
        // Heal sound
        AudioManager.Instance.OnHealed();
        CurrentHealth += healAmount;
        healthBar.SetHealth(CurrentHealth);
    }
    
}

