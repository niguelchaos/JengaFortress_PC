using UnityEngine;

[CreateAssetMenu(fileName = "PlayerGravity", menuName ="PlayerControl/Gravity")]
public class PlayerGravityData : ScriptableObject
{
    [SerializeField] public float wallRunGravity = 2f;
    [SerializeField] public float defaultGravityScale = 5;

    [SerializeField] public float currentAirGravity = 5f;
    [SerializeField] public float inAirGravMultiplier = 10f;
}