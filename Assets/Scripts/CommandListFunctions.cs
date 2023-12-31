using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;


public class CommandListFunctions : MonoBehaviour {
    // these aren't really commands but it kinda works that way.

    public delegate void CallTheUndoAction(GameObject g1, Transform origParent, int row, bool flipped);
    public static event CallTheUndoAction UndoMove;

    public struct CardAction {
        public GameObject Card; // name of the card that acted
        public Transform MovedFrom; // parent obj of the card that moved
//        public CardMovement CardMovedFrom; // code telling where the card moved from.
        public int Row; // row that the card was in initially in. it will be null for cards from the deck, so DT, DF, and DD all have null Row.
        public bool Flip; // if a card under the moved card was flipped face up after the move (only in SS, SF moves)

        public override string ToString() {
            string str = (Card.name == null) ? "null" : Card.name.ToString();
            string str1 = (MovedFrom == null) ? "null" : MovedFrom.name.ToString();
            string str2 = Row.ToString();
            string str3 = Flip.ToString();
            return "Card : " + str + " move : " + str1 + " row : " + str2 + " flipped : " + str3;
        }
    }

    List<CardAction> _moveList = new List<CardAction>(); // list of all the moves
    List<CardAction> _undo = new List<CardAction>();  // list that will hold card actions that need to be undone

    public void OnEnable() {
        UIButtons.GameRenewed += ClearUndoList;
        UserInput.AddToMoveList += AddToMoveList;
        UIButtons.UndoClicked += DoUndo;
    }
    private void OnDisable() {
        UIButtons.GameRenewed -= ClearUndoList;
        UserInput.AddToMoveList -= AddToMoveList;
        UIButtons.UndoClicked -= DoUndo;
    }

    public string ShowListInConsole() { // remember the subs were commented out above
        
        return _moveList[_moveList.Count - 1].ToString();
    }

    public void ClearUndoList() {
        _moveList.Clear(); 
    }

    public void RemoveFromMoveList() {
        
        _undo.Add(_moveList[_moveList.Count - 1]); // add the last thing to the list of things that has to be done.
        _moveList.RemoveAt(_moveList.Count - 1); // remove the last added item
        
    }

    void AddToMoveList(GameObject g1, Transform origParent, int row, bool wasFlip) {

        _undo.Clear(); // do not have any undo actions when adding new actions to the Move List.
        Transform move = origParent;
        GameObject card = g1;
        int localRow;
        CardAction ca;

       // check if this is a valid move in the CALLING function

        if (g1.CompareTag(Constants.DECK_TAG)) { // It was a deal of the deck talon
            move = g1.transform; 
            card = g1;
            localRow = 0;
        }
        // if it's not a deck deal get the row
        else {
            localRow = row;
        }

        //cardName = g1.CompareTag(Constants.CARD_TAG) ? g1.name : null;

        ca.Card = card;
        ca.MovedFrom = move;
        ca.Row = localRow;
        ca.Flip = wasFlip;

        _moveList.Add(ca);
        Debug.Log("Added to move list : " + ca.ToString());
    }

    public void DoUndo() {
        if (_moveList.Count < 1) {
            return;
        }
        CardAction ca = _moveList[_moveList.Count - 1];
        Debug.Log("Undo move: " + ca.Card.name);
        UndoMove?.Invoke(ca.Card, ca.MovedFrom, ca.Row, ca.Flip);
        RemoveFromMoveList();

    }

    //void AddCardFlipToMoveList(GameObject g1) { // removing FLIP as an option, we know if something was flipped... I think?
    //    CardAction ca;
    //    ca.cardMovedFrom = CardMovement.flip;
    //    ca.Row = g1.GetComponent<Selectable>().row;
    //    ca.CardName = null;
    //    _moveList.Add(ca);
    //}

    //CardMovement GetCardMoves(string move) {
    //    switch (move) {
    //        case Constants.CARDMOVE_DECK + Constants.CARDMOVE_FOUNDATION:
    //            return CardMovement.DF;
    //        case Constants.CARDMOVE_DECK + Constants.CARDMOVE_STACK:
    //            return CardMovement.DS;
    //        case Constants.CARDMOVE_STACK + Constants.CARDMOVE_FOUNDATION:
    //            return CardMovement.SF;
    //        case Constants.CARDMOVE_STACK + Constants.CARDMOVE_STACK:
    //            return CardMovement.SS;
    //        case Constants.CARDMOVE_FOUNDATION + Constants.CARDMOVE_FOUNDATION:
    //            return CardMovement.FF;
    //        case Constants.CARDMOVE_FOUNDATION + Constants.CARDMOVE_STACK:
    //            return CardMovement.FS;
    //        default:
    //            return CardMovement.none;
    //    }
    //}

    //string MovePart(Selectable localS1) {

    //    // cases when the place to move to isn't a card but a spot like Foundation or stacks:
    //    switch (localS1.tag) {
    //        case Constants.TOP_TAG:
    //            return Constants.CARDMOVE_FOUNDATION;
    //        case Constants.BOTTOM_TAG:
    //            return Constants.CARDMOVE_STACK;
    //        default:
    //            break;
    //    }

    //    string move = "";

    //    if (localS1.top) {
    //        move = Constants.CARDMOVE_FOUNDATION;
    //    }
    //    else if (localS1.inDeckPile) {
    //        move = Constants.CARDMOVE_DECK;
    //    }
    //    else {
    //        move = Constants.CARDMOVE_STACK;
    //    }
    //    return move;
    //}
   
}
