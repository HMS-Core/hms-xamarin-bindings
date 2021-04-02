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
using Huawei.Hms.Ads.Nativead;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "NativeActivity")]
    public class NativeActivity : BaseActivity, NativeAd.INativeAdLoadedListener, IDislikeAdListener
    {
        private const string TAG = "NativeActivity";
        private RadioButton small;
        private RadioButton video;
        private Button loadBtn;
        private ScrollView adScrollView;

        private int layoutId;
        private NativeAd globalNativeAd;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = GetString(Resource.String.native_ad);
            SetContentView(Resource.Layout.activity_native);

            small = FindViewById<RadioButton>(Resource.Id.radio_button_small);
            video = FindViewById<RadioButton>(Resource.Id.radio_button_video);
            loadBtn = FindViewById<Button>(Resource.Id.btn_load);
            adScrollView = FindViewById<ScrollView>(Resource.Id.scroll_view_ad);

            loadBtn.Click += LoadBtn_Click;
            LoadAd(GetAdId());
        }

        protected override void OnDestroy()
        {
            if (null != globalNativeAd)
            {
                globalNativeAd.Destroy();
                Log.Info(TAG, "Destroy function called successfully");
            }
            base.OnDestroy();
        }
        /// <summary>
        /// Display native ad.
        /// </summary>
        /// <param name="nativeAd"></param>
        private void ShowNativeAd(NativeAd nativeAd)
        {
            // Destroy the original native ad.
            if (globalNativeAd != null)
            {
                globalNativeAd.Destroy();
            }
            globalNativeAd = nativeAd;

            // Obtain NativeView
            INativeView nativeView = (INativeView)LayoutInflater.Inflate(layoutId, null);

            // Register and populate a native ad material view.
            InitNativeView(globalNativeAd, nativeView);

            // Add NativeView to the app UI.

            adScrollView.RemoveAllViews();
            adScrollView.AddView((View)nativeView);
        }

        /// <summary>
        /// Register and populate a native ad material view.
        /// </summary>
        /// <param name="globalNativeAd"></param>
        /// <param name="nativeView"></param>
        private void InitNativeView(NativeAd nativeAd, INativeView nativeView)
        {
            View mNativeView = (View)nativeView;
            // Register a native ad material view.
            nativeView.TitleView = mNativeView.FindViewById(Resource.Id.ad_title);
            nativeView.MediaView = (MediaView)mNativeView.FindViewById(Resource.Id.ad_media);
            nativeView.AdSourceView = mNativeView.FindViewById(Resource.Id.ad_source);
            nativeView.CallToActionView = mNativeView.FindViewById(Resource.Id.ad_call_to_action);

            // Populate a native ad material view.
            ((TextView)nativeView.TitleView).Text = nativeAd.Title;

            nativeView.MediaView.SetMediaContent(nativeAd.MediaContent);

            if (nativeAd.AdSource != null)
            {
                ((TextView)nativeView.AdSourceView).Text = nativeAd.AdSource;
                Log.Info(TAG, "AdSource: " + nativeAd.AdSource);
            }
            nativeView.AdSourceView.Visibility = (nativeAd.AdSource != null ? ViewStates.Visible : ViewStates.Invisible);

            if (nativeAd.CallToAction != null)
            {
                ((Button)nativeView.CallToActionView).Text = nativeAd.CallToAction;
                Log.Info(TAG, "CallToAction: " + nativeAd.CallToAction);
            }
            nativeView.CallToActionView.Visibility = (nativeAd.CallToAction != null ? ViewStates.Visible : ViewStates.Invisible);

            // Obtain a video controller
            IVideoOperator videoOperator = nativeAd.VideoOperator;
            Log.Info(TAG, "VideoOperator: " + nativeAd.VideoOperator);
            // Check whether a native ad contains video materials.
            if (videoOperator.HasVideo)
            {
                // Add a video lifecycle event listener.
                videoOperator.VideoLifecycleListener = new VideoOperatorListener(this);
            }

            // Register a native ad object.
            nativeView.SetNativeAd(nativeAd);
            Log.Info(TAG, "NativeView.SetNativeAd called successfully");

        }

        /// <summary>
        /// Initialize ad slot ID and layout template.
        /// </summary>
        /// <returns>ad slot ID</returns>
        private string GetAdId()
        {
            string adId;
            layoutId = Resource.Layout.native_video_template;
            if (small.Checked)
            {
                adId = GetString(Resource.String.ad_id_native_small);
                layoutId = Resource.Layout.native_small_template;
            }
            else if (video.Checked)
            {
                adId = GetString(Resource.String.ad_id_native_video);
            }
            else
            {
                adId = GetString(Resource.String.ad_id_native);
            }
            return adId;
        }

        /// <summary>
        /// Load a native ad.
        /// </summary>
        /// <param name="adId"></param>
        private void LoadAd(string adId)
        {
            UpdateStatus(null, false);
            NativeAdLoader.Builder builder = new NativeAdLoader.Builder(this, adId);


            builder.SetNativeAdLoadedListener(this)
                .SetAdListener(new AdsListener(this));

            Log.Info(TAG, "SetNativeAdLoadedListener function called successfully.");
            Log.Info(TAG, "SetAdListener function called successfully.");


            NativeAdConfiguration adConfiguration = new NativeAdConfiguration.Builder()
                .SetChoicesPosition(NativeAdConfiguration.ChoicesPosition.BottomRight)// Set custom attributes.
                .Build();

            NativeAdLoader nativeAdLoader = builder.SetNativeAdOptions(adConfiguration)
                .Build();

            Log.Info(TAG, "SetNativeAdOptions function called successfully.");
            Log.Info(TAG, "Build function called successfully.");

            nativeAdLoader.LoadAd(new AdParam.Builder().Build());
            Log.Info(TAG, "NativeAdLoader.LoadAd called successfully.");
            Log.Info(TAG, "NativeAdLoader.IsLoading: " + nativeAdLoader.IsLoading);
            UpdateStatus(GetString(Resource.String.status_ad_loading), false);

        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            LoadAd(GetAdId());
        }

        /// <summary>
        /// Update tip and status of the load button.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="loadBtnEnabled"></param>
        private void UpdateStatus(string text, bool loadBtnEnabled)
        {
            if (text != null)
            {
                Toast.MakeText(this, text, ToastLength.Short).Show();
            }
            loadBtn.Enabled = loadBtnEnabled;
        }
        private class VideoOperatorListener : VideoOperatorVideoLifecycleListener
        {
            readonly NativeActivity nativeActivity;
            public VideoOperatorListener(NativeActivity nativeActivity)
            {
                this.nativeActivity = nativeActivity;
            }
            public override void OnVideoStart()
            {
                nativeActivity.UpdateStatus(nativeActivity.GetString(Resource.String.status_play_start), false);
            }
            public override void OnVideoPlay()
            {
                nativeActivity.UpdateStatus(nativeActivity.GetString(Resource.String.status_playing), false);
            }
            public override void OnVideoEnd()
            {
                // If there is a video, load a new native ad only after video playback is complete.
                nativeActivity.UpdateStatus(nativeActivity.GetString(Resource.String.status_play_end), true);
            }
        }
        protected class AdsListener : AdListener
        {
            const string TAG = "AdsListener";
            readonly NativeActivity nativeActivity;
            public AdsListener(NativeActivity nativeActivity)
            {
                this.nativeActivity = nativeActivity;
            }
            public override void OnAdClicked()
            {
                // Called when a user taps an ad.
                Log.Info(TAG, "OnAdClicked");
            }
            public override void OnAdClosed()
            {
                // Called when an ad is closed.
                Log.Info(TAG, "OnAdClosed");
            }
            public override void OnAdImpression()
            {
                Log.Info(TAG, "OnAdImpression");
            }
            public override void OnAdLeave()
            {
                // Called when a user has left the app.
                Log.Info(TAG, "OnAdLeave");
            }
            public override void OnAdOpened()
            {
                // Called when an ad is opened.
                Log.Info(TAG, "OnAdOpened");
            }
            public override void OnAdLoaded()
            {
                // Called when an ad is loaded successfully.
                Log.Info(TAG, "OnAdLoaded");

            }
            public override void OnAdFailed(int errorCode)
            {
                // Call this method when an ad fails to be loaded.
                nativeActivity.UpdateStatus(nativeActivity.GetString(Resource.String.status_load_ad_fail) + errorCode, true);
            }
        }
        public void OnNativeAdLoaded(NativeAd nativeAd)
        {
            Log.Info(TAG, "OnNativeAdLoaded");
            // Call this method when an ad is successfully loaded.
            UpdateStatus(GetString(Resource.String.status_load_ad_success), true);

            // Display native ad.
            ShowNativeAd(nativeAd);
            nativeAd.SetAllowCustomClick();
            Log.Info(TAG, "Called SetAllowCustomClick successfully.");
            nativeAd.SetDislikeAdListener(this);
        }

        public void OnAdDisliked()
        {
            // Call this method when an ad is closed.
            Log.Info("NativeActivity", "OnAdDisliked");
            UpdateStatus(GetString(Resource.String.ad_is_closed), true);
        }
    }
}