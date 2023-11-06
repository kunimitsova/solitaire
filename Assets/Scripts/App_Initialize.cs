using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App_Initialize : MonoBehaviour {

    public int talonDealAmount; // the number of cards to deal from the talon each time
    public bool leftHandedMode;

    public float initXDeckOffset;
    public float xDeckOffset;

    void Start() {
        talonDealAmount = PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT, 1);
        leftHandedMode = PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE, 0) == 1; // if leftHandedMode = 1 then it's true otherwise false
        initXDeckOffset = leftHandedMode ? Constants.INIT_DECK_X_OFFSET : -Constants.INIT_DECK_X_OFFSET; // offset towards the right in LHM, towards teh left in RHM
        xDeckOffset = leftHandedMode ? Constants.DECK_X_OFFSET : -Constants.DECK_X_OFFSET; // move them toward the right in LHM, toward the left in RHM
    }

}
