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
    private AudioSource audioSource;
    private Transform playerHead;
    private AudioLowPassFilter lowPassFilter;
    private float clearFrequency = 22000.0f;
    private float occludedFrequency = 500.0f; // for muffled sound

    [SerializeField] private Transform enemyEyes;
    
    [Header("Idle SFX")] [SerializeField] private AudioClip[] idleSFX;
    [Header("Scream SFX")] [SerializeField] private AudioClip[] screamSFX;
    [Header("Run SFX")] [SerializeField] private AudioClip[] runSFX;
    [Header("Attack SFX")] [SerializeField] private AudioClip[] attackSFX;
    [Header("Death SFX")] [SerializeField] private AudioClip[] deathSFX;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        lowPassFilter.cutoffFrequency = clearFrequency;
        playerHead = GameManager.Instance.playerHead;
    }

    private void Update()
    {
        CheckOcclusion();
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
                audioSource.volume = 0.7f;
                yield return new WaitForSeconds(2.0f);
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
        Vector3 dirToPlayer = playerHead.position - enemyEyes.position;
        RaycastHit hit;

        if (Physics.Raycast(enemyEyes.position, dirToPlayer, out hit))
        {
            
            if (hit.transform.CompareTag(playerHead.tag))
            {
                lowPassFilter.cutoffFrequency = clearFrequency;
            }

            else
            {
                Debug.Log("oh no man, this is the hit transform: " + hit.transform.name);
                lowPassFilter.cutoffFrequency = occludedFrequency;
            }
            
        }
    }
}