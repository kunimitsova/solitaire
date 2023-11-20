using UnityEngine;
using GoogleMobileAds.Api;

public class Banner : MonoSingleton<Banner> { // TODO: should this be a singleton? I mean yes, but also should it connect to a GO? I kinda liked it like that.
    [SerializeField] private string _androidGameId = Constants.APP_ID_ANDROID;
    [SerializeField] private string _iOSGameId = Constants.APP_ID_IOS;

    [SerializeField] string _androidBannerID = Constants.ANDROID_AD_UNIT_ID_BANNER;
    [SerializeField] string _iOSBannerID = Constants.IOS_AD_UNIT_ID_BANNER;

    private BannerView banner;

    string _bannerAdId = null;
    private string _gameId;

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

}

