using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Solitaire : MonoBehaviour {

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public static List<string> red = new List<string> { "D", "H" };
    public static List<string> black = new List<string> { "C", "S" };

    public Sprite[] cardfaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;

    public List<string>[] bottoms; // lol
    public List<string>[] tops;
    //public List<string> tripsOnDisplay = new List<string>(); // the visible cards dealt from the talon
    //public List<string> tripsUnderDisplay = new List<string>(); // the previous cards dealt from the talon that are not visible unless all tripsOnDisplay are used DO I NEED THIS??????????????????
    //public List<List<string>> deckTrips = new List<List<string>>();

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> deck;
    public List<string> SavedDeck; // for replay THIS game
    public List<string> discardPile = new List<string>();

    List<GameObject> talon = new List<GameObject>(); // for the list of card items in the talon before they become discard pile

    //public int deckLocation;
    //private int triples;
    //private int tripRemainder;
    // keep track of z-offset on the talon discard pile so the cards will stack as they are dealt
    float talonZOffset = 0f;
    int localDealAmount;

    private App_Initialize appInit;

    // Start is called before the first frame update
    void Start() {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        appInit = FindObjectOfType<App_Initialize>();

        localDealAmount = 3; // for testing purposes. Actua value should be :  appInit.TalonDealAmount; ******************************************************************

        InitTableau();

        PrepDeck();
        // make the new deck list once
        PlayCards();
    }

    public void InitTableau() {
        foreach (GameObject tops in topPos) {
            tops.GetComponent<Selectable>().suit = null;
            tops.GetComponent<Selectable>().value = 0;
        }
        foreach (GameObject bottom in bottomPos) {
            bottom.GetComponent<Selectable>().suit = null;
            bottom.GetComponent<Selectable>().value = 0;
        }
    }
    /// <summary>
    /// Create a new shuffled deck for a new game
    /// </summary>
    public void PrepDeck() {
        SavedDeck = GenerateDeck();
        Shuffle(SavedDeck);
    }
    public void PlayCards() {
        
        foreach (List<string> list in bottoms) {
            list.Clear();
        }
        deck.Clear();
        discardPile.Clear();

        deck = new List<string>(SavedDeck);

        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        // at the end of SolitaireDeal, the deck should only contain the cards that would be set in the talon apot 

        SetUpTalon(deck);
    }

    public static List<string> GenerateDeck() {
        List<string> newDeck = new List<string>();
        
        foreach (string s in suits) {
            foreach (string v in values) {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }

    public void Shuffle<T>(List<T> list) {
        System.Random rand = new System.Random();
        int n = list.Count;
        while (n > 1) {
            int k = rand.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void SolitaireSort() {
        //Debug.Log("Solitaire Sort called. Deck has " + deck.Count + " cards in it.");
        for (int i = 0; i < 7; i++) {
            for (int j = i; j < 7; j++) {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    IEnumerator SolitaireDeal() {

        for (int i = 0; i < 7; i++) {

            float yOffset = 0f;
            float zOffset = Constants.Z_OFFSET;

            foreach (string card in bottoms[i]) {
                yield return new WaitForSeconds(0.01f);
                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y + yOffset, bottomPos[i].transform.position.z + zOffset), Quaternion.identity, bottomPos[i].transform);    
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i;

                if (card == bottoms[i][bottoms[i].Count - 1]) { // ex. bottoms[0][0] faceUp = true, bottoms[4][1] faceUp = false
                    newCard.GetComponent<Selectable>().faceUp = true;
                    newCard.GetComponent<UpdateSprite>().ShowCardFace();
                }

                yOffset += Constants.STACK_Y_OFFSET;
                zOffset += Constants.Z_OFFSET;
                discardPile.Add(card);
            }

        }
        foreach (string card in discardPile) {
            if (deck.Contains(card)) {
                deck.Remove(card);
            }
        }
        discardPile.Clear();
    }

    void SetUpTalon(List<string> talonDeck) {
        // instantiate cards behind the deckButton so that deckButton still calls them 
        // but they are not the collider first hit
        // since the deck stacks from top to bottom we can use the deck in order 
        talon.Clear();
        talonZOffset = 0;
        float zOffset = Constants.UNDEALT_CARD_Z_OFFSET;
        foreach (string card in talonDeck) {
            GameObject newCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity);
            newCard.name = card;
            talon.Add(newCard);
            //Debug.Log("Card z-value is " + newCard.transform.position.z.ToString());
            zOffset += Constants.UNDEALT_CARD_Z_OFFSET;
        }
    }

    public void DealFromTalon() {
        int dealAtOnceAmount = localDealAmount;
        // pull cards from the top of the stack under deckbutton - they are in the List called talon; not worrying about timing right now, we will add animation or at least lerp later
        float xOffset = appInit.InitXDeckOffset; // the x-offset o the first card is Init x-offset, this is how much offset for this 'round' of dealing
        float incrXOffset = appInit.XDeckOffset; // xOffset will be different depending on LeftHandedMode
        float incrZOffset = Constants.Z_OFFSET; // zOffset is coming towards the camera when the card is dealt and going away when they go back into the deck (UNDEALT_CARD_Z_OFFSET)
        GameObject newCard;
        Transform child;

        if (talon.Count > 0) {

            SlideIntoStack(deckButton.transform, deckButton.transform.position.x + appInit.InitXDeckOffset); // align the cards into a stack under the newly dealt talon cards
            
            talonZOffset += incrZOffset; // add some space between the previous card and the card we are about to move....

            // deal the proper amount of cards out

            for (int i = 0; i < dealAtOnceAmount; i++) {
                if (talon.Count > 0) {
                    newCard = talon[0]; // because talon[0] is always the top card.
                                        //newCard.transform.position = new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset);
                    LeanTween.move(newCard, new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + talonZOffset), Constants.ANIMATE_DEAL_FROM_TALON);
                    
                    PullFromDeck(newCard);
                    newCard.transform.parent = deckButton.transform;
                    talon.Remove(newCard); // does this remove the slot it's in or just the object attached to that slot?
                                           //Debug.Log("The removed card was " + newCard.name.ToString() + " and the current talon[0] card is : " + talon[0].name.ToString());
                    talonZOffset += incrZOffset;
                    xOffset += incrXOffset;

                } else {
                    break;
                }
            }
            talon.RemoveAll(GameObject => GameObject == null); // this should remove all the empty slots, so now the topmost card is Talon[0]

            deckButton.GetComponent<TalonSpriteUpdate>().setTalonSprite(!(talon.Count > 0)); // there are no cards in the talon, then set empty sprite.

        } else {
            float zOffset = Constants.UNDEALT_CARD_Z_OFFSET;
            talon.RemoveAll(GameObject => GameObject == null);
            while (deckButton.transform.childCount > 0) {
                child = deckButton.transform.GetChild(0);
                child.position = new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z + zOffset);
                BackIntoDeck(child.gameObject);
                child.SetParent(null);
                talon.Add(child.gameObject);
                zOffset += Constants.UNDEALT_CARD_Z_OFFSET;
            }
            talonZOffset = 0f; // the deal will reset at z = 0
        }
    }

    /// <summary>
    /// Common settings when pulling one card from the talon
    /// </summary>
    /// <param name="card">GameObject that is being pulled from the Talon</param>
    void PullFromDeck(GameObject card) {
        Selectable s1 = card.GetComponent<Selectable>();
        s1.faceUp = true;
        s1.inDeckPile = true;
        card.GetComponent<UpdateSprite>().ShowCardFace();
    }
    /// <summary>
    /// Common settings when reloading a talon with cards
    /// </summary>
    /// <param name="card">GameObject that is being put back into the deck</param>
    void BackIntoDeck(GameObject card) {
        Selectable s1 = card.GetComponent<Selectable>();
        s1.faceUp = false;
        s1.inDeckPile = true;
        card.GetComponent<UpdateSprite>().ShowCardFace();
    }

    /// <summary>
    /// Slides cards into one stack after a new set of cards is drawn from the talon
    /// </summary>
    /// <param name="parent">The parent of the cards to be stacked (usually talon)</param>
    /// <param name="xPos">The x-position to slide the cards to. They will keep their y and z values.</param>
    void SlideIntoStack(Transform parent, float xPos) {
        if (parent.childCount > 0) {
            foreach (Transform child in parent) {
                if (child.CompareTag(Constants.CARD_TAG)) {
                    if (child.position.x != xPos) {
                        // keep y and z pos and update x pos
                        child.position = new Vector3(xPos, child.position.y, child.position.z);
                    }
                }
            }
        }
    }

    public void ResetTable() {
        DestroyCards();
        InitTableau();
        Debug.Log("Table reset");
    }

    public bool Blocked(GameObject selected) {
        Selectable s2 = selected.GetComponent<Selectable>();
        Transform parent = selected.transform.parent;
        if (s2.inDeckPile || s2.top) {
            //Debug.Log("The card either inDeckPile or top is : " + selected.name);
            if (s2.name == Utilities.FindYoungestChild(parent.transform).name) {
                return false;
            }
            else {
                return true;
            }
        }
        else  {
            //Debug.Log("The card not in top or deckpile is : " + selected.name);
            if (s2.faceUp) {
                return false;
            }
            else {
                return true;
            }
        }
    }

    void DestroyCards() {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach(GameObject card in talon) {
            if (card.CompareTag(Constants.CARD_TAG)) {
                Debug.Log("Destroying : " + card.name);
                Destroy(card);
            }
        }
        foreach (UpdateSprite card in cards) {
            if (card.CompareTag(Constants.CARD_TAG)) {
                Debug.Log("Destroying : " + card.name);
                Destroy(card.gameObject);
            }
        }
    }

    public bool WinCondition() {
        GameObject card;
        int foundationsCompleted = 0;
        bool winCondition = false;

        for (int i = 0; i < topPos.Length; i++) {
            // check if there is a king on all stacks
            card = Utilities.FindYoungestChild(topPos[i].transform).gameObject;
            if (card.GetComponent<Selectable>().value == Constants.KING_VALUE) {
                foundationsCompleted++;
            }
        }
        winCondition = foundationsCompleted == 4;
        return winCondition;
    }

    public bool TableauStackCondition(GameObject cardToMove, GameObject placeToMove ) { // placeToMove might be a non-card gameobject
        // when is stacking OK
        Selectable s1 = cardToMove.GetComponent<Selectable>();
        Selectable s2 = placeToMove.GetComponent<Selectable>();

        if (s1.value == Constants.KING_VALUE && s2.value == 0) { // moving a King to an open spot
            return true;
        }
        bool b1 = red.Contains(s1.suit) && black.Contains(s2.suit);
        bool b2 = black.Contains(s1.suit) && red.Contains(s2.suit);

        return (b1 || b2) && (s1.value == s2.value - 1) ; // this should be true when there is alternating red/black
    }

    public bool FoundationCondition(GameObject cardToMove, GameObject placeToMove) {
        Selectable s1 = cardToMove.GetComponent<Selectable>();
        Selectable s2 = placeToMove.GetComponent<Selectable>();
        //Debug.Log("Foundation condition is called");
        if (s1.value == Constants.ACE_VALUE && s2.value == 0) { // Stack an Ace onto the empty foundation
            return true;
        }
        return (s1.suit == s2.suit) && (s1.value == s2.value + 1);
    }
}
