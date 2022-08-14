using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLook", menuName ="PlayerControl/Look")]
public class PlayerLookData : ScriptableObject
{
   public Vector2 sens = new Vector2(15f, 15f);
   public float rotMultiplier = 0.01f;

   public float fov = 90f;
   public float wallRunFov = 110f;
   public float wallRunFovTime = 15f;
   public float camTilt = 20f;
   public float camTiltTime = 15f;
}