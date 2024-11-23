using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public enum EnemyAudioState : byte
{
    Idle,
    Scream,
    Run,
    Attack,
    Death
}

public class EnemyAudio : MonoBehaviour
{
    private EnemyController controller;
    private AudioSource audioSource;
    private Transform playerHead;
    private AudioLowPassFilter lowPassFilter;
    private float clearFrequency = 22000.0f;
    private float occludedFrequency = 500.0f; // for muffled sound
    private float minOcclusionDistance = 2.0f;
    private float maxOcclusionDistance = 10.0f;

    [SerializeField] private Transform enemyEyes;
    
    [Header("Idle SFX")] [SerializeField] private AudioClip[] idleSFX;
    [Header("Scream SFX")] [SerializeField] private AudioClip[] screamSFX;
    [Header("Run SFX")] [SerializeField] private AudioClip[] runSFX;
    [Header("Attack SFX")] [SerializeField] private AudioClip[] attackSFX;
    [Header("Death SFX")] [SerializeField] private AudioClip[] deathSFX;

    private bool isPaused;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        controller = GetComponent<EnemyController>();
        lowPassFilter.cutoffFrequency = clearFrequency;
        playerHead = GameManager.Instance.playerHead;

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

    private void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        if (controller.CurrentState != controller.Death) CheckOcclusion();
    }

    public IEnumerator PlaySound(EnemyAudioState state)
    {
        AudioClip[] selectedClips = null;
        bool overrideCurrentClip = false;

        switch (state)
        {
            case EnemyAudioState.Idle:
                selectedClips = idleSFX;
                break;
            case EnemyAudioState.Scream:
                selectedClips = screamSFX;
                overrideCurrentClip = true;
                audioSource.volume = 0.8f;

                // Wait while respecting the pause state
                float screamDelay = 1.5f;
                float elapsedTime = 0f;
                while (elapsedTime < screamDelay)
                {
                    if (!isPaused)
                    {
                        elapsedTime += Time.deltaTime;
                    }
                    yield return null;
                }
                break;
            case EnemyAudioState.Run:
                selectedClips = runSFX;
                break;
            case EnemyAudioState.Attack:
                selectedClips = attackSFX;
                break;
            case EnemyAudioState.Death:
                selectedClips = deathSFX;
                overrideCurrentClip = true;
                lowPassFilter.cutoffFrequency = clearFrequency;
                break;
        }

        if (selectedClips != null && selectedClips.Length > 0)
        {
            if (overrideCurrentClip || !audioSource.isPlaying)
            {
                AudioClip clipToPlay = selectedClips[Random.Range(0, selectedClips.Length)];
                audioSource.clip = clipToPlay;
                audioSource.Play();
            }
        }
    }


    public void StopSound()
    {
        audioSource.Stop();
    }
    
    private void CheckOcclusion()
    {
        float distToPlayer = Vector3.Distance(enemyEyes.position, playerHead.position);
        Vector3 dirToPlayer = playerHead.position - enemyEyes.position;
        RaycastHit hit;

        float targetFrequency;

        if (Physics.Raycast(enemyEyes.position, dirToPlayer, out hit))
        {
            
            if (hit.transform.CompareTag(playerHead.tag))
            {
                targetFrequency = clearFrequency;
                //lowPassFilter.cutoffFrequency = clearFrequency;
            }

            else
            {
                float occlusionFactor = Mathf.InverseLerp(minOcclusionDistance, maxOcclusionDistance, distToPlayer);
                targetFrequency = Mathf.Lerp(clearFrequency, occludedFrequency, occlusionFactor);
            }
            
        }
        else
        {
            targetFrequency = clearFrequency;
        }
        
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, targetFrequency, Time.deltaTime * 5f);
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        audioSource.Pause();
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        audioSource.UnPause();
    }

    private void OnGamePaused()
    {
        isPaused = true;
        audioSource.Pause();
    }

    private void OnGameContinued()
    {
        isPaused = false;
        audioSource.UnPause();
    }
}