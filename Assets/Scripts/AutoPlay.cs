//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AutoPlay : MonoBehaviour {
//    // attach to ... SolitaireGame? or make a new one?
//    Solitaire solitaire;

//    private void Start() {
//        solitaire = GetComponent<Solitaire>();
//        // add the AutoPlay items to the Action in UIButtons
//    }

//    private void OnDisable() {
//        // remove the AutoPlay items from the Action in UIButtons
//    }

//    public bool IsAutoplayAnOption() {        
//        // talon must be able to uncover --- how will I know this.
//        // talonDealAmount from Settings = 1 
//        if (PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT) <= 1) {
//            // total amount of cards in Dealt and Undealt talon <=1.
//            // TODO: make a property in Solitaire for dealt + undealt talon?
           
//        }
//        // criteria: all TABLEAU cards must be Uncovered
//        // +++ OR +++

//        return false;
//    }
//}
