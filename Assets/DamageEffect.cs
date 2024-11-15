using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageEffect : MonoBehaviour
{
    public Volume volume; 
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private Coroutine damageEffectCoroutine;

    void Start()
    {
        if (volume.profile.TryGet(out vignette) && volume.profile.TryGet(out colorAdjustments))
        {
            vignette.intensity.value = 0.0f; 
            vignette.color.value = Color.red;
            colorAdjustments.postExposure.value = 0.0f; 
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
        float intensityMultiplier = 1f - healthPercentage; 

        float duration = 0.5f; 
        float fadeOutTime = 1.0f; 

        // Calculate vignette intensity based on health
        float targetVignetteIntensity = Mathf.Lerp(0.2f, 0.5f, intensityMultiplier); 
        float targetExposure = Mathf.Lerp(0f, 0.5f, intensityMultiplier); // Slightly increase exposure
        
        vignette.intensity.value = targetVignetteIntensity;
        colorAdjustments.postExposure.value = targetExposure;
        
        yield return new WaitForSeconds(duration);
        
        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetVignetteIntensity, 0f, elapsed / fadeOutTime);
            colorAdjustments.postExposure.value = Mathf.Lerp(targetExposure, 0f, elapsed / fadeOutTime);
            yield return null;
        }
        
        vignette.intensity.value = 0f;
        colorAdjustments.postExposure.value = 0f;

        damageEffectCoroutine = null;
    }
}
