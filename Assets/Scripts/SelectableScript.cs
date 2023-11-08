using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    public bool top = false;
    public string suit;
    public int value = 0; 
    public int row;  // i think by Row they mean column...
    public bool faceUp = false;
    public bool inDeckPile = false;
//    public bool isPlayable = false; // is this a card that you could play another card on (tableau only)

    private string valueString; 

    // Start is called before the first frame update
    void Start() {
        string[] mValues = Solitaire.values;
        int startPos = 1; // strings are indexed starting at 0 so string[0] 9s the first char and then substring(1) is everything except the first char.
        if (CompareTag(Constants.CARD_TAG)) {
            suit = transform.name[0].ToString();
            valueString =  transform.name.Substring(startPos);
            value = System.Array.IndexOf(mValues, valueString) + 1; // plus one so that A = 1 , 2 = 2 etc.
            //print($"valueString is {valueString} value is {value}"); // for testing
        }
    }
}
