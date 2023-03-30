using System;
using System.Collections.Generic;
using System.Linq;
using Stores;
using UnityEngine.Purchasing.Interfaces;
using UnityEngine.Purchasing.Models;
using UnityEngine.Purchasing.Utils;

namespace UnityEngine.Purchasing
{
    class GoogleQueryPurchasesService : IGoogleQueryPurchasesService
    {
        IGoogleBillingClient m_BillingClient;
        IGoogleCachedQuerySkuDetailsService m_CachedQuerySkuDetailsService;

        internal GoogleQueryPurchasesService(IGoogleBillingClient billingClient, IGoogleCachedQuerySkuDetailsService cachedQuerySkuDetailsService)
        {
            m_BillingClient = billingClient;
            m_CachedQuerySkuDetailsService = cachedQuerySkuDetailsService;
        }

        public void QueryPurchases(Action<List<GooglePurchase>> onQueryPurchaseSucceed)
        {
            HandleGooglePurchaseResult(QueryPurchasesWithSkuType(GoogleSkuTypeEnum.Sub()), googlePurchasesInSubs =>
            {
                HandleGooglePurchaseResult(QueryPurchasesWithSkuType(GoogleSkuTypeEnum.InApp()), googlePurchasesInApps =>
                {
                    HandleOnQueryPurchaseReceived(onQueryPurchaseSucceed, googlePurchasesInSubs, googlePurchasesInApps);
                });
            });

        }

        static void HandleOnQueryPurchaseReceived(Action<List<GooglePurchase>> onQueryPurchaseSucceed, List<GooglePurchase> googlePurchasesInSubs, List<GooglePurchase> googlePurchasesInApps)
        {
            List<GooglePurchase> queriedPurchase = googlePurchasesInSubs;
            if (googlePurchasesInApps.Count > 0)
            {
                queriedPurchase.AddRange(googlePurchasesInApps);
            }

            onQueryPurchaseSucceed(queriedPurchase);
        }

        GooglePurchaseResult QueryPurchasesWithSkuType(string skuType)
        {
            AndroidJavaObject javaPurchaseResult = m_BillingClient.QueryPurchase(skuType);
            return new GooglePurchaseResult(javaPurchaseResult, m_CachedQuerySkuDetailsService);
        }

        void HandleGooglePurchaseResult(GooglePurchaseResult purchaseResult, Action<List<GooglePurchase>> onPurchaseSucceed)
        {
            if (purchaseResult.m_ResponseCode == BillingClientResponseEnum.OK())
            {
                onPurchaseSucceed(purchaseResult.m_Purchases);
            }
        }
    }
}
