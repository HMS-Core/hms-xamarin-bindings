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
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Kit.Awareness.Barrier;
using System;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities.Barrier
{
    [Activity(Label = "BehaviorBarrierActivity")]
    public class BehaviorBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string KeepingBarrierLabel = "behavior keeping barrier label";
        private const string BeginningBarrierLabel = "behavior beginning barrier label";
        private const string EndingBarrierLabel = "behavior ending barrier label";
        private PendingIntent mPendingIntent;
        private BehaviorBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_behavior_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}BEHAVIOR_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new BehaviorBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));
            InitializeButtons();
        }
        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_behaviorBarrier_keeping).Click += BehaviorBarrierKeeping_Click;
            FindViewById(Resource.Id.add_behaviorBarrier_beginning).Click += BehaviorBarrierBeginning_Click;
            FindViewById(Resource.Id.add_behaviorBarrier_ending).Click += BehaviorBarrierEnding_Click;
            FindViewById(Resource.Id.delete_barrier).Click += BehaviorBarrierDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void BehaviorBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, mPendingIntent);
        }

        private void BehaviorBarrierEnding_Click(object sender, EventArgs e)
        {
            AwarenessBarrier endCyclingBarrier = BehaviorBarrier.Ending(BehaviorBarrier.BehaviorOnBicycle);
            BarrierUtils.AddBarrier(this, EndingBarrierLabel, endCyclingBarrier, mPendingIntent);
        }

        private void BehaviorBarrierBeginning_Click(object sender, EventArgs e)
        {
            AwarenessBarrier beginWalkingBarrier = BehaviorBarrier.Beginning(BehaviorBarrier.BehaviorWalking);
            BarrierUtils.AddBarrier(this, BeginningBarrierLabel, beginWalkingBarrier, mPendingIntent);
        }

        private void BehaviorBarrierKeeping_Click(object sender, EventArgs e)
        {
            AwarenessBarrier keepStillBarrier = BehaviorBarrier.Keeping(BehaviorBarrier.BehaviorStill);
            BarrierUtils.AddBarrier(this, KeepingBarrierLabel, keepStillBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class BehaviorBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((BehaviorBarrierActivity)context).log.ShowLog;
                switch (label)
                {

                    case KeepingBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The user is still.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The user is not still.");
                        }
                        else
                        {
                            ShowLog("The user behavior status is unknown.");
                        }
                        break;

                    case BeginningBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The user begins to walk.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The beginning barrier status is false.");
                        }
                        else
                        {
                            ShowLog("The user behavior status is unknown.");
                        }
                        break;

                    case EndingBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The user stops cycling.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The ending barrier status is false.");
                        }
                        else
                        {
                            ShowLog("The user behavior status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}