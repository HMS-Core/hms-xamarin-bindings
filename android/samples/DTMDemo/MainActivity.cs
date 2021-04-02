/*
        Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Huawei.Hms.Analytics;
using Android.Views;
using Android.Util;
using Firebase.Analytics;
using Com.Appsflyer;
using System;
using Java.Interop;
using System.Collections.Generic;
using XamarinHmsDTMDemo.AppsFlyer;

namespace XamarinHmsDTMDemo
{
    [Activity(Name= "Com.XamarinHmsDTMDemo.MainActivity", Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, Android.Views.View.IOnClickListener
    {
        HiAnalyticsInstance instance;
        Button btnAppsFlyerTag,btnHiAnalyticsTag,btnFirebaseTag;
        private FirebaseAnalytics mFirebaseAnalytics;

        // Enter the AppsFlyer Dev key
        private const string AppsFlyer_Dev_Key = "YOUR_AppsFlyer_DEV_KEY";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Bind UI components.
            btnAppsFlyerTag = FindViewById<Button>(Resource.Id.btnAppsFlyerTag);
            btnHiAnalyticsTag = FindViewById<Button>(Resource.Id.btnHiAnalyticsTag);
            btnFirebaseTag = FindViewById<Button>(Resource.Id.btnFirebaseTag);

            // Set OnClickListener for Buttons.
            btnAppsFlyerTag.SetOnClickListener(this);
            btnHiAnalyticsTag.SetOnClickListener(this);
            btnFirebaseTag.SetOnClickListener(this);

            // Obtain the HiAnalytics instance.
            instance = HiAnalytics.GetInstance(this);
            HiAnalyticsTools.EnableLog();

            // Obtain the FirebaseAnalytics instance.
            mFirebaseAnalytics = FirebaseAnalytics.GetInstance(this);

            // Obtain the AppsFlyer instance.
            AppsFlyerLib.Instance.Init(AppsFlyer_Dev_Key, new AppsFlyerConversionDelegate(this), this.Application);           
            AppsFlyerLib.Instance.Start(this.Application, AppsFlyer_Dev_Key);
            AppsFlyerLib.Instance.SetDebugLog(true);

            // Find the classpaths for DTM console.
            string classpathCustomTag = Java.Lang.Class.FromType(typeof(Interfaces.CustomTag)).Name;
            Log.Debug("MainActivity", "ClassPathFor_CustomTag: "+ classpathCustomTag);
            string classpathCustomVariable = Java.Lang.Class.FromType(typeof(Interfaces.CustomDTMVariable)).Name;
            Log.Debug("MainActivity", "ClassPathFor_CustomVariable: " + classpathCustomVariable);
        }

        /// <summary>
        /// OnClickListener method.
        /// </summary>
        /// <param name="v">View</param>
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btnAppsFlyerTag:
                    CreateAppsFlyerTag();
                    break;
                case Resource.Id.btnHiAnalyticsTag:
                    CreateHiAnalyticsTag();
                    break;
                case Resource.Id.btnFirebaseTag:
                    CreateFirebaseTag();
                    break;
            }
        }

        /// <summary>
        /// Sent event for AppsFlyer.
        /// </summary>
        private void CreateAppsFlyerTag()
        {
            string eventName = "AppFlyers";

            Bundle bundle = new Bundle();
            bundle.PutString("appsflyer", "testappsflyer");

            //report
            if(instance != null)
            {
                instance.OnEvent(eventName, bundle);
                Log.Debug("AppsFlyerTag", "Sent event for AppsFlyer. Please check AppsFlyer dashboard. Event name:" + eventName);
                Toast.MakeText(this, "Sent event for AppsFlyer. Please check AppsFlyer dashboard. Event name:" + eventName, ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Sent event for HiAnalytics.
        /// </summary>
        private void CreateHiAnalyticsTag()
        {
            string eventName = "Purchase";

            Bundle bundle = new Bundle();
            bundle.PutString("price", "testcallfunction");
            //report
            if (instance != null)
            {
                instance.OnEvent(eventName, bundle);
                Log.Debug("HiAnalyticsTag", "OnEvent function called successfully. EventName:" + eventName);
                Toast.MakeText(this, "Sent event for HiAnalytics. Please check HiAnalytics dashboard. Event name:" + eventName, ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Sent event for Firebase.
        /// </summary>
        private void CreateFirebaseTag()
        {
            string eventName = "FirebaseEvent";

            Bundle bundle = new Bundle();
            bundle.PutString("quantitiy", "40");
            //report
            if(instance != null)
            {
                instance.OnEvent(eventName, bundle);
                Log.Debug("FirebaseTag", "Sent event for Firebase Event name:" + eventName);
                Toast.MakeText(this, "Sent event for Firebase. Please check Firebase dashboard. Event name:" + eventName, ToastLength.Short).Show();
            }
        }
    }
}