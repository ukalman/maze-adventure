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
    [SerializeField] private float jumpForce = 10.0f;
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
    
    

    [HideInInspector] public Animator anim;
    private static readonly int XInput = Animator.StringToHash("xInput");
    private static readonly int ZInput = Animator.StringToHash("zInput");

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(Idle);
    }
    
    void Update()
    {
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

}
