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
    [Header("Input")]
    private PlayerInput playerInput;

    [Header("Entity")]
    [SerializeField] public float playerHeight;

    [Header("Movement")]
    [SerializeField] public bool isGrounded;
    [SerializeField] public bool hasJumped;
    [SerializeField] public bool isOnSlope;
    [SerializeField] public bool isFlying;

    [Header("Crouch")]
    public float startYScale;
    
    [Header("Wallrun")]
    [SerializeField] public bool isWallRunning;
    [SerializeField] public bool isWallLeft = false;
    [SerializeField] public bool isWallRight = false;
    [SerializeField] public RaycastHit leftWallHit;
    [SerializeField] public RaycastHit rightWallHit;

    [Header("Camera")]
    [SerializeField] public Camera cam;
    public float currentTilt;

    [Header("Physics")]
    [SerializeField] public Rigidbody rb;

    [Header("Weapon")]
    [SerializeField] public bool hasFired;

    private PlayerNum playerNum;

    


    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.playerInput = GetComponent<PlayerInput>();
        // this.cam = GetComponent<Camera>();
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

