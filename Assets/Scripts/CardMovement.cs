using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardMovement : MonoBehaviour {

    GetSettingsValues getSettings;

    public delegate void UndoBackIntoTalonList(List<GameObject> dealtCards);
    public static event UndoBackIntoTalonList UndoTalonList;

    public delegate float GettingTheLastTalonZValue();
    public static event GettingTheLastTalonZValue GetPrevCardZ;

    private void Start() {
        getSettings = FindObjectOfType<GetSettingsValues>();
        Undo.UndoDealtCards += MoveUndealDealtCards;
        Undo.MoveCard += MoveCard;
    }

    private void OnDisable() {
        Undo.UndoDealtCards -= MoveUndealDealtCards;
        Undo.MoveCard -= MoveCard;
    }

    /// <summary>
    /// Move one card to the specified place. Do not update the parent object until after the move is complete.
    /// </summary>
    /// <param name="cardToMove"></param>
    /// <param name="placeToMove"></param>
    /// <param name="timeToMove"></param>
    public void MoveCard(GameObject cardToMove, GameObject placeToMove, float timeToMove) {
        // this ONLY moves the card and updates card location data, parent data is not updated.

        GameObject placeYoungest;// make sure that we are stacking at the YOUNGEST position (will only change in STACKS, right?
        placeYoungest = Utilities.FindYoungestChild(placeToMove.transform).gameObject;
        Selectable cardSel = cardToMove.GetComponent<Selectable>();
        Selectable placeSel = placeYoungest.GetComponent<Selectable>();

        Debug.Log("placeToMOve = " + placeToMove.name + " placeYoungest (Youngest child) = " + placeYoungest.name + "; cardToMove = " + cardToMove.name);

        float xPos = GetXPos(placeToMove, placeYoungest.transform);
        float yPos = GetYPos(placeToMove, placeYoungest.transform);
        float zPos = GetZPos(placeToMove, placeYoungest.transform);

        bool inDeck;
        bool inTop;
        int inRow;

        if (placeToMove.CompareTag(Constants.DECK_TAG)) { // we are moving a card back into the deck dealt card stack it is the top parent of the other cards
            inDeck = true;
            inTop = false;
            inRow = 0;
        }
        else if (placeToMove.CompareTag(Constants.TOP_TAG)) { // the parent of the card was originally a TOP spot
            inDeck = false;
            inTop = true;
            inRow = placeSel.row;
        }
        else if (placeToMove.CompareTag(Constants.BOTTOM_TAG)) { // the card was in the bottom stacks and the youngest child of the BOTTOM spot OR to an empty BOTTOM spot
            Debug.Log("MoveCard (CardMovement) move to BOTTOM after a flip, placeObject (youngest child) = " + placeYoungest.name);
            inDeck = false;
            inTop = false;
            inRow = placeSel.row;
        }
        else { // card is moving to CARD
            inDeck = placeSel.inDeckPile;
            inTop = placeSel.top;
            inRow = placeSel.row;
        }
        MovedCardSelectablesUpdate(cardSel, inRow, inDeck, inTop);

        Vector3 v1 = new Vector3(xPos, yPos, zPos);
        LeanTween.move(cardToMove, v1, timeToMove);
    }

    float GetXPos(GameObject placeToMove, Transform placeYoungest) {
        // if the place is DECK, and the YoungestChild is DECK, then absolute xPos = initXDeckOffset
        if (placeToMove.CompareTag(Constants.DECK_TAG)) {
            if (placeYoungest.CompareTag(Constants.DECK_TAG)) {
                return getSettings.InitXDeckOffset;
            }
            else {         // if the place is DECK, and the YoungestChild is NOT DECK, then absolute xPos = Youngest + deckXOffset
                return (placeYoungest.position.x + getSettings.XDeckOffset);
            }
        }
        // for everything else, absolute xPos is parent xPos.
        return placeToMove.transform.position.x;
    }

    float GetYPos(GameObject placeToMove, Transform placeYoungest) {

        Selectable placeSel;

        if (placeToMove.CompareTag(Constants.DECK_TAG)) {
            placeSel = placeYoungest.GetComponent<Selectable>();
        }
        else {
            placeSel = placeToMove.GetComponent<Selectable>();
        }
        // only cards in the BOTTOM stacks get y-Offset
        if (placeToMove.CompareTag(Constants.BOTTOM_TAG)) { // card was in a bottom stack
            if (placeToMove.transform == placeYoungest) { // card moved from an empty stack, no y-Offset
                return placeYoungest.position.y;
            }
            else { // card moved from a non-empty stack and was the final face-up card from the deal
                return placeYoungest.position.y + Constants.STACK_Y_OFFSET;
            }
        }
        else if (!placeSel.inDeckPile && !placeSel.top) { // not in Deck pile and not in Top = it's in the Bottom stacks with a card as the parent 
            return placeYoungest.position.y + Constants.STACK_Y_OFFSET;
        }
        // for everything else, yPos = placeToMove.y
        return placeToMove.transform.position.y;
    }

    float GetZPos(GameObject placeToMove, Transform placeYoungest) {
        return placeYoungest.position.z + Constants.Z_OFFSET; // I feel like this isn't technically right wrt cards in the talon but also those cards are orphans so 
    }

    public void MovedCardSelectablesUpdate(Selectable cardMoved, int currentRow, bool deckPile, bool topStack) {
        cardMoved.row = currentRow;
        cardMoved.inDeckPile = deckPile;
        cardMoved.top = topStack;
        foreach (Transform child in cardMoved.transform) {
            child.GetComponent<Selectable>().row = currentRow;
        }
    }

    public void MoveUndealDealtCards(GameObject deckButton) {
        // The assumption is that undo-ing a deal still contains the entire deal (i.e., no movement has happened since the deal)
        // this DOES update parent info (current = deckButton; orig = Null)
        List<GameObject> dealtList = new List<GameObject>();
        Transform card;
        Selectable cardSel;
        int localDealtAmount = (deckButton.transform.childCount < getSettings.TalonDealAmount) ? deckButton.transform.childCount : getSettings.TalonDealAmount; // covers cases when there are fewer than TalonDaelAmount in the deal being undone

        float zValue = ( GetPrevCardZ == null) ? 0f : GetPrevCardZ.Invoke(); // get the z-position of the last item in the current Talon

        for (int i = 0; i < localDealtAmount; i++) { // put items into the list in reverse order that they were drawn. so now the small list is exactly as if it hadn't been dealt.
            card = deckButton.transform.GetChild(deckButton.transform.childCount - 1);  //  we only need the LAST item because it is removed each time this runs.
            card.SetParent(null);
            dealtList.Add(card.gameObject);
            Debug.Log("Cards number i: " + card.name);
        }
        foreach (GameObject item in dealtList) { // This might be a mess but it kinda doesn't matter? As long as cards go back into TALON properly?
            zValue += Constants.UNDEALT_CARD_Z_OFFSET; // add the offset so the next card is not in the same z-space as the previous Last Talon Item.
            item.transform.position = new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, zValue); 
            cardSel = item.GetComponent<Selectable>();
            MovedCardSelectablesUpdate(cardSel, currentRow: 0, deckPile: true, topStack: false);
            cardSel.faceUp = false;
            item.GetComponent<UpdateSprite>().ShowCardFace();
        }

        UndoTalonList?.Invoke(dealtList);
    }
}
