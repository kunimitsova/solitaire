using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealListManager : MonoBehaviour {
    // contains Start, OnEnable, OnDisable

    List<List<GameObject>> dealList = new List<List<GameObject>>();
    List<GameObject> cardList = new List<GameObject>();

    void Start() {
        
    }

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }

    void AddSublist() {
        dealList.Add(cardList);
    }

    void RemoveSublist(int subListIndex) {
        if ((subListIndex >= 0) && (subListIndex < dealList.Count)) {
            dealList.RemoveAt(subListIndex);
        }
    }

    void RemoveCardFromSublist(int dealIndex, int cardIndex) {
        dealList[dealIndex].RemoveAt(cardIndex);
    }

    void MakeSublistFromDeal(List<GameObject> items) {
        int talonDealAmount = GetSettingsValues.GetTalonDealAmount();
        for (int i = 0; i < talonDealAmount; i++) {
            if (items[i].CompareTag(Constants.CARD_TAG)) {
                cardList.Add(items[i]);
            }
        }
    }
}

