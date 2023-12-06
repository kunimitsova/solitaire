using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSettingsValues : MonoBehaviour {
    // attached to _scenemanager. 
    // contains Awake

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

    void Awake() {
        SetInitValues();
        PlayerSettings.SettingsUpdated += SetInitValues;
    }

    private void OnDisable() {
        PlayerSettings.SettingsUpdated -= SetInitValues;
    }

    public void SetInitValues() {
        talonDealAmount = PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT, 1);
        leftHandedMode = Utilities.GetLeftHandMode(PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_FALSE));
        initXDeckOffset = Utilities.GetInitDeckXOffset(leftHandedMode);
        xDeckOffset = Constants.DECK_X_OFFSET; 
    }
}
