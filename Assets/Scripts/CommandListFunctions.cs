using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;


public class CommandListFunctions : MonoBehaviour {
    // these aren't really commands but it kinda works that way.

    // what if I restructured my system, so it woudl say what I wrote down.

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

        public override string ToString() {
            string str = (CardName == null) ? "null" : CardName.ToString();
            string str1 = (cardMovedFrom == null) ? "null sb none" : cardMovedFrom.ToString();
            string str2 = Row == null ? "null" : Row.ToString();
            return "Card : " + str + " move : " + str1 + " row : " + str2;
        }
    }

    List<CardAction> _moveList = new List<CardAction>(); // list of all the moves
    List<CardAction> _undo = new List<CardAction>();  // list that will hold card actions that need to be undone

    public void OnEnable() {
        UIButtons.GameRenewed += ClearUndoList;
        //UIButtons.AutoplayClicked += ShowListInConsole;
        UserInput.Moved += AddToMoveList;
        UserInput.Flipped += AddCardFlipToMoveList;
        UIButtons.UndoClicked += RemoveFromMoveList; // if we decide to add Redo , just remove this line
    }
    private void OnDisable() {
        UIButtons.GameRenewed -= ClearUndoList;
        //UIButtons.AutoplayClicked -= ShowListInConsole;
        UserInput.Moved -= AddToMoveList;
        UserInput.Flipped -= AddCardFlipToMoveList;
        UIButtons.UndoClicked -= RemoveFromMoveList;
    }

    public string ShowListInConsole() { // remember the subs were commented out above
        //string myString = string.Join<CardAction>("\nl", _moveList);
        //Debug.Log(myString);

        return _moveList[_moveList.Count - 1].ToString();
    }

    public void ClearUndoList() {
        _moveList.Clear(); 
    }

    public void RemoveFromMoveList() {
        int removeItems = _moveList[_moveList.Count - 1].cardMovedFrom == CardMovement.flip ? 2 : 1; // number of items to remove when Undo is clicked
        for (int i = 0; i < removeItems; i++) {
            _undo.Add(_moveList[_moveList.Count - 1]); // add the last thing to the list of things that has to be done.
            _moveList.RemoveAt(_moveList.Count - 1); // remove the last added item or the last item plus the flip that happened afterwards
        }
    }

    void AddToMoveList(GameObject g1, GameObject g2) {
        _undo.Clear(); // do not have any undo actions when adding new actions to the Move List.
        CardMovement move;
        string cardMoveString = "";
        string cardName;
        int? row;
        CardAction ca;

        if (ThisIsNotAValidMove(g1, g2)) {
            return;
        }

        // if it's two deck buttons, then the move is DD so do that and exit
        else if (g1.CompareTag(Constants.DECK_TAG) && g2.CompareTag(Constants.DECK_TAG)) {
            move = CardMovement.DD;
            cardName = null;
            row = null;
        }
        // if it's not either of those two cases, get the cardMovement based on Selectables
        else {
            Selectable s1 = g1.GetComponent<Selectable>();
            Selectable s2 = g2.GetComponent<Selectable>();
            cardMoveString += MovePart(s1);  // first prt of CardMovement
            cardMoveString += MovePart(s2); // second part of CardMovement
            row = s1.row;
        }

        move = GetCardMoves(cardMoveString);

        cardName = g1.CompareTag(Constants.CARD_TAG) ? g1.name : null;

        ca.CardName = cardName;
        ca.cardMovedFrom = move;
        ca.Row = row;

        _moveList.Add(ca);

    }

    void AddCardFlipToMoveList(GameObject g1) { // we only need the 1st object really....
        CardAction ca;
        ca.cardMovedFrom = CardMovement.flip;
        ca.Row = g1.GetComponent<Selectable>().row;
        ca.CardName = null;
        _moveList.Add(ca);
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

        if (g1.CompareTag(Constants.DECK_TAG) && (g2.CompareTag(Constants.DECK_TAG))) {
            return false; // that means it is a DEAL FROM TALON move.
        }

        if (g1.CompareTag(Constants.TOP_TAG) || g1.CompareTag(Constants.BOTTOM_TAG) || g1.CompareTag(Constants.DECK_TAG)) {
            Debug.Log("TOP_TAG, BOTTOM_TAG, DECK_TAG items cannot move to another place.");
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
   
}
