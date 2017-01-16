using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

public class UnityAnalyticsManager : MonoBehaviour
{
    public static UnityAnalyticsManager SingletonInstance = null;

    public string GameStart = "GameStart";
    public string LevelStart = "LevelStart";
    public string GameOver = "GameOver";

    public string LeftBrainNormal = "LeftBrainNormal";
    public string LeftBrainHard = "LeftBrainHard";
    public string LeftBrainFrenzyNormal = "LeftBrainFrenzyNormal";
    public string LeftBrainFrenzyHard = "LeftBrainFrenzyHard";

    public string RightBrainNormal = "RightBrainNormal";
    public string RightBrainHard = "RightBrainHard";
    public string RightBrainFrenzyNormal = "RightBrainFrenzyNormal";
    public string RightBrainFrenzyHard = "RightBrainFrenzyHard";

    public string RemoveAdsViewed = "RemoveAdsViewed";

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

    public void CustomAnalyticsEvent(string eventName, Dictionary<string, object> inDictionary)
    {
        Analytics.CustomEvent(eventName, inDictionary);
    }

    public void GameLaunchAnalyticsEvent(Dictionary<string, object> inDictionary)
    {
        Analytics.CustomEvent(GameStart, inDictionary);
    }

    public void LevelStartAnalyticsEvent(Dictionary<string, object> inDictionary)
    {
        Analytics.CustomEvent(LevelStart, inDictionary);
    }

    public void GameOverAnalyticsEvent(Dictionary<string, object> inDictionary)
    {
        
    }

    public void RemoveAdsViewedAnalyticsEvent(Dictionary<string, object> inDictionary)
    {
        Analytics.CustomEvent(RemoveAdsViewed, inDictionary);
    }
}
