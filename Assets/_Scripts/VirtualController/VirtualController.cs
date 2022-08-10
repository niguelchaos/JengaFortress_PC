using UnityEngine;

// Virtual controller (abstract input layer) that sits between an 
// input source and the layer that acts on the input.
public class VirtualController : MonoBehaviour
{
   public Vector2 moveInputDir;
   public bool jump;
}