using UnityEngine;
using System.Collections;
using System;
using Diggames.Utilities;

public class PlayerConnect : MonoBehaviour
{
    public static PlayerConnect SingletonInstance = null;
    public bool IsConnected
    {
        get
        {
#if UNITY_ANDROID
            if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
                return true;
#elif UNITY_IOS
            if(GameCenterManager.IsPlayerAuthenticated)
                return true;
#endif
            return false;
        }
    }

    void Awake()
    {
        if(SingletonInstance == null)
        {
            SingletonInstance = this;
        }
        else if(SingletonInstance != null)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
#if UNITY_ANDROID
        GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
#elif UNITY_IOS
        GameCenterManager.OnAuthFinished += OnAuthFinished;
#endif
    }

    void OnDisable()
    {
#if UNITY_ANDROID
        GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
#endif
    }

    void Start()
    {
        PlayServiceConnect();
    }

#if UNITY_ANDROID
    private void ActionConnectionResultReceived(GooglePlayConnectionResult result)
    {
        if(result.IsSuccess)
        {
            DebugLogger.LogMessage("Connected!");

            //Player logged in successfully, so let us go back through the store data to check if ads removed was purchased or not.
            ZWIAPManager.IsInitialized = false;
            ZWIAPManager.Initialize();
        }
        else
        {
            DebugLogger.LogMessage("Connection failed with code: " + result.code.ToString());
        }
    }
#endif

#if UNITY_IOS
    private void OnAuthFinished(ISN_Result res)
    {
        if(res.IsSucceeded)
        {
            DebugLogger.LogMessage("Connected!");
            
        }
        else
        {
            DebugLogger.LogMessage("Connection failed.");   
        }
    }
#endif

    private void OnPlayerConnected()
    {
        DebugLogger.LogMessage("Player already connected.");
    }

    private void OnDestroy()
    {
#if UNITY_ANDROID
        GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
#endif
    }

    public void PlayServiceConnect()
    {
#if UNITY_ANDROID
        if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
        {
            //checking if player already connected
            OnPlayerConnected();
        }
        else
        {
            DebugLogger.LogMessage("Attempting GooglePlay connect...");
            GooglePlayConnection.Instance.Connect();
        }
#elif UNITY_IOS
        GameCenterManager.Init();
#endif
    }

    public void PlayServiceDisconnect()
    {
#if UNITY_ANDROID
        GooglePlayConnection.Instance.Disconnect();
#elif UNITY_IOS
        //iOS disconnect logic here
        //no iOS disconnect logic due to only being able to call Game Center connection logic once per session - Apple rules
#endif
    }

    public void PlayServiceDisconnectAndClearDefaultAccount()
    {
#if UNITY_ANDROID
        DebugLogger.LogMessage("Disconnect and clear default account.");
        GooglePlusAPI.Instance.ClearDefaultAccount();
#elif UNITY_IOS
        //iOS disconnect and clear default account logic here
        //No disconnect here for same reasons and above and managed outside of app in GameCenter.
#endif
    }
}
