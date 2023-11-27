using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SolitaireStringDeckSetup : MonoBehaviour {
    // this began from a tutorial and so it is very bloated and messy. I don't know if I want to put the effort in to update it.
    // edit I will split this after I have fixed the UX problems (undo and autoplay)
    // attach to SolitaireGame
    // contains Start, OnEnable, OnDisable

    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public List<string>[] bottoms; // lol

    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();

    public List<string> deck;
    public List<string> SavedDeck; // for replay THIS game

    void Start() {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };

    }

    /// <summary>
    /// Create a new shuffled deck for a new game
    /// </summary>
    public void PrepDeck() {
        SavedDeck = GenerateDeck();
        Shuffle(SavedDeck);
    }

    public void InitialDeal() {
        // using the strings, deal the cards.
        foreach (List<string> list in bottoms) {
            list.Clear();
        }
        deck.Clear();
        //discardPile.Clear();

        deck = new List<string>(SavedDeck);

        SolitaireSort();

        // at the end of SolitaireDeal, the deck should only contain the cards that would be set in the talon spot 
    }

    public static List<string> GenerateDeck() {
        List<string> newDeck = new List<string>();

        foreach (string s in suits) {
            foreach (string v in values) {
                newDeck.Add(s + v);
            }
        }

        return newDeck;
    }

    public void Shuffle<T>(List<T> list) {
        System.Random rand = new System.Random();
        int n = list.Count;
        while (n > 1) {
            int k = rand.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void SolitaireSort() {
        //Debug.Log("Solitaire Sort called. Deck has " + deck.Count + " cards in it.");
        for (int i = 0; i < 7; i++) {
            for (int j = i; j < 7; j++) {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
        // the remaining cards are the starting TALON string list
    }
}
