using UnityEngine;
using Diggames.Utilities;
using System.Collections;

public class ZWIAPManager
{
    public static bool IsInitialized = false;
    public const string AndroidRemoveAds = "remove_ads";
    public const string iOSRemoveAds = "com.diggames.colorsbrain.remove_ads";

    public static void Initialize()
    {
        //Already initiated, so do not initiate again
        if(IsInitialized)
            return;

#if UNITY_ANDROID
        //Fill the product list
        AndroidInAppPurchaseManager.Client.AddProduct(AndroidRemoveAds);

        //Listen for purchase events
        AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;

        //Listen for finish of store initialization
        AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;

        //Use connect without parameter because of supplied base64EncodedPublicKey in plugin settings
        AndroidInAppPurchaseManager.Client.Connect();
#elif UNITY_IOS
        //Subscribe to events
        IOSInAppPurchaseManager.OnVerificationComplete += HandleOnVerificationComplete;
        IOSInAppPurchaseManager.OnStoreKitInitComplete += OnStoreKitInitComplete;


        IOSInAppPurchaseManager.OnTransactionComplete += OnTransactionComplete;
        IOSInAppPurchaseManager.OnRestoreComplete += OnRestoreComplete;

        IOSInAppPurchaseManager.Instance.AddProductId(iOSRemoveAds);

        IsInitialized = true;

        IOSInAppPurchaseManager.Instance.LoadStore();
#endif
    }

#if UNITY_ANDROID
    private static void OnBillingConnected(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;

        if(result.IsSuccess)
        {
            //Store connection is Successful. Next we loading product and customer purchasing details
            AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinished;
            AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
        }

        DebugLogger.LogMessage("Connection Response: " + result.Response.ToString() + " " + result.Message);
    }

    private static void OnProductPurchased(BillingResult result)
    {
        if(result.IsSuccess)
        {
            DebugLogger.LogMessage("Product Purchased " +result.Purchase.SKU + "\n Full Response: " + result.Purchase.OriginalJson);
            ProcessPurchasedProduct(result.Purchase);
        }
        else
        {
            DebugLogger.LogMessage("Product Purchase Failed " +result.Response.ToString() + " " + result.Message);
        }

        DebugLogger.LogMessage("Purchased Response: " + result.Response.ToString() + " " + result.Message);

    }

    //Handle a purchase after it is made
    //This will be unique to each game
    private static void ProcessPurchasedProduct(GooglePurchaseTemplate purchase)
    {
        switch(purchase.SKU)
        {
            case AndroidRemoveAds :
                UnityAdManager.SingletonInstance.UpdateAdPurchases(true);
                break;
        }
    }

    private static void OnRetrieveProductsFinished(BillingResult result)
    {
        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinished;

        if(result.IsSuccess)
        {
            UpdateStoreData();
            IsInitialized = true;
            DebugLogger.LogMessage("Success! Billing init complete inventory contains: " + AndroidInAppPurchaseManager.Client.Inventory.Purchases.Count + " products");

            DebugLogger.LogMessage("Loaded products names: ");

            foreach(GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products)
            {
                DebugLogger.LogMessage(tpl.Title);
                DebugLogger.LogMessage(tpl.OriginalJson);
            }
        }

        DebugLogger.LogMessage("Connection Response: " + result.Response.ToString() + " " + result.Message);
    }

    //This is where we handle setting the proper states for each purchase
    //This will be game specific for each game
    private static void UpdateStoreData()
    {
        foreach(GoogleProductTemplate p in AndroidInAppPurchaseManager.Client.Inventory.Products)
            DebugLogger.LogMessage("Loaded product: " + p.Title);

        //Check if a non-consumeable product is purchased and ensure we have local data for it.
        //This covers the case of buying it on device A, and then reinstalling on device B
        //This replaces the need for restore purchases functionality
        if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased(AndroidRemoveAds))
        {
            UnityAdManager.SingletonInstance.UpdateAdPurchases(true);
        }
        else
        {
            UnityAdManager.SingletonInstance.UpdateAdPurchases(false);
        }
    }
#endif

    public static void Purchase(string SKU)
    {
#if UNITY_ANDROID
        AndroidInAppPurchaseManager.Client.Purchase(SKU);
#elif UNITY_IOS
        IOSInAppPurchaseManager.Instance.BuyProduct(SKU);
#endif
    }

#if UNITY_IOS
    private static void HandleOnVerificationComplete(IOSStoreKitVerificationResponse response)
    {
        IOSNativePopUpManager.showMessage("Verification", "Transaction verification status: " + response.status.ToString());
        ISN_Logger.Log("ORIGINAL JSON: " + response.originalJSON);
    }

    private static void OnStoreKitInitComplete(ISN_Result result)
    {
        if(result.IsSucceeded)
        {
            int avaliableProductsCount = 0;
            foreach(IOSProductTemplate tpl in IOSInAppPurchaseManager.Instance.Products)
            {
                if(tpl.IsAvaliable)
                {
                    avaliableProductsCount++;
                }
            }

            IOSNativePopUpManager.showMessage("StoreKit Init Succeeded", "Available products count: " + avaliableProductsCount);
            ISN_Logger.Log("StoreKit Init Succeeded Available products count: " + avaliableProductsCount);
        }
        else
        {
            IOSNativePopUpManager.showMessage("StoreKit Init Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
        }
    }

    private static void OnTransactionComplete(IOSStoreKitResult result)
    {

        ISN_Logger.Log("OnTransactionComplete: " + result.ProductIdentifier);
        ISN_Logger.Log("OnTransactionComplete: state: " + result.State);

        switch(result.State)
        {
            case InAppPurchaseState.Purchased:
            case InAppPurchaseState.Restored:
                //Our product been succsesly purchased or restored
                //So we need to provide content to our user depends on productIdentifier
                UnlockProducts(result.ProductIdentifier);
                break;
            case InAppPurchaseState.Deferred:
                //iOS 8 introduces Ask to Buy, which lets parents approve any purchases initiated by children
                //You should update your UI to reflect this deferred state, and expect another Transaction Complete  to be called again with a new transaction state 
                //reflecting the parent’s decision or after the transaction times out. Avoid blocking your UI or gameplay while waiting for the transaction to be updated.
                break;
            case InAppPurchaseState.Failed:
                //Our purchase flow is failed.
                //We can unlock intrefase and repor user that the purchase is failed. 
                ISN_Logger.Log("Transaction failed with error, code: " + result.Error.Code);
                ISN_Logger.Log("Transaction failed with error, description: " + result.Error.Description);
                break;
        }

        if(result.State == InAppPurchaseState.Failed)
        {
            IOSNativePopUpManager.showMessage("Transaction Failed", "Error code: " + result.Error.Code + "\n" + "Error description:" + result.Error.Description);
        }
        else
        {
            IOSNativePopUpManager.showMessage("Store Kit Response", "product " + result.ProductIdentifier + " state: " + result.State.ToString());
        }

    }


    private static void OnRestoreComplete(IOSStoreKitRestoreResult res)
    {
        if(res.IsSucceeded)
        {
            IOSNativePopUpManager.showMessage("Success", "Restore Compleated");
        }
        else
        {
            IOSNativePopUpManager.showMessage("Error: " + res.Error.Code, res.Error.Description);
        }
    }
    private static void UnlockProducts(string productIdentifier)
    {
        switch(productIdentifier)
        {
            case iOSRemoveAds:
                UnityAdManager.SingletonInstance.UpdateAdPurchases(true);
                break;
        }
    }
#endif
}

