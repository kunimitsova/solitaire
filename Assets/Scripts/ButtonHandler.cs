using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButtonHandler : MonoBehaviour {
    // attach to Canvas I guess?

    TestMessagesScript tmsc;

    private void Start() {
        tmsc = FindObjectOfType<TestMessagesScript>();
        UIButtons.TestButtonClicked += tmsc.OpenTestMessages;
    }

    private void OnDisable() {
        UIButtons.TestButtonClicked -= tmsc.OpenTestMessages;
    }

}
