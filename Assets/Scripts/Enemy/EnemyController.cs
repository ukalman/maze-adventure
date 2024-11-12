
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 3.5f;
    public float walkSpeed = 1.0f;
    public float distToPlayer;
    public float attackDist = 2.0f;
    
    public bool playerSeen;
    public bool isDead;
    private bool isDeadDoubleCheck;
    
    public NavMeshAgent enemyAgent;
    
    #region EnemyStates
    
    public EnemyBaseState CurrentState { get; private set; }

    public EnemyIdleState Idle = new EnemyIdleState();
    public EnemyRunState Run = new EnemyRunState();
    public EnemyAttackState Attack = new EnemyAttackState();
    public EnemyDeathState Death = new EnemyDeathState();
    public EnemyScreamingState Scream = new EnemyScreamingState();
    
    #endregion
    
    [HideInInspector] public Animator anim;

    private EnemyHealth health;
    public RagdollManager ragdollManager;

    [SerializeField] private Material transparentMat1, transparentMat2;
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        ragdollManager = GetComponent<RagdollManager>();
        SwitchState(Idle);
        isDead = health.isDead;
    }

    private void Update()
    {
        isDead = health.isDead;
        CurrentState.UpdateState(this);
        if (isDead && !isDeadDoubleCheck)
        {
            StartCoroutine(OnDeath());
        }
    }
    
    public void SwitchState(EnemyBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
    }

    public bool IsPlayerInAttackingDist()
    {
        if (distToPlayer != 0.0f)
        {
            return distToPlayer <= attackDist;
        }

        return false;
    }

    private IEnumerator OnDeath()
    {
        isDead = false;
        isDeadDoubleCheck = true;
        //health.isDead = false;
        Destroy(enemyAgent); 

        // if enemy is in running or screaming state
        if (anim != null)
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(anim);
            ragdollManager.TriggerRagdoll();
        }

        yield return new WaitForSeconds(2.0f);
        ragdollManager.DeactivateRagdoll();

        yield return StartCoroutine(FadeAway());
        
        Destroy(gameObject);
    }

    private IEnumerator FadeAway()
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null)
        {
            // Set the transparent materials
            skinnedMeshRenderer.materials = new Material[] { transparentMat1, transparentMat2 };

            // Fade duration
            float fadeDuration = 3.0f;
            float elapsedTime = 0f;

            // Begin fading out by adjusting the alpha over time
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newOpacity = Mathf.Lerp(1.0f, 0, elapsedTime / fadeDuration);

                // Update the alpha for each material
                foreach (var material in skinnedMeshRenderer.materials)
                {
                    Color color = material.color;
                    color.a = newOpacity;
                    material.color = color;
                }
                yield return null;
            }
        }
    }

    

    
}
