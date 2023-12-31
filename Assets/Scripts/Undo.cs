using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undo : MonoBehaviour {
    // attach to UNDO object
    // has Start

    // delegate for Flipping a card, takes a Gameobject to flip
    public delegate void FlippingACard(GameObject card);
    public static event FlippingACard FlipCard;

    public delegate void MovingACard(GameObject card, GameObject moveTo, float timeToMove);
    public static event MovingACard MoveCard;

    public delegate void MovingDealtTalonItems(GameObject deckButton);
    public static event MovingDealtTalonItems UndoDealtCards;

    private void Start() {
        CommandListFunctions.UndoMove += UndoOneMove;
    }

    private void OnDisable() {
        CommandListFunctions.UndoMove -= UndoOneMove;
    }

    void UndoOneMove(GameObject g1, Transform origParent, int row, bool flipped) { // get the move to undo and undo it. I think remove it from the list is in CommandListFunctions

        Transform youngestChild = Utilities.FindYoungestChild(origParent); // the Youngest Child may not be the parent of the object that moved, but it is the location in question for flipping etc.

        // if the g1 == DeckBUtton then UNDO the deal , meaning place the last [user talon deal amount] back into the DeckButton object
        if (g1.CompareTag(Constants.DECK_TAG)) {
            UndoDealtCards?.Invoke(g1);
        }

        // make sure g1 is card before doing card actions
        if (g1.CompareTag(Constants.CARD_TAG)) {
            if ((youngestChild != origParent) && flipped) { // if the current faceup card in the stack is NOT the parent of the g1 and a flip was registered (tbh do I need both of these? idk.)
                FlipCard?.Invoke(youngestChild.gameObject);
            }

            MoveCard?.Invoke(g1, origParent.gameObject, Constants.ANIMATE_DEAL_FROM_TALON);

            g1.transform.SetParent(origParent);// give the card the original parent
        }
    }
}
