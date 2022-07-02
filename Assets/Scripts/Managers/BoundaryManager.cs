using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    // singleton
    public static BoundaryManager Instance;

    [SerializeField] private GameObject playerBoundaryPrefab;
    private PlayerBoundary playerBoundary_P1, playerBoundary_P2;

    private GameObject sessionOrigin;
    [SerializeField] private GameObject groundPlaneGO;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        sessionOrigin = GameObject.Find("AR Session Origin");

        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        //GameManager.OnCurrentPlayerChanged += UpdateOnCurrentPlayerChanged;
    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        if (gameState == GameState.SET_BOUNDARIES)
        {
            // groundPlaneGO = sessionOrigin.GetComponent<Setup>().groundPlane;

            playerBoundary_P1 = Instantiate(playerBoundaryPrefab).GetComponent<PlayerBoundary>();
            playerBoundary_P2 = Instantiate(playerBoundaryPrefab).GetComponent<PlayerBoundary>();

            playerBoundary_P1.player = CurrentPlayer.PLAYER_1;
            playerBoundary_P2.player = CurrentPlayer.PLAYER_2;

            playerBoundary_P1.SetBoundaryTransform(groundPlaneGO, 1.0f);
            playerBoundary_P2.SetBoundaryTransform(groundPlaneGO, 1.0f);
            
            GameManager.Instance.SetGameState(GameState.PLACE_FORTRESS);
        }
    }
    public bool isWithinBoundary(CurrentPlayer player) {
        switch (player) {
            case CurrentPlayer.PLAYER_1:
                return playerBoundary_P1.isWithinBoundary;
            case CurrentPlayer.PLAYER_2:
                return playerBoundary_P2.isWithinBoundary;
        }
        return false;
    }

}
