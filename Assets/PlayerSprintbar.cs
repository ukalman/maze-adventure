using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprintbar : MonoBehaviour
{
    private Slider slider;
    public Image fill;
    private Coroutine currentCoroutine; // Track current coroutine to avoid overlap

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetValue(float value)
    { 
        // Stop any running coroutine to prevent multiple overlapping transitions
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Start a new coroutine for smooth transition
        currentCoroutine = StartCoroutine(SmoothHealthTransition(value));
    }

    private IEnumerator SmoothHealthTransition(float targetValue)
    {
        float initialHealth = slider.value;
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(initialHealth, targetValue, elapsed / duration);
            yield return null;
        }
        
        slider.value = targetValue;
        currentCoroutine = null; 
    }
}