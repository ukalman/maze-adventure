using System;
using System.Collections;
using UnityEngine;

public class SprayPaintManager : MonoBehaviour
{
    [SerializeField] private Camera playerCamera; 
    [SerializeField] private float maxSprayDistance = 5.0f; 
    [SerializeField] private GameObject sprayPaintPrefab;
    
    [SerializeField] private int maxSprays = 5;

    [SerializeField] private float decalOffset = 0.01f;
    
    private int currentSprays = 0;
    
    private bool isPaused;

    private bool currentlyPainting;
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

    void Update()
    {
        
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        if (Input.GetKeyDown(KeyCode.Q) && !currentlyPainting) 
        {
            StartCoroutine(SprayPaint());
        }
    }

    IEnumerator SprayPaint()
    {
        if (currentSprays < maxSprays)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxSprayDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    currentlyPainting = true;
                    AudioManager.Instance.OnSprayPaintUsed();
                    yield return new WaitForSeconds(1.0f);
                    PlaceSprayMark(hit.point, hit.normal);
                    currentSprays++; 
                }
            }
        }
    }

    void PlaceSprayMark(Vector3 position, Vector3 normal)
    {
        GameObject sprayMark = Instantiate(sprayPaintPrefab, position, Quaternion.LookRotation(normal));
        sprayMark.transform.position += normal * decalOffset;
        currentlyPainting = false;
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