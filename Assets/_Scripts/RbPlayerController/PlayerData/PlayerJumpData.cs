using UnityEngine;

[CreateAssetMenu(fileName = "PlayerJump", menuName ="PlayerControl/Jump")]
public class PlayerJumpData : ScriptableObject
{
   [SerializeField] public float jumpForce = 40f;
   [SerializeField] public float wallJumpForce = 20f;
}