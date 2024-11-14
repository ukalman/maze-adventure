using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageEffect : MonoBehaviour
{
    public Volume volume; // Reference to your Global Volume
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private Coroutine damageEffectCoroutine;

    void Start()
    {
        // Get the Vignette and Color Adjustments components
        if (volume.profile.TryGet(out vignette) && volume.profile.TryGet(out colorAdjustments))
        {
            vignette.intensity.value = 0f; // Set initial vignette intensity to 0
            vignette.color.value = Color.red; // Set vignette color to red
            colorAdjustments.postExposure.value = 0f; // Set initial exposure to 0
        }
    }

    public void TriggerDamageEffect(float healthPercentage)
    {
        // Start or restart the damage effect coroutine
        if (damageEffectCoroutine != null)
        {
            StopCoroutine(damageEffectCoroutine);
        }
        damageEffectCoroutine = StartCoroutine(DamageEffectCoroutine(healthPercentage));
    }

    private IEnumerator DamageEffectCoroutine(float healthPercentage)
    {
        // Adjust intensity based on health (low health -> higher intensity)
        float intensityMultiplier = 1f - healthPercentage; // e.g., if health is 50%, multiplier is 0.5

        float duration = 0.5f; // Duration of the effect
        float fadeOutTime = 1.0f; // Fade-out time

        // Calculate vignette intensity based on health
        float targetVignetteIntensity = Mathf.Lerp(0.2f, 0.5f, intensityMultiplier); // Adjust between 0.2 and 0.5
        float targetExposure = Mathf.Lerp(0f, 0.5f, intensityMultiplier); // Slightly increase exposure

        // Apply the initial intense values
        vignette.intensity.value = targetVignetteIntensity;
        colorAdjustments.postExposure.value = targetExposure;

        // Wait for the effect to last
        yield return new WaitForSeconds(duration);

        // Gradually fade out the effect
        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetVignetteIntensity, 0f, elapsed / fadeOutTime);
            colorAdjustments.postExposure.value = Mathf.Lerp(targetExposure, 0f, elapsed / fadeOutTime);
            yield return null;
        }

        // Reset the intensity after fade-out
        vignette.intensity.value = 0f;
        colorAdjustments.postExposure.value = 0f;

        damageEffectCoroutine = null;
    }
}
