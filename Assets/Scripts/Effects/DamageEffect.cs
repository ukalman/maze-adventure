using System;
using System.Collections;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    private PlayerHealth playerHealth;

    [SerializeField] private Material damageEffectMaterial; // Reference to the custom shader material
    private Coroutine damageEffectCoroutine;

    private bool isPaused;
    private bool isLowHealthWarningActive;
    private bool isDamageEffectActive;
    private float currentHealthPercentage;

    void Start()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;

        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();

        // Ensure the damage effect material starts with zero intensity
        if (damageEffectMaterial != null)
        {
            damageEffectMaterial.SetFloat("_Intensity", 0f);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
    }

    public void TriggerDamageEffect()
    {
        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
        }

        damageEffectCoroutine = StartCoroutine(DamageEffectCoroutine());
    }

    private IEnumerator DamageEffectCoroutine()
    {
        isDamageEffectActive = true; // Flag the damage effect as active

        float flashIntensity = 1.0f; // Maximum flash intensity
        float duration = 0.5f;
        float fadeOutTime = 1.0f;

        // Apply red flash by setting intensity
        damageEffectMaterial.SetFloat("_Intensity", flashIntensity);

        // Wait for the duration, respecting pause state
        float elapsedDuration = 0f;
        while (elapsedDuration < duration)
        {
            if (isPaused)
            {
                yield return null; // Wait while paused
            }
            else
            {
                elapsedDuration += Time.deltaTime;
                yield return null;
            }
        }

        // Fade out the effect if not in low health
        if (!isLowHealthWarningActive)
        {
            float elapsedFadeOut = 0f;
            while (elapsedFadeOut < fadeOutTime)
            {
                if (isPaused)
                {
                    yield return null; // Wait while paused
                }
                else
                {
                    elapsedFadeOut += Time.deltaTime;
                    float intensity = Mathf.Lerp(flashIntensity, 0f, elapsedFadeOut / fadeOutTime);
                    damageEffectMaterial.SetFloat("_Intensity", intensity);
                    yield return null;
                }
            }

            // Reset intensity after fading out
            damageEffectMaterial.SetFloat("_Intensity", 0f);
        }

        isDamageEffectActive = false; // Damage effect has ended
        damageEffectCoroutine = null;
    }

    void Update()
    {
        if (!LevelManager.Instance.LevelInstantiated) return;

        if (isPaused) return;

        UpdateHealthPercentage(playerHealth.CurrentHealth / 100.0f); // Adjust for actual health scaling

        // Check for low health warning, but only if the damage effect is not active
        if (!isDamageEffectActive)
        {
            if (currentHealthPercentage <= 0.25f)
            {
                if (!isLowHealthWarningActive)
                {
                    ActivateLowHealthWarning();
                }
            }
            else
            {
                if (isLowHealthWarningActive)
                {
                    DeactivateLowHealthWarning();
                }
            }
        }
    }

    public void UpdateHealthPercentage(float healthPercentage)
    {
        currentHealthPercentage = healthPercentage;
    }

    private void ActivateLowHealthWarning()
    {
        isLowHealthWarningActive = true;
        damageEffectMaterial.SetFloat("_Intensity", 0.5f); // Constant intensity for low health warning
        damageEffectMaterial.SetColor("_DamageColor", Color.red); // Ensure the red tint is active
    }

    private void DeactivateLowHealthWarning()
    {
        isLowHealthWarningActive = false;

        // Reset values only if the damage effect is not active
        if (!isDamageEffectActive)
        {
            damageEffectMaterial.SetFloat("_Intensity", 0f);
        }
    }

    private void OnDroneCamActivated()
    {
        isPaused = true;
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
    }
}
