using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSettingsValues : MonoBehaviour {
    // attached to _scenemanager. 
    // contains Awake

    private static int talonDealAmount; // the number of cards to deal from the talon each time
    public int TalonDealAmount { // good example of property
        get {
            return talonDealAmount;
        }
        set {
            talonDealAmount = value;
        }
    }
    private static bool leftHandedMode;
    public bool LeftHandedMode {
        get {
            return leftHandedMode;
        }
        set {
            leftHandedMode = value;
        }
    }

    private static float initXDeckOffset;
    public float InitXDeckOffset {
        get {
            return initXDeckOffset;
        }
    }
    private static float xDeckOffset;
    public float XDeckOffset {
        get {
            return xDeckOffset;
        }
        set {
            xDeckOffset = value;
        }
    }

    private static float talonDealtStackXPos;
    public float TalonDealtStackXPos {
        get {
            return talonDealtStackXPos;
        }
        set {
            talonDealtStackXPos = value;
        }
    }

    void Awake() {
        SetInitValues();
        PlayerSettings.SettingsUpdated += SetInitValues;
        Debug.Log("TalonDeckXOffset = " + talonDealtStackXPos.ToString());
    }

    private void OnDisable() {
        PlayerSettings.SettingsUpdated -= SetInitValues;
    }

    public void SetInitValues() {
        talonDealAmount = GetTalonDealAmount();
        leftHandedMode = GetLeftHandModeFromInt(PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_FALSE));
        initXDeckOffset = GetInitDeckXOffset(leftHandedMode);
        xDeckOffset = GetXDeckOffset(leftHandedMode);
        talonDealtStackXPos = GetTalonDeckStackXPos(leftHandedMode);
    }

    float GetXDeckOffset(bool leftHandMode) {
        float x = leftHandMode ? Constants.DECK_X_OFFSET : Constants.DECK_X_OFFSET;
        return x;
    }

    float GetTalonDeckStackXPos(bool leftHandMode) {
        float x = leftHandMode ? Constants.LHM_INIT_DECK_X_OFFSET : -Constants.LHM_INIT_DECK_X_OFFSET;
        return x;
    }

    public static int GetTalonDealAmount() {
        int i = PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT, 1);
        return i;
    }


    float GetInitDeckXOffset(bool leftHandMode) {
        float xOffset = GetXDeckOffset(leftHandMode);
        int talonDealAmount = GetTalonDealAmount();
        float x = leftHandMode ? Constants.LHM_INIT_DECK_X_OFFSET : -Constants.LHM_INIT_DECK_X_OFFSET - ((talonDealAmount - 1) * xOffset);
        return x;
    }

    bool GetLeftHandModeFromInt(int lhmInt) {
        bool b = lhmInt == Constants.LEFT_HAND_MODE_TRUE;
        return b;
    }
}
