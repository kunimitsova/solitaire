using UnityEngine;

public class PlayerSettings : MonoBehaviour{

    [SerializeField] GameObject SettingsUI;

    public void OnEnable() {
        UIButtons.SettingsClicked += OpenPlayerSettings;
    }

    public void OnDisable() {
        UIButtons.SettingsClicked -= OpenPlayerSettings;
        ClosePlayerSettings();
    }

    public void OpenPlayerSettings() {
        SettingsUI.SetActive(true);
    }

    public void ClosePlayerSettings() {
        SettingsUI.SetActive(false);
    }

    public void LHMtoggle(bool isLeftHandMode) {
        if (isLeftHandMode) {
            PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_TRUE);
        }
        else {
            PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_FALSE);
        }
    }


}
