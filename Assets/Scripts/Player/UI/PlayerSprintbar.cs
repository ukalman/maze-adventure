using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSprintbar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public Image fill;
    private Coroutine currentCoroutine; // Track current coroutine to avoid overlap

    private bool isPaused;
    
    private void Start()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
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
        currentCoroutine = StartCoroutine(SmoothSprintRefillTransition(value));
    }

    private IEnumerator SmoothSprintRefillTransition(float targetValue)
    {
        float initialHealth = slider.value;
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            while (isPaused) yield return null;
            
            elapsed += Time.deltaTime;
            slider.value = Mathf.Lerp(initialHealth, targetValue, elapsed / duration);
            yield return null;
        }
        
        slider.value = targetValue;
        currentCoroutine = null; 
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }
}