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
using Java.Lang;

namespace XamarinAdsOAIDDemo.Oaid
{
    [Activity(Label = "OaidActivity")]
    public class OaidActivity : BaseActivity, OaidCallback
    {
        private static readonly string TAG = "OaidActivity";
        private TextView adIdTv;
        private TextView disableAdIdTv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ad_id);
            InitView();
        }

        protected void InitView()
        {
            Init();
            //Define xml elements.
            adIdTv = FindViewById<TextView>(Resource.Id.ad_id_tv);
            disableAdIdTv = FindViewById<TextView>(Resource.Id.disable_ad_id_tv);
            GetIdentifierThread getIdentifierThread = new GetIdentifierThread(this);
            getIdentifierThread.Start();
        }

        
        public void OnFail(string errMsg)
        {
            Log.Error(TAG, "Oaid Fail: " + errMsg);
        }

        public void OnSuccess(string oaid, bool isOaidTrackLimited)
        {
            Log.Info(TAG, "oiad=" + oaid + ", isLimitAdTrackingEnabled=" + isOaidTrackLimited);
            UpdateAdIdInfo(oaid, isOaidTrackLimited);
        }
            
       

        private void UpdateAdIdInfo(string oaid,bool isLimitAdTrackingEnabled)
        {
            RunOnUiThread(new GetRunnable(this, oaid, isLimitAdTrackingEnabled));
        }
        private void GetOaid()
        {
            //  Get OAID by sdk mode.
            OaidSdkUtil.GetOaid(this, this);
        }
        private void SetVerifyId(string adId)
        {
          OaidSdkUtil.SetVerifyID(this, adId);
        }
        private class GetIdentifierThread : Thread
        {
            OaidActivity oaidActivity;
            public GetIdentifierThread(OaidActivity oaidActivity)
            {
                this.oaidActivity = oaidActivity;
            }
            public override void Run()
            {
                oaidActivity.GetOaid();
            }
        }

        private class GetRunnable : Java.Lang.Object, IRunnable
        {
            OaidActivity oaidActivity;
            string oaid;
            bool isLimitAdTrackingEnabled;
            public GetRunnable(OaidActivity oaidActivity, string oaid, bool isLimitAdTrackingEnabled)
            {
                this.oaid = oaid;
                this.oaidActivity = oaidActivity;
                this.isLimitAdTrackingEnabled = isLimitAdTrackingEnabled;
            }
            public void Run()
            {
                if (!TextUtils.IsEmpty(oaid))
                {

                    oaidActivity.adIdTv.Text = oaid;
                    oaidActivity.SetVerifyId(oaid);
                }
                oaidActivity.disableAdIdTv.Text = isLimitAdTrackingEnabled.ToString();
            }
        }
    }
}