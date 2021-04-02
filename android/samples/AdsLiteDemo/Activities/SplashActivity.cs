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
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ads;
using Huawei.Hms.Ads.Splash;
using static Android.OS.Handler;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        private static readonly String TAG = "SplashActivity";

        // Ad display timeout interval, in milliseconds.
        private static readonly int AdTimeout = 5000;

        // Ad display timeout message flag.
        private static readonly int MsgAdTimeout = 1001;
        /// <summary>
        /// Pause flag.
        /// On the splash ad screen:
        /// Set this parameter to true when exiting the app to ensure that the app home screen is not displayed.
        /// Set this parameter  ter to false when returning to the splash ad screen from another screen to ensure that the app home screen can be displayed properly.
        /// </summary>

        private bool hasPaused = false;

        // Callback handler used when the ad display timeout message is received.
        private Handler timeoutHandler;

        private SplashView splashView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_splash);
            timeoutHandler = new Handler(new HandlerCallback(this));
            // Create your application here
            LoadAd();
        }

        protected class SplashListener : SplashView.SplashAdLoadListener
        {
            readonly SplashActivity splashActivity;
            public SplashListener(SplashActivity splashActivity)
            {
                this.splashActivity = splashActivity;
            }
            public override void OnAdLoaded()
            {
                // Call this method when an ad is successfully loaded.
                Log.Info(TAG, "SplashAdLoadListener onAdLoaded.");
                Toast.MakeText(splashActivity, splashActivity.GetString(Resource.String.status_load_ad_success), ToastLength.Short).Show();
            }
            public override void OnAdDismissed()
            {
                // Call this method when the ad display is complete.
                Log.Info(TAG, "SplashAdLoadListener onAdDismissed.");
                Toast.MakeText(splashActivity, splashActivity.GetString(Resource.String.status_ad_dismissed), ToastLength.Short).Show();
                splashActivity.Jump();
            }
            public override void OnAdFailedToLoad(int errorCode)
            {
                // Call this method when an ad fails to be loaded.
                Log.Info(TAG, "SplashAdLoadListener onAdFailedToLoad, errorCode: " + errorCode);
                Toast.MakeText(splashActivity, splashActivity.GetString(Resource.String.status_load_ad_fail) + errorCode, ToastLength.Short).Show();
                splashActivity.Jump();
            }
        }
        private void LoadAd()
        {
            Log.Info(TAG, "Start to load ad");

            AdParam adParam = new AdParam.Builder().Build();


            splashView = FindViewById<SplashView>(Resource.Id.splash_ad_view);
            splashView.SetAdDisplayListener(new SplashAdDisplayListeners());
            Log.Info(TAG, "SplashView.SetAdDisplayListener function called successfully.");
            // Set a default app launch image.
            splashView.SetSloganResId(Resource.Drawable.default_slogan);
            Log.Info(TAG, "SplashView.SetSloganResId function called successfully.");
            splashView.SetWideSloganResId(Resource.Drawable.default_slogan);
            Log.Info(TAG, "SplashView.SetWideSloganResId function called successfully.");

            splashView.SetLogoResId(Resource.Mipmap.ic_launcher);
            Log.Info(TAG, "SplashView.SetLogoResId function called successfully.");
            // Set logo description.
            splashView.SetMediaNameResId(Resource.String.media_name);
            Log.Info(TAG, "SplashView.SetMediaNameResId function called successfully.");
            // Set the audio focus type for a video splash ad.
            splashView.SetAudioFocusType(AudioFocusType.NotGainAudioFocusWhenMute);
            Log.Info(TAG, "SplashView.SetAudioFocusType function called successfully.");
            SplashView.SplashAdLoadListener splashListener = new SplashListener(this);

            splashView.Load(GetString(Resource.String.ad_id_splash), (int)ScreenOrientation.Portrait, adParam, splashListener);
            Log.Info(TAG, "SplashView.Load function called successfully.");

            // Remove the timeout message from the message queue.
            timeoutHandler.RemoveMessages(MsgAdTimeout);
            // Send a delay message to ensure that the app home screen can be displayed when the ad display times out.
            timeoutHandler.SendEmptyMessageDelayed(MsgAdTimeout, AdTimeout);

        }

        /// <summary>
        /// Switch from the splash ad screen to the app home screen when the ad display is complete.
        /// </summary>
        private void Jump()
        {
            Log.Info(TAG, "jump hasPaused: " + hasPaused);
            if (!hasPaused)
            {
                hasPaused = true;
                Log.Info(TAG, "jump into application");

                StartActivity(new Intent(this, typeof(MainActivity)));

                Handler mainHandler = new Handler();
                mainHandler.PostDelayed(Finish, 1000);
            }
        }


        /// <summary>
        /// Set this parameter to true when exiting the app to ensure that the app home screen is not displayed.
        /// </summary>
        protected override void OnStop()
        {
            Log.Info(TAG, "SplashActivity OnStop.");
            // Remove the timeout message from the message queue.
            timeoutHandler.RemoveMessages(MsgAdTimeout);
            hasPaused = true;
            base.OnStop();
        }

        /// <summary>
        /// Call this method when returning to the splash ad screen from another screen to access the app home screen.
        /// </summary>
        protected override void OnRestart()
        {
            Log.Info(TAG, "SplashActivity OnRestart.");
            base.OnRestart();
            hasPaused = false;
            Jump();
        }


        protected override void OnDestroy()
        {
            Log.Info(TAG, "SplashActivity OnDestroy.");
            base.OnDestroy();
            if (splashView != null)
            {
                splashView.DestroyView();
                Log.Info(TAG, "SplashView.DestroyView function called successfully.");
            }
        }

        protected override void OnPause()
        {
            Log.Info(TAG, "SplashActivity OnPause.");
            base.OnPause();
            if (splashView != null)
            {
                splashView.PauseView();
                Log.Info(TAG, "SplashView.PauseView function called successfully.");
            }
        }

        protected override void OnResume()
        {
            Log.Info(TAG, "SplashActivity OnResume.");
            base.OnResume();
            if (splashView != null)
            {
                splashView.ResumeView();
                Log.Info(TAG, "SplashView.ResumeView function called successfully.");
            }
        }

        protected class SplashAdDisplayListeners : SplashAdDisplayListener
        {
            public override void OnAdShowed()
            {
                // Call this method when an ad is displayed.
                Log.Info(TAG, "SplashAdDisplayListener OnAdShowed.");
            }
            public override void OnAdClick()
            {
                // Call this method when an ad is clicked.
                Log.Info(TAG, "SplashAdDisplayListener OnAdClick.");
            }
        }
        protected class HandlerCallback : Java.Lang.Object, ICallback
        {
            private SplashActivity activity;
            public HandlerCallback(SplashActivity mActivity)
            {
                this.activity = mActivity;
            }

            public bool HandleMessage(Message msg)
            {
                if (activity.HasWindowFocus)
                {
                    activity.Jump();
                }
                return false;
            }
        }
    }
}