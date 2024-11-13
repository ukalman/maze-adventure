using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private Gradient gradient;
    public Image fill;
    private Coroutine currentCoroutine; // Track current coroutine to avoid overlap

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1.0f);
    }

    public void SetHealth(float health)
    { 
        // Stop any running coroutine to prevent multiple overlapping transitions
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Start a new coroutine for smooth transition
        currentCoroutine = StartCoroutine(SmoothHealthTransition(health));
    }

    private IEnumerator SmoothHealthTransition(float targetHealth)
    {
        float initialHealth = slider.value;
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(initialHealth, targetHealth, elapsed / duration);
            fill.color = gradient.Evaluate(slider.normalizedValue);
            yield return null;
        }
        
        slider.value = targetHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        currentCoroutine = null; // Reset coroutine tracker
    }
}