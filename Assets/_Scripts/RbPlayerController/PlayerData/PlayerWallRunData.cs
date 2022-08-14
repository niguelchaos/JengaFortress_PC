using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWallRun", menuName ="PlayerControl/WallRun")]
public class PlayerWallRunData : ScriptableObject
{
    public float wallDist = 0.6f;
    public float minJumpHeight = 1.5f;
}