using UnityEngine;

public class PlayerSettings : MonoBehaviour{

    [SerializeField] GameObject settingsUI;


    public void Awake() {
        Debug.Log("Settings is NOT active. (Awake)");
        UIButtons.SettingsClicked += OpenPlayerSettings;
    }

    public void OnEnable() {
        Debug.Log("Settings is Enabled. (OnEnable)");
        UIButtons.SettingsClicked += OpenPlayerSettings;
    }

    public void OnDisable() {
        Debug.Log("Settings is Disabled. (OnDisable"); 
        UIButtons.SettingsClicked -= OpenPlayerSettings;
        ClosePlayerSettings();
    }

    public void OpenPlayerSettings() {
        settingsUI.SetActive(true);
    }

    public void ClosePlayerSettings() {
        PlayerPrefs.Save();
        settingsUI.SetActive(false);
    }

    public void LHMtoggleClicked(bool isLeftHandMode) {
        Debug.Log("isLeftHandMode = " + isLeftHandMode.ToString());
        if (isLeftHandMode) {
            PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_TRUE);
        }
        else {
            PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_FALSE);
        }
    }

    public void DealSliderChanged(float value) {
        int dealNumber = (int)value;
        Debug.Log("The slider changed and the new value is : " + dealNumber.ToString());
        PlayerPrefs.SetInt(Constants.TALON_DEAL_AMOUNT, dealNumber);
    }
}
