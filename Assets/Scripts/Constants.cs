using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {
    // card instantiate values
    public const float STACK_Y_OFFSET = -0.25f;
    public const float INIT_DECK_X_OFFSET = 0.786f;
    public const float DECK_X_OFFSET = 0.2f;
    public const float Z_OFFSET = -0.01f;
    public const float UNDEALT_CARD_Z_OFFSET = 0.01f;  // for cards to stack behind deckButton

    // camera values
    public const float CAMERA_DISTANCE = 10f;

    // tags
    public const string CARD_TAG = "Card";
    public const string DECK_TAG = "Deck";
    public const string TOP_TAG = "Top";
    public const string BOTTOM_TAG = "Bottom";

    // player prefs
    public const string LEFT_HAND_MODE = "LeftHandMode";
    public const string TALON_DEAL_AMOUNT = "TalonDealAmount";

    // card value constants
    public const int KING_VALUE = 13;
    public const int QUEEN_VALUE = 12;
    public const int JACK_VALUE = 11;
    public const int ACE_VALUE = 1;
    public const string KING_STRING = "K";
    public const string QUEEN_STRING = "Q";
    public const string JACK_STRING = "J";
    public const string ACE_STRING = "A";
}
