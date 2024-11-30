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
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;
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

    [Header("Footstep SFX")] 
    [SerializeField] private AudioClip footstepSFX_1;
    [SerializeField] private AudioClip footstepSFX_2;

    private bool isPaused;
    
    private void Start()
    {
        ResetVolume();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        controller = GetComponent<EnemyController>();
        lowPassFilter.cutoffFrequency = clearFrequency;
        playerHead = GameManager.Instance.playerHead;

        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;

        EventManager.Instance.OnPlayerDied += OnPlayerDied;
        EventManager.Instance.OnCountdownEnded += OnCountdownEnded;

        EventManager.Instance.OnVolumeChanged += ResetVolume;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnPlayerDied -= OnPlayerDied;
        EventManager.Instance.OnCountdownEnded -= OnCountdownEnded;
        
        EventManager.Instance.OnVolumeChanged -= ResetVolume;
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
                voiceAudioSource.volume = 0.8f;

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
            if (overrideCurrentClip || !voiceAudioSource.isPlaying)
            {
                AudioClip clipToPlay = selectedClips[Random.Range(0, selectedClips.Length)];
                voiceAudioSource.clip = clipToPlay;
                voiceAudioSource.Play();
            }
        }
    }


    public void StopSound()
    {
        voiceAudioSource.Stop();
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
        voiceAudioSource.Pause();
        footstepAudioSource.Pause();
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        voiceAudioSource.UnPause();
        footstepAudioSource.UnPause();
    }

    private void OnGamePaused()
    {
        isPaused = true;
        voiceAudioSource.Pause();
        footstepAudioSource.Pause();
    }

    private void OnGameContinued()
    {
        isPaused = false;
        voiceAudioSource.UnPause();
        footstepAudioSource.UnPause();
    }

    private void OnPlayerDied()
    {
        StartCoroutine(FadeOut(2.0f));
    }

    private void OnCountdownEnded()
    {
        StartCoroutine(FadeOut(1.0f));
    }
    
    private IEnumerator FadeOut(float duration)
    {
        float startVolume = voiceAudioSource.volume;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            voiceAudioSource.volume = Mathf.Lerp(startVolume, 0.0f, elapsedTime / duration);
            footstepAudioSource.volume = Mathf.Lerp(startVolume, 0.0f, elapsedTime / duration);
            yield return null;
        }

        voiceAudioSource.volume = 0.0f;
        footstepAudioSource.volume = 0.0f;
        voiceAudioSource.Stop();
        footstepAudioSource.Stop();
    }

    private void ResetVolume()
    {
        voiceAudioSource.volume = AudioManager.Instance.masterVolume * AudioManager.Instance.sfxVolume;
        footstepAudioSource.volume = AudioManager.Instance.masterVolume * AudioManager.Instance.sfxVolume;
    }

    public void PlayFirstFootstepSound()
    {
        footstepAudioSource.PlayOneShot(footstepSFX_1);
    }

    public void PlaySecondFootstepSound()
    {
        footstepAudioSource.PlayOneShot(footstepSFX_2);
    }
}