using UnityEngine;

public class DroneCamShake : MonoBehaviour
{
    [SerializeField] private float positionShakeAmount = 0.02f; 
    [SerializeField] private float shakeSpeed = 1.0f; 

    private Vector3 basePosition;
    private float fixedY; 

    void Start()
    {
        basePosition = transform.localPosition;
        fixedY = transform.localPosition.y;
    }

    void LateUpdate()
    {
        basePosition = transform.localPosition;
        basePosition.y = fixedY;

        // Add subtle position shake using Perlin Noise
        Vector3 positionOffset = new Vector3(
            (Mathf.PerlinNoise(Time.time * shakeSpeed, 0.0f) * 2.0f - 1.0f) * positionShakeAmount,
            0, // No Y shake
            (Mathf.PerlinNoise(0.0f, Time.time * shakeSpeed) * 2.0f - 1.0f) * positionShakeAmount
        );

        
        transform.localPosition = basePosition + positionOffset;
    }
}