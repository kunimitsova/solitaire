using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestMessagesScript : MonoBehaviour {
    // attach to test panel
    [SerializeField] TMP_Text testMessageButtonText;
    
    public void CloseTextMessages() {
        gameObject.SetActive(false);
    }

    public void OpenTestMessages(string text) {
        Debug.Log("OpenTestMessges referenced");
        TestButtonTextUpdate(text);
        gameObject.SetActive(true);
    }

    public void TestButtonTextUpdate(string text) {
        testMessageButtonText.text = text;
    }
}
