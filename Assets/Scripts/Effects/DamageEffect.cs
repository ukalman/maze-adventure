using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Game Manager has it
public class DamageEffect : MonoBehaviour
{
    private PlayerHealth playerHealth;
    
    public Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
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
        
        if (volume.profile.TryGet(out vignette) && volume.profile.TryGet(out colorAdjustments))
        {
            vignette.intensity.value = 0.0f;
            vignette.color.value = Color.red;
            colorAdjustments.postExposure.value = 0.0f;
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

        float flashIntensity = 0.5f; // Constant flash intensity
        float duration = 0.5f;
        float fadeOutTime = 1.0f;

        // Apply red flash
        vignette.intensity.value = flashIntensity;

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
                    vignette.intensity.value = Mathf.Lerp(flashIntensity, 0f, elapsedFadeOut / fadeOutTime);
                    yield return null;
                }
            }

            // Reset vignette intensity after fading out
            vignette.intensity.value = 0f;
        }

        isDamageEffectActive = false; // Damage effect has ended
        damageEffectCoroutine = null;
    }

    void Update()
    {
        if (!LevelManager.Instance.LevelInstantiated) return;
        
        if (isPaused) return;
        
        UpdateHealthPercentage(playerHealth.CurrentHealth / 100.0f); // change the magic number later
        
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
        vignette.intensity.value = 0.5f; // Constant intensity for low health warning
        colorAdjustments.postExposure.value = 0.5f; // Slight exposure boost for dramatic effect
    }

    private void DeactivateLowHealthWarning()
    {
        isLowHealthWarningActive = false;

        // Reset values only if the damage effect is not active
        if (!isDamageEffectActive)
        {
            vignette.intensity.value = 0f;
            colorAdjustments.postExposure.value = 0f;
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
