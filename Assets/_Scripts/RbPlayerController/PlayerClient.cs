using UnityEngine;


/// <summary>
/// Base class for FSMs that provides a reference to the blackboard.
/// </summary>
public class PlayerClient : MonoBehaviour
{
    public Player Player { get; private set; }

    protected virtual void Awake()
    {
        Player = GetComponent<Player>();
    }

}

