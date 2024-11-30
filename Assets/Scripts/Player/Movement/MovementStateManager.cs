using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementStateManager : MonoBehaviour
{
    #region Movement
    public float currentMoveSpeed;
    public float walkSpeed = 3.0f, walkBackSpeed = 2.0f;
    public float runSpeed = 7.0f, runBackSpeed = 5.0f;
    public float crouchSpeed = 2.0f, crouchBackSpeed = 1.0f;
    public float airSpeed = 1.5f;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float xInput, zInput;
    private CharacterController controller;
    #endregion

    #region GroundCheck

    [SerializeField] private float groundYOffset;
    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;

    #endregion

    #region Gravity

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpForce = 5.0f;
    [HideInInspector] public bool jumped;
    private Vector3 velocity;

    #endregion

    #region States

    public MovementBaseState PreviousState;
    public MovementBaseState CurrentState { get; private set; }

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    #endregion

    #region Sprinting

    public PlayerSprintbar sprintbarUI;
    [SerializeField] public float maxSprintTime = 7.0f;
    [SerializeField] public float sprintRefillRate = 1.0f;
    [SerializeField] public float sprintDepletionRate = 1.0f;
    [SerializeField] public float refillDelay = 2.0f;
    public float sprintThreshold = 0.15f; 
    public float currentSprintTime;
    private Coroutine sprintRefillCoroutine;
    private Coroutine sprintDepletionCoroutine;

    #endregion

    [HideInInspector] public Animator anim;
    private static readonly int XInput = Animator.StringToHash("xInput");
    private static readonly int ZInput = Animator.StringToHash("zInput");

    public bool isCrouching;
    public bool isFlashlightOn;
    
    private bool isPaused;

    [SerializeField] private GameObject arrowIndicator;
    
    void Start()
    {
        arrowIndicator.SetActive(false);
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        currentSprintTime = maxSprintTime;
        sprintbarUI.SetMaxValue(maxSprintTime); 
        SwitchState(Idle);
        
        EventManager.Instance.OnDroneCamActivated += OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated += OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused += OnGamePaused;
        EventManager.Instance.OnGameContinued += OnGameContinued;
        
        EventManager.Instance.OnFlashlightTurnedOn += OnFlashlightTurnedOn;
        EventManager.Instance.OnFlashlightTurnedOff += OnFlashlightTurnedOff;

        EventManager.Instance.OnMazeExit += OnMazeExit;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnDroneCamActivated -= OnDroneCamActivated;
        EventManager.Instance.OnDroneCamDeactivated -= OnDroneCamDeactivated;
        EventManager.Instance.OnGamePaused -= OnGamePaused;
        EventManager.Instance.OnGameContinued -= OnGameContinued;
        
        EventManager.Instance.OnFlashlightTurnedOn -= OnFlashlightTurnedOn;
        EventManager.Instance.OnFlashlightTurnedOff -= OnFlashlightTurnedOff;
        
        EventManager.Instance.OnMazeExit -= OnMazeExit;
    }

    void Update()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        Move();
        Gravity();
        Falling();
        
        anim.SetFloat(XInput,xInput);
        anim.SetFloat(ZInput,zInput);
        CurrentState.UpdateState(this);
    }

    public void SwitchState(MovementBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
    }

    private void Move()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero;
        if (!IsGrounded()) airDir = transform.forward * zInput + transform.right * xInput;
        else dir = transform.forward * zInput + transform.right * xInput;

        if (dir.magnitude >= 0.1f)
        {
            controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime); 
        }
    }

    public void StartSprinting()
    {
        if (sprintRefillCoroutine != null)
        {
            StopCoroutine(sprintRefillCoroutine);
            sprintRefillCoroutine = null; // Stop refilling if sprinting starts
        }

        if (sprintDepletionCoroutine == null)
        {
            sprintDepletionCoroutine = StartCoroutine(HandleSprintDepletion());
        }
    }

    public void StopSprinting()
    {
        if (sprintDepletionCoroutine != null)
        {
            StopCoroutine(sprintDepletionCoroutine);
            sprintDepletionCoroutine = null;
        }

        // Start the refill coroutine with delay
        if (sprintRefillCoroutine == null)
        {
            sprintRefillCoroutine = StartCoroutine(HandleSprintRefill());
        }
    }

    private IEnumerator HandleSprintDepletion()
    {
        while (currentSprintTime > 0)
        {
            // Pause handling
            while (isPaused)
            {
                yield return null; // Wait until unpaused
            }

            // Decrease sprint time
            currentSprintTime -= sprintDepletionRate * Time.deltaTime;
            currentSprintTime = Mathf.Max(currentSprintTime, 0);

            // Update sprint bar UI
            sprintbarUI.SetValue(currentSprintTime);

            yield return null; // Wait for the next frame
        }

        StopSprinting(); // Start refill when sprint meter is depleted
    }


    private IEnumerator HandleSprintRefill()
    {
        // Handle refill delay with pause checking
        float elapsedDelay = 0f;
        while (elapsedDelay < refillDelay)
        {
            // Pause handling
            while (isPaused)
            {
                yield return null; // Wait until unpaused
            }

            elapsedDelay += Time.deltaTime;
            yield return null;
        }

        // Refill sprint bar over time
        while (currentSprintTime < maxSprintTime)
        {
            // Pause handling
            while (isPaused)
            {
                yield return null; // Wait until unpaused
            }

            currentSprintTime += sprintRefillRate * Time.deltaTime;
            currentSprintTime = Mathf.Min(currentSprintTime, maxSprintTime);
            sprintbarUI.SetValue(currentSprintTime); // Update sprint bar UI
            yield return null;
        }

        sprintRefillCoroutine = null; // Reset coroutine reference when done
    }
    
    public bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y + controller.radius - 0.08f, transform.position.z);
        return Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);
    }

    private void Gravity()
    {
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0.0f)
        {
            velocity.y = -2.0f;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void Falling() => anim.SetBool("Falling", !IsGrounded());

    public void JumpForce() => velocity.y += jumpForce;

    public void Jumped() => jumped = true;

    // Animation Event
    public void PlayFirstFootstepSound()
    {
        if (CurrentState == Walk)
        {
            AudioManager.Instance.OnFirstFootstep(0.1f);
        }
        if (CurrentState == Run)
        {
            AudioManager.Instance.OnFirstFootstep(0.2f);
        }
        if (CurrentState == Crouch)
        {
            AudioManager.Instance.OnFirstFootstep(0.01f);
        }
    }

    // Animation Event
    public void PlaySecondFootstepSound()
    {
        if (CurrentState == Walk)
        {
            AudioManager.Instance.OnSecondFootstep(0.1f);
        }
        if (CurrentState == Run)
        {
            AudioManager.Instance.OnSecondFootstep(0.2f);
        }
        if (CurrentState == Crouch)
        {
            AudioManager.Instance.OnSecondFootstep(0.01f);
        }
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        anim.speed = 0;
        arrowIndicator.SetActive(true);
        LevelManager.Instance.levelUIManager.RegisterTrackedObject(transform);
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        anim.speed = 1;
        arrowIndicator.SetActive(false);
        LevelManager.Instance.levelUIManager.UnregisterTrackedObject(transform);
    }

    private void OnGamePaused()
    {
        isPaused = true;
        anim.speed = 0;
    }

    private void OnGameContinued()
    {
        isPaused = false;
        anim.speed = 1;
    }

    private void OnFlashlightTurnedOn()
    {
        isFlashlightOn = true;
    }

    private void OnFlashlightTurnedOff()
    {
        isFlashlightOn = false;
    }

    private void OnMazeExit()
    {
        transform.GetComponent<ActionStateManager>().enabled = false;
        transform.GetComponent<AimStateManager>().enabled = false;
        transform.GetComponent<WeaponClassManager>().enabled = false;
        transform.GetComponent<CharacterController>().enabled = false;
        enabled = false;
    }

}
