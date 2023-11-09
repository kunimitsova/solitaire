using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour {

    Solitaire solitaire;
    App_Initialize appInit;
    public GameObject winScreenUI;

    private void Start() {
        solitaire = FindObjectOfType<Solitaire>();
        winScreenUI.SetActive(false);
    }
 
    public void ReplayGame() {
        winScreenUI.SetActive(false);
        solitaire.ResetTable();
        // keep the same shuffled deck for this replay
        solitaire.PlayCards();
    }

    public void NewGame() {
        winScreenUI.SetActive(false);
        solitaire.ResetTable();
        // get a new shuffle for this game
        solitaire.PrepDeck();
        solitaire.PlayCards();
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
        
    }
}
