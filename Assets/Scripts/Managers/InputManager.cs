using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction attackAction;
    private List<InputAction> weaponSwapActions = new List<InputAction>();
    
    // Debug
    private InputAction endTurnAction;



    [SerializeField] public Vector2 moveDir { get; private set; }
    [SerializeField] public Vector2 lookInput { get; private set; }
    [SerializeField] public bool jumpInput { get; private set; }
    [SerializeField] public bool isSprinting { get; private set; }
    [SerializeField] public bool isCrouching { get; private set; }
    [SerializeField] public bool fireInput { get; private set; }
    [SerializeField] public bool reloadInput { get; private set; }
    [SerializeField] public bool attackInput { get; private set; }
    // [SerializeField] public bool weaponSwap1Input { get; private set; }

    // debug
    [SerializeField] public bool endTurnInput { get; private set; }


    private void OnEnable()
    {

    } 

    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (lookAction != null) lookAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (sprintAction != null) sprintAction.Disable();
        if (crouchAction != null) crouchAction.Disable();
        if (fireAction != null) fireAction.Disable();
        if (reloadAction != null) reloadAction.Disable();
        if (attackAction != null) attackAction.Disable();
        if (weaponSwapActions != null) {
            foreach (InputAction weaponAction in weaponSwapActions)
            { weaponAction.Disable(); }   
        }

        // Debug
        if (endTurnAction != null) endTurnAction.Disable();
    }

    private void Awake()
    {
        Instance = this;
        Debug.Log("Input Alive");
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
        if (playerInput.inputIsActive)
        {
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            jumpAction = playerInput.actions["Jump"];
            sprintAction = playerInput.actions["Sprint"];
            crouchAction = playerInput.actions["Crouch"];
            fireAction = playerInput.actions["Fire"];
            reloadAction = playerInput.actions["Reload"];
            attackAction = playerInput.actions["Attack"];
            // weaponSwap1Action = playerInput.actions["WeaponSwap1"];

            // debug
            endTurnAction = playerInput.actions["EndTurnDebug"];


            moveAction.Enable();
            lookAction.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
            crouchAction.Enable();
            fireAction.Enable();
            reloadAction.Enable();
            attackAction.Enable();
            // if (weaponSwapActions.Length > 0) {
            //     foreach (InputAction weaponAction in weaponSwapActions)
            //     { weaponAction.Enable(); }   
            // }

            // debug
            endTurnAction.Enable();

            print("PlayerInput Connected");

            InitPlayerInput();
        }
        else {
            print("Playerinput not active");
        }
    }

    private void InitPlayerInput()
    {
        moveAction.started += OnMoveInput;
        moveAction.performed += OnMoveInput;
        moveAction.canceled += OnMoveInput;

        lookAction.started += OnLookInput;
        lookAction.performed += OnLookInput;
        lookAction.canceled += OnLookInput;

        jumpAction.performed += OnJumpInput;
        jumpAction.canceled += OnJumpInput;
        
        sprintAction.performed += OnSprintPressed;
        sprintAction.canceled += OnSprintPressed;

        crouchAction.performed += OnCrouchPressed;
        crouchAction.canceled += OnCrouchPressed;

        fireAction.performed += OnFireInput;
        fireAction.canceled += OnFireInput;
        
        reloadAction.performed += OnReloadInput;
        reloadAction.canceled += OnReloadInput;
        
        attackAction.performed += OnAttackInput;
        attackAction.canceled += OnAttackInput;

        // if (weaponSwapActions.Length > 0) {
        //     foreach (InputAction weaponAction in weaponSwapActions)
        //     { 
        //         attackAction.performed += OnAttackInput;
        //         attackAction.canceled += OnAttackInput;
        //     }   
        // }

        // Debug
        endTurnAction.performed += OnEndTurnInput;
        endTurnAction.canceled += OnEndTurnInput;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        this.moveDir = moveAction.ReadValue<Vector2>();
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        this.lookInput = lookAction.ReadValue<Vector2>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {  
        // print("started jump " + context.phase);
        this.jumpInput = context.performed;
    }
    
    public void OnSprintPressed(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
        // print("sprint - " + context.phase);
    }
    public void OnCrouchPressed(InputAction.CallbackContext context)
    {
        isCrouching = context.performed;
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {  
        this.fireInput = context.performed;
    }

    public void OnReloadInput(InputAction.CallbackContext context)
    {  
        this.reloadInput = context.performed;
    }
    public void OnAttackInput(InputAction.CallbackContext context)
    {  
        this.attackInput = context.performed;
    }
    public void OnWeaponSwapInput(InputAction.CallbackContext context)
    {  
        Debug.Log(context);
    }

    public void AddWeaponNumberBindings(int weaponNum)
    {
        for (int i = 0; i < weaponNum; i++)
        {
            InputAction newWeaponSwapAction = new InputAction(name: "WeaponSwap" + (i+1), binding:"<Keyboard>/" + (i+1));
            weaponSwapActions.Add(newWeaponSwapAction);
            newWeaponSwapAction.Enable();
            newWeaponSwapAction.performed += OnWeaponSwapInput;
            print("added " + i);
        }
    }


    // debug
    public void OnEndTurnInput(InputAction.CallbackContext context)
    {  
        this.endTurnInput = context.performed;
    }

    private void OnDestroy()
    {
        if (playerInput == null) { return; }
        if (playerInput.inputIsActive)
        {
            moveAction.started -= OnMoveInput;
            moveAction.performed -= OnMoveInput;
            moveAction.canceled -= OnMoveInput;

            lookAction.started -= OnLookInput;
            lookAction.performed -= OnLookInput;
            lookAction.canceled -= OnLookInput;

            jumpAction.performed -= OnJumpInput;
            jumpAction.canceled -= OnJumpInput;
            
            sprintAction.performed -= OnSprintPressed;
            sprintAction.canceled -= OnSprintPressed;

            crouchAction.performed -= OnCrouchPressed;
            crouchAction.canceled -= OnCrouchPressed;

            fireAction.performed -= OnFireInput;
            fireAction.canceled -= OnFireInput;
            
            reloadAction.performed -= OnReloadInput;
            reloadAction.canceled -= OnReloadInput;
            
            attackAction.performed -= OnAttackInput;
            attackAction.canceled -= OnAttackInput;

            // weaponSwapAction.performed -= OnWeaponSwapInput;
            // weaponSwapAction.canceled -= OnWeaponSwapInput;

            
            // Debug
            endTurnAction.performed -= OnEndTurnInput;
            endTurnAction.canceled -= OnEndTurnInput;
        }

    }





}