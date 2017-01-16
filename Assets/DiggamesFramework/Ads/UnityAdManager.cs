#if UNITY_ADS
using UnityEngine;
using UnityEngine.Advertisements;
using Diggames.Utilities;

public class UnityAdManager : MonoBehaviour
{
    public static UnityAdManager SingletonInstance = null;
        
    public int NumberOfPlaysBeforeShowAd = 5;
    public string PlayerPrefsAdCounterKey = "NumberAdsShown";
    private string playerPrefsPurchasedAdFree = "PurchasedAdsFree";
    private bool areAdsEnabled = true;
    public bool AreAdsEnabled
    {
        get
        {
            if(PlayerPrefs.HasKey(PlayerPrefsPurchasedAdFree))
                return false;

            return true;
        }
    }

    public string PlayerPrefsPurchasedAdFree
    {
        get
        {
            return playerPrefsPurchasedAdFree;
        }
    }

    void Awake()
    {
        if(SingletonInstance == null)
        {
            SingletonInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);       
    }

    void Start()
    {
        if(PlayerPrefs.HasKey(PlayerPrefsPurchasedAdFree))
            areAdsEnabled = false;
    }

    public void AddToPlayCounter()
    {
        if(PlayerPrefs.HasKey(PlayerPrefsPurchasedAdFree))
            return;

        if(!PlayerPrefs.HasKey(PlayerPrefsAdCounterKey))
            PlayerPrefs.SetInt(PlayerPrefsAdCounterKey, 0);

        int currentAdCounter = PlayerPrefs.GetInt(PlayerPrefsAdCounterKey) + 1;
        if(currentAdCounter >= NumberOfPlaysBeforeShowAd)
        {
            PlayerPrefs.SetInt(PlayerPrefsAdCounterKey, 0);
            ShowAd();
        }
        else
            PlayerPrefs.SetInt(PlayerPrefsAdCounterKey, currentAdCounter);
    }

    public void ShowAd()
    {
        if(!areAdsEnabled)
            return;

        if(Advertisement.IsReady())
            Advertisement.Show();
    }

    public void ShowRewardedAd()
    {
        if(!areAdsEnabled)
            return;

        if(Advertisement.IsReady("rewardedVideoZone"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideoZone", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch(result)
        {
            case ShowResult.Finished:
                DebugLogger.LogMessage("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                DebugLogger.LogMessage("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                DebugLogger.LogMessage("The ad failed to be shown.");
                break;
        }
    }

    public void UpdateAdPurchases(bool adsPurchased)
    {
        if(adsPurchased)
        {
            PlayerPrefs.SetInt(playerPrefsPurchasedAdFree, 1);
            DebugLogger.LogMessage("Ads purchased player prefs created.");
        }
        else
        {
            PlayerPrefs.DeleteKey(playerPrefsPurchasedAdFree);
            DebugLogger.LogMessage("Ads purchased player prefs deleted.");
        }
    }
}
#endif