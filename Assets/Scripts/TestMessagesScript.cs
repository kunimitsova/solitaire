using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class TestMessagesScript : MonoBehaviour {
    // attach to test panel

    [SerializeField] Button testMessageButton; 
    
    public void CloseTextMessages() {
        this.gameObject.SetActive(false);
    }

    public void OpenTestMessages(string text) {
        TestButtonTextUpdate(text);
        this.gameObject.SetActive(true);
    }

    public void TestButtonTextUpdate(string text) {
        testMessageButton.GetComponentInChildren<Text>().text = text;
    }
}
