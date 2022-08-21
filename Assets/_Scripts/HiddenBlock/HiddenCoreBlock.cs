using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: ultra refactor

public class HiddenCoreBlock : MonoBehaviour
{
    [SerializeField] private Player player;
    private Rigidbody rb;
    private Outline outline;


    private void Awake()
    {
    }

    private void Start()
    {
        // run update on game state change
        outline = this.GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        player = gameObject.transform.parent.gameObject.GetComponent<Player>();
        
        // subscribe to state changes
        GameManager.BeforeGameStateChanged += UpdateOnGameStateChanged;
        GameManager.CurrentPlayerChanged += UpdateOnCurrentPlayerChanged;

    }

    private void UpdateOnGameStateChanged(GameState gameState)
    {
        CheckOutline();
    }
    private void UpdateOnCurrentPlayerChanged(CurrentPlayer currentPlayer)
    {
        CheckOutline();
    }

    private void OnDestroy()
    {
        // unsubscribe if destroy obj
        GameManager.BeforeGameStateChanged -= UpdateOnGameStateChanged;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == EditorConstants.GROUND_LAYER)
        {
            // Debug.Log("hit ground");
            if (GameManager.Instance.GetWinCondition() == WinCondition.HitFloor)
            {
                GameManager.Instance.ChangeState(GameState.GAME_OVER);
            }
        }
    }

    // show player's outlined hidden block only on their turn. 
    public void CheckOutline()
    {
        DisableOutline();
        switch (GameManager.Instance.currentPlayer, player.GetPlayerNum())
        {
            case (CurrentPlayer.PLAYER_1, PlayerNum.P1):
            case (CurrentPlayer.PLAYER_2, PlayerNum.P2):
                EnableOutline();
                break;
        }
    }

    public void EnableOutline()     {   outline.enabled = true;  }
    public void DisableOutline()    {   outline.enabled = false; }
}
