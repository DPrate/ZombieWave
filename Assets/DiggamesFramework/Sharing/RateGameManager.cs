using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Singleton class to handle the rating of apps.
public class RateGameManager : MonoBehaviour
{
    public static RateGameManager SingletonInstance;

    public string AndroidAppURL = "market://details?id=YOUR_ID";
    public string iOSAppUrl = "itms - apps://itunes.apple.com/app/idYOUR_ID";
    public string ForumFeedbackURL = "http://www.diggames.com/forum/";

    public int NumberOfPlaysBetweenRateAsks = 12;

    public string PlayerPrefsPlayCounterKey = "NumberOfPlays";
    public string PlayerPrefsRatedApp = "HasRatedApp";

    public GameObject RateGameUIObject;
    public Toggle DoNotAskAgainToggle;

    private bool DoNotShowRateUI = false;

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

    void OnEnable()
    {
        DoNotAskAgainToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDisable()
    {
        DoNotAskAgainToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    public void AddToPlayCounter()
    {
        if(!PlayerPrefs.HasKey(PlayerPrefsPlayCounterKey))
            PlayerPrefs.SetInt(PlayerPrefsPlayCounterKey, 0);

        int currentCount = PlayerPrefs.GetInt(PlayerPrefsPlayCounterKey) + 1;

        //If they have not rated the app yet and have not selected to not show the rate app UI
        if(PlayerPrefs.GetInt(PlayerPrefsRatedApp) != 1 && !DoNotShowRateUI)
        {
            //If enough plays have passed since last asking, ask to rate again
            if(currentCount >= NumberOfPlaysBetweenRateAsks)
            {
                PlayerPrefs.SetInt(PlayerPrefsPlayCounterKey, 0);
                RateGameUIObject.SetActive(true);
            }
            else
                PlayerPrefs.SetInt(PlayerPrefsPlayCounterKey, currentCount);
        }
    }

    public void LeaveFeedback()
    {
        Application.OpenURL(ForumFeedbackURL);
    }

    public void RateApp()
    {
#if UNITY_ANDROID
        Application.OpenURL(AndroidAppURL);
#elif UNITY_IPHONE
 Application.OpenURL("iOSAppURL");
#endif

        PlayerPrefs.SetInt(PlayerPrefsRatedApp, 1);
        RateGameUIObject.SetActive(false);
    }

    private void OnToggleChanged(bool inValue)
    {
        //On toggle change update
        DoNotShowRateUI = inValue;
    }
}
