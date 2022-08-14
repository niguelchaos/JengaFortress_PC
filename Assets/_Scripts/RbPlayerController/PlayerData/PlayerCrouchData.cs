using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCrouch", menuName ="PlayerControl/Crouch")]
public class PlayerCrouchData : ScriptableObject
{
   [SerializeField] public float crouchYScale = 0.75f;
   [SerializeField] public float downForceMultiplier = 5f;
}