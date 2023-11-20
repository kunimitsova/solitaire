using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class Rewarded : MonoSingleton<Rewarded> { // TODO: should this be a singleton? I mean yes, but also should it connect to a GO? I kinda liked it like that.
    [SerializeField] private string _androidGameId = Constants.APP_ID_ANDROID;
    [SerializeField] private string _iOSGameId = Constants.APP_ID_IOS;

    //[SerializeField] private bool testMode = false;

    //[SerializeField] private GameObject _sceneManager;

    [SerializeField] string _androidBannerID = Constants.ANDROID_AD_UNIT_ID_BANNER;
    [SerializeField] string _iOSBannerID = Constants.IOS_AD_UNIT_ID_BANNER;
    //[SerializeField] string _androidRewardedAd = Constants.ANDROID_AD_UNIT_ID_REWARDEDAD;
    //[SerializeField] string _iOSRewardedAd = Constants.IOS_AD_UNIT_ID_REWARDEDAD;

    //private RewardedAd ad;
    private BannerView banner;

    //string _adUnitId = null; // in case there's unsupported platforms . In case we add a rewarded ad.
    string _bannerAdId = null;
    private string _gameId;
    //private bool isAdLoaded = false;
    //private bool isShowingAd = false;
    //private bool isAdCompleted = false;

    void Awake() {
#if UNITY_IOS
        _gameId = _iOSGameId;
        _bannerAdId = _iOSBannerID;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
        _bannerAdId = _androidBannerID;
#elif UNITY_EDITOR
      _gameId = _androidGameId;
#endif
    }

    private void Start() {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) => { });
        // LoadAd(); // Don't need this until we have rewarded ads?
        this.RequestBanner();
    }

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void RequestBanner() {
        banner = new BannerView(_bannerAdId, AdSize.Banner, AdPosition.Top);

        AdRequest request = new AdRequest();

        banner.LoadAd(request);
    }

    //public void LoadAd() {  
    //    //Debug.Log("Loading Ad: " + _adUnitId);
    //    AdRequest request = new AdRequest();
    //    //AdRequest addd = new AdRequest.Builder().Build(); // it told me this is obsolete how old is this tutorial 

    //    AppOpenAd.Load(_adUnitId, request, ((appAdd, error) => {
    //        if (error != null) {
    //            Debug.Log("Failed to load ad, why? " + error.GetMessage());
    //            return;
    //        }
    //        ad = appAdd;
    //    }));
    //}

    //public void ShowAd() {
    //    if (!isAdLoaded || isShowingAd) {
    //        return;
    //    }

    //    //ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
    //    //ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
    //    //ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
    //    //ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
    //    //ad.OnPaidEvent += HandlePaidEvent;

    //    ad.Show();
    //}


    //public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) {
    //    Debug.Log("Unity ads complete state = " + showCompletionState.ToString());
    //    isAdCompleted = true;
    //    float wait = Constants.WAIT_AFTERREWARDEDAD;
    //    switch (showCompletionState) {
    //        case UnityAdsShowCompletionState.SKIPPED:
    //        case UnityAdsShowCompletionState.UNKNOWN:
    //            break;
    //        case UnityAdsShowCompletionState.COMPLETED:
    //            if (adUnitId == _adUnitId) {
    //                _showAdButton.interactable = false;
    //                _showAdButton.GetComponent<Animator>().enabled = false;
    //                _showAdButton.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
    //                _sceneManager.GetComponent<App_Initialize>().StartGame(wait);
    //                App_Initialize.hasSeenRewardAd = true;
    //            }
    //            break;
    //        default:
    //            Debug.Log("The showAdsCompletionState was out of range");
    //            break;
    //    }
    //    // Time.timeScale = 1; // (we don't really need it since the game is already stopped)
    //    Advertisement.Banner.Show(_bannerAdId);
    //}

    //public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) {
    //    Debug.Log($"Error loading ad unit {adUnitId}: {error.ToString()} - {message}");
    //    isAdLoaded = false;
    //    _showAdButton.interactable = false;
    //    _showAdButton.GetComponent<Animator>().enabled = false;
    //}

    //public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) {
    //    Debug.Log($"Error showing ad unit {adUnitId}: {error.ToString()} - {message}");
    //    isAdCompleted = false;
    //}
    //public void OnInitializationComplete() {
    //    //Debug.Log("Unity ads initialization complete");
    //    LoadBannerAd();  // this is also where we would load an interstitial ad but I hate those
    //}

    //public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
    //    Debug.Log($"Unity ads initialization failed {error.ToString()} - {message}");
    //    isAdLoaded = false;
    //}

    //public void OnUnityAdsShowStart(string adUnitId) {
    //    //Debug.Log("OnUnityAdsShowStart ad has started...?");
    //    isAdLoaded = true;
    //    //    Time.timeScale = 0;  // (we don't really need it since the game is already stopped)
    //    Advertisement.Banner.Hide(); // this hides the banner when the reward ad is showing
    //}

    //public void OnUnityAdsShowClick(string adUnitId) {
    //    //Debug.Log("OnUnityAdsShowClick what does this mean");
    //}

    //// directly from tutorial here (tutorial is https://www.youtube.com/watch?v=7pu_CpjBFtI&t=1412s from Smart Penguins )
    //public void LoadBannerAd() {
    //    Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    //    Advertisement.Banner.Load(_bannerAdId,
    //        new BannerLoadOptions {
    //            loadCallback = OnBannerLoaded,
    //            errorCallback = OnBannerError
    //        }
    //        );
    //}

    //void OnBannerLoaded() {
    //    Advertisement.Banner.Show(_bannerAdId);
    //}

    //void OnBannerError(string message) {

    //}
}

