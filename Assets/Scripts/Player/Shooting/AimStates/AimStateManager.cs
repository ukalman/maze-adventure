using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class AimStateManager : MonoBehaviour
{
    public AimBaseState CurrentState { get; private set; }
    
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();
    
    [SerializeField] private float mouseSensitivity = 1.0f;
    [SerializeField] private Transform camFollowPos;
    
    private float xAxis, yAxis;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    public float aimFOV = 40.0f;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10.0f;

    [HideInInspector] public Transform aimPos;
    [HideInInspector] public Vector3 actualAimPos;
    [SerializeField] private float aimSmoothSpeed = 20.0f;
    [SerializeField] private LayerMask aimMask;

    private MultiAimConstraint[] multiAims;
    private WeightedTransform aimPosWeightedTransform;

    private float xFollowPos;
    private float yFollowPos, originalYPos;
    [SerializeField] private float crouchCamHeight = 0.6f;
    [SerializeField] private float shouldSwapSpeed = 10.0f;
    private MovementStateManager moving;


    [SerializeField] private Image crosshair;
    [SerializeField] private Sprite whiteHip;
    [SerializeField] private Sprite knob;
    private float hipWidth = 43.7006f;
    private float hipHeight = 42.7765f;
    private float knobWidth = 6.0f, knobHeight = 6.0f;

    private bool isPaused;
    
    private void Awake()
    {
        aimPos = new GameObject().transform;
        aimPos.name = "AimPosition";

        aimPosWeightedTransform.transform = aimPos;
        aimPosWeightedTransform.weight = 1.0f;

        multiAims = GetComponentsInChildren<MultiAimConstraint>();

        foreach (MultiAimConstraint constraint in multiAims)
        {
            var data = constraint.data.sourceObjects;
            data.Clear();
            data.Add(aimPosWeightedTransform);
            constraint.data.sourceObjects = data;
        }
    }

    void Start()
    {
        moving = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        originalYPos = camFollowPos.localPosition.y;
        yFollowPos = originalYPos;
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView;
        anim = GetComponent<Animator>();
        SwitchState(Hip);
        
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
        
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        yAxis = Mathf.Clamp(yAxis, -80.0f, 80.0f);

        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
            actualAimPos = hit.point;
        }
        
        MoveCamera();
        
        CurrentState.UpdateState(this);
    }

    private void LateUpdate()
    {
        if (!LevelManager.Instance.HasLevelStarted) return;
        
        if (isPaused) return;
        
        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y,camFollowPos.localEulerAngles.z);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    public void SwitchState(AimBaseState stateToSwitch)
    {
        CurrentState = stateToSwitch;
        CurrentState.EnterState(this);
    }

    private void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) xFollowPos = -xFollowPos;
        if (moving.CurrentState == moving.Crouch) yFollowPos = crouchCamHeight;
        else yFollowPos = originalYPos;

        Vector3 newFollowPosition = new Vector3(xFollowPos, yFollowPos,camFollowPos.localPosition.z);
        camFollowPos.localPosition =
            Vector3.Lerp(camFollowPos.localPosition, newFollowPosition, shouldSwapSpeed * Time.deltaTime);
    }

    public void SetCrosshairHip()
    {
        crosshair.sprite = whiteHip;
        crosshair.rectTransform.sizeDelta = new Vector2(hipWidth, hipHeight);
    }

    public void SetCrosshairAim()
    {
        crosshair.sprite = knob;
        crosshair.rectTransform.sizeDelta = new Vector2(knobWidth, knobHeight);
    }
    
    private void OnDroneCamActivated()
    {
        isPaused = true;
        anim.speed = 0;
    }

    private void OnDroneCamDeactivated()
    {
        isPaused = false;
        anim.speed = 1;
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
    
}
