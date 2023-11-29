using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIButtons : MonoBehaviour {

    //Solitaire solitaire;
    //App_Initialize appInit;
    public GameObject winScreenUI;
    public GameObject settingsUI;
    public GameObject testUI;
    public GameObject inGameUI;

    public static event Action NewGameClicked;

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
    }
 
    public void ReplayGame() {
        SetOnlyInGameUIActive();
        ReplayClicked?.Invoke();
        // keep the same shuffled deck for this replay, do NOT ccall NewGameClicked!
        GameStarted?.Invoke();
    }

    public void NewGame() {
        SetOnlyInGameUIActive();
        NewGameClicked?.Invoke();
        // get a new shuffle for this game
        ReplayClicked?.Invoke();
        GameStarted?.Invoke();
    }

    public void Autoplay() { // right now this just has testing stuff related to the undo system
        TestButtonClicked?.Invoke(Constants.TEST_AUTOPLAY);
    }

    public void Undo() {
        TestButtonClicked?.Invoke(Constants.TEST_UNDO);
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
    
    void CloseWin() {
        winScreenUI.SetActive(false);
    }
}
