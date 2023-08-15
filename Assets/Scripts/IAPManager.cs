using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener, IStoreController
{
    public DisableAds disableAds;
    IStoreController m_StoreController;
    IAppleExtensions m_AppleExtensions;

    public string noAdsProductId = "com.josephusher.eyeseehue.disableads";

    public ProductCollection products => throw new NotImplementedException();

    void Start()
    {
        //If there is no adsdisabled playerpref
        if (PlayerPrefs.HasKey("AdsDisabled"))
        {
            if(PlayerPrefs.GetInt("AdsDisabled") == 1)
            {
                //Disable ads and track this purchase in playerprefs
                TrackNoAds();
            }
            else if(PlayerPrefs.GetInt("AdsDisabled") != 1)
            {
                //Initialize iap
                InitializePurchasing();
                //show the buttons
                disableAds.ShowAdButtons();
                Debug.Log("Show ads");
            }
        }
        else
        {
            //Initialize iap
            InitializePurchasing();
            //show the buttons
            disableAds.ShowAdButtons();
            Debug.Log("Show ads");
        }
    }

    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(noAdsProductId, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");

        m_StoreController = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
    }

    public void Restore()
    {
        m_AppleExtensions.RestoreTransactions(OnRestore);
        if (HasNoAds())
            BuyNoAds();
    }

    void OnRestore(bool success)
    {
        var restoreMessage = "";
        if (success)
        {
            // This does not mean anything was restored,
            // merely that the restoration process succeeded.
            restoreMessage = "Restore Successful";
            if (HasNoAds())
                TrackNoAds();
        }
        else
        {
            // Restoration failed.
            restoreMessage = "Restore Failed";
        }

        Debug.Log(restoreMessage);
    }

    void TrackNoAds()
    {
        //Hide the ads
        disableAds.HideAdButtons();
        //Hide any active banner
        if (Advertisement.isInitialized)
            Advertisement.Banner.Hide();
        //Keep track of this purchase
        if(PlayerPrefs.HasKey("AdsDisabled") == false)
        {
            PlayerPrefs.SetInt("AdsDisabled", 1);
            PlayerPrefs.Save();
            Debug.Log("Saving AdsDisabled");
        }
    }

    public void BuyNoAds()
    {
        m_StoreController.InitiatePurchase(noAdsProductId);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;

        Debug.Log($"Processing Purchase: {product.definition.id}");
        if (product.definition.id == "com.josephusher.eyeseehue.disableads")
        {
            TrackNoAds();
            Debug.Log("Logging");
        }

        return PurchaseProcessingResult.Complete;
    }

    bool HasNoAds()
    {
        //Get a reference to noAdsProductId's entry in m_StoreController's list of products
        var noAdsProduct = m_StoreController.products.WithID(noAdsProductId);
        //return true if there is a record of the product and it has a receipt of purchase.
        return noAdsProduct != null && noAdsProduct.hasReceipt;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void InitiatePurchase(Product product, string payload)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(string productId, string payload)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(Product product)
    {
        throw new NotImplementedException();
    }

    public void InitiatePurchase(string productId)
    {
        throw new NotImplementedException();
    }

    public void FetchAdditionalProducts(HashSet<ProductDefinition> additionalProducts, Action successCallback, Action<InitializationFailureReason> failCallback)
    {
        throw new NotImplementedException();
    }

    public void ConfirmPendingPurchase(Product product)
    {
        throw new NotImplementedException();
    }
}
