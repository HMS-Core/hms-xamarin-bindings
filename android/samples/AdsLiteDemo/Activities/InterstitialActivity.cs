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
using Huawei.Hms.Ads.Reward;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "InterstitialActivity")]
    public class InterstitialActivity : BaseActivity
    {
        private static readonly string TAG = "InterstitialActivity";
        private RadioGroup displayRadioGroup;
        private Button loadAdButton;
        private InterstitialAd interstitialAd;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = GetString(Resource.String.interstitial_ad);
            SetContentView(Resource.Layout.activity_interstitial);

            // Initialize the HUAWEI Ads SDK.
            HwAds.Init(this);
            Log.Info(TAG, "HwAds.Init called successfully.");
            Log.Info(TAG, "HwAds.RequestOptions: " + HwAds.RequestOptions);
            Log.Info(TAG, "HwAds.SDKVersion: " + HwAds.SDKVersion);

            displayRadioGroup = FindViewById<RadioGroup>(Resource.Id.display_radio_group);
            loadAdButton = FindViewById<Button>(Resource.Id.load_ad);
            loadAdButton.Click += LoadAdButton_Click;

        }

        private void LoadAdButton_Click(object sender, EventArgs e)
        {
            LoadInterstitialAd();
        }

        private void LoadInterstitialAd()
        {
            interstitialAd = new InterstitialAd(this);
            interstitialAd.AdId = GetAdId();
            Log.Info(TAG, "AdId has been successfully set.");
            interstitialAd.SetRewardAdListener(new RewardListener());
            Log.Info(TAG, "SetRewardAdListener has been successfully set.");
            interstitialAd.AdListener = new AdsListener(this);
            Log.Info(TAG, "AdListener has been successfully set.");

            Log.Info(TAG, "AdId has been successfully get. AdId: " + interstitialAd.AdId);
            Log.Info(TAG, "AdListener has been successfully get. AdId: " + interstitialAd.AdListener);
            Log.Info(TAG, "IsLoaded: " + interstitialAd.IsLoaded);
            Log.Info(TAG, "IsLoading: " + interstitialAd.IsLoading);


            AdParam adParam = new AdParam.Builder().Build();
            interstitialAd.LoadAd(adParam);
            Log.Info(TAG, "LoadAd function has been successfully set.");
        }

        private string GetAdId()
        {
            if (displayRadioGroup.CheckedRadioButtonId == Resource.Id.display_image)
            {
                return GetString(Resource.String.image_ad_id);
            }
            else
            {
                return GetString(Resource.String.video_ad_id);
            }
        }
        private void ShowInterstitial()
        {
            // Display an interstitial ad.
            if (interstitialAd != null && interstitialAd.IsLoaded)
            {
                interstitialAd.Show();
                Log.Info(TAG, "Show function has been successfully set.");
            }
            else
            {
                Toast.MakeText(this, "Ad did not load", ToastLength.Short).Show();
            }
        }

        private class RewardListener : Java.Lang.Object, IRewardAdListener
        {
            const string TAG = "IRewardAdListener";
            public void OnRewardAdClosed()
            {
                Log.Info(TAG, "OnRewardAdClosed");
            }

            public void OnRewardAdCompleted()
            {
                Log.Info(TAG, "OnRewardAdCompleted");
            }

            public void OnRewardAdFailedToLoad(int p0)
            {
                Log.Info(TAG, "OnRewardAdFailedToLoad Error code is: " + p0);
            }

            public void OnRewardAdLeftApp()
            {
                Log.Info(TAG, "OnRewardAdLeftApp");
            }

            public void OnRewardAdLoaded()
            {
                Log.Info(TAG, "OnRewardAdLoaded");
            }

            public void OnRewardAdOpened()
            {
                Log.Info(TAG, "OnRewardAdOpened");
            }

            public void OnRewardAdStarted()
            {
                Log.Info(TAG, "OnRewardAdStarted");
            }

            public void OnRewarded(IReward p0)
            {
                Log.Info(TAG, "OnRewarded IReward: " + p0);
            }
        }
        private class AdsListener : AdListener
        {
            readonly InterstitialActivity interstitialActivity;
            public AdsListener(InterstitialActivity interstitialActivity)
            {
                this.interstitialActivity = interstitialActivity;
            }
            public override void OnAdClicked()
            {
                Log.Info(TAG, "OnAdClicked");
                base.OnAdClicked();
            }

            public override void OnAdClosed()
            {
                Toast.MakeText(interstitialActivity, "Ad closed", ToastLength.Short).Show();
                Log.Info(TAG, "OnAdClosed");
                base.OnAdClosed();
            }

            public override void OnAdFailed(int errorCode)
            {
                Toast.MakeText(interstitialActivity, "Ad load failed with error code: " + errorCode, ToastLength.Short).Show();
                Log.Info(TAG, "Ad load failed with error code: " + errorCode);
                base.OnAdFailed(errorCode);
            }
            public override void OnAdLeave()
            {
                Log.Info(TAG, "OnAdLeave");
                base.OnAdLeave();
            }
            public override void OnAdLoaded()
            {
                base.OnAdLoaded();
                Toast.MakeText(interstitialActivity, "Ad loaded", ToastLength.Short).Show();
                Log.Info(TAG, "OnAdLoaded");

                // Display an interstitial ad.
                interstitialActivity.ShowInterstitial();
            }

            public override void OnAdOpened()
            {
                Log.Info(TAG, "OnAdOpened");
                base.OnAdOpened();
            }
        }
    }
}