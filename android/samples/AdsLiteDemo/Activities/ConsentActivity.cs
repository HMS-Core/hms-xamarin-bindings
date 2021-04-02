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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ads;
using Huawei.Hms.Ads.Banner;
using Huawei.Hms.Ads.Consent.Bean;
using Huawei.Hms.Ads.Consent.Constant;
using Huawei.Hms.Ads.Consent.Inter;
using XamarinAdsLiteDemo.Dialogs;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "ConsentActivity")]
    public class ConsentActivity : BaseActivity, ConsentDialog.IConsentDialog, IConsentUpdateListener
    {
        private const string TAG = "ConsentActivity";
        private BannerView adView;

        private TextView adTypeTv;

        private RequestOptions requestOptions;

        private IList<AdProvider> mAdProviders = new List<AdProvider>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = GetString(Resource.String.consent_settings);
            SetContentView(Resource.Layout.activity_consent);

            adTypeTv = FindViewById<TextView>(Resource.Id.ad_load_tip);
            adView = FindViewById<BannerView>(Resource.Id.consent_ad_view);

            //Check consent status
            CheckConsentStatus();

        }
        /// <summary>
        /// Check consent status.
        /// </summary>
        private void CheckConsentStatus()
        {
            Consent consentInfo = Consent.GetInstance(this);

            // To ensure that a dialog box is displayed each time you access the code demo, set ConsentStatus to UNKNOWN. In normal cases, the code does not need to be added.
            consentInfo.SetConsentStatus(ConsentStatus.Unknown);
            string testDeviceId = consentInfo.TestDeviceId;
            consentInfo.AddTestDeviceId(testDeviceId);

            // After DebugNeedConsent is set, ensure that the consent is required even if a device is not located in a specified area.
            consentInfo.SetDebugNeedConsent(DebugNeedConsent.DebugNeedConsentField);
            consentInfo.RequestConsentUpdate(this);

        }

        public void OnFail(string errorDescription)
        {
            Log.Error(TAG, "User's consent status failed to update: " + errorDescription);
            Toast.MakeText(ApplicationContext, "User's consent status failed to update: " + errorDescription,
                                ToastLength.Short).Show();

            // In this demo,if the request fails ,you can load a non-personalized ad by default.
            LoadBannerAd(ConsentStatus.NonPersonalized.Value);
        }

        public void OnSuccess(ConsentStatus consentStatus, bool isNeedConsent, IList<AdProvider> adProviders)
        {
            Log.Debug(TAG, "ConsentStatus: " + consentStatus + ", isNeedConsent: " + isNeedConsent);

            // The parameter indicating whether the consent is required is returned.
            if (isNeedConsent)
            {
                // If ConsentStatus is set to Unknown, re-collect user consent.
                if (consentStatus == ConsentStatus.Unknown)
                {
                    mAdProviders.Clear();
                    mAdProviders = adProviders;
                    ShowConsentDialog();

                }
                else
                {
                    // If ConsentStatus is set to Personalized or NonPersonalized, no dialog box is displayed to collect user consent.
                    LoadBannerAd(consentStatus.Value);
                }
            }
            else
            {
                // If a country does not require your app to collect user consent before displaying ads, your app can request a personalized ad directly.
                Log.Debug(TAG, "User is not need Consent");
                LoadBannerAd(ConsentStatus.Personalized.Value);
            }
        }

        private void LoadBannerAd(int value)
        {
            Log.Debug(TAG, "Load banner ad, consent success: " + value);
            if (value == ConsentStatus.Unknown.Value)
            {
                RemoveBannerAd();
            }

            // Obtain global ad singleton variables and add personalized ad request parameters.
            if (HwAds.RequestOptions == null)
            {
                requestOptions = new RequestOptions();
            }
            else
            {
                requestOptions = HwAds.RequestOptions;
            }

            // For non-personalized ads, reset this parameter.
            requestOptions = requestOptions.ToBuilder()
                .SetTagForUnderAgeOfPromise((Java.Lang.Integer)UnderAge.PromiseTrue)
                .SetNonPersonalizedAd((Java.Lang.Integer)value)
                .Build();

            HwAds.RequestOptions = requestOptions;
            AdParam adParam = new AdParam.Builder().Build();
            adView.AdId = GetString(Resource.String.banner_ad_id);
            adView.BannerAdSize = BannerAdSize.BannerSizeSmart;
            adView.AdListener = new AdsListener();
            adView.LoadAd(adParam);

            UpdateTextViewTips(value);
        }

        private void RemoveBannerAd()
        {
            if (adView != null)
            {
                adView.RemoveAllViews();
            }
            UpdateTextViewTips(ConsentStatus.Unknown.Value);
        }

        private void UpdateTextViewTips(int value)
        {
            if (ConsentStatus.NonPersonalized.Value == value)
            {
                adTypeTv.Text = GetString(Resource.String.load_non_personalized_text);
            }
            else if (ConsentStatus.Personalized.Value == value)
            {
                adTypeTv.Text = GetString(Resource.String.load_personalized_text);
            }
            else
            {
                adTypeTv.Text = GetString(Resource.String.no_ads_text);
            }
        }

        /// <summary>
        /// Display the consent dialog box.
        /// </summary>
        private void ShowConsentDialog()
        {
            // Start to process the consent dialog box.
            ConsentDialog consentDialog = new ConsentDialog(this, mAdProviders);
            consentDialog.SetCallback(this);
            consentDialog.SetCanceledOnTouchOutside(false);
            consentDialog.Show();
        }

        public void UpdateConsentStatus(ConsentStatus consentStatus)
        {
            LoadBannerAd(consentStatus.Value);
        }

        private class AdsListener : AdListener
        {
            public override void OnAdFailed(int errorCode)
            {
                // Called when an ad fails to be loaded.
                Log.Info(TAG, "Ad failed to load ERROR CODE:" + errorCode);
            }
            public override void OnAdLoaded()
            {
                // Called when an ad is loaded successfully.
                Log.Info(TAG, "Ad loaded successfully");
            }
        }
    }
}