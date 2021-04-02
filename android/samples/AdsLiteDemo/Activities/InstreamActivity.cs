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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Ads;
using Huawei.Hms.Ads.Instreamad;
using Java.Util;
using static Huawei.Hms.Ads.Instreamad.InstreamView;

namespace XamarinAdsLiteDemo.Activities
{
    [Activity(Label = "InstreamActivity")]
    public class InstreamActivity : BaseActivity
    {
        private const string TAG = "InstreamActivity";

        private TextView videoContent;
        private TextView skipAd;
        private TextView countDown;
        private TextView callToAction;

        private Button loadButton;
        private Button registerButton;
        private Button muteButton;
        private Button pauseButton;

        private RelativeLayout instreamContainer;
        private InstreamView instreamView;
        private ImageView whyThisAd;

        private Context context;
        private long maxAdDuration;
        private string whyThisAdUrl;
        private bool isMuted = false;

        private InstreamAdLoader adLoader;
        private IList<InstreamAd> instreamAds = new List<InstreamAd>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            context = ApplicationContext;
            Title = GetString(Resource.String.instream_ad);
            SetContentView(Resource.Layout.activity_instream);

            InitInstreamAdViews();
            InitButtons();
            ConfigAdLoader();
        }
        private void InitInstreamAdViews()
        {
            instreamContainer = FindViewById<RelativeLayout>(Resource.Id.instream_ad_container);
            videoContent = FindViewById<TextView>(Resource.Id.instream_video_content);
            skipAd = FindViewById<TextView>(Resource.Id.instream_skip);
            skipAd.Click += SkipAd_Click;
            countDown = FindViewById<TextView>(Resource.Id.instream_count_down);
            callToAction = FindViewById<TextView>(Resource.Id.instream_call_to_action);
            whyThisAd = FindViewById<ImageView>(Resource.Id.instream_why_this_ad);

            instreamView = FindViewById<InstreamView>(Resource.Id.instream_view);
            instreamView.SetInstreamMediaChangeListener(new InstreamMediaChangeListener(this));
            instreamView.SetInstreamMediaStateListener(new InstreamMediaStateListener(this));
            instreamView.SetMediaMuteListener(new MediaMuteListener(this));
            instreamView.SetOnInstreamAdClickListener(new OnInStreamAdClickListener());
        }

        private void InitButtons()
        {
            loadButton = FindViewById<Button>(Resource.Id.instream_load);
            registerButton = FindViewById<Button>(Resource.Id.instream_register);
            muteButton = FindViewById<Button>(Resource.Id.instream_mute);
            pauseButton = FindViewById<Button>(Resource.Id.instream_pause_play);

            loadButton.Click += LoadButton_Click;
            registerButton.Click += RegisterButton_Click;
            muteButton.Click += MuteButton_Click;
            pauseButton.Click += PauseButton_Click;
        }

        private void ConfigAdLoader()
        {
            /*
            if the maximum total duration is 60 seconds and the maximum number of roll ads is eight,
            at most four 15-second roll ads or two 30-second roll ads will be returned.
            If the maximum total duration is 120 seconds and the maximum number of roll ads is four,
            no more roll ads will be returned after whichever is reached.
            */
            int totalDuration = 48;
            int maxCount = 4;
            InstreamAdLoader.Builder builder = new InstreamAdLoader.Builder(context, GetString(Resource.String.instream_ad_id));
            adLoader = builder.SetTotalDuration(totalDuration)
                .SetMaxCount(maxCount)
                .SetInstreamAdLoadListener(new InstreamAdLoadListener(this))
                .Build();
        }
        // play your normal video content.
        private void PlayVideo()
        {
            HideAdViews();
            videoContent.Text = GetString(Resource.String.instream_normal_video_playing);
        }
        private void HideAdViews()
        {
            instreamContainer.Visibility = ViewStates.Gone;
        }
        private void PlayInstreamAds(IList<InstreamAd> ads)
        {
            maxAdDuration = GetMaxInstreamDuration(ads);
            instreamContainer.Visibility = ViewStates.Visible;
            loadButton.Text = GetString(Resource.String.instream_load);
            instreamView.SetInstreamAds(ads);
        }
        private void UpdateCountDown(long playTime)
        {
            var time = Math.Round((decimal)(maxAdDuration - playTime) / 1000);
            RunOnUiThread(() => {
                countDown.Text = time + "s";
            });
        }

        private long GetMaxInstreamDuration(IList<InstreamAd> ads)
        {
            long duration = 0;
            foreach (var item in ads)
            {
                duration += item.Duration;
            }
            return duration;
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (instreamView.IsPlaying)
            {
                instreamView.Pause();
                pauseButton.Text = GetString(Resource.String.instream_play);
            }
            else
            {
                instreamView.Play();
                pauseButton.Text = GetString(Resource.String.instream_pause);
            }
        }

