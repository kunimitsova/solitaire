using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour {

    public Sprite cardFace;
    public Sprite cardBack;
    public SolitaireInstantiate solitaire;

    public SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private UserInput userInput;

    private void Start() {
        List<string> deck = SolitaireStringDeckSetup.GenerateDeck();
        //userInput = FindObjectOfType<UserInput>();

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

    public void ShowCardFace() {
        selectable = this.GetComponent<Selectable>();
        if (selectable.faceUp) {
            this.GetComponent<SpriteRenderer>().sprite = cardFace;
        } else {
            this.GetComponent<SpriteRenderer>().sprite = cardBack;
        }
    }
}
