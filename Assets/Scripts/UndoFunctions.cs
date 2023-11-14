using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoFunctions : MonoBehaviour {

    public enum CardMovement {
        SS, // stack to stack (e.g. King to empty slot)
        SF, // stack to foundation (e.g. Two in the 7th column to the Ace in the top0 spot)
        FS, // foundation to stack (when you need to pull those cards back out to help you move other cards)
        FF, // idk but there's always this, moving the ace around to different foundation spots
        DS, // deck to stack
        DF, // deck to foundation
        DD, // deck deal (no movement to tab or found)
        flip, // when a card is flipped on the tableau - this is not done by the user but automatically when the user moves a card
        none // for when we can't find anything that matches (this will only happen if there's a problem)
    }

    public struct CardAction {
        public string CardName; // name of the card that acted
        public CardMovement cardMovedFrom; // code telling where the card moved from.
        public int? Row; // row that the card was in initially in. it will be null for cards from the deck, so DT, DF, and DD all have null Row.
    }

    List<CardAction> _moveList = new List<CardAction>();

    private void Start() {
        
    }

    void AddToMoveList(GameObject g1, GameObject g2) {
        CardMovement move;
        string cardMoveString = "";
        string cardName;
        int row;

        if (ThisIsNotAValidMove(g1, g2)) {
            return;
        }

        // if it's two deck buttons, then the move is DD so do that and exit
        else if (g1.CompareTag(Constants.DECK_TAG) && g2.CompareTag(Constants.DECK_TAG)) {
            move = CardMovement.DD;
        }
        // if it's not either of those two cases, get the cardMovement based on Selectables
        else {
            Selectable s1 = g1.GetComponent<Selectable>();
            Selectable s2 = g2.GetComponent<Selectable>();
            cardMoveString += MovePart(s1);  // first prt of CardMovement
            cardMoveString += MovePart(s2); // second part of CardMovement
        }

        cardName = g1.CompareTag(Constants.CARD_TAG) ? g1.name : "";


    }

    CardMovement GetCardMoves(string move) {
        switch (move) {
            case Constants.CARDMOVE_DECK + Constants.CARDMOVE_FOUNDATION:
                return CardMovement.DF;
            case Constants.CARDMOVE_DECK + Constants.CARDMOVE_STACK:
                return CardMovement.DS;
            case Constants.CARDMOVE_STACK + Constants.CARDMOVE_FOUNDATION:
                return CardMovement.SF;
            case Constants.CARDMOVE_STACK + Constants.CARDMOVE_STACK:
                return CardMovement.SS;
            case Constants.CARDMOVE_FOUNDATION + Constants.CARDMOVE_FOUNDATION:
                return CardMovement.FF;
            case Constants.CARDMOVE_FOUNDATION + Constants.CARDMOVE_STACK:
                return CardMovement.FS;
            default:
                return CardMovement.none;
        }
    }

    bool ThisIsNotAValidMove(GameObject g1, GameObject g2) {
        // make sure it's not trying to move things that are not cards
        if (g1.CompareTag(Constants.TOP_TAG) || (g1.CompareTag(Constants.BOTTOM_TAG)) || (g1.CompareTag(Constants.DECK_TAG))) {
            Debug.Log("TOP_TAG, BOTTOM_TAG, and DECK_TAG items cannot move to another place.");
            return true;
        }
        // make sure it's not trying to move INTO the deck pile
        else if (g2.CompareTag(Constants.DECK_TAG)) {
            Debug.Log("You cannot move from somewhere to the deck pile.");
            return true;
        }

        return false;
    }

    string MovePart(Selectable localS1) {

        // cases when the place to move to isn't a card but a spot like Foundation or stacks:
        switch (localS1.tag) {
            case Constants.TOP_TAG:
                return Constants.CARDMOVE_FOUNDATION;
            case Constants.BOTTOM_TAG:
                return Constants.CARDMOVE_STACK;
            default:
                break;
        }

        string move = "";

        if (localS1.top) {
            move = Constants.CARDMOVE_FOUNDATION;
        }
        else if (localS1.inDeckPile) {
            move = Constants.CARDMOVE_DECK;
        }
        else {
            move = Constants.CARDMOVE_STACK;
        }
        return move;
    }
    
    // subscribe to a thing that triggers when any card is actually moved?
    // so, another delegate that invokes whenever the card moves. so, right before the actual move takes place OR when the user clicks on Deal from Talon.
    // so what we want to subscribe to it is a method from here that invokes whenever a card is moved. Or, the event that a card is moved.
    // so the delegate is in the solitaire class? or the userinput class? probably userinput.
    // then in the start method in here, subscribe to it by adding the "add last move to the list" sub
    // make sure to unsubscribe in a destroyed or disabled or whatever, idk


    // one function for collecting the list, so set up the list in here.
    // List should be string based. Maybe a type enum will help.

}
