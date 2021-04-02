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
using Huawei.Hms.Kit.Awareness.Status;
using System;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities.Barrier
{
    [Activity(Label = "BarrierCombinationActivity")]
    public class BarrierCombinationActivity : Activity
    {
        #region Variables
        Logger log;

        private const string CombinedBehaviorHeadsetBarrierLabel = "behavior headset barrier label";
        private const string CombinedTimeBlueToothBarrierLabel = "time bluetooth barrier label";
        private PendingIntent mPendingIntent;
        private CombinedBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_combination_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}COMBINED_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new CombinedBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));
            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_combinedBarrier_behavior_headset).Click += BarrierCombinationBehaviorHeadset_Click;
            FindViewById(Resource.Id.add_combinedBarrier_time_bluetooth).Click += BarrierCombinationTimeBluetooth_Click;
            FindViewById(Resource.Id.delete_barrier).Click += BarrierCombinationDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void BarrierCombinationDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, mPendingIntent);
        }

        private void BarrierCombinationTimeBluetooth_Click(object sender, EventArgs e)
        {
            // When the Bluetooth car stereo is connected on a weekend, the barrier status is true.
            AwarenessBarrier combinedTimeBluetoothBarrier = AwarenessBarrier.And(
                    TimeBarrier.InTimeCategory(TimeBarrier.TimeCategoryWeekend),
                    BluetoothBarrier.Keep(BluetoothStatus.DeviceCar, BluetoothStatus.Connected));
            BarrierUtils.AddBarrier(this, CombinedTimeBlueToothBarrierLabel,
                    combinedTimeBluetoothBarrier, mPendingIntent);
        }

        private void BarrierCombinationBehaviorHeadset_Click(object sender, EventArgs e)
        {
            AwarenessBarrier combinedBehaviorHeadsetBarrier = AwarenessBarrier.And(
                        HeadsetBarrier.Keeping(HeadsetStatus.Connected),
                        AwarenessBarrier.Or(BehaviorBarrier.Keeping(BehaviorBarrier.BehaviorRunning)
                        , BehaviorBarrier.Keeping(BehaviorBarrier.BehaviorOnBicycle)));
            BarrierUtils.AddBarrier(this, CombinedBehaviorHeadsetBarrierLabel,
                    combinedBehaviorHeadsetBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class CombinedBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((BarrierCombinationActivity)context).log.ShowLog;
                switch (label)
                {
                    case CombinedBehaviorHeadsetBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The user is taking exercise and the headset is connected.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The user is not taking exercise or the headset is not connected.");
                        }
                        else
                        {
                            ShowLog("The combined barrier status is unknown.");
                        }
                        break;

                    case CombinedTimeBlueToothBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("Bluetooth car stereo is connected on a weekend.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("Bluetooth car stereo is not connected or the current day is not a weekend.");
                        }
                        else
                        {
                            ShowLog("The combined barrier status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}