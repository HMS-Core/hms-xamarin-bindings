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
    [Activity(Label = "RewardActivity")]
    public class RewardActivity : BaseActivity
    {
        private const int PlusScore = 1;
        private const int MinusScore = 5;
        private const int Range = 2;
        private const int DefaultScore = 10;
        private int score = 1;
        private const string TAG = "RewardActivity";

        private TextView rewardedTitle;
        private TextView scoreView;
        private Button playButton;
        private Button watchAdButton;
        private RewardAd rewardedAd;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = GetString(Resource.String.reward_ad);
            SetContentView(Resource.Layout.activity_reward);
            // Create your application here
            rewardedTitle = FindViewById<TextView>(Resource.Id.text_reward);
            rewardedTitle.Text = GetString(Resource.String.reward_ad_title);

            //Load a rewarded ad.
            LoadRewardAd();

            // Load a score view.
            LoadScoreView();

            // Load the button for watching a rewarded ad.
            LoadWatchButton();

            // Load the button for starting a game.
            LoadPlayButton();
        }
        /// <summary>
        /// Load the button for starting a game.
        /// </summary>
        private void LoadPlayButton()
        {
            playButton = FindViewById<Button>(Resource.Id.play_button);
            playButton.Click += PlayButton_Click;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            Play();
        }

        /// <summary>
        /// Load the button for watching a rewarded ad.
        /// </summary>
        private void LoadWatchButton()
        {
            watchAdButton = FindViewById<Button>(Resource.Id.show_video_button);
            watchAdButton.Click += WatchAdButton_Click;
        }

        private void WatchAdButton_Click(object sender, EventArgs e)
        {
            RewardAdShow();
        }

        private void LoadScoreView()
        {
            scoreView = FindViewById<TextView>(Resource.Id.score_count_text);
            scoreView.Text = "Score :" + score;
        }

        /// <summary>
        /// Load a rewarded ad.
        /// </summary>
        private void LoadRewardAd()
        {
            if (rewardedAd == null)
            {
                rewardedAd = new RewardAd(this, GetString(Resource.String.ad_id_reward));
            }
            rewardedAd.LoadAd(new AdParam.Builder().Build(), new RewardListener(this));

            Log.Info(TAG, "RewardAd.LoadAd function called successfully.");
        }
        /// <summary>
        /// Display a rewarded ad.
        /// </summary>
        private void RewardAdShow()
        {
            if (rewardedAd.IsLoaded)
            {
                rewardedAd.Show(this, new RewardStatusListener(this));
            }
        }


        /// <summary>
        /// Set a score.
        /// </summary>
        /// <param name="score"></param>
        private void SetScore(int score)
        {
            scoreView.Text = "Score: " + score;
        }

        private void Play()
        {
            // If the score is 0, a message is displayed, asking users to watch the ad in exchange for scores.
            if (score == 0)
            {
                Toast.MakeText(this, "Watch video ad to add score", ToastLength.Short).Show();
                return;
            }

            // The value 0 or 1 is returned randomly. If the value is 1, the score increases by 1. If the value is 0, the
            // score decreases by 5. If the score is a negative number, the score is set to 0.

            int random = new Random().Next(Range);
            if (random.Equals(1))
            {
                score += PlusScore;
                Toast.MakeText(this, "You win!", ToastLength.Short).Show();
            }
            else
            {
                score -= MinusScore;
                score = score < 0 ? 0 : score;
                Toast.MakeText(this, "You lose!", ToastLength.Short).Show();
            }
            SetScore(score);
        }



        private class RewardStatusListener : RewardAdStatusListener
        {
            const string TAG = "RewardAdStatusListener";
            readonly RewardActivity activity;
            public RewardStatusListener(RewardActivity rewardActivity)
            {
                activity = rewardActivity;
            }
            public override void OnRewardAdClosed()
            {
                Log.Info(TAG, "OnRewardAdClosed");
                activity.LoadRewardAd();

            }
            public override void OnRewardAdFailedToShow(int errorCode)
            {
                Log.Info(TAG, "OnRewardAdFailedToShow Error code is " + errorCode);
                Toast.MakeText(activity, "onRewardAdFailedToShow " + "errorCode is :" + errorCode, ToastLength.Short).Show();
            }
            public override void OnRewardAdOpened()
            {
                Log.Info(TAG, "OnRewardAdOpened");
                Toast.MakeText(activity, "onRewardAdOpened", ToastLength.Short).Show();
            }
            public override void OnRewarded(IReward reward)
            {
                Log.Info(TAG, "OnRewarded");
                // You are advised to grant a reward immediately and at the same time, check whether the reward
                // takes effect on the server. If no reward information is configured, grant a reward based on the
                // actual scenario.
                int addScore = reward.Amount == 0 ? DefaultScore : reward.Amount;
                Log.Info("RewardActivity", "IReward.Amount: " + reward.Amount);
                Log.Info("RewardActivity", "IReward.Name: " + reward.Name);

                Toast.MakeText(activity, "Watch video show finished , add " + addScore + " scores", ToastLength.Short).Show();
                activity.score += addScore;
                activity.SetScore(activity.score);
                activity.LoadRewardAd();
            }
        }

        private class RewardListener : RewardAdLoadListener
        {
            const string TAG = "RewardAdLoadListener";
            readonly RewardActivity activity;
            public RewardListener(RewardActivity rewardActivity)
            {
                activity = rewardActivity;
            }
            public override void OnRewardAdFailedToLoad(int errorCode)
            {
                Toast.MakeText(activity, "onRewardAdFailedToLoad " + "errorCode is :" + errorCode, ToastLength.Short).Show();
                Log.Info(TAG, "OnRewardAdFailedToLoad Error code is " + errorCode);
            }
            public override void OnRewardedLoaded()
            {
                Toast.MakeText(activity, "onRewardedLoaded", ToastLength.Short).Show();
                Log.Info(TAG, "OnRewardedLoaded");
            }
        }
    }
}