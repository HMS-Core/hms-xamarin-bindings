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
using Android.Nfc;
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
    [Activity(Label = "Consumption Activity")]
    public class ConsumptionActivity : Activity
    {
        public Activity CurrentContext { get; private set; }

        private const String TAG = "ConsumptionActivity";
        private TextView countTextView;

        private ListView consumableProductsListview;
        private IList<ProductInfo> consumableProducts = new List<ProductInfo>();
        private ProductListAdapter adapter;
        private Button purchaseHisBtn;

        private IIapClient mClient;
        public ConsumptionActivity()
        {
            CurrentContext = this;
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_consumption);
            mClient = Iap.GetIapClient(this);
            InitView();
            QueryPurchases(null);

        }
        private void InitView()
        {
            Log.Info(TAG, "InitView");
            FindViewById(Resource.Id.progressBar1).Visibility = ViewStates.Visible;
            FindViewById(Resource.Id.content).Visibility = ViewStates.Gone;
            countTextView = (TextView)FindViewById(Resource.Id.gems_count);
            countTextView.Text = (DeliveryUtils.getCountOfGems(this)).ToString();
            consumableProductsListview = (ListView)FindViewById(Resource.Id.consumable_product_list1);
            consumableProductsListview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs e)
            {
                Buy(e.Position);
            };
            purchaseHisBtn = (Button)FindViewById(Resource.Id.enter_purchase_his);
            purchaseHisBtn.Click += delegate
            {
                Intent intent = new Intent(this, typeof(PurchaseHistoryActivity));
                StartActivity(intent);
            };
            QueryProducts();

        }

        private void QueryProducts()
        {
            Log.Info(TAG, "QueryProducts");
            List<String> productIdList = new List<String>();
            //Add the product ID that has been created in AppGallery console
            productIdList.Add("ProductCons1");

            ProductInfoReq req = new ProductInfoReq(); 
            req.PriceType = IapClientPriceType.InAppConsumable;
            req.ProductIds = productIdList;
            
            Task task = Iap.GetIapClient(this).ObtainProductInfo(req);
            task.AddOnSuccessListener(new QueryProductListenerImp(this)).AddOnFailureListener(new QueryProductListenerImp(this));
        }

        private void ShowProducts()
        {
            Log.Info(TAG, "ShowProducts");
            FindViewById(Resource.Id.progressBar1).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.content).Visibility = ViewStates.Visible;
            adapter = new ProductListAdapter(this, consumableProducts);
            consumableProductsListview.Adapter = adapter;
            adapter.NotifyDataSetChanged();
            
        }

        private void QueryPurchases(String continuationToken)
        {
            Log.Info(TAG, "QueryPurchase");

            OwnedPurchasesReq req = new OwnedPurchasesReq();
            req.PriceType = IapClientPriceType.InAppConsumable;
            req.ContinuationToken = continuationToken;

            Task task = mClient.ObtainOwnedPurchases(req).AddOnSuccessListener(new OwnedListenerImp(this)).AddOnFailureListener(new OwnedListenerImp(this));


        }

        private void Buy(int index)
        {
            Log.Info(TAG, "Buy");
            PurchaseIntentReq req = new PurchaseIntentReq();
            ProductInfo productInfo = consumableProducts.ElementAt(index);
            req.ProductId = productInfo.ProductId;
            req.PriceType = productInfo.PriceType;
            
            Task task = mClient.CreatePurchaseIntent(req);
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
                    case OrderStatusCode.OrderStateFailed:
                        break;
                    case OrderStatusCode.OrderProductOwned:
                        QueryPurchases(null);
                        break;
                    case OrderStatusCode.OrderStateSuccess:
                        DeliverProduct(purchaseIntentResult.InAppPurchaseData, purchaseIntentResult.InAppDataSignature);
                        break;
                    default:
                        break;
                }
                return;
            }

        }

        private void DeliverProduct(String inAppPurchaseDataStr, String inAppPurchaseDataSignature)
        {
            try
            {
                    InAppPurchaseData inAppPurchaseDataBean = new InAppPurchaseData(inAppPurchaseDataStr);
                    if (inAppPurchaseDataBean.PurchaseStatus!= InAppPurchaseData.PurchaseState.Purchased)
                    {
                        return;
                    }
                    String purchaseToken = inAppPurchaseDataBean.PurchaseToken;
                String productId = inAppPurchaseDataBean.ProductId;
                if (DeliveryUtils.isDelivered(this, purchaseToken))
                {
                    Toast.MakeText(this, productId + " has been delivered", ToastLength.Long).Show();
                    ConsumeOwnedPurchase(mClient, purchaseToken);
                }
                else
                {
                    if (DeliveryUtils.DeliverProduct(this, productId, purchaseToken))
                    {
                        Log.Info(TAG, "delivery success");
                        Toast.MakeText(this, productId + " delivery success", ToastLength.Long).Show();
                        UpdateNumberOfGems();
                        ConsumeOwnedPurchase(mClient, purchaseToken);
                    }
                    else
                    {
                        Log.Error(TAG, productId + " delivery fail");
                        Toast.MakeText(this, productId + " delivery fail", ToastLength.Long).Show();
                    }
                }

            }
            catch (JSONException e)
            {
                Log.Error(TAG, "delivery:" + e.Message);
            }
        }

        private void UpdateNumberOfGems()
        {
            String countOfGems = (DeliveryUtils.getCountOfGems(this)).ToString();
            countTextView.Text=countOfGems;
        }

        private  void ConsumeOwnedPurchase(IIapClient iapClient, String purchaseToken)
        {
            ConsumeOwnedPurchaseReq req = new ConsumeOwnedPurchaseReq();
            req.PurchaseToken=purchaseToken;

            Log.Info(TAG, "call consumeOwnedPurchase");
            Task task = iapClient.ConsumeOwnedPurchase(req);
            task.AddOnSuccessListener(new ConsumListenerImp()).AddOnFailureListener(new ConsumListenerImp());

    }

        class ConsumListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public void OnSuccess(Java.Lang.Object result)
            {
                Log.Info(TAG, "consumeOwnedPurchase success");

            }
            public void OnFailure(Java.Lang.Exception e)
            {
                if (e.GetType()==typeof(IapApiException))
                {
                    IapApiException apiException = (IapApiException)e;
                    int returnCode = apiException.Status.StatusCode;
                    Log.Error(TAG, "consumeOwnedPurchase fail, IapApiException returnCode: " + returnCode);
                }
                else
                {
                    Log.Error(TAG, e.Message);
                }
            }
        }

       

        class OwnedListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
                public ConsumptionActivity CurrentActivity { get; private set; }
                public OwnedListenerImp(ConsumptionActivity activity)
                {
                    this.CurrentActivity = activity;
                }
                public void OnSuccess(Java.Lang.Object result)
                {
                    if (result == null)
                    {
                        Log.Error(TAG," result is null");
                        return;
                    }
                    Log.Info(TAG, "obtainOwnedPurchases, success");
                    OwnedPurchasesResult InResult = (OwnedPurchasesResult)result;
                    if (InResult.InAppPurchaseDataList != null)
                    {
                        IList<String> inAppPurchaseDataList = InResult.InAppPurchaseDataList;
                        IList<String> inAppSignature = InResult.InAppSignature ;
                        for (int i = 0; i < inAppPurchaseDataList.Count; i++)
                        {
                            String inAppPurchaseData = inAppPurchaseDataList.ElementAt(i);
                            String inAppPurchaseDataSignature = inAppSignature.ElementAt(i);
                            CurrentActivity.DeliverProduct(inAppPurchaseData, inAppPurchaseDataSignature);
                        }
                    }
                    if (!TextUtils.IsEmpty(InResult.ContinuationToken))
                    {
                        CurrentActivity.QueryPurchases(InResult.ContinuationToken);
                    }

                }

                public void OnFailure(Java.Lang.Exception e)
                {
                    Log.Error(TAG, "obtainOwnedPurchases, type=" + IapClientPriceType.InAppConsumable + ", " + e.Message);
                    ExceptionHandle.handle(CurrentActivity, e);

                }
        }



        class BuyListenerImp : Java.Lang.Object, IOnSuccessListener , IOnFailureListener
        {
                public ConsumptionActivity CurrentActivity { get; private set; }
                public BuyListenerImp(ConsumptionActivity activity)
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


        class QueryProductListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
                public ConsumptionActivity CurrentActivity { get; private set; }
                public QueryProductListenerImp(ConsumptionActivity activity)
                {
                    this.CurrentActivity = activity;
                }


                public void OnSuccess(Java.Lang.Object result)
                {
                    Log.Info(TAG, "obtainProductInfo, success");
                    if (result == null)
                    {
                        return;
                    }

                    ProductInfoResult productlistwrapper = (ProductInfoResult)result;
                    IList<ProductInfo> productList = productlistwrapper.ProductInfoList;
                    Log.Info(TAG, productList.Count.ToString());
                    if (productList.Count != 0)
                    {
                        CurrentActivity.consumableProducts = productList;
                    }
                    CurrentActivity.ShowProducts();


                }

                public void OnFailure(Java.Lang.Exception e)
                {
                    Log.Error(TAG, "obtainProductInfo: " + e.Message);
                    ExceptionHandle.handle(CurrentActivity, e);
                    CurrentActivity.ShowProducts();
                }

        }


    }

}