using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {

    public GameObject gameOverUI;

    public GameObject slot1;

    private Solitaire solitaire;

    delegate bool StackCondition(GameObject cardToMove, GameObject placeToMove); // let's practice using delegates....
    StackCondition stackCondition;

    delegate void StackCards(GameObject cardToStack, GameObject placeToStack);
    StackCards stackCards;

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
        //Debug.Log("Card hit is : " + selected.name);
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
            // the card is in the tab or the found
            if (slot1 == this.gameObject) { 
                slot1 = selected;
                //Debug.Log("slot1.name = " + slot1.name);
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
        
        GameObject newSpot = this.gameObject;  // the location that the card might be moved to if the conditions work out
        Selectable s1 = selected.GetComponent<Selectable>(); // selectable from the card to be moved
        Transform lastChild;

        // check foundation first
        stackCondition = solitaire.FoundationCondition;

        if (s1.transform.childCount <= 0) { // make sure the card is not in the middle of a stack on the tableau before checking foundations
            for (int i = 0; i < solitaire.topPos.Length; i++) { // Iterate through top locations

                newSpot = Utilities.FindYoungestChild(solitaire.topPos[i].transform).gameObject;
                //Debug.Log("in Check Foundation area of CanBeStackedHere, selected : " + selected.name + " and newSpot : " + newSpot.name);

                if (stackCondition(selected, newSpot)) {

                    stackCards = StackOnFoundation;

                    stackCondition = null;

                    return newSpot;
                }
            }
        }           

        // we did not return yet so keep going
        // check Tableau cards
        stackCondition = solitaire.TableauStackCondition;

        for (int i = 0; i < solitaire.bottomPos.Length; i++) {

            lastChild = Utilities.FindYoungestChild(solitaire.bottomPos[i].transform);
            newSpot = lastChild.gameObject;
 
            if (stackCondition(selected, newSpot)) {
                stackCards = StackOnStack;
                stackCondition = null;
                return newSpot;
            }
        }
        stackCondition = null;
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
        //Debug.Log("In StackOnFoundation, s1 is : " + s1.name + " and s2 : " + s2.name);

        LeanTween.move(cardToStack, new Vector3(placeToStack.transform.position.x, placeToStack.transform.position.y, placeToStack.transform.position.z + zOffset), Constants.ANIMATE_MOVE_TO_FOUND);
        cardToStack.transform.parent = placeToStack.transform;
        s1.row = s2.row;
        s1.top = true;
        s1.inDeckPile = false;
        if (s1.value == Constants.KING_VALUE) {
            // check for win condition
            if (solitaire.WinCondition()) {
                StartCoroutine(winPopup(0.2f));
            }
        }
    }

    public IEnumerator winPopup(float secs) {
        yield return new WaitForSeconds(secs);
        gameOverUI.SetActive(true);
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
            parent = child;
        }
        s1.top = false;
        s1.inDeckPile = false;

    }

    void Stack(GameObject cardToStack, GameObject placeToStack) {
        //Debug.Log("Stack method, cardToStack is " + cardToStack.name + " and placeToStack is " + placeToStack.name);
        if (placeToStack == this.gameObject) {
            return;
        }

        Selectable s1 = cardToStack.GetComponent<Selectable>();
        Selectable s2 = placeToStack.GetComponent<Selectable>();
        Transform lastChild;

        int row = s1.row;  // save this row position so we can look at it after the card is moved
        bool movedFromBottomStacks = !s1.top && !s1.inDeckPile; // has to be checked before cards are moved

        if (s2.top && cardToStack.transform.childCount > 0) {
            // if the place to put the card is in the top section and the cardToStack has childcards, then exit
            return;

        } else {

            stackCards(cardToStack, placeToStack);
            
        }

        // flip any card under the moved card that is still face down
        if (movedFromBottomStacks) {
            lastChild = Utilities.FindYoungestChild(solitaire.bottomPos[row].transform);
            if (!lastChild.GetComponent<Selectable>().faceUp) {
                FlipCard(lastChild.gameObject);
            }
        }

        slot1 = this.gameObject;
        stackCondition = null;
        stackCards = null;
    }

    void AutoPlay() { // this does not do the thing yet.
        GameObject oneSelected;
        oneSelected = Utilities.FindYoungestChild(solitaire.deckButton.transform).gameObject;
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
}
