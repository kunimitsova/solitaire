using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour {

    public GameObject gameOverUI;

    public GameObject slot1;

    private Solitaire solitaire;

    void Start() {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;  // this is the kindof inelegant way to determine if a card is currently selected.
    }

    void Update() {
        GetMouseClick();
    }

    // we will have to edit this for touching...
    void GetMouseClick() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.DrawRay(mousePos, transform.forward * 10, Color.red, 0.5f);
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit) {
                // what has been hit? deck, card,empty slot...
                if (hit.collider.CompareTag(Constants.DECK_TAG)) {
                    Deck();
                } else if (hit.collider.CompareTag(Constants.CARD_TAG)) {
                    Card(hit.collider.gameObject);
                } else if (hit.collider.CompareTag(Constants.TOP_TAG)) {
                    Top(hit.collider.gameObject);
                } else if (hit.collider.CompareTag(Constants.BOTTOM_TAG)) {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck() {
        // deck click actions
        solitaire.DealFromDeck();
    }

    void Card(GameObject selected) {
        // card click actions

        // if the card is not blocked and not face up
        if (!selected.GetComponent<Selectable>().faceUp) {
            if (!Blocked(selected)) {
                //flip it over
                FlipCard(selected);
                slot1 = this.gameObject;
            }
        }
        // if the card is in the deck and not blocked (top card from the deck deal)
        else if (selected.GetComponent<Selectable>().inDeckPile) {
            if (!Blocked(selected)) {
                slot1 = selected;
                Stack(CanBeStackedHere(slot1));
            }
        }
        else {
            // the card is face up, there is nothing else selected
            if (slot1 == this.gameObject) { 
                slot1 = selected;
                Stack(CanBeStackedHere(slot1));
            }
        }
        slot1 = this.gameObject; // reset slot1?
    }

    void Top(GameObject selected) {
        // top empty spot click options
        //Debug.Log("Clicked on Top");
        if (slot1.CompareTag(Constants.CARD_TAG)) {
            if (slot1.GetComponent<Selectable>().value == Constants.ACE_VALUE) { // *****************************hard coded value pls check
                Stack(selected);
            }
        }
    }

    void Bottom(GameObject selected) {
        // bottom empty slot click options
        // the card is a king and the empty slot is bottom then stack
        if (slot1.CompareTag(Constants.CARD_TAG)) {
            if (slot1.GetComponent<Selectable>().value == Constants.KING_VALUE) { // *****************************************hard coded value pls check
                Stack(selected);
            }
        }
    }

    void FlipCard(GameObject selected) {
        bool facing = selected.GetComponent<Selectable>().faceUp;
        selected.GetComponent<Selectable>().faceUp = !facing;
        selected.GetComponent<UpdateSprite>().ShowCardFace();
    }

    bool Stackable(GameObject selected) {
        Selectable s1 = slot1.GetComponent<Selectable>(); // selectable from slot1
        Selectable s2 = selected.GetComponent<Selectable>(); // selectable from 'selected' gameobj

        if (!s2.inDeckPile) {
            if (s2.top) { // moving a card to the foundation
                if ((s1.suit == s2.suit) || (s1.value == 1 && s2.suit == null)) {
                    if (s1.value == s2.value + 1) {
                        return true;
                    }
                }
                else {
                    return false;
                }
            }
            else { // moving a card to a tableau stack
                if ((s1.value == s2.value - 1) || ((s1.value == 13) && (s2.value == 0))) { // if the card values are one apart OR the 1st click is Kint and the 2nd is null space
                    if ((Solitaire.red.Contains(s1.suit) && Solitaire.black.Contains(s2.suit)) || (Solitaire.black.Contains(s1.suit) && Solitaire.red.Contains(s2.suit))) {
                        //Debug.Log($"Stackable on tableau {s1.suit.ToString()}{s1.value.ToString()} on {s2.suit.ToString()}{s2.value.ToString()}");
                        return true;
                    }
                }
                else {
                    //Debug.Log($"Not Stackable {s2.suit.ToString()}{s2.value.ToString()} on {s1.suit.ToString()}{s1.value.ToString()}");
                    return false;
                }
            }
        }
        return false;
    }

    GameObject CanBeStackedHere(GameObject selected) {
        // This replaces 'Stackable' and determines the FIRST location that the selected card can be stacked.
        // We will keep using the concepts of "slot1" and "slot2" to represent the card selected and the location
        // gameobject to which it can be moved.

        GameObject newSpot = this.gameObject;  // the location that the card might be moved to if the conditions work out
        Selectable s1 = selected.GetComponent<Selectable>(); // selectable from the card to be moved
        Selectable s2; // selectable from the potential place to move the card to
        Transform lastChild;
          
        for (int i = 0; i < solitaire.topPos.Length; i++) { // Iterate through top locations
            newSpot = solitaire.topPos[i].gameObject;
            s2 = newSpot.GetComponent<Selectable>();
            if ((s1.suit == s2.suit) || (s1.value == Constants.ACE_VALUE && s2.suit == null)) { // if the suits are the same OR the card is an Ace and the topPos is blank
                if (s1.value == s2.value + 1) {
                    return newSpot;
                }
            }
        }
        // we did not return yet so keep going
        // look at every playable card in the bottom section?
        for (int i = 0; i < solitaire.bottomPos.Length; i++) {
            lastChild = solitaire.FindYoungestChild(solitaire.bottomPos[i].transform);
            //Debug.Log("Looking at tableau lastChild = " + lastChild.gameObject.name.ToString());
            newSpot = lastChild.gameObject;
            s2 = newSpot.GetComponent<Selectable>();
            if ((s1.value == s2.value - 1) || ((s1.value == Constants.KING_VALUE) && (s2.value == 0))) {
                if ((Solitaire.red.Contains(s1.suit) && Solitaire.black.Contains(s2.suit)) || (Solitaire.red.Contains(s2.suit) && Solitaire.black.Contains(s1.suit))  ) {
                    return newSpot;
                }
            }
        }
        return this.gameObject;
    }

    void Stack(GameObject selected) {

        if (selected == this.gameObject) {
            return;
        }

        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        int row = !s1.top ? s1.row : 0; 

        float yOffset = Constants.STACK_Y_OFFSET;
        float zOffset = Constants.Z_OFFSET;

        Transform parentCard;
        Transform childCard;

        if (s2.top || (!s2.top && s1.value == Constants.KING_VALUE)) {
            yOffset = 0; // don't offset the card from the stack origination
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y + yOffset, selected.transform.position.z + zOffset);
        parentCard = slot1.transform.parent; // save this position so we can look at it after the card is moved
        slot1.transform.parent = selected.transform; // this is so the children move with the parent. 

        // check if the original parent of slot1 still has any children after moving slot1 then check 
        // if the s1 card is not in the Top or in the Deck, check if the row it was in prior to moving has an unflipped card
        if (parentCard.childCount > 0) {
            if (row > 0 && !s1.inDeckPile) {
                childCard = solitaire.bottomPos[row].transform.GetChild(solitaire.bottomPos[row].transform.childCount - 1);
                if (!childCard.GetComponent<Selectable>().faceUp) {
                    FlipCard(childCard.gameObject);
                }
            }
        }

        // set the new sort order based on the parent card
        parentCard = selected.transform;

        while (parentCard.childCount > 0) { // this only for sortingOrder which we may not even be using about now...
            //Debug.Log("Parent card start = " + parentCard.GetComponent<Selectable>().suit.ToString() + parentCard.GetComponent<Selectable>().value.ToString());
            childCard = parentCard.GetChild(parentCard.childCount - 1); // get the most recently added child of the current parent
            childCard.GetComponent<SpriteRenderer>().sortingOrder = parentCard.GetComponent<SpriteRenderer>().sortingOrder + 1;
            parentCard = childCard;
            //Debug.Log("Parent card end = " + parentCard.GetComponent<Selectable>().suit.ToString() + parentCard.GetComponent<Selectable>().value.ToString());
        }

        if (s1.inDeckPile) { // removes cards from the deck when they are moved to tab or found
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }
        else if (s1.top && s2.top && (s1.value == 1)) { // allows movement of cards between top spots
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.top) {                                // moving a card from the foundations back to the tableau
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else {                                            // removes the card string from the appropriate bottom list
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; 
        //Debug.Log($"Slot1 row is {s1.row} slot2 row is {s2.row}");
        s1.row = s2.row;

        // I understand the problem now - when s1 is moved to the foundation, it is removed from the deck list and therefore is no longer a gameobject.
        if (s2.top) {
            Debug.Log("s2.top is True and s1.suit = " + s1.suit.ToString() + " and s1.value = " + s1.value.ToString() + " AND s2 = " + s2.suit.ToString() + s2.value.ToString());
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }
        else {
            s1.top = false;
        }
        // after moving make slot1 (first card to move) work like null without using null (bc logic probs)
        slot1 = this.gameObject;

       // Debug.Log("Cards have been stacked and parentCard = " + parentCard.GetComponent<Selectable>().suit + parentCard.GetComponent<Selectable>().value.ToString() + " and isPlayable is " + parentCard.GetComponent<Selectable>().isPlayable.ToString());

    }

    bool Blocked(GameObject selected) {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile) {
            if (s2.name == solitaire.tripsOnDisplay.Last()) {
                return false;
            }
            else {
                Debug.Log($" {s2.name} is blocked by {solitaire.tripsOnDisplay.Last()}");
                return true;
            }
        }
        else {
            if (s2.name == solitaire.bottoms[s2.row].Last()) {
                return false;
            }
            else {
                return true;
            }
        }
    }

    void AutoStack(GameObject oneSelected) {
        // for later: add the option if you click on an empty spot whatever might fit there will automatically lerp there
        for (int i = 0; i < solitaire.topPos.Length; i++) {
            // can't I just go through the positions using selected = solitaire.topPos[i].GetComponent<GameObject> and use Stackable?
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (oneSelected.GetComponent<Selectable>().value == Constants.ACE_VALUE) { // card is an Ace
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0) {
                    slot1 = oneSelected;
                    Stack(stack.gameObject);
                    break;
                }
            } else {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && 
                    (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1)) {
                    if (oneSelected.transform.childCount == 0) {  // if it is the last card in the cardstack
                        slot1 = oneSelected;
                        string lastCardName = stack.suit + stack.value.ToString();
                        if (stack.value == Constants.ACE_VALUE) {
                            lastCardName = stack.suit + Constants.ACE_STRING;
                        }
                        if (stack.value == Constants.JACK_VALUE) {
                            lastCardName = stack.suit + Constants.JACK_STRING;
                        }
                        if (stack.value == Constants.QUEEN_VALUE) {
                            lastCardName = stack.suit + Constants.QUEEN_STRING;
                        }
                        if (stack.value == Constants.KING_VALUE) {
                            lastCardName = stack.suit + Constants.KING_STRING;
                        }
                        GameObject lastCard = GameObject.Find(lastCardName);
                        Stack(lastCard);
                        break;
                    }
                }
            }
        }
    }
}
