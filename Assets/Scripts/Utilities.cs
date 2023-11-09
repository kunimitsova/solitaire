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

}
