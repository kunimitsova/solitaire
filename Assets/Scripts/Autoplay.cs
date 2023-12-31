//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;

//public class Autoplay : MonoBehaviour {

//    public delegate bool HaveWeWonTheGame();
//    public static event HaveWeWonTheGame GameWon;

    

//    public delegate GameObject GettingThePlaceToMove();
//    public static event GettingThePlaceToMove GotPlaceToMove;

//    private void Start() {
//        UIButtons.AutoplayClicked += AutoPlay;
//    }

//    void AutoPlay() {

//        bool win = GameWon();
//        bool cardStacked = false;
//        GameObject card;

//        while (!win) {
//            // go through the TOP positions and see what card is needed 
//            // look for that card
//            //      if it's not blocked, play to foundation
//            //      else go to the next TOP pos
//            // if no cards are playable, deal from talon

//        }

//    }
//}
