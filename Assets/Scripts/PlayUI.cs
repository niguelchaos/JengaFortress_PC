using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayUI : MonoBehaviour
{

    public TMP_Text currentPlayerText;
    public Button endTurnButton;
    public Image endTurnButtonImage;
    public GameObject debugCanvas;
    public GameObject playCanvas; 
    public GameObject oldCanvas;
    public GameObject startScreen;
    public GameObject placeGroundScreen;
    public GameObject adjustFireScreen;
    public GameObject placePlayer1Screen;
    public GameObject placePlayer2Screen;
    public GameObject playingScreen;
    public GameObject currentScreen;
    public GameObject gameOverScreen;
    public static TMP_Text messageText;

    void Start()
    {
        endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        endTurnButtonImage = GameObject.Find("EndTurnButton").GetComponent<Image>();
        currentPlayerText = GameObject.Find("CurrentPlayerText").GetComponent<TMP_Text>();
        messageText = GameObject.Find("messageText").GetComponent<TMP_Text>();

        //oldCanvas = GameObject.Find("oldCanvas");
        // //startScreen = GameObject.Find("startScreen");
        // placeGroundScreen = GameObject.Find("placeGroundScreen");
        // adjustFireScreen = GameObject.Find("adjustFireScreen");
        // placePlayer1Screen = GameObject.Find("placePlayer1");
        // placePlayer2Screen = GameObject.Find("placePlayer2");
        // playingScreen = GameObject.Find("playingScreen");
        UpdateText();
        GameManager.OnPlayingStateChanged += UpdateOnPlayingStateChanged;
        GameManager.OnGameStateChanged += UpdateOnGameStateChanged;
        currentScreen = startScreen;



        playCanvas.SetActive(false);
        debugCanvas.SetActive(false);
        startScreen.SetActive(true);
        
        // playCanvas.SetActive(true);
        // debugCanvas.SetActive(true);
        // startScreen.SetActive(false);

        playCanvas.SetActive(false);
        oldCanvas.SetActive(false);
        placeGroundScreen.SetActive(false);
        adjustFireScreen.SetActive(false);
        placePlayer1Screen.SetActive(false);
        placePlayer2Screen.SetActive(false);
        playingScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    public static void showMessage(string message)
    {
        messageText.text = message;
    }

    // private static IEnumerator _showMessage()
    // {
    //     yield return new WaitForSeconds(5);
    //     messageText.text = "";
    // }

    private void UpdateText()
    {
        currentPlayerText.text = "Player " + (int) GameManager.Instance.currentPlayer;
    }

    public void ChangePlayer()
    {
        GameManager.Instance.ChangePlayer();
        UpdateText();
        messageText.text = "";
    }

    private void UpdateOnPlayingStateChanged(PlayingState playingState)
    {
        if(playingState is PlayingState.END_TURN)
            SetEndTurnButtonActive(true);
        else
            SetEndTurnButtonActive(false);
    }

    public void SetEndTurnButtonActive(bool active)
    {
        endTurnButton.interactable = active;
        if (active)
            StartCoroutine("startBlinkButton");
        else
            StopCoroutine("startBlinkButton");
    }

    public void switchToSetup(){
        GameManager.Instance.SetGameState(GameState.SETUP);
    }
    public void switchToMainMenu() {
        GameManager.Instance.SetGameState(GameState.MAIN_MENU);
    }

    public void activatePlaceGround(){
        switchScreen(startScreen, placeGroundScreen);
    }

    public void activateAdjustFireScreen(){
        switchScreen(placeGroundScreen, adjustFireScreen);
    }

    public void activatePlacePlayer1() {
        GameManager.Instance.SetGameState(GameState.PLACE_FORTRESS);
    }

    public void activatePlacePlayer2() {
        switchScreen(placePlayer1Screen, placePlayer2Screen);
    }

    public void activatePlayingScreen() {
        switchScreen(placePlayer2Screen, playingScreen);
    }

    public void activateGameOverScreen() {
        switchScreen(playingScreen, gameOverScreen);
    }

    public void switchToNoScreen(){ 
        currentScreen.SetActive(false);
    }

    public void UpdateOnGameStateChanged(GameState state) { 
        switch(state){
            case GameState.MAIN_MENU:
                oldCanvas.SetActive(false);
                startScreen.SetActive(true);
                placeGroundScreen.SetActive(false);
                adjustFireScreen.SetActive(false);
                placePlayer1Screen.SetActive(false);
                placePlayer2Screen.SetActive(false);
                playingScreen.SetActive(false);
                break;
            case GameState.SETUP:
                activatePlaceGround();
                break;
            case GameState.PLACE_FORTRESS:
                //CallswitchScreen(adjustFireScreen, placePlayer1Screen);
                switchToNoScreen();
                break;
            case GameState.PLAYING:
                activatePlayingScreen();
                break;
            case GameState.GAME_OVER:
                activateGameOverScreen();
                break;     
        }
    }

    private void switchScreen(GameObject a, GameObject b) {
        a.SetActive(false);
        b.SetActive(true);
        currentScreen = b;
    }

    private IEnumerator startBlinkButton()
    {
        while (true)
        {
            endTurnButtonImage.color = new Color(0.7f, 0.85f, 0.75f, 1f);
            yield return new WaitForSeconds(1.5f);
            endTurnButtonImage.color = new Color(0.75f, 0.90f, 0.8f, 1f);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
