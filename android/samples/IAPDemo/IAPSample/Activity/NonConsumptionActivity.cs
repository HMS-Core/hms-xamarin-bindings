/*
       Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

       Licensed under the Apache License, Version 2.0 (the "License");
       you may not use this file except in compliance with the License.
       You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

       Unless required by applicable law or agreed to in writing, software
       distributed under the License is distributed on an "AS IS" BASIS,
       WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
       See the License for the specific language governing permissions and
       limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Iap;
using Huawei.Hms.Iap.Entity;
using Huawei.Hms.Support.Api.Client;
using Org.Json;

namespace XamarinHmsIAPDemo
{
    [Activity(Label = "NonConsumptionActivity")]
    public class NonConsumptionActivity : Activity
    {
        private static String TAG = "NonConsumptionActivity";

        private IIapClient mClient;

        private ListView nonconsumableProductListview;
        private IList<ProductInfo> nonconsumableProducts = new List<ProductInfo>();
        private ProductListAdapter productListAdapter;
        private LinearLayout hasOwnedHiddenLevelLayout;
        //Change this with the product ID that has been created in AppGallery console
        private static  String HIDDEN_LEVEL_PRODUCTID = "ProductNonCons1";
        private bool isHiddenLevelPurchased = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_non_consumption);
            mClient = Iap.GetIapClient(this);
            InitView(); 
            QueryPurchases(null);

        }

        private void InitView()
        {
            FindViewById(Resource.Id.progressBar3).Visibility = ViewStates.Visible;
            FindViewById(Resource.Id.content_non).Visibility = ViewStates.Gone;
            nonconsumableProductListview = (ListView)  FindViewById(Resource.Id.nonconsumable_product_list);
            hasOwnedHiddenLevelLayout = (LinearLayout) FindViewById(Resource.Id.layout_hasOwnedHiddenLevel);
        }

        private void QueryPurchases(String continuationToken)
        {
            Log.Info(TAG, "QueryPurchase");

            OwnedPurchasesReq req = new OwnedPurchasesReq();
            req.PriceType = IapClientPriceType.InAppNonconsumable;
            req.ContinuationToken = continuationToken;

            Task task = mClient.ObtainOwnedPurchases(req).AddOnSuccessListener(new OwnedListenerImp(this)).AddOnFailureListener(new OwnedListenerImp(this));


        }

        private void CheckHiddenLevelPurchaseState(OwnedPurchasesResult result)
        {
            if (result == null || result.InAppPurchaseDataList == null)
            {
                Log.Info(TAG, "result is null");
                QueryProducts();
                return;
            }
            IList<String> inAppPurchaseDataList = result.InAppPurchaseDataList;
            for (int i = 0; i < inAppPurchaseDataList.Count(); i++)
            {
                    try
                    {
                        InAppPurchaseData inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseDataList.ElementAt(i));
                        if (inAppPurchaseDataBean.PurchaseStatus == InAppPurchaseData.PurchaseState.Purchased)
                        {
                            if (HIDDEN_LEVEL_PRODUCTID.Equals(inAppPurchaseDataBean.ProductId))
                            {
                                isHiddenLevelPurchased = true;
                            }
                        }

                    }
                    catch (JSONException e)
                    {
                        Log.Error(TAG, "delivery:" + e.Message);
                    }
               
            }
            if (isHiddenLevelPurchased)
            {
                DeliverProduct();
            }
            else
            {
                QueryProducts();
            }
        }

         private void DeliverProduct()
        {
            
            FindViewById(Resource.Id.progressBar3).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.content_non).Visibility = ViewStates.Visible;

            nonconsumableProductListview.Visibility = ViewStates.Gone;
            hasOwnedHiddenLevelLayout.Visibility = ViewStates.Visible;
        }

        private void QueryProducts()
        {
            Log.Info(TAG, "QueryProducts");
            List<String> productIds = new List<String>();
            productIds.Add(HIDDEN_LEVEL_PRODUCTID);
            ProductInfoReq req = new ProductInfoReq();
            req.PriceType = IapClientPriceType.InAppNonconsumable;
            req.ProductIds = productIds;
            
            Task task = mClient.ObtainProductInfo(req);
            task.AddOnSuccessListener(new QueryProductListenerImp(this)).AddOnFailureListener(new QueryProductListenerImp(this));
        }

        private void ShowProducts(IList<ProductInfo> products)
        {
            FindViewById(Resource.Id.progressBar3).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.content_non).Visibility = ViewStates.Visible;

            nonconsumableProductListview.Visibility = ViewStates.Visible;
            hasOwnedHiddenLevelLayout.Visibility = ViewStates.Gone;

            foreach (ProductInfo product in products)
            {
                nonconsumableProducts.Add(product);
            }

            productListAdapter = new ProductListAdapter(this, nonconsumableProducts);
            nonconsumableProductListview.Adapter=productListAdapter;
            productListAdapter.NotifyDataSetChanged();
            nonconsumableProductListview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                Buy(e.Position);
            };

        }

        private void Buy(int index)
        {
            Log.Info(TAG, "buy");
            PurchaseIntentReq req = new PurchaseIntentReq();
            ProductInfo productInfo = nonconsumableProducts.ElementAt(index);
            req.ProductId = productInfo.ProductId;
            req.PriceType = productInfo.PriceType;

            Task task =mClient.CreatePurchaseIntent(req);
            task.AddOnSuccessListener(new BuyListenerImp(this)).AddOnFailureListener(new BuyListenerImp(this));
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)

        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == Constants.REQ_CODE_BUY)
            {
                if (data == null)
                {
                    Log.Error(TAG, "data is null");
                    return;
                }

                PurchaseResultInfo purchaseIntentResult = mClient.ParsePurchaseResultInfoFromIntent(data);
                switch (purchaseIntentResult.ReturnCode)
                {
                    case OrderStatusCode.OrderStateCancel:
                        Toast.MakeText(this, "Order has been canceled!", ToastLength.Long).Show();
                        break;
                    case OrderStatusCode.OrderProductOwned:
                        QueryPurchases(null);
                        break;
                    case OrderStatusCode.OrderStateSuccess:
                        isHiddenLevelPurchased = true;
                        DeliverProduct();
                        break;
                    default:
                        break;
                }
                return;
            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            isHiddenLevelPurchased = false;
        }

        class BuyListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public NonConsumptionActivity CurrentActivity { get; private set; }
            public BuyListenerImp(NonConsumptionActivity activity)
            {
                this.CurrentActivity = activity;
            }


            public void OnSuccess(Java.Lang.Object result)
            {
                
                if (result == null)
                {
                    Log.Error(TAG, "result is null");
                    return;
                }
                PurchaseIntentResult inResult = (PurchaseIntentResult)result;
                if (inResult.Status == null)
                {
                    Log.Error(TAG, "status is null");
                    return;
                }
                inResult.Status.StartResolutionForResult(CurrentActivity, Constants.REQ_CODE_BUY);

            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(TAG, "Buy OnFailure");
                int errorCode = ExceptionHandle.handle(CurrentActivity, e);
                if (errorCode != ExceptionHandle.SOLVED)
                {
                    Log.Error(TAG, "createPurchaseIntent, returnCode: " + errorCode);
                    switch (errorCode)
                    {
                        case OrderStatusCode.OrderProductOwned:
                            CurrentActivity.QueryPurchases(null);
                            break;
                        default:
                            break;
                    }
                }
            }

        }


        class OwnedListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {

            public NonConsumptionActivity CurrentActivity { get; private set; }
            public OwnedListenerImp(NonConsumptionActivity activity)
            {
                this.CurrentActivity = activity;
            }
            public void OnSuccess(Java.Lang.Object result)
            {
                Log.Info(TAG, "obtainOwnedPurchases, success");
                OwnedPurchasesResult InResult = (OwnedPurchasesResult)result;
                CurrentActivity.CheckHiddenLevelPurchaseState(InResult);
                if (InResult != null && !TextUtils.IsEmpty(InResult.ContinuationToken))
                {
                    CurrentActivity.QueryPurchases(InResult.ContinuationToken);
                }
                

            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Error(TAG, "obtainOwnedPurchases, type=" + IapClientPriceType.InAppNonconsumable + ", " + e.Message);
                ExceptionHandle.handle(CurrentActivity, e);

            }
        }

 

        class QueryProductListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public NonConsumptionActivity CurrentActivity { get; private set; }
            public QueryProductListenerImp(NonConsumptionActivity activity)
            {
                this.CurrentActivity = activity;
            }


            public void OnSuccess(Java.Lang.Object result)
            {
                Log.Info(TAG, "obtainProductInfo, success");
                ProductInfoResult InResult = (ProductInfoResult)result;
                if (InResult == null || InResult.ProductInfoList == null)
                {
                    Toast.MakeText(CurrentActivity, "error", ToastLength.Long).Show();
                    return;
                }
                // to show product information
                CurrentActivity.ShowProducts(InResult.ProductInfoList);
               
                if (result == null)
                {
                    return;
                }


            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Error(TAG, "obtainProductInfo: " + e.Message);
                ExceptionHandle.handle(CurrentActivity, e);

            }
        }

    }
}