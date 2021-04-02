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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Iap;
using Huawei.Hms.Iap.Entity;
using Org.Json;

namespace XamarinHmsIAPDemo
{
    [Activity(Label = "SubscriptionActivity")]
    public class SubscriptionActivity : Activity , SubscriptionContract.View
    {
        private static  String TAG = "SubscriptionActivity";
        //populate this array with the product IDs that have been created in AppGallery console
        private static  String[] SUBSCRIPTION_PRODUCT = new String[]{ "ProductSub01", "ProductSub02", "ProductSub201", "ProductSub202" };
        private SubscriptionContract.Presenter presenter;

         protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_subscription);

            FindViewById(Resource.Id.progressBar4).Visibility=ViewStates.Visible;
            FindViewById(Resource.Id.content_sub).Visibility = ViewStates.Invisible;
            Button button = (Button)FindViewById(Resource.Id.manageSubscription);
            button.Click += (sender, e) => {
                ManageSubscription();
            };
            List<String> list = new List<string> ();
            list.Add("ProductSub01");
            list.Add("ProductSub02");
            list.Add("ProductSub201");
            list.Add("ProductSub202");

            presenter = new SubscriptionPresenter(this);
            presenter.Load(list);
            presenter.IsSandboxActivated();
        }

        public Activity GetActivity()
        {
            return this;
        }
        public void ManageSubscription()
        {
            presenter.ShowSubscription("");
        }

        public void ShowProducts(IList<ProductInfo> productInfos)
        {
            if (null == productInfos)
            {
                Toast.MakeText(this, "External Erro", ToastLength.Long).Show();
                return;
            }

            foreach (ProductInfo productInfo in productInfos)
            {
                ShowProduct(productInfo);
            }

            FindViewById(Resource.Id.progressBar4).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.content_sub).Visibility = ViewStates.Visible;
        }

        private void ShowProduct(ProductInfo productInfo)
        {
            View view = GetView(productInfo.ProductId);

            if (view != null)
            {
                TextView productName =(TextView) view.FindViewById(Resource.Id.product_name);
                TextView productDesc = (TextView)view.FindViewById(Resource.Id.product_desc);
                TextView price = (TextView)view.FindViewById(Resource.Id.price);

                productName.Text=productInfo.ProductName;
                productDesc.Text=productInfo.ProductDesc;
                price.Text=productInfo.Price;

            }
        }

        private View GetView(String productId)
        {
            View view = null;
            if (SUBSCRIPTION_PRODUCT[0].Equals(productId))
            {
                view = FindViewById(Resource.Id.service_one_product_one);
            }
            if (SUBSCRIPTION_PRODUCT[1].Equals(productId))
            {
                view = FindViewById(Resource.Id.service_one_product_two);
            }
            if (SUBSCRIPTION_PRODUCT[2].Equals(productId))
            {
                view = FindViewById(Resource.Id.service_two_product_one);
            }
            if (SUBSCRIPTION_PRODUCT[3].Equals(productId))
            {
                view = FindViewById(Resource.Id.service_two_product_two);
            }
            return view;
        }

        public void UpdateProductStatus(OwnedPurchasesResult ownedPurchasesResult)
        {
            foreach (String productId in SUBSCRIPTION_PRODUCT)
            {
                View view = GetView(productId);
                Button button = (Button)view.FindViewById(Resource.Id.action);
                button.Tag=productId;
                if (ShouldOfferService(ownedPurchasesResult, productId))
                {
                    button.Text=GetString(Resource.String.active);
                    button.Click += (sender, e) => {
                        presenter.ShowSubscription((sender as Button).Tag.ToString());
                    };
                    
                }
                else
                {
                    button.Text= GetString(Resource.String.buy);
                    button.Click += (sender, e) => {
                        presenter.Buy((sender as Button).Tag.ToString());
                    };
                }
            }
        }

        public bool ShouldOfferService(OwnedPurchasesResult result, String productId)
        {
            if (null == result)
            {
                Log.Error(TAG, "OwnedPurchasesResult is null");
                return false;
            }

            IList<String> inAppPurchaseDataList = result.InAppPurchaseDataList;
            foreach (String data in inAppPurchaseDataList)
            {
                try
                {
                    InAppPurchaseData inAppPurchaseData = new InAppPurchaseData(data);
                    if (productId.Equals(inAppPurchaseData.ProductId))
                    {                       
                            return inAppPurchaseData.IsSubValid;
                       
                    }
                }
                catch (JSONException e)
                {
                    Log.Error(TAG, "parse InAppPurchaseData JSONException", e);
                    return false;
                }
            }
            return false;
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

                PurchaseResultInfo purchaseIntentResult = Iap.GetIapClient(this).ParsePurchaseResultInfoFromIntent(data);
                switch (purchaseIntentResult.ReturnCode)
                {
                    case OrderStatusCode.OrderStateCancel:
                        Toast.MakeText(this, "Order has been canceled!", ToastLength.Long).Show();
                        break;
                    case OrderStatusCode.OrderStateFailed:
                        Toast.MakeText(this, GetString(Resource.String.pay_fail), ToastLength.Long).Show();
                        break;
                    case OrderStatusCode.OrderStateSuccess:
                        Toast.MakeText(this,GetString( Resource.String.pay_success), ToastLength.Long).Show();
                        presenter.RefreshSubscription();
                        break;
                    default:
                        break;
                }
                return;
            }

        }

    }
}