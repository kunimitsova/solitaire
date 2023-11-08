using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtons : MonoBehaviour {

    Solitaire solitaire;
    App_Initialize appInit;
    public GameObject gameOverUI;

    private void Start() {
        solitaire = FindObjectOfType<Solitaire>();
    }
 
    public void ReplayGame() {
        gameOverUI.SetActive(false);
        solitaire.ResetTable();
        // keep the same shuffled deck for this replay
        solitaire.PlayCards();
    }

    public void NewGame() {
        gameOverUI.SetActive(false);
        solitaire.ResetTable();
        // get a new shuffle for this game
        solitaire.PrepDeck();
        solitaire.PlayCards();
    }

    public void SeeSettings() {
        gameOverUI.SetActive(false);
        // show Settings menu

        // how to set bool values :
        // PlayerPrefs.SetInt("myBool", myBool ? 1 : 0); // 1 is true and 0 is false

        // when exiting settings menu :
        PlayerPrefs.Save();
    }

}
