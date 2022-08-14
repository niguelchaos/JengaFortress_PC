using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName ="PlayerControl/Player")]
public class PlayerData : ScriptableObject
{
    [Header("Crouch")]
    public float startYScale;

    [Header("Masks")]
    public LayerMask groundMask;



}