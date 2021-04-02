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
using Android.Text;
using Android.Util;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Iap;
using Huawei.Hms.Iap.Entity;
using Java.Lang;

namespace XamarinHmsIAPDemo
{
    class SubscriptionPresenter : SubscriptionContract.Presenter
    {
        private static System.String TAG = "IapPresenter";

        private SubscriptionContract.View view;

        private static OwnedPurchasesResult cacheOwnedPurchasesResult;

        private IIapClient mClient;

        public SubscriptionPresenter(SubscriptionContract.View view)
        {
            SetView(view);
            mClient = Iap.GetIapClient(view.GetActivity());
        }
        public void Buy(string productId)
        {
            Log.Info(TAG, "buy");
            cacheOwnedPurchasesResult = null;
            PurchaseIntentReq req = new PurchaseIntentReq();
            req.ProductId = productId;
            req.PriceType = IapClientPriceType.InAppSubscription;
           

            Task task = mClient.CreatePurchaseIntent(req);
            task.AddOnSuccessListener(new BuyListenerImpl(this.view, this, productId)).AddOnFailureListener(new BuyListenerImpl(this.view,this,productId));
        }

        public void IsSandboxActivated()
        {

            IsSandboxActivatedReq req = new IsSandboxActivatedReq();
            Task task = mClient.IsSandboxActivated(req).AddOnSuccessListener(new SandboxListener(this.view)).AddOnFailureListener(new SandboxListener(this.view));
        }
        public void Load(List<string> productIds)
        {
            QueryProducts(productIds);
            RefreshSubscription();
        }

        private void QueryProducts(List<string> productIds)
        {
            Log.Info(TAG, "QueryProducts");
            ProductInfoReq req = new ProductInfoReq();
            req.PriceType = IapClientPriceType.InAppSubscription;
            req.ProductIds = productIds;

            Task task = mClient.ObtainProductInfo(req);
            task.AddOnSuccessListener(new QueryProductListenerImp(this.view)).AddOnFailureListener(new QueryProductListenerImp(this.view));
        }

        public void RefreshSubscription()
        {
            QuerySubscriptions(null);

        }

        public void SetView(SubscriptionContract.View view)
        {

            if (null == view)
            {
                throw new NullPointerException("can not set null view");
            }
            this.view= view;
        }

        public void ShowSubscription(string productId)
        {
            StartIapActivityReq req = new StartIapActivityReq();
            if (TextUtils.IsEmpty(productId))
            {
                req.Type= StartIapActivityReq.TypeSubscribeManagerActivity;
            }
            else
            {
                req.Type = StartIapActivityReq.TypeSubscribeEditActivity;
                req.SubscribeProductId=productId;
            }
            Task task = mClient.StartIapActivity(req).AddOnSuccessListener(new StartIAPListener(this.view)).AddOnFailureListener(new StartIAPListener(this.view));


    }

        private void QuerySubscriptions(System.String continuationToken)
        {

            Log.Info(TAG, "QuerySubscriptions");

            OwnedPurchasesReq req = new OwnedPurchasesReq();
            req.PriceType = IapClientPriceType.InAppSubscription;
            req.ContinuationToken = continuationToken;

            Task task = mClient.ObtainOwnedPurchases(req).AddOnSuccessListener(new OwnedListenerImp(this.view)).AddOnFailureListener(new OwnedListenerImp(this.view));

        }

        class QueryProductListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public SubscriptionContract.View view { get; private set; }
            public QueryProductListenerImp(SubscriptionContract.View view)
            {
                this.view = view;
            }


            public void OnSuccess(Java.Lang.Object result)

            {
                Log.Info(TAG, "obtainProductInfo, success");
                ProductInfoResult InResult = (ProductInfoResult)result;
                if (InResult == null || InResult.ProductInfoList == null)
                {
                    Toast.MakeText(view.GetActivity(), "error", ToastLength.Long).Show();
                    return;
                }
                IList<ProductInfo> productInfos = InResult.ProductInfoList;
                view.ShowProducts(productInfos);


            }

            public void OnFailure(Java.Lang.Exception e)
            {
                int errorCode = ExceptionHandle.handle(view.GetActivity(), e);
                if (ExceptionHandle.SOLVED != errorCode)
                {
                    Log.Error(TAG, "obtainProductInfo: " + e.Message);
                }
                view.ShowProducts(null);



            }
        }


        class OwnedListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {

            public SubscriptionContract.View view { get; private set; }
            
            public OwnedListenerImp(SubscriptionContract.View view)
            {
                this.view = view;
               
            }
            public void OnSuccess(Java.Lang.Object result)
            {
                Log.Info(TAG, "obtainOwnedPurchases, success");
                OwnedPurchasesResult InResult = (OwnedPurchasesResult)result;
                cacheOwnedPurchasesResult = InResult;
                view.UpdateProductStatus(cacheOwnedPurchasesResult);


        }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Error(TAG, "obtainOwnedPurchases, type=" + IapClientPriceType.InAppNonconsumable + ", " + e.Message);
                ExceptionHandle.handle(view.GetActivity(), e);


            }

        }

        class BuyListenerImpl : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public SubscriptionContract.View view { get; private set; }

            public SubscriptionContract.Presenter presenter { get; private set; }

            string Productid;
            public BuyListenerImpl(SubscriptionContract.View view, SubscriptionContract.Presenter presenter, string Productid)
            {
                this.view = view;
                this.presenter = presenter;
                this.Productid = Productid;
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
                inResult.Status.StartResolutionForResult(view.GetActivity(), Constants.REQ_CODE_BUY);

            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(TAG, "Buy OnFailure");
                int errorCode = ExceptionHandle.handle(view.GetActivity(), e);
                if (errorCode != ExceptionHandle.SOLVED)
                {
                    Log.Error(TAG, "createPurchaseIntent, returnCode: " + errorCode);
                    switch (errorCode)
                    {
                        case OrderStatusCode.OrderProductOwned:
                            presenter.ShowSubscription(Productid);
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        class StartIAPListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public SubscriptionContract.View view { get; private set; }
            public StartIAPListener(SubscriptionContract.View view)
            {
                this.view = view;
            }


            public void OnSuccess(Java.Lang.Object result)
            {

                if (result == null)
                {
                    Log.Error(TAG, "result is null");
                    return;
                }
                StartIapActivityResult inResult = (StartIapActivityResult)result;
                inResult.StartActivity(view.GetActivity());

            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(TAG, "Start OnFailure");
                ExceptionHandle.handle(view.GetActivity(), e);

            }
        }

        class SandboxListener : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
            public SubscriptionContract.View view { get; private set; }
            public SandboxListener(SubscriptionContract.View view)
            {
                this.view = view;
            }

          
            public void OnSuccess(Java.Lang.Object result)
            {

                if (result == null)
                {
                    Log.Error(TAG, "result is null");
                    return;
                }
                Log.Info(TAG, "SandBox success");
                Toast.MakeText(view.GetActivity(), "SandBox is Supported", ToastLength.Long).Show();
            }

            public void OnFailure(Java.Lang.Exception e)
            {
                Log.Info(TAG, "Sandbox");
                Toast.MakeText(view.GetActivity(), "SandBox is not Supported", ToastLength.Long).Show();
                ExceptionHandle.handle(view.GetActivity(), e);

            }
        }


    }
}