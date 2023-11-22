using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButtonHandler : MonoBehaviour {
    // attach to Canvas I guess?

    public GameObject testPanel;
    TestMessagesScript tmsc;

    private void Start() {
        Debug.Log("TestButtonHandler Start method called");
        tmsc = testPanel.GetComponent<TestMessagesScript>();
        UIButtons.TestButtonClicked += tmsc.OpenTestMessages;
    }

    private void OnDisable() {
        UIButtons.TestButtonClicked -= tmsc.OpenTestMessages;
    }

}
