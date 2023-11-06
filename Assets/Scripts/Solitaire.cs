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
    public List<string> tripsOnDisplay = new List<string>(); // the visible cards dealt from the talon
    public List<string> tripsUnderDisplay = new List<string>(); // the previous cards dealt from the talon that are not visible unless all tripsOnDisplay are used DO I NEED THIS??????????????????
    public List<List<string>> deckTrips = new List<List<string>>();

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

    List<GameObject> talon; // for the list of card items in the talon before they become discard pile

    public int deckLocation;
    private int triples;
    private int tripRemainder;
    // keep track of z-offset on the talon discard pile so the cards will stack as they are dealt
    float talonZOffset = 0f;

    private App_Initialize appInit;

    // Start is called before the first frame update
    void Start() {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        appInit = FindObjectOfType<App_Initialize>();

        appInit.talonDealAmount = 3; // ************************************************************ FOR TESTING PURPOSES ************************************************************************

        InitTableau();

        PrepDeck();
        // make the new deck list once
        PlayCards();
    }

    // Update is called once per frame
    void Update() {
        
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
        foreach (List<string> trips in deckTrips) {
            trips.Clear();
        }
        deck = new List<string>(SavedDeck);
        deckTrips.Clear();

        //Debug.Log("deck has " + deck.Count + " cards and SavedDeck has " + SavedDeck.Count + " cards.");

        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        // at the end of SolitaireDeal, the deck should only contain the cards that would be set in the talon apot 

        SetUpTalon(deck);
        // test the GOs and make sure there are 52 cards in the scene
        GameObject[] gob = FindObjectsOfType<GameObject>();
        int cardCount = 0;
        foreach (GameObject obj in gob) {
            if (obj.CompareTag(Constants.CARD_TAG)) {
                cardCount++;
            }
        }
        Debug.Log("The number of cards instantiated is : " + cardCount.ToString());
        // I don't think we need these string handlers if we have instantiated all the cards?
        //deckTrips = SortDeckIntoTriples(deck);
        //triples = deckTrips.Count(); // this is the correct way to get how many Lists are in the List of lists.
        //Debug.Log("Solitaire PlayCards triples = deckTrips.Count() = " + triples);
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
        float zOffset = Constants.UNDEALT_CARD_Z_OFFSET;
        foreach (string card in talonDeck) {
            GameObject newCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity);
            newCard.name = card;
            talon.Add(newCard);
            zOffset += Constants.UNDEALT_CARD_Z_OFFSET;
        }
    }

    void DealFromTalon(int dealAtOnceAmount) {
        // pull cards from the top of the stack under deckbutton - they are in the List called talon; not worrying about timing right now, we will add animation or at least lerp later
        float xInitOffset = appInit.initXDeckOffset;
        float xOffset = appInit.xDeckOffset;
        float zOffset = 0f;
        GameObject newCard;

        if (talon.Count > 0) {

            SlideIntoStack(deckButton.transform, deckButton.transform.position.x + xOffset);

            talonZOffset += Constants.Z_OFFSET; // add some space between the previous card and the card we are about to move....

            // deal the proper amount of cards out
            xOffset = appInit.initXDeckOffset; // starting offset from deckButton
            zOffset = talonZOffset; // starting offset from deckButton
            for (int i = 0; i < dealAtOnceAmount; i++) {
                newCard = talon[i];
                newCard.transform.position = new Vector3(deckButton.transform.position.x + xOffset, deckButton.transform.position.y, deckButton.transform.position.z + zOffset);
                PullFromDeck(newCard);
                newCard.transform.parent = deckButton.transform;
                discardPile.Add(newCard.name);
                deck.Remove(newCard.name);
                talon.Remove(newCard); // does this remove the slot it's in or just the object attached to that slot?
                Debug.Log("The removed card was " + newCard.name.ToString() + " and the current talon[0] card is : " + talon[0].name.ToString());

                zOffset += Constants.Z_OFFSET;
                xOffset += Constants.DECK_X_OFFSET;
            }
            talon.RemoveAll(GameObject => GameObject == null); // this should remove all the empty slots, so now the topmost card is slot 0

            deckButton.GetComponent<TalonSpriteUpdate>().setTalonSprite(!(talon.Count > 0)); // there are no cards in the talon, then set empty sprite.
        } else {
            zOffset = Constants.UNDEALT_CARD_Z_OFFSET;
            talon.RemoveAll(GameObject => GameObject == null);
            foreach (Transform child in deckButton.transform) {
                deck.Add(child.GetComponent<GameObject>().name);
                discardPile.Remove(child.GetComponent<GameObject>().name);
                Debug.Log("Child.GetComponent<GameObject>().name = " + child.GetComponent<GameObject>().name.ToString() + ", child.gameObject.name = " + child.gameObject.name.ToString() + ", child.name = " + child.name.ToString());

                child.position = new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z + zOffset);
                BackIntoDeck(child.gameObject);
                child.SetParent(null);
                talon.Add(child.gameObject);

                zOffset += Constants.UNDEALT_CARD_Z_OFFSET;
            }
            discardPile.Clear();
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
    

    public List<List<string>> SortDeckIntoTriples(List<string> sortingDeck) { // it actually sorts deck into groups of whatever TalonDealAmount is
        // also I kind don't like how this works, anyway probably keep it since it DOES work and I don't have to re-design anything

        int sets = sortingDeck.Count / appInit.talonDealAmount;
        int remainder = sortingDeck.Count % appInit.talonDealAmount;
        List<List<string>> cardSets = new List<List<string>>();
        int modifier;
        int deckLoc = 0;

        for (int i = 0; i < sets; i++) {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < appInit.talonDealAmount; j++) {
                myTrips.Add(sortingDeck[j + deckLoc]);
            }
            cardSets.Add(myTrips);
            deckLoc += appInit.talonDealAmount;
        }
        if (remainder != 0) {
            List<string> myRemainders = new List<string>();
            modifier = 0; // modifier to iterate through the remainder cards;
            for (int k = 0; k < remainder; k++) {
                myRemainders.Add(sortingDeck[sortingDeck.Count - remainder + modifier]);
                modifier++;
            }
            cardSets.Add(myRemainders);
        }
        return cardSets;
    }

    public void DealFromDeck() {

        foreach (Transform child in deckButton.transform) { // slides all dealt cards under the first card in the TripsOnDisplay stack so they're accessible only after all TripsOnDisplay are used
            if (child.CompareTag(Constants.CARD_TAG)) {
                child.position = new Vector3(deckButton.transform.position.x + Constants.INIT_DECK_X_OFFSET, child.position.y, child.position.z);
            }
        }

        talonZOffset += Constants.Z_OFFSET; // make sure the next card is not stacked on the same place as any other cards???????????????????????????????????????????????????
        Debug.Log("The current value of talonZOffset = " + talonZOffset.ToString());

        if (deckLocation < triples) {
            foreach (string card in tripsOnDisplay) {
                deck.Remove(card);
                discardPile.Add(card);
            }
            tripsOnDisplay.Clear();

            StartCoroutine(DealFromTalon(deckTrips[deckLocation], 0, deckButton.transform.position, Constants.INIT_DECK_X_OFFSET, 0f, talonZOffset, deckButton.transform, incrXOffset: Constants.DECK_X_OFFSET, incrZOffset: Constants.Z_OFFSET));
            Debug.Log("After coroutine, the current value of talonZOffset is : " + talonZOffset.ToString());

            deckLocation++;
            if (deckLocation >= triples) {
                deckButton.GetComponent<TalonSpriteUpdate>().setTalonSprite(true);
            }
        } else {
            deckButton.GetComponent<TalonSpriteUpdate>().setTalonSprite(false);
            RestackTopDeck();
        }
    }

    IEnumerator DealFromTalon(List<string> cardSet, int sortOrder, Vector3 startPos, float initXOffset, float initYOffset, float initZOffset, Transform parentObj, float incrXOffset = 0f, float incrYOffset = 0f, float incrZOffset = 0f) {
       // this is set up to potentially be usable by both deal subs but i don't have an elegant way to apply the correct 
       // selectable settings and other stuff without kindof knowing which dealing this is.
        float xOffset = initXOffset;
        float yOffset = initYOffset;
        float zOffset = initZOffset;
        Vector3 currPos = startPos;
        foreach (string card in cardSet) {
            yield return new WaitForSeconds(0.01f);
            GameObject newTopCard = Instantiate(cardPrefab, new Vector3(currPos.x + xOffset, currPos.y + yOffset, currPos.z + zOffset), Quaternion.identity, parentObj);
            newTopCard.name = card;
            tripsOnDisplay.Add(card);
            newTopCard.GetComponent<Selectable>().faceUp = true;
            newTopCard.GetComponent<UpdateSprite>().ShowCardFace();
            newTopCard.GetComponent<Selectable>().inDeckPile = true;

            if (incrXOffset != 0) {
                xOffset = incrXOffset;
            }
            if (incrYOffset != 0) {
                yOffset = incrYOffset;
            }
            if (incrZOffset != 0) {
                zOffset = incrZOffset;
            }
            currPos = newTopCard.transform.position;
        }
    }

    void SurfaceTrips() {

    }

    void RestackTopDeck() {
        // don't forget to reset talonZOffset
        // also don't destroy gameobjects.
        deck.Clear();
        Debug.Log("The number of items still in the deck list at the beginnig of Restack is : " + deck.Count.ToString());
        Debug.Log("The number of items in Discard Pile at the beginning of Restack is : " + discardPile.Count.ToString());
        foreach (Transform child in deckButton.transform) {
            if (child.CompareTag(Constants.CARD_TAG)) {
                Destroy(child.gameObject);
            }
        }
        foreach (string card in discardPile) {
            deck.Add(card);
        }
        Debug.Log("The number of cards added to the deck (midway in Restack) is : " + deck.Count.ToString());
        discardPile.Clear();
        Debug.Log("the number of cards still in Discard Pile (midway to Restack) is : " + discardPile.Count.ToString());
        foreach (List<string> trip in deckTrips) {
            trip.Clear();
        }
        deckTrips.Clear();
        Debug.Log("The number of DeckTrips before SortDeckIntoTriples is : " + deckTrips.Count.ToString());
        deckTrips = SortDeckIntoTriples(deck);

        triples = deckTrips.Count();
        deckLocation = 0;
    }

    public void ResetTable() {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach (UpdateSprite card in cards) {
            Destroy(card.gameObject);
        }
        ClearTopValues();
        
        Debug.Log("Table reset");
    }

    void ClearTopValues() {
        for (int i = 0; i < topPos.Length; i++) {
            Selectable selectable = topPos[i].GetComponent<Selectable>();
            if (selectable.CompareTag(Constants.TOP_TAG)) {
                selectable.suit = null;
                selectable.value = 0;
            }
        }
        Debug.Log("Top values cleared");
    }

    public Transform FindYoungestChild(Transform parentObj) {
        // gets the last-added child of an object and checks if that obj has children, if so it checks that child obj for the last added child, etc.
        Transform t1 = parentObj;
        Transform t2;
        while (t1.childCount > 0) {
            t2 = t1.GetChild(t1.childCount - 1);
            t1 = t2;
        }
        return t1;
    }
}
