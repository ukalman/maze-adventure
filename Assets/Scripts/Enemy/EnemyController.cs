
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float runSpeed = 5.0f;
    public float distToPlayer;
    public float attackDist = 2.0f;
    
    public bool playerSeen;
    public bool isDead;

    public bool diedWhenRunning;

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
       
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        enemyAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        SwitchState(Idle);

        isDead = health.isDead;
    }

    private void Update()
    {
        isDead = health.isDead;
        CurrentState.UpdateState(this);
        if (isDead)
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
        Destroy(enemyAgent);

        // Means enemy got killed in a state other than running
        if (anim != null)
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(anim);
        }
        
    }

}
