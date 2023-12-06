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

    public static bool IsThereAFlip(GameObject card) {

        Transform parent = card.transform.parent;

        if (parent.CompareTag(Constants.BOTTOM_TAG)) { // the parent of the card is from the tableau stacks
            int i = parent.childCount - 1 - 1; // minus 1 for index starts at 0 and minus 1 for the card we already know about
            if (i < 0) {
                return false; // if there is no other child card then there is no card to flip
            }
            else {
                Transform child2 = parent.GetChild(i);
                if (child2.CompareTag(Constants.CARD_TAG)) { // make sure it's a card
                    if (!child2.GetComponent<Selectable>().faceUp) { // if it is NOT faceup
                        return true;
                    }
                }
            }
        }

        return false;
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

    public static float GetXDeckOffset(bool leftHandMode) {
        // TODO: design it so the cards stack under the card closest to the deck in BOTH modes but in RHM the cards
        // are dealt STARTING at the spot furthest away and in LHM the cards are dealt ENDING at the spot furthest away.
        float x = leftHandMode ? Constants.DECK_X_OFFSET : Constants.DECK_X_OFFSET;
        return x;
    }

    bool ThisIsNotAValidMove(GameObject g1, GameObject g2) {
        // make sure it's not trying to move things that are not cards

        if (g1.CompareTag(Constants.DECK_TAG) && (g2.CompareTag(Constants.DECK_TAG))) {
            return false; // that means it is a DEAL FROM TALON move.
        }

        if (g1.CompareTag(Constants.TOP_TAG) || g1.CompareTag(Constants.BOTTOM_TAG) || g1.CompareTag(Constants.DECK_TAG)) {
            Debug.Log("TOP_TAG, BOTTOM_TAG, DECK_TAG items cannot move to another place.");
            return true;
        }
        // make sure it's not trying to move INTO the deck pile
        else if (g2.CompareTag(Constants.DECK_TAG)) {
            Debug.Log("You cannot move from somewhere to the deck pile.");
            return true;
        }

        return false;
    }

}
