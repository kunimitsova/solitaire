using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour {

    public Sprite cardFace;
    public Sprite cardBack;

    public SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;
    private UserInput userInput;

    // Start is called before the first frame update
    void Start() {
        List<string> deck = Solitaire.GenerateDeck();
        solitaire = FindObjectOfType<Solitaire>();
        userInput = FindObjectOfType<UserInput>();

        int i = 0;
        foreach (string card in deck) {
            if (this.name == card) {
                cardFace = solitaire.cardfaces[i];
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
        ShowCardFace();
    }

    // Update is called once per frame
    void Update() {
        //if (selectable.faceUp) {
        //    spriteRenderer.sprite = cardFace;
        //} else {
        //    spriteRenderer.sprite = cardBack;
        //}
        //// ************* delete this when we change to touch and drag only inputs!! 
        //if (userInput.slot1) {
        //    if (name == userInput.slot1.name) {
        //        spriteRenderer.color = Color.yellow;
        //    }
        //    else {
        //        spriteRenderer.color = Color.white;
        //    }
        //}
    }
    public void ShowCardFace() {
        selectable = this.GetComponent<Selectable>();
        if (selectable.faceUp) {
            this.GetComponent<SpriteRenderer>().sprite = cardFace;
        } else {
            this.GetComponent<SpriteRenderer>().sprite = cardBack;
        }
    }
}
