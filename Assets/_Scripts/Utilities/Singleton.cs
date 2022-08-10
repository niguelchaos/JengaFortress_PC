using Unity.Netcode;
using UnityEngine;

/// <summary>
/// instead of destroying new instances, overrides current instance.
/// </summary>

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
    
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// actually destorys any new versions created, preserving original
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

/// <summary>
/// persistent as in survive through scene loads
/// ex. audio playing through loading scenes, input
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}




//////////////////////////////////////////

public abstract class StaticNetworkInstance<T> : NetworkBehaviour where T : Component
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
    
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// actually destorys any new versions created, preserving original
/// </summary>
public abstract class NetworkSingleton<T> : StaticNetworkInstance<T> where T : Component
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

/// <summary>
/// persistent as in survive through scene loads
/// ex. audio playing through loading scenes, input
/// </summary>
public abstract class PersistentNetworkSingleton<T> : NetworkSingleton<T> where T : Component
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
