using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientsManager : NetworkBehaviour
{
    public static ClientsManager Instance;
    private NetworkVariable<int> playersInGame;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        playersInGame = new NetworkVariable<int>();

        NetworkManager.Singleton.OnClientConnectedCallback += (id) => 
        {
            if (IsServer)
            {
                playersInGame.Value++;
                Debug.Log("Player Joined");
            }
        };
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) => 
        {
            if (IsServer)
            {
                playersInGame.Value--;
                Debug.Log("Player Disconnected");
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetPlayersInGame()
    {
        return playersInGame.Value;   
    }
}
