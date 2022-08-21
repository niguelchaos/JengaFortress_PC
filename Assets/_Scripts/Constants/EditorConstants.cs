using UnityEngine;

/// <summary>
/// A simple static class that keeps track of layer + tag names, holds ready to use layermasks for most common layers and layermasks combinations
/// Of course if you happen to change the layer order or numbers, you'll want to update this class.
/// </summary>
public class EditorConstants : MonoBehaviour
{
   //==================== LAYERS ====================//
   public static readonly string GROUND_LAYER_NAME = "Ground";
   public static readonly string BLOCK_LAYER_NAME = "Block";
   
   public static readonly int UI_LAYER = 5;
   public static readonly int GROUND_LAYER = 6;
   public static readonly int BLOCK_LAYER = 7;
   public static readonly int PLAYER_LAYER = 8;

   public static readonly int GROUND_LAYER_MASK = 1 << GROUND_LAYER;
   public static readonly int BLOCK_LAYER_MASK = 1 << BLOCK_LAYER;
   public static readonly int PLAYER_LAYER_MASK = 1 << PLAYER_LAYER;

   //==================== TAGS ====================//
   public static readonly string PLAYER_TAG = "Player";

}
