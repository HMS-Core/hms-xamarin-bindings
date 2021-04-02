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
using Huawei.Hms.Ads;
using Huawei.Hms.Ads.Banner;
using Android.Util;
using System.Collections.Generic;
using System;
using Java.Interop;
using static Android.OS.Handler;
using Android.Content;
using XamarinAdsLiteDemo.Dialogs;
using XamarinAdsLiteDemo.Activities;
using Java.Lang;
using XamarinAdsLiteDemo.Adapters;
using Huawei.Hms.Ads.Consent.Bean;
using Huawei.Hms.Ads.Consent.Inter;
using Huawei.Hms.Ads.Consent.Constant;
using Android.Views;
using Android.Content.Res;
using XamarinAdsLiteDemo.Utils;
using System.Threading.Tasks;

namespace XamarinAdsLiteDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity,ProtocolDialog.IProtocolDialog, IConsentUpdateListener
    {
        private static readonly string TAG = "MainActivity";

        private const int ProtocolMsgType = 100;

        private const int ConsentMsgType = 200;

        private const int MsgDelayMs = 1000;

        private ListView listView;

        private List<AdFormat> adFormats = new List<AdFormat>();

        private Handler handler;

        private AdSampleAdapter adSampleAdapter;
        IList<AdProvider> adProviderList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitAdItems();

            listView = FindViewById<ListView>(Resource.Id.item_list_view);

            adSampleAdapter = new AdSampleAdapter(this, adFormats);
            listView.Adapter = adSampleAdapter;
            listView.ItemClick += ListView_ItemClick;


            handler = new Handler(new HandlerCallback(this));

            // Show the dialog for setting user privacy.
            SendMessage(ProtocolMsgType, MsgDelayMs);
        }


        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            AdFormat adFormat = adSampleAdapter.GetItem(e.Position);

            Intent intent = new Intent(this, adFormat.GetTargetClass());
            intent.AddFlags(ActivityFlags.ClearTop);
            StartActivity(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater menuInflater = MenuInflater;
            menuInflater.Inflate(Resource.Menu.ad_sample_menu, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.privacy_settings:
                    StartActivity(new Intent(this, typeof(ProtocolActivity)));
                    break;
                case Resource.Id.consent_settings:
                    StartActivity(new Intent(this, typeof(ConsentActivity)));
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
        }

        private int GetPreferences(string key, int defValue)
        {
            ISharedPreferences preferences = GetSharedPreferences(AdsConstant.SpName, FileCreationMode.Private);
            int value = preferences.GetInt(key, defValue);
            Log.Info(TAG, "Key:" + key + ", Preference value is: " + value);
            return value;
        }
        private void InitAdItems()
        {
            adFormats.Clear();
            adFormats.Add(new AdFormat(GetString(Resource.String.banner_ad), typeof(BannerActivity)));
            adFormats.Add(new AdFormat(GetString(Resource.String.native_ad), typeof(Activities.NativeActivity)));
            adFormats.Add(new AdFormat(GetString(Resource.String.reward_ad), typeof(RewardActivity)));
            adFormats.Add(new AdFormat(GetString(Resource.String.interstitial_ad), typeof(InterstitialActivity)));
            adFormats.Add(new AdFormat(GetString(Resource.String.instream_ad), typeof(InstreamActivity)));
        }
        public void ShowPrivacyDialog()
        {
            // If a user does not agree to the service agreement, the service agreement dialog is displayed.
            if (GetPreferences(AdsConstant.SpProtocolKey, AdsConstant.DefaultSpProtocolValue) == 0)
            {
                Log.Info(TAG, "Show protocol dialog.");
                ProtocolDialog protocolDialog = new ProtocolDialog(this);
                protocolDialog.SetCallback(this);
                protocolDialog.SetCanceledOnTouchOutside(false);
                protocolDialog.Show();
            }
            else
            {
                SendMessage(ConsentMsgType, MsgDelayMs);
            }
        }

        private void CheckConsentStatus()
        {
            adProviderList = new List<AdProvider>();
            Consent consentInfo = Consent.GetInstance(this);
            consentInfo.AddTestDeviceId("********");
            consentInfo.SetDebugNeedConsent(DebugNeedConsent.DebugNeedConsentField);
            consentInfo.RequestConsentUpdate(this);
        }
        private void SendMessage(int what, int delayMillis)
        {
            Message msg = Message.Obtain();
            msg.What = what;
            handler.SendMessageDelayed(msg, delayMillis);
        }
        public void Agree()
        {
            SendMessage(ConsentMsgType, MsgDelayMs);
        }

        public void Cancel()
        {
            // if the user selects the CANCEL button, exit application.
            Finish();
        }

        public void OnFail(string errorDescription)
        {
            Log.Error(TAG, "User's consent status failed to update: " + errorDescription);
            if (GetPreferences(AdsConstant.SpConsentKey, AdsConstant.DefaultSpConsentValue) < 0)
            {
                // In this example, if the request fails, the consent dialog box is still displayed. In this case, the ad publisher list is empty.
                ShowConsentDialog(adProviderList);
            }
        }

        public void OnSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
        {
            Log.Info(TAG, "ConsentStatus: " + consentStatus + " , isNeedConsent: " + isNeedConsent);
            if (isNeedConsent)
            {
                if (adProviders != null && adProviders.Count > 0)
                {
                    adProviderList = adProviders;
                }
                ShowConsentDialog(adProviderList);
            }
        }
        /// <summary>
        /// If a user has not set consent, the consent dialog box is displayed.
        /// </summary>
        /// <param name="adProviders"></param>
        private void ShowConsentDialog(IList<AdProvider> adProviders)
        {
            Log.Info(TAG, "Show consent dialog.");
            ConsentDialog dialog = new ConsentDialog(this, adProviders);
            dialog.SetCanceledOnTouchOutside(false);
            dialog.Show();
        }

        protected class HandlerCallback : Java.Lang.Object, ICallback
        {
            private MainActivity activity;
            public HandlerCallback(MainActivity mActivity)
            {
                this.activity = mActivity;
            }

            public bool HandleMessage(Message msg)
            {
                if (activity.HasWindowFocus)
                {
                    switch (msg.What)
                    {
                        case ProtocolMsgType:
                            activity.ShowPrivacyDialog();
                            break;

                        case ConsentMsgType:
                            activity.CheckConsentStatus();
                            break;
                        default:
                            break;
                    }
                }
                return false;
            }
        }
    }
}