        private void MuteButton_Click(object sender, EventArgs e)
        {
            if (isMuted)
            {
                instreamView.Unmute();
                muteButton.Text = GetString(Resource.String.instream_mute);
            }
            else
            {
                instreamView.Mute();
                muteButton.Text = GetString(Resource.String.instream_unmute);
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if(instreamAds ==null || instreamAds.Count == 0)
            {
                PlayVideo();
            }
            else
            {
                PlayInstreamAds(instreamAds);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if(adLoader != null)
            {
                loadButton.Text = GetString(Resource.String.instream_loading);
                adLoader.LoadAd(new AdParam.Builder().Build());
            }
        }
        private void SkipAd_Click(object sender, EventArgs e)
        {
            if(instreamView != null)
            {
                instreamView.OnClose();
                instreamView.Destroy();
                instreamContainer.Visibility = ViewStates.Gone;
            }
        }
        private void ToastMessage(string message)
        {
            Toast.MakeText(this, message,ToastLength.Short).Show();
        }
        protected override void OnPause()
        {
            base.OnPause();
            if(instreamView != null && instreamView.IsPlaying)
            {
                instreamView.Pause();
                pauseButton.Text = GetString(Resource.String.instream_play);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (instreamView != null && !instreamView.IsPlaying)
            {
                instreamView.Play();
                pauseButton.Text = GetString(Resource.String.instream_pause);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(instreamView != null)
            {
                instreamView.RemoveInstreamMediaChangeListener();
                instreamView.RemoveInstreamMediaStateListener();
                instreamView.RemoveMediaMuteListener();
                instreamView.Destroy();
            }
        }
        public class InstreamMediaChangeListener : Java.Lang.Object, IInstreamMediaChangeListener
        {
            readonly InstreamActivity activity;
            public InstreamMediaChangeListener(InstreamActivity activity)
            {
                this.activity = activity;
            }
            public void OnSegmentMediaChange(InstreamAd instreamAd)
            {
                activity.whyThisAdUrl = null;
                activity.whyThisAdUrl = instreamAd.WhyThisAd;
                Log.Info(TAG, "OnSegmentMediaChange, whyThisAd: " + activity.whyThisAdUrl);
                activity.ToastMessage("OnSegmentMediaChange, whyThisAd: " + activity.whyThisAdUrl);
                if (!TextUtils.IsEmpty(activity.whyThisAdUrl))
                {
                    activity.whyThisAd.Visibility = ViewStates.Visible;
                    activity.whyThisAd.Click += WhyThisAd_Click;
                }
                else
                {
                    activity.whyThisAd.Visibility = ViewStates.Gone;
                }

                string callToAction = instreamAd.CallToAction;
                if (!TextUtils.IsEmpty(callToAction))
                {
                    activity.callToAction.Visibility = ViewStates.Visible;
                    activity.callToAction.Text = callToAction;
                    activity.instreamView.CallToActionView = activity.callToAction;
                }
            }

            private void WhyThisAd_Click(object sender, EventArgs e)
            {
                activity.StartActivity(new Intent(Intent.ActionView, Android.Net.Uri.Parse(activity.whyThisAdUrl)));
            }
        }
        public class InstreamMediaStateListener : Java.Lang.Object, IInstreamMediaStateListener
        {
            readonly InstreamActivity activity;
            public InstreamMediaStateListener(InstreamActivity activity)
            {
                this.activity = activity;
            }
            public void OnMediaCompletion(int playTime)
            {
                activity.UpdateCountDown(playTime);
                activity.PlayVideo();
            }

            public void OnMediaError(int playTime, int errorCode, int extra)
            {
                activity.UpdateCountDown(playTime);
            }

            public void OnMediaPause(int playTime)
            {
                activity.UpdateCountDown(playTime);
            }

            public void OnMediaProgress(int per, int playTime)
            {
                activity.UpdateCountDown(playTime);
            }

            public void OnMediaStart(int playTime)
            {
                activity.UpdateCountDown(playTime);
            }

            public void OnMediaStop(int playTime)
            {
                activity.UpdateCountDown(playTime);
            }
        }

        public class MediaMuteListener : Java.Lang.Object, IMediaMuteListener
        {
            readonly InstreamActivity activity;
            public MediaMuteListener(InstreamActivity activity)
            {
                this.activity = activity;
            }
            public void OnMute()
            {
                activity.isMuted = true;
                Log.Info(TAG, "Ad muted.");
                activity.ToastMessage("Ad muted.");
            }

            public void OnUnmute()
            {
                activity.isMuted = false;
                Log.Info(TAG, "Ad unmuted.");
                activity.ToastMessage("Ad unmuted.");
            }
        }

        public class OnInStreamAdClickListener : Java.Lang.Object, IOnInstreamAdClickListener
        {
            public void OnClick()
            {
                Log.Info(TAG, "Instream ad clicked.");
            }
        }

        public class InstreamAdLoadListener : Java.Lang.Object, IInstreamAdLoadListener
        {
            readonly InstreamActivity instreamActivity;
            public InstreamAdLoadListener(InstreamActivity instreamActivity)
            {
                this.instreamActivity = instreamActivity;
            }
            public void OnAdFailed(int errorCode)
            {
                Log.Warn(TAG, "OnAdFailed: " + errorCode);
                instreamActivity.ToastMessage("OnAdFailed: " + errorCode);
                instreamActivity.loadButton.Text = instreamActivity.GetString(Resource.String.instream_load);
                instreamActivity.PlayVideo();
            }

            public void OnAdLoaded(IList<InstreamAd> ads)
            {
                if (ads == null || ads.Count == 0)
                {
                    instreamActivity.PlayVideo();
                    return;
                }
                instreamActivity.loadButton.Text = instreamActivity.GetString(Resource.String.instream_loaded);
                instreamActivity.instreamAds = ads;
                Log.Debug(TAG, "OnAdLoaded, ad count: " + ads.Count + ", click REGISTER to play.");
                instreamActivity.ToastMessage("OnAdLoaded, ad count: " + ads.Count + ", click REGISTER to play.");
            }
        }
    }
}