using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;


public class Solitaire : MonoBehaviour {
    // attach to SolitaireGame
    // contains Start, OnEnable, OnDisable

    [SerializeField] GameObject sceneMgr; // so I can get the appInit object
    [SerializeField] GameObject deckObject; // for repositioning , probably should move this functionality to another class.
    [SerializeField] GameObject topObject;

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public static List<string> red = new List<string> { "D", "H" };
    public static List<string> black = new List<string> { "C", "S" };

    public Sprite[] cardfaces;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    public TMP_Text talonAmountText;

    public List<string>[] bottoms; // lol
    public List<string>[] tops;

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

    float talonZOffset = 0f;
    int localDealAmount;
    float initDeckXOffset;
    bool leftHandMode;
    float localXDeckOffset;

    // Start is called before the first frame update
    void Start() {
        GetSettingsValues appInit = sceneMgr.GetComponent<GetSettingsValues>();
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };

        CardMovement.UndoTalonList += PushBackToDeck;
        CardMovement.GetPrevCardZ += GetLastTalonCardZ;

        localDealAmount = appInit.TalonDealAmount;
        initDeckXOffset = appInit.InitXDeckOffset;
        leftHandMode = appInit.LeftHandedMode;
        localXDeckOffset = appInit.XDeckOffset; // this used to be tied to lefthandmode but is not anymore

        InitTableau(); // there should be an interface for this

        PrepDeck();
        // make the new deck list once
        PlayCards(); // there should be an interface for this
    }

    private void OnEnable() {
        UIButtons.GameRenewed += ResetTable;
        UIButtons.GameStarted += PlayCards;
        UIButtons.ReplayClicked += PrepDeck;
        UserInput.DeckClicked += DealFromTalon;
        PlayerSettings.SettingsUpdated += ResetForPlayerPrefChanges;
    }
    private void OnDisable() {

        UIButtons.GameRenewed -= ResetTable;
        UIButtons.GameStarted -= PlayCards;
        UIButtons.ReplayClicked -= PrepDeck;
        UserInput.DeckClicked -= DealFromTalon;
        PlayerSettings.SettingsUpdated -= ResetForPlayerPrefChanges;
    }

    public void ArrangeTopForHand() { // idk if this is the best way to do this ....
        float deckX = leftHandMode ? Constants.LHM_DECK_X : Constants.RHM_DECK_X;
        float deckY = leftHandMode ? Constants.LHM_DECK_Y : Constants.RHM_DECK_Y;
        float topX = leftHandMode ? Constants.LHM_TOP_X : Constants.RHM_TOP_X;
        float topY = leftHandMode ? Constants.LHM_TOP_Y : Constants.RHM_TOP_Y;
        float z = 0f;
        deckObject.transform.position = new Vector3(deckX, deckY, z);
        topObject.transform.position = new Vector3(topX, topY, z);
    }

    public void ResetForPlayerPrefChanges() {
        leftHandMode = Utilities.GetLeftHandMode(PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE));
        initDeckXOffset = Utilities.GetInitDeckXOffset(leftHandMode);
        localXDeckOffset = Utilities.GetXDeckOffset(leftHandMode);
        ArrangeTopForHand();
        RestackUndealtTalon(leftHandMode);
        RestackDealtCards(leftHandMode);
        localDealAmount = PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT);
    }

    public void InitTableau() {
        ArrangeTopForHand();
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
        talonAmountText.text = talon.Count.ToString();
    }

    public void DealFromTalon() {
        int dealAtOnceAmount = localDealAmount;
        // pull cards from the top of the stack under deckbutton - they are in the List called talon; not worrying about timing right now, we will add animation or at least lerp later
        float xOffset = initDeckXOffset; // the x-offset o the first card is Init x-offset, this is how much offset for this 'round' of dealing
        float incrXOffset = localXDeckOffset; // xOffset will be different depending on LeftHandedMode
        float incrZOffset = Constants.Z_OFFSET; // zOffset is coming towards the camera when the card is dealt and going away when they go back into the deck (UNDEALT_CARD_Z_OFFSET)
        GameObject newCard;
        Transform child;

        if (talon.Count > 0) {

            SlideIntoStack(deckButton.transform, deckButton.transform.position.x + initDeckXOffset); // align the cards into a stack under the newly dealt talon cards
            
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
        talonAmountText.text = talon.Count.ToString();
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

    void PushBackToDeck(List<GameObject> putTheseBack) {
        // the card movement is in the CardMovement script. This is just for putting the GOs back into the talon list.
        for (int i = 0; i < putTheseBack.Count; i++) {
            talon.Insert(0, putTheseBack[i]);
        } // ok this should put them back in the right order??
    }

    float GetLastTalonCardZ() {
        return talon[talon.Count - 1].transform.position.z;
    }

    void RestackUndealtTalon(bool leftHandMode) {
        // find all the cards that aren't a child of deckbutton or bottom or top
        // so maybe find updatesprite and if it's a card and if it has parent = null?
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        float x = deckButton.transform.position.x;
        foreach (UpdateSprite card in cards) {
            if (card.CompareTag(Constants.CARD_TAG)) {
                if (card.transform.parent == null) {
                    card.transform.position = new Vector3(x, card.transform.position.y, card.transform.position.z);
                }
            }
        }
    }

    void RestackDealtCards(bool leftHandMode) {
        // get the cards that were already dealt and move them to the appropriate place of the deal
        float xOffset = initDeckXOffset;
        int dealtCards = 0;
        Transform child;
        foreach (Transform card in deckButton.transform) {
            if (card.CompareTag(Constants.CARD_TAG)) {
                card.position = new Vector3(deckButton.transform.position.x + xOffset, card.position.y, card.position.z);
                dealtCards++;
            }
        }
        if (dealtCards > 0) { // only do this if there are some cards that have been dealt
            for (int i = 0; i < localDealAmount - 1; i++) { // display those last few cards out like if they had been dealt (I know this doesn't cover cases when a card from the deal has been used and then the settings are updated but I've decided it's fine)
                child = deckButton.transform.GetChild(deckButton.transform.childCount - 1 - i); // e.g. if i = 0, get the last card. if i = 1, get the 2nd to last card.
                xOffset = initDeckXOffset + ((localDealAmount - 1 - i) * localXDeckOffset);
                child.position = new Vector3(deckButton.transform.position.x + xOffset, child.position.y, child.position.z);
            }
        }
    }

    /// <summary>
    /// Place one card back into the deck pile
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
