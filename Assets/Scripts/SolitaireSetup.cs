//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

// ***************************************** Conceptually this should be a whole thing and cards should be their own thing and the play should be it's own thing
// anyway I can't use it now so just save it for later I guess.... 

//public class SolitaireSetup : MonoBehaviour {
//    // attach to Solitaire emtpy GO

//    [SerializeField] GameObject deckObject; // for repositioning , probably should move this functionality to another class.
//    [SerializeField] GameObject topObject;

//    public static string[] suits = new string[] { "C", "D", "H", "S" };
//    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
//    public static List<string> red = new List<string> { "D", "H" };
//    public static List<string> black = new List<string> { "C", "S" };

//    public Sprite[] cardfaces;
//    public GameObject cardPrefab;
//    public GameObject deckButton;
//    public GameObject[] bottomPos;
//    public GameObject[] topPos;

//    public List<string>[] bottoms; // lol
//    public List<string>[] tops;

//    private List<string> bottom0 = new List<string>();
//    private List<string> bottom1 = new List<string>();
//    private List<string> bottom2 = new List<string>();
//    private List<string> bottom3 = new List<string>();
//    private List<string> bottom4 = new List<string>();
//    private List<string> bottom5 = new List<string>();
//    private List<string> bottom6 = new List<string>();

//    public List<string> deck;
//    public List<string> SavedDeck; // for replay THIS game
//    public List<string> discardPile = new List<string>();

//    List<GameObject> talon = new List<GameObject>(); // for the list of card items in the talon before they become discard pile

//    float talonZOffset = 0f;
//    int localDealAmount;

//    private App_Initialize appInit;

//    // Start is called before the first frame update
//    void Start() {
//        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
//        appInit = FindObjectOfType<App_Initialize>();

//        localDealAmount = 3; // for testing purposes. Actua value should be :  appInit.TalonDealAmount; ******************************************************************

//        InitTableau(); // there should be an interface for this

//        PrepDeck();
//        // make the new deck list once
//        PlayCards(); // there should be an interface for this
//    }

//    private void OnEnable() {
//        UIButtons.GameRenewed += ResetTable;
//        UIButtons.GameStarted += PlayCards;
//        UIButtons.ReplayClicked += PrepDeck;
//    }
//    private void OnDisable() {
//        UIButtons.GameRenewed -= ResetTable;
//        UIButtons.GameStarted -= PlayCards;
//        UIButtons.ReplayClicked -= PrepDeck;
//    }

//    public void ArrangeTopForHand(bool leftHandedMode) { // idk if this is the best way to do this ....
//        float deckX = leftHandedMode ? Constants.LHM_DECK_X : Constants.RHM_DECK_X;
//        float deckY = leftHandedMode ? Constants.LHM_DECK_Y : Constants.RHM_DECK_Y;
//        float topX = leftHandedMode ? Constants.LHM_TOP_X : Constants.RHM_TOP_X;
//        float topY = leftHandedMode ? Constants.LHM_TOP_Y : Constants.RHM_TOP_Y;
//        float z = 0f;

//        deckObject.transform.position = new Vector3(deckX, deckY, z);
//        topObject.transform.position = new Vector3(topX, topY, z);
//    }

//    /// <summary>
//    /// Set the table with all the empty slots as null suit and 0 value, and the top design for the correct hand setup based on PlayerPrefs
//    /// </summary>
//    public void InitTableau() {
//        ArrangeTopForHand(appInit.LeftHandedMode);
//        foreach (GameObject tops in topPos) {
//            tops.GetComponent<Selectable>().suit = null;
//            tops.GetComponent<Selectable>().value = 0;
//        }
//        foreach (GameObject bottom in bottomPos) {
//            bottom.GetComponent<Selectable>().suit = null;
//            bottom.GetComponent<Selectable>().value = 0;
//        }
//    }
//    /// <summary>
//    /// Create a new shuffled deck for a new game
//    /// </summary>
//    public void PrepDeck() {
//        SavedDeck = GenerateDeck();
//        Shuffle(SavedDeck);
//    }
//    public void PlayCards() {

//        foreach (List<string> list in bottoms) {
//            list.Clear();
//        }
//        deck.Clear();
//        discardPile.Clear();

//        deck = new List<string>(SavedDeck);

//        SolitaireSort();
//        StartCoroutine(SolitaireDeal());
//        // at the end of SolitaireDeal, the deck should only contain the cards that would be set in the talon apot 

//        SetUpTalon(deck);
//    }

//    public static List<string> GenerateDeck() {
//        List<string> newDeck = new List<string>();

//        foreach (string s in suits) {
//            foreach (string v in values) {
//                newDeck.Add(s + v);
//            }
//        }

//        return newDeck;
//    }

//    public void Shuffle<T>(List<T> list) {
//        System.Random rand = new System.Random();
//        int n = list.Count;
//        while (n > 1) {
//            int k = rand.Next(n);
//            n--;
//            T temp = list[k];
//            list[k] = list[n];
//            list[n] = temp;
//        }
//    }

//    void SolitaireSort() {
//        //Debug.Log("Solitaire Sort called. Deck has " + deck.Count + " cards in it.");
//        for (int i = 0; i < 7; i++) {
//            for (int j = i; j < 7; j++) {
//                bottoms[j].Add(deck.Last<string>());
//                deck.RemoveAt(deck.Count - 1);
//            }
//        }
//    }

//    IEnumerator SolitaireDeal() {

//        for (int i = 0; i < 7; i++) {

//            float yOffset = 0f;
//            float zOffset = Constants.Z_OFFSET;

//            foreach (string card in bottoms[i]) {
//                yield return new WaitForSeconds(0.01f);
//                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y + yOffset, bottomPos[i].transform.position.z + zOffset), Quaternion.identity, bottomPos[i].transform);
//                newCard.name = card;
//                newCard.GetComponent<Selectable>().row = i;

//                if (card == bottoms[i][bottoms[i].Count - 1]) { // ex. bottoms[0][0] faceUp = true, bottoms[4][1] faceUp = false
//                    newCard.GetComponent<Selectable>().faceUp = true;
//                    newCard.GetComponent<UpdateSprite>().ShowCardFace();
//                }

//                yOffset += Constants.STACK_Y_OFFSET;
//                zOffset += Constants.Z_OFFSET;
//                discardPile.Add(card);
//            }

//        }
//        foreach (string card in discardPile) {
//            if (deck.Contains(card)) {
//                deck.Remove(card);
//            }
//        }
//        discardPile.Clear();
//    }

//    void SetUpTalon(List<string> talonDeck) {
//        // instantiate cards behind the deckButton so that deckButton still calls them 
//        // but they are not the collider first hit
//        // since the deck stacks from top to bottom we can use the deck in order 
//        talon.Clear();
//        talonZOffset = 0;
//        float zOffset = Constants.UNDEALT_CARD_Z_OFFSET;
//        foreach (string card in talonDeck) {
//            GameObject newCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z + zOffset), Quaternion.identity);
//            newCard.name = card;
//            talon.Add(newCard);
//            //Debug.Log("Card z-value is " + newCard.transform.position.z.ToString());
//            zOffset += Constants.UNDEALT_CARD_Z_OFFSET;
//        }
//    }

//    public void ResetTable() {
//        DestroyCards();
//        InitTableau();
//        Debug.Log("Table reset");
//    }

//    void DestroyCards() {
//        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
//        foreach (GameObject card in talon) {
//            if (card.CompareTag(Constants.CARD_TAG)) {
//                Debug.Log("Destroying : " + card.name);
//                Destroy(card);
//            }
//        }
//        foreach (UpdateSprite card in cards) {
//            if (card.CompareTag(Constants.CARD_TAG)) {
//                Debug.Log("Destroying : " + card.name);
//                Destroy(card.gameObject);
//            }
//        }
//    }
//}
