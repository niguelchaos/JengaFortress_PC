using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas; 


    private void Start()
    {
        LobbyManager.Instance.MatchFoundEvent += OnMatchFound;
        LobbyManager.Instance.MatchHostedEvent += OnMatchFound;

        if (menuCanvas == null)
        {
            menuCanvas = GameObject.Find("MenuCanvas");
        }
    }

    private void OnMatchFound()
    {
        menuCanvas.SetActive(false);
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.MatchFoundEvent -= OnMatchFound;
        LobbyManager.Instance.MatchHostedEvent -= OnMatchFound;
    }

}