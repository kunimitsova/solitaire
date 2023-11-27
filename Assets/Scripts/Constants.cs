using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    // AdMob values
    public const string APP_ID_ANDROID = "ca-app-pub-6465163841396855~3281093860";
    public const string ANDROID_AD_UNIT_ID_BANNER = "ca-app-pub-6465163841396855/6222117820";
    public const string ANDROID_AD_UNIT_ID_REWARDEDAD = "ca-app-pub-6465163841396855/4881766365";
    public const string IOS_AD_UNIT_ID_REWARDEDAD = "";
    public const string APP_ID_IOS = "";
    public const string IOS_AD_UNIT_ID_BANNER = "";

    // Top area left/right handed option values
    public const float LHM_DECK_X = 0.17f; // this is for DECK ITEM not DECK BUTTON
    public const float LHM_DECK_Y = 0.22f; // this is for DECK ITEM not DECK BUTTON
    public const float LHM_TOP_X = 0.01f;
    public const float LHM_TOP_Y = 0.08f; // this is for TOP SECTION not TOP SLOTS
    public const float TOP_Z = 0f;    // Z-VALUE also does not change regardless 

    public const float RHM_DECK_X = 3.989f;
    public const float RHM_DECK_Y = 0.22f;
    public const float RHM_TOP_X = -1.904f;
    public const float RHM_TOP_Y = 0.08f;

    // card instantiate values
    public const float STACK_Y_OFFSET = -0.25f;
    public const float LHM_INIT_DECK_X_OFFSET = 0.686f;
    public const float RHM_INIT_DECK_X_OFFSET = -1.126f;
    public const float DECK_X_OFFSET = 0.22f;
    public const float Z_OFFSET = -0.02f;
    public const float UNDEALT_CARD_Z_OFFSET = 0.02f;  // for cards to stack behind deckButton

    // animating movement time values
    public const float ANIMATE_MOVE_TO_FOUND = 0.3f;
    public const float ANIMATE_MOVE_TO_STACK = 0.2f;
    public const float ANIMATE_DEAL_FROM_TALON = 0.1f;

    // card move values
    public const string CARDMOVE_DECK = "D";
    public const string CARDMOVE_FOUNDATION = "F";
    public const string CARDMOVE_STACK = "S";
    public const string CARDMOVE_FLIP = "flip";

    // camera values
    public const float CAMERA_DISTANCE = 10f;

    // tags
    public const string CARD_TAG = "Card";
    public const string DECK_TAG = "Deck";
    public const string TOP_TAG = "Top";
    public const string BOTTOM_TAG = "Bottom";

    // player prefs
    public const string LEFT_HAND_MODE = "LeftHandMode";
    public const int LEFT_HAND_MODE_TRUE = 1;
    public const int LEFT_HAND_MODE_FALSE = 0;
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

    // test message text
    public const string TEST_AUTOPLAY = "Sorry, AutoPlay is not working yet. (touch to dismiss)";
    public const string TEST_UNDO = "Sorry, Undo is not working yet. (touch to dismiss)";
}
