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
        Undo.MoveUndealt += MoveUndealDealtCards;
    }

    public void MoveCard(GameObject cardToMove, GameObject placeToMove, float timeToMove) { 
        // this ONLY moves the card and updates card location data, not parent data

        GameObject placeObject = Utilities.FindYoungestChild(placeToMove.transform).gameObject; // make sure that we are stacking at the YOUNGEST position
        Selectable cardSel = cardToMove.GetComponent<Selectable>();
        Selectable placeSel = placeObject.GetComponent<Selectable>();

        float xOffset = 0f;
        float yOffset = 0f;
        float zOffset = Constants.Z_OFFSET;

        if (placeToMove.CompareTag(Constants.DECK_TAG)) { // we are moving a card back into the deck dealt card stack
            xOffset = getSettings.XDeckOffset;
            yOffset = 0f;
            MovedCardSelectablesUpdate(cardSel, placeSel.row, deckPile: true, topStack: false);
        }
        else { // the card is moving into either Foundations or Tableau stacks
            xOffset = 0f;
            yOffset = placeSel.top ? 0f : Constants.STACK_Y_OFFSET;
            MovedCardSelectablesUpdate(cardSel, placeSel.row, placeSel.inDeckPile, placeSel.top);
        }

        Vector3 v1 = new Vector3(placeObject.transform.position.x + xOffset, placeObject.transform.position.y + yOffset, placeObject.transform.position.z + zOffset);
        LeanTween.move(cardToMove, v1, timeToMove);
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
        int localDealtAmount = (deckButton.transform.childCount > getSettings.TalonDealAmount) ? deckButton.transform.childCount : getSettings.TalonDealAmount; // covers cases when there are fewer than TalonDaelAmount in the deal being undone

        float zValue = ( GetPrevCardZ == null) ? 0f : GetPrevCardZ.Invoke(); // get the z-position of the last item in the current Talon

        for (int i = 0; i < localDealtAmount; i++) { // put items into the list in reverse order that they were drawn. so now the small list is exactly as if it hadn't been dealt.
            card = deckButton.transform.GetChild(deckButton.transform.childCount - 1);  //  we only need the LAST item because it is removed each time this runs.
            card.SetParent(null);
            dealtList.Add(card.gameObject);
        }
        foreach (GameObject item in dealtList) {
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
