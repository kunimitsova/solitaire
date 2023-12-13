using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIButtons : MonoBehaviour {

    //Solitaire solitaire;
    //App_Initialize appInit;
    [SerializeField] GameObject winScreenUI;
    [SerializeField] GameObject settingsUI;
    [SerializeField] GameObject testUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject creditsUI;

    public static event Action GameRenewed;

    public static event Action ReplayClicked;

    public static event Action GameStarted;

    public delegate void TestItem(string text);
    public static TestItem TestButtonClicked;

    public static event Action SettingsClicked;

    public static event Action UndoClicked;

    public static event Action AutoplayClicked;

    private void Start() {
        winScreenUI.SetActive(false);
        settingsUI.SetActive(false);
        testUI.SetActive(false);
        inGameUI.SetActive(true);
        creditsUI.SetActive(false);
    }
 
    public void ReplayGame() {
        SetOnlyInGameUIActive();
        GameRenewed?.Invoke();
        // keep the same shuffled deck for this replay, do NOT ccall ReplayClicked!
        GameStarted?.Invoke();
    }

    public void NewGame() {
        SetOnlyInGameUIActive();
        GameRenewed?.Invoke();
        // get a new shuffle for this game
        ReplayClicked?.Invoke();
        GameStarted?.Invoke();
    }

    public void Autoplay() { // right now this just has testing stuff related to the undo system
        Debug.Log("Autoplay Clicked");
        TestButtonClicked?.Invoke(Constants.TEST_AUTOPLAY);
    }

    public void Undo() {
        Debug.Log("Undo clicked");
        UndoClicked?.Invoke();
      //  TestButtonClicked?.Invoke(Constants.TEST_UNDO);
    }

    public void SeeSettings() {
        SettingsClicked?.Invoke();
    }
    
    public void InstaWin() {
        winScreenUI.SetActive(true);
    }

    public void SetOnlyInGameUIActive() {
        settingsUI.SetActive(false);
        winScreenUI.SetActive(false);
        testUI.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void Credits() {
        creditsUI.SetActive(true);
    }

    public void CloseCredits() {
        creditsUI.SetActive(false);
    }
    
    void CloseWin() {
        winScreenUI.SetActive(false);
    }
}
