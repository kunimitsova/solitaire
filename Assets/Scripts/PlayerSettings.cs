using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSettings : MonoBehaviour {
    // attached to Canvas

    [SerializeField] GameObject settingsUI;
    [SerializeField] Toggle tog;
    [SerializeField] Slider slide;

    public delegate void OnPlayerPrefsUpdate(); // it's every time, not just when something is changed. It's any time settings is opened.
    public static OnPlayerPrefsUpdate SettingsUpdated;

    public void Awake() {
        UIButtons.SettingsClicked += OpenPlayerSettings;
    }

    public void OnEnable() {
        UIButtons.SettingsClicked += OpenPlayerSettings;
        SetStartingPrefs();
        //ApplySettings();
    }

    public void OnDisable() {
        UIButtons.SettingsClicked -= OpenPlayerSettings;
    }

    public void ApplySettings() {
        SettingsUpdated?.Invoke();
    }

    public void SetStartingPrefs() {
        tog.isOn = (PlayerPrefs.GetInt(Constants.LEFT_HAND_MODE) == Constants.LEFT_HAND_MODE_TRUE);
        slide.value = (float)PlayerPrefs.GetInt(Constants.TALON_DEAL_AMOUNT);
        slide.GetComponentInChildren<TMP_Text>().text = ((int)slide.value).ToString();
        tog.onValueChanged.AddListener(delegate { LHMtoggleClicked(tog.isOn); });
        slide.onValueChanged.AddListener(delegate { DealSliderChanged(slide.value); });
    }

    public void OpenPlayerSettings() {
        settingsUI.SetActive(true);
    }

    public void ClosePlayerSettings() {
        int lhmInt = tog.isOn ? Constants.LEFT_HAND_MODE_TRUE : Constants.LEFT_HAND_MODE_FALSE;
        int dealAmt = (int)slide.value;
        SetPlayerPrefs(lhmInt, dealAmt);
        ApplySettings();
        settingsUI.SetActive(false);
    }
    public void SetPlayerPrefs(int LhmInt, int dealAmt) {
        PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, LhmInt);
        PlayerPrefs.SetInt(Constants.TALON_DEAL_AMOUNT, dealAmt);
        PlayerPrefs.Save();
    }
    
    public void LHMtoggleClicked(bool isLeftHandMode) {
        //if (isLeftHandMode) {
        //    PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_TRUE);
        //}
        //else {
        //    PlayerPrefs.SetInt(Constants.LEFT_HAND_MODE, Constants.LEFT_HAND_MODE_FALSE);
        //}
    }

    public void DealSliderChanged(float value) {
        int dealNumber = (int)value;
        slide.GetComponentInChildren<TMP_Text>().text = dealNumber.ToString();
        //PlayerPrefs.SetInt(Constants.TALON_DEAL_AMOUNT, dealNumber);
    }
}
