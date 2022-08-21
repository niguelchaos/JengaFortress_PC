using UnityEngine;
using TMPro;

// also currently unused 

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas; 


    private void Start()
    {
        LobbyManager.LobbyEntered += OnMatchFound;
    }

    private void OnMatchFound()
    {
        menuCanvas.SetActive(false);
    }

    private void OnCurrentPlayerChanged()
    {
        
    }

    private void OnDestroy()
    {
        LobbyManager.LobbyEntered -= OnMatchFound;
    }

}