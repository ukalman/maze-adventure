
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NexusCore : MonoBehaviour
{
    private GameObject interactionText;
    [SerializeField] private GameObject artifactModel;
    [SerializeField] private GameObject energyShield;

    [SerializeField] private Animator coreAnimator;
    
    [SerializeField] private Material transparentMat;
    
    
    private bool isPlayerIn;
    private bool canCollect;

    private bool isPaused;

    private bool isRemoving;
    private bool removed;
    
    private float removingDuration = 8.0f;
    private float fadingOutDuration = 1.0f;

    private void Awake()
    {
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnLightsTurnedOn += OnLightsTurnedOn;
    }

    void Start()
    {
        interactionText = LevelManager.Instance.levelUIManager.interactionText;
    }
    
    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnLightsTurnedOn -= OnLightsTurnedOn;
    }

    private void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;

        if (isPlayerIn && canCollect && !isRemoving && !removed)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isRemoving = true;
                StartCoroutine(NexusCoreRemoval());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (!isPlayerIn && !removed)
            {
                isPlayerIn = true;
                interactionText.SetActive(true);

                if (!LevelManager.Instance.VeinsActivated)
                {
                    interactionText.GetComponent<TMP_Text>().text = "THE NEXUS CORE CANNOT BE REMOVED UNTIL the veins are activated.";
                    canCollect = false;
                }
                else
                {
                    interactionText.GetComponent<TMP_Text>().text = "PRESS \"E\" TO REMOVE THE NEXUS CORE.";
                    canCollect = true;
                }
                
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            if (isPlayerIn)
            {
                interactionText.GetComponent<TMP_Text>().text = "";
                interactionText.SetActive(false);
                isPlayerIn = false;
            }

            if (isRemoving && !removed)
            {
                StartCoroutine(AudioManager.Instance.OnCoreRemovalCancelled());
                coreAnimator.SetBool("isRemoving",false);
                isRemoving = false;
            }
        }
    }
    
    private IEnumerator NexusCoreRemoval()
    {
        interactionText.GetComponent<TMP_Text>().text = "";
        interactionText.SetActive(false);
        
        AudioManager.Instance.OnNexusCoreRemoving();
        coreAnimator.SetBool("isRemoving",true);
        float elapsed = 0.0f;

        while (elapsed < removingDuration)
        {
            if (!isRemoving) yield break;
            
            if (isPaused) yield return null;
            else
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        
        Renderer renderer = artifactModel.GetComponent<Renderer>();
        renderer.material = transparentMat;
        Material material = renderer.material;
        if (material.HasProperty("_Color"))
        {
            Color startColor = material.color;
            float elapsedTime = 0.0f;

            while (elapsedTime < fadingOutDuration)
            {
                if (isPaused) yield return null;
                else
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(startColor.a, 0.0f, elapsedTime / fadingOutDuration);
                    material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                    yield return null;
                }
            }

            material.color = new Color(startColor.r, startColor.g, startColor.b, 0.0f);
        }
        
        EventManager.Instance.InvokeOnNexusCoreObtained();
        Destroy(artifactModel);
        removed = true;
    }

    private void OnDroneCamActivated()
    {
        isPaused = true;
        if (LevelManager.Instance.VeinsActivated) LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
    }

    private void OnGameContinued()
    {
        isPaused = false;
    }

    private void OnLightsTurnedOn()
    {
        energyShield.SetActive(false);
    }
}
