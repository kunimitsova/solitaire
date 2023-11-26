using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

    public static Transform FindYoungestChild(Transform parentObj) {
        // gets the last-added child of an object and checks if that obj has children, if so it checks that child obj for the last added child, etc.
        Transform t1 = parentObj;
        Transform t2;
        while (t1.childCount > 0) {
            t2 = t1.GetChild(t1.childCount - 1);
            t1 = t2;
        }
        return t1;
    }

    public static float GetInitDeckXOffset(bool leftHandMode) {
        float x = leftHandMode ? Constants.LHM_INIT_DECK_X_OFFSET : Constants.RHM_INIT_DECK_X_OFFSET;
        return x;
    }

    public static bool GetLeftHandMode(int lhmInt) {
        bool b = lhmInt == Constants.LEFT_HAND_MODE_TRUE;
        return b;
    }

    public static float TestingCalculations(float initDeckXOffset, float localXDeckOffset, int localDealAmount, int i) {
        float finalXpos = initDeckXOffset + ((localDealAmount - 1 - i) * localXDeckOffset);
        return finalXpos;
    }

}
