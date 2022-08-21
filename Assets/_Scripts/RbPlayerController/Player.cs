using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

/// <summary>
/// Shared data used by FSMs.
/// I'm playing a little loose with the idea of a blackboard here since it
/// also holds references to components for convenience.
/// This basically global shiet
/// </summary>

public enum PlayerNum {P1, P2}

public class Player : NetworkBehaviour
{
    // Note: Public to make it easy to see in inspector.
    // Ideally they'd be private with [SerializeField], but keeping code simple.

    [SerializeField] public PlayerData Data;

    [Header("Input")]
    private PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField] public bool isGrounded;
    // [SerializeField] public bool hasJumped;
    [SerializeField] public bool isOnSlope;
    [SerializeField] public bool isFalling;
    
    [Header("Wallrun")]
    [SerializeField] public bool isWallRunning;
    [SerializeField] public bool isWallLeft = false;
    [SerializeField] public bool isWallRight = false;
    [SerializeField] public RaycastHit leftWallHit;
    [SerializeField] public RaycastHit rightWallHit;

    [Header("Physics")]
    public Rigidbody rb;
    public bool flyMode = true;
    
    [Header("Entity")]
    [SerializeField] public float playerHeight;

    [Header("Camera")]
    [SerializeField] public Camera cam;
    [SerializeField] public Transform camHolder;
    [SerializeField] public Transform weaponHolder;


    [Header("Weapon")]
    // [SerializeField] public bool hasFired;

    public PlayerNum playerNum;
    

    private void Awake()
    {
        this.playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        Data.groundMask = LayerMask.GetMask(EditorConstants.GROUND_LAYER_NAME, EditorConstants.BLOCK_LAYER_NAME);
    }

    private void Start()
    {
        if (IsHost || IsServer)
        {
            playerNum = PlayerNum.P1;
        }
        else {
            playerNum = PlayerNum.P2;
        }
    }

    public PlayerNum GetPlayerNum() { return playerNum;}
    public void SetPlayerNum(PlayerNum newPlayer) { playerNum = newPlayer;}

    public void SetupPlayerInput()
    {
        InputManager.Instance.SetPlayerInput(playerInput);
    }

    public void DisablePlayerInput()
    {
        playerInput.DeactivateInput();
        playerInput.enabled = false;
        GetComponent<PlayerGunFight>().enabled = false;
        GetComponent<PlayerGunstick>().enabled = false;
        GetComponent<PlayerCrouch>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
    }


}

