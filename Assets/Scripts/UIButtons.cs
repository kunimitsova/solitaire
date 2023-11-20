using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIButtons : MonoBehaviour {

    //Solitaire solitaire;
    App_Initialize appInit;
    public GameObject winScreenUI;

    public static event Action GameRenewed;

    public static event Action ReplayClicked;

    public static event Action GameStarted;

    public static event Action AutoplayClicked;

    public static event Action UndoClicked;

    public static event Action SettingsClicked;

    private void Start() {
        winScreenUI.SetActive(false);
    }
 
    public void ReplayGame() {
        winScreenUI.SetActive(false);
        GameRenewed?.Invoke();
        // keep the same shuffled deck for this replay, do NOT ccall ReplayClicked!
        GameStarted?.Invoke();
    }

    public void NewGame() {
        winScreenUI.SetActive(false);
        GameRenewed?.Invoke();
        // get a new shuffle for this game
        ReplayClicked?.Invoke();
        GameStarted?.Invoke();
    }

    public void Autoplay() { // right now this just has testing stuff related to the undo system
        AutoplayClicked?.Invoke();
    }

    public void Undo() {
        UndoClicked?.Invoke();
    }

    public void SeeSettings() {
        winScreenUI.SetActive(false);
        // show Settings menu

        // how to set bool values :
        // PlayerPrefs.SetInt("myBool", myBool ? 1 : 0); // 1 is true and 0 is false

        // when exiting settings menu :
        PlayerPrefs.Save();
    }
    
    public void InstaWin() {
        winScreenUI.SetActive(true);
    }
}
