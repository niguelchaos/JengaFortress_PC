using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovement", menuName ="PlayerControl/Movement")]
public class PlayerMovementData : ScriptableObject
{
   [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 24f;
    public float crouchSpeed;
    public float accelSpeed = 10f;

   [Header("Fly")]
    public float flyWalkSpeed = 6f;
    public float flySprintSpeed = 24f;

   [Header("Movement Multipliers")]
    public float groundMultiplier = 10f;
    public float airMultiplier = 0.4f;
    
    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 2f;

    public float slopeRayExtraLength = 1f;


}