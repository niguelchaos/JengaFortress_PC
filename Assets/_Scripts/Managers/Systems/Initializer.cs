using UnityEngine;

public static class Initializer
{
    /// <summary>
    ///     This will run once before any other scene script
    /// start persistent singletons/systems before scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize() 
    {
        MatchmakingService.ResetStatics();
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
    } 
}
