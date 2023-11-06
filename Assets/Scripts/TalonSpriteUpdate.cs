using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalonSpriteUpdate : MonoBehaviour {

    public Sprite talonEmpty;
    public Sprite talonFull;

    public void setTalonSprite(bool isTalonEmpty) {
        if (isTalonEmpty) {
            this.GetComponent<SpriteRenderer>().sprite = talonEmpty;
        } else {
            this.GetComponent<SpriteRenderer>().sprite = talonFull;
        }
    }
}
