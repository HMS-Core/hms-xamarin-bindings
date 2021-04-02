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

namespace XamarinHmsIAPDemo
{
    [Activity(Label = "PurchaseHistoryActivity")]
    public class PurchaseHistoryActivity : Activity
    {
        private static String TAG = "PurchaseHistoryActivity";

        private ListView billListView;

        IList<String> billList = new List<String>();

        private BillListAdapter adapter;

        private static String continuationToken = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_purchase_history);
            billListView = (ListView)FindViewById(Resource.Id.bill_listview);
            FindViewById(Resource.Id.progressBar2).Visibility = ViewStates.Visible;
            FindViewById(Resource.Id.bill_listview).Visibility = ViewStates.Gone;
        }

        private void QueryHistoryInterface()
        {
            IIapClient iapClient = Iap.GetIapClient(this);
            OwnedPurchasesReq req = new OwnedPurchasesReq();
            req.PriceType= IapClientPriceType.InAppConsumable;
            req.ContinuationToken= continuationToken;
            iapClient.ObtainOwnedPurchaseRecord(req).AddOnSuccessListener(new ObtainListenerImp(this)).AddOnFailureListener(new ObtainListenerImp(this));

    }

        private void OnFinish()
        {
            FindViewById(Resource.Id.progressBar2).Visibility = ViewStates.Gone;
            FindViewById(Resource.Id.bill_listview).Visibility = ViewStates.Visible;
            Log.Info(TAG, "onFinish");
            adapter = new BillListAdapter(this, billList);
            billListView.Adapter= adapter;
            adapter.NotifyDataSetChanged();
        }

        protected override void OnResume()
        {
            base.OnResume();
            QueryHistoryInterface();
        }

        class ObtainListenerImp : Java.Lang.Object, IOnSuccessListener, IOnFailureListener
        {
        public PurchaseHistoryActivity CurrentActivity { get; private set; }
        public ObtainListenerImp(PurchaseHistoryActivity activity)
        {
            this.CurrentActivity = activity;
        }

        public void OnSuccess(Java.Lang.Object result)
        {
                Log.Info(TAG, "obtainOwnedPurchaseRecord, success");
                OwnedPurchasesResult InResult = (OwnedPurchasesResult)result;
                IList<String> inAppPurchaseDataList = InResult.InAppPurchaseDataList;
              
                if (inAppPurchaseDataList == null)
                {
                    CurrentActivity.OnFinish();
                    return;
                }
                for (int i = 0; i < inAppPurchaseDataList.Count(); i++)
                {

                    CurrentActivity.billList.Add(inAppPurchaseDataList.ElementAt(i));
                }
                continuationToken = InResult.ContinuationToken;
                if (!TextUtils.IsEmpty(continuationToken))
                {
                    CurrentActivity.QueryHistoryInterface();
                }
                else
                {
                    CurrentActivity.OnFinish();
                }
            }

        public void OnFailure(Java.Lang.Exception e)
            {
                Log.Error(TAG, "obtainOwnedPurchaseRecord, " + e.Message);
                ExceptionHandle.handle(CurrentActivity, e);
                CurrentActivity.OnFinish();
            }
        }

    }
}