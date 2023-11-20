using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App_Initialize : MonoBehaviour {

    [SerializeField] GameObject deck;
    [SerializeField] GameObject top;

    private int talonDealAmount; // the number of cards to deal from the talon each time
    public int TalonDealAmount { // good example of property
        get {
            return talonDealAmount;
        }
        set {
            talonDealAmount = value;
        }
    }
    private bool leftHandedMode;
    public bool LeftHandedMode {
        get {
            return leftHandedMode;
        }
        set {
            leftHandedMode = value;
        }
    }

    private float initXDeckOffset;
    public float InitXDeckOffset {
        get {
            return initXDeckOffset;
        }
    }
    private float xDeckOffset;
    public float XDeckOffset { 
        get {
            return xDeckOffset;
        }
        set {
            xDeckOffset = value;
        }
    }

    void Start() {
        talonDealAmount = PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT, 1);
        leftHandedMode = PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_TRUE ) == Constants.LEFT_HAND_MODE_TRUE; // if leftHandedMode = 1 then it's true otherwise false
        initXDeckOffset = leftHandedMode ? Constants.INIT_DECK_X_OFFSET : -Constants.INIT_DECK_X_OFFSET; // offset towards the right in LHM, towards teh left in RHM
        xDeckOffset = leftHandedMode ? Constants.DECK_X_OFFSET : -Constants.DECK_X_OFFSET; // move them toward the right in LHM, toward the left in RHM
        ArrangeTopForHand();
    }

    void ArrangeTopForHand() { // idk if this is the best way to do this ....
        float deckX = leftHandedMode ? Constants.LHM_DECK_X : Constants.RHM_DECK_X;
        float deckY = leftHandedMode ? Constants.LHM_DECK_Y : Constants.RHM_DECK_Y;
        float topX = leftHandedMode ? Constants.LHM_TOP_X : Constants.RHM_TOP_X;
        float topY = leftHandedMode ? Constants.LHM_TOP_Y : Constants.RHM_TOP_Y;
        float z = 0f;

        deck.transform.position = new Vector3(deckX, deckY, z);
        top.transform.position = new Vector3(topX, topY, z);
    }
}
