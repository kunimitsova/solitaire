using UnityEngine;

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
        //Debug.Log("Hit the deck");
        solitaire.DealFromTalon();
    }

    void Card(GameObject selected) {
        // card click actions

        // if the card is not blocked and not face up
        if (!selected.GetComponent<Selectable>().faceUp) {
            if (!solitaire.Blocked(selected)) {
                //flip it over
                FlipCard(selected);
                slot1 = this.gameObject;
            }
        }
        // if the card is in the deck and not blocked (top card from the deck deal)
        else if (selected.GetComponent<Selectable>().inDeckPile) {
            if (!solitaire.Blocked(selected)) {
                slot1 = selected;
                Stack(slot1, CanBeStackedHere(slot1));
            }
        }
        else {
            // the card is face up, there is nothing else selected
            if (slot1 == this.gameObject) { 
                slot1 = selected;
                Stack(slot1, CanBeStackedHere(slot1));
            }
        }
        slot1 = this.gameObject; // reset slot1?
    }

    void Top(GameObject selected) {
        // top empty spot click options
        // Need to update for when the stack has cards? Maybe? or will they just be cards?
        if (slot1.CompareTag(Constants.CARD_TAG)) {
            if (slot1.GetComponent<Selectable>().value == Constants.ACE_VALUE) { 
                Stack(slot1, selected);
            }
        }
    }

    void Bottom(GameObject selected) {
        // bottom empty slot click options
        // the card is a king and the empty slot is bottom then stack
        if (slot1.CompareTag(Constants.CARD_TAG)) {
            if (slot1.GetComponent<Selectable>().value == Constants.KING_VALUE) {
                Stack(slot1, selected);
            }
        }
    }

    void FlipCard(GameObject selected) {
        if (selected.CompareTag(Constants.CARD_TAG)) {
            bool facing = selected.GetComponent<Selectable>().faceUp;
            selected.GetComponent<Selectable>().faceUp = !facing;
            selected.GetComponent<UpdateSprite>().ShowCardFace();
        }
    }

    GameObject CanBeStackedHere(GameObject selected) {
        // This replaces 'Stackable' and determines the FIRST location that the selected card can be stacked.

        GameObject newSpot = this.gameObject;  // the location that the card might be moved to if the conditions work out
        Selectable s1 = selected.GetComponent<Selectable>(); // selectable from the card to be moved
        Selectable s2; // selectable from the potential place to move the card to
        Transform lastChild;
          
        for (int i = 0; i < solitaire.topPos.Length; i++) { // Iterate through top locations
            newSpot = solitaire.FindYoungestChild(solitaire.topPos[i].transform).gameObject;
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
            if ((s1.value == Constants.KING_VALUE) && (s2.value == 0)) {
                return newSpot;
            } else if (s1.value == s2.value - 1) {
                if ((Solitaire.red.Contains(s1.suit) && Solitaire.black.Contains(s2.suit)) || (Solitaire.red.Contains(s2.suit) && Solitaire.black.Contains(s1.suit))  ) {
                    return newSpot;
                }
            }
        }
        return this.gameObject;
    }

    void StackOnFoundation(GameObject cardToStack, GameObject placeToStack) {
        float zOffset = Constants.Z_OFFSET;
        Selectable s1 = cardToStack.GetComponent<Selectable>();
        Selectable s2 = placeToStack.GetComponent<Selectable>();
        int row = s2.row;

        if(!s2.top) {
            return;
        }
        // stack a card onto the foundations. The check that it is the correct foundation and the youngest child is on the calling sub

        LeanTween.move(cardToStack, new Vector3(placeToStack.transform.position.x, placeToStack.transform.position.y, placeToStack.transform.position.z + zOffset), Constants.ANIMATE_MOVE_TO_FOUND);
        cardToStack.transform.parent = placeToStack.transform;
        s1.row = s2.row;
        s1.top = true;
        s1.inDeckPile = false;
    }

    void StackOnStack(GameObject cardToStack, GameObject placeToStack) {
        Selectable s1 = cardToStack.GetComponent<Selectable>();
        Selectable s2 = placeToStack.GetComponent<Selectable>();
        int row = s2.row; // the row value that will be given to all the child cards that are moved
        Transform parent;
        Transform child;

        if (s2.top || s2.inDeckPile) {
            return;
        }
        float yOffset = s2.value == 0 ? 0f : Constants.STACK_Y_OFFSET; // no y-Offset when moving to an empty stack
        float zOffset = Constants.Z_OFFSET;

        // stack a card or a stack onto an existing cardstack or empty space. The check that the receiving stack is correct is on the calling sub
        LeanTween.move(cardToStack, new Vector3(placeToStack.transform.position.x, placeToStack.transform.position.y + yOffset, placeToStack.transform.position.z + zOffset), Constants.ANIMATE_MOVE_TO_STACK);
        cardToStack.transform.parent = placeToStack.transform;

        s1.row = s2.row;
        parent = cardToStack.transform;
        while (parent.childCount > 0) {
            child = parent.GetChild(0);
            child.GetComponent<Selectable>().row = row;
            Debug.Log("Card " + child.name + " row= " + child.GetComponent<Selectable>().row.ToString());
            parent = child;
        }
        s1.top = false;
        s1.inDeckPile = false;

    }

    void Stack(GameObject cardToStack, GameObject placeToStack) {

        if (placeToStack == this.gameObject) {
            return;
        }

        Selectable s1 = cardToStack.GetComponent<Selectable>();
        Selectable s2 = placeToStack.GetComponent<Selectable>();
        Transform lastChild;

        int row = s1.row;  // save this row position so we can look at it after the card is moved
        bool movedFromBottomStacks = !s1.top && !s1.inDeckPile; // has to be checked before cards are moved

        if (s2.top) {
            // if the place to put the card is in the top section
            if (cardToStack.transform.childCount > 0) {
                return;
            }
            StackOnFoundation(cardToStack, placeToStack);
        } else if (!s2.top && !s2.inDeckPile) {
            StackOnStack(cardToStack, placeToStack);
        }

        // flip any card under the moved card that is still face down
        if (movedFromBottomStacks) {
            lastChild = solitaire.FindYoungestChild(solitaire.bottomPos[row].transform);
            if (!lastChild.GetComponent<Selectable>().faceUp) {
                FlipCard(lastChild.gameObject);
            }
        }

        slot1 = this.gameObject;
    }

    void AutoPlay() { // this does not do the thing yet.
        GameObject oneSelected;
        oneSelected = solitaire.FindYoungestChild(solitaire.deckButton.transform).gameObject;
            for (int i = 0; i < solitaire.topPos.Length; i++) {
            // can't I just go through the positions using selected = solitaire.topPos[i].GetComponent<GameObject> and use Stackable?
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (oneSelected.GetComponent<Selectable>().value == Constants.ACE_VALUE) { // card is an Ace
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0) {
                    slot1 = oneSelected;
                    Stack(slot1, stack.gameObject);
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
                        Stack(slot1, lastCard);
                        break;
                    }
                }
            }
        }
    }

    //bool Stackable(GameObject selected) {
    //    Selectable s1 = slot1.GetComponent<Selectable>(); // selectable from slot1
    //    Selectable s2 = selected.GetComponent<Selectable>(); // selectable from 'selected' gameobj

    //    if (!s2.inDeckPile) {
    //        if (s2.top) { // moving a card to the foundation
    //            if ((s1.suit == s2.suit) || (s1.value == 1 && s2.suit == null)) {
    //                if (s1.value == s2.value + 1) {
    //                    return true;
    //                }
    //            }
    //            else {
    //                return false;
    //            }
    //        }
    //        else { // moving a card to a tableau stack
    //            if ((s1.value == s2.value - 1) || ((s1.value == 13) && (s2.value == 0))) { // if the card values are one apart OR the 1st click is Kint and the 2nd is null space
    //                if ((Solitaire.red.Contains(s1.suit) && Solitaire.black.Contains(s2.suit)) || (Solitaire.black.Contains(s1.suit) && Solitaire.red.Contains(s2.suit))) {
    //                    //Debug.Log($"Stackable on tableau {s1.suit.ToString()}{s1.value.ToString()} on {s2.suit.ToString()}{s2.value.ToString()}");
    //                    return true;
    //                }
    //            }
    //            else {
    //                //Debug.Log($"Not Stackable {s2.suit.ToString()}{s2.value.ToString()} on {s1.suit.ToString()}{s1.value.ToString()}");
    //                return false;
    //            }
    //        }
    //    }
    //    return false;
    //}
    //void Stack(GameObject cardToStack, GameObject placeToStack) {  // the revised Stack with the old Stack code still in it.

    //    if (placeToStack == this.gameObject) {
    //        return;
    //    }

    //    Selectable s1 = cardToStack.GetComponent<Selectable>();
    //    Selectable s2 = placeToStack.GetComponent<Selectable>();

    //    //float yOffset = Constants.STACK_Y_OFFSET;
    //    //float zOffset = Constants.Z_OFFSET;

    //    //Transform parentCard;
    //    Transform lastChild;

    //    //if (s2.top || (!s2.top && s1.value == Constants.KING_VALUE)) {
    //    //    yOffset = 0; // don't offset the card from the stack origination
    //    //}
    //    int row = s1.row;  // save this position so we can look at it after the card is moved
    //    bool movedFromBottomStacks = !s1.top && !s1.inDeckPile;
    //    Debug.Log("The bool is !s1.top = " + (!s1.top).ToString() + " and !s1.inDeckPile = " + (!s1.inDeckPile).ToString() + " therefore (!s1.top && !s1.inDeckPile = " + (!s1.top && !s1.inDeckPile).ToString());

    //    if (s2.top) {
    //        // if the place to put the card is in the top section
    //        if (cardToStack.transform.childCount > 0) {
    //            return;
    //        }
    //        StackOnFoundation(cardToStack, placeToStack);
    //    }
    //    else if (!s2.top && !s2.inDeckPile) {
    //        StackOnStack(cardToStack, placeToStack);
    //    }
    //    // make sure all the cards that moved have the same row assigned
    //    s1.row = s2.row;
    //    foreach (Transform card in cardToStack.transform) {
    //        card.GetComponent<Selectable>().row = s2.row;
    //    }
    //    // flip any card under the moved card that is still face down
    //    if (movedFromBottomStacks) {
    //        lastChild = solitaire.FindYoungestChild(solitaire.bottomPos[row].transform);
    //        if (!lastChild.GetComponent<Selectable>().faceUp) {
    //            FlipCard(lastChild.gameObject);
    //        }
    //    }

    //    //slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y + yOffset, selected.transform.position.z + zOffset);
    //    //slot1.transform.parent = selected.transform; // this is so the children move with the parent. 

    //    // check if the original parent of slot1 still has any children after moving slot1 then check 
    //    // if the s1 card is not in the Top or in the Deck, check if the row it was in prior to moving has an unflipped card
    //    //if (parentCard.childCount > 0) {
    //    //    if (row > 0 && !s1.inDeckPile) {
    //    //        childCard = solitaire.bottomPos[row].transform.GetChild(solitaire.bottomPos[row].transform.childCount - 1);
    //    //        if (!childCard.GetComponent<Selectable>().faceUp) {
    //    //            FlipCard(childCard.gameObject);
    //    //        }
    //    //    }
    //    //}

    //    // set the new sort order based on the parent card
    //    //parentCard = selected.transform;

    //    //while (parentCard.childCount > 0) { // this only for sortingOrder which we may not even be using about now...
    //    //    //Debug.Log("Parent card start = " + parentCard.GetComponent<Selectable>().suit.ToString() + parentCard.GetComponent<Selectable>().value.ToString());
    //    //    childCard = parentCard.GetChild(parentCard.childCount - 1); // get the most recently added child of the current parent
    //    //    childCard.GetComponent<SpriteRenderer>().sortingOrder = parentCard.GetComponent<SpriteRenderer>().sortingOrder + 1;
    //    //    parentCard = childCard;
    //    //    //Debug.Log("Parent card end = " + parentCard.GetComponent<Selectable>().suit.ToString() + parentCard.GetComponent<Selectable>().value.ToString());
    //    //}

    //    //if (s1.top && s2.top && (s1.value == 1)) { // allows movement of cards between top spots
    //    //    solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
    //    //    solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
    //    //}
    //    //else if (s1.top) {                                // moving a card from the foundations back to the tableau
    //    //    solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
    //    //}
    //    //else {                                            // removes the card string from the appropriate bottom list
    //    //    solitaire.bottoms[s1.row].Remove(slot1.name);
    //    //}

    //    //s1.inDeckPile = false; 
    //    ////Debug.Log($"Slot1 row is {s1.row} slot2 row is {s2.row}");
    //    //s1.row = s2.row;

    //    // I understand the problem now - when s1 is moved to the foundation, it is removed from the deck list and therefore is no longer a gameobject.
    //    //if (s2.top) {
    //    //    solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
    //    //    solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
    //    //    s1.top = true;
    //    //}
    //    //else {
    //    //    s1.top = false;
    //    //}
    //    // after moving make slot1 (first card to move) work like null without using null (bc logic probs)
    //    slot1 = this.gameObject;
    //}
}
