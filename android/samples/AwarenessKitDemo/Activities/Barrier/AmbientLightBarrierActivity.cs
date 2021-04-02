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
    [Activity(Label = "AmbientLightBarrierActivity")]
    public class AmbientLightBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string RangeBarrierLabel = "range barrier label";
        private const string AboveBarrierLabel = "above barrier label";
        private const string BelowBarrierLabel = "below barrier label";
        private const float LowLuxValue = 10.0f;
        private const float HighLuxValue = 2500.0f;
        private PendingIntent mPendingIntent;
        private AmbientLightBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_ambient_light_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}LIGHT_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new AmbientLightBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));
            InitializeButtons();
        }
        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_ambientLightBarrier_range).Click += AmbientLightBarrierRange_Click; ; ;
            FindViewById(Resource.Id.add_ambientLightBarrier_above).Click += AmbientLightBarrierAbove_Click; ;
            FindViewById(Resource.Id.add_ambientLightBarrier_below).Click += AmbientLightBarrierBelow_Click; ;
            FindViewById(Resource.Id.delete_barrier).Click += AmbientLightBarrierDelete_Click; ;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void AmbientLightBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, RangeBarrierLabel, AboveBarrierLabel, BelowBarrierLabel);
        }

        private void AmbientLightBarrierBelow_Click(object sender, EventArgs e)
        {
            AwarenessBarrier lightBelowBarrier = AmbientLightBarrier.Below(LowLuxValue);
            BarrierUtils.AddBarrier(this, BelowBarrierLabel, lightBelowBarrier, mPendingIntent);
        }

        private void AmbientLightBarrierAbove_Click(object sender, EventArgs e)
        {
            AwarenessBarrier lightAboveBarrier = AmbientLightBarrier.Above(HighLuxValue);
            BarrierUtils.AddBarrier(this, AboveBarrierLabel, lightAboveBarrier, mPendingIntent);
        }

        private void AmbientLightBarrierRange_Click(object sender, EventArgs e)
        {
            AwarenessBarrier lightRangeBarrier = AmbientLightBarrier.Range(LowLuxValue, HighLuxValue);
            BarrierUtils.AddBarrier(this, RangeBarrierLabel, lightRangeBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class AmbientLightBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((AmbientLightBarrierActivity)context).log.ShowLog;
                switch (label)
                {

                    case RangeBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog($"Ambient light intensity is in the range of {LowLuxValue} to {HighLuxValue} lux");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog($"Ambient light intensity is not in the range of {LowLuxValue} to {HighLuxValue} lux");
                        }
                        else
                        {
                            ShowLog("Ambient light status is unknown.");
                        }
                        break;

                    case AboveBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog($"Ambient light intensity is above {HighLuxValue} lux");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog($"Ambient light intensity is not above {HighLuxValue} lux");
                        }
                        else
                        {
                            ShowLog("Ambient light status is unknown.");
                        }
                        break;

                    case BelowBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog($"Ambient light intensity is below {LowLuxValue} lux");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog($"Ambient light intensity is not below {LowLuxValue} lux");
                        }
                        else
                        {
                            ShowLog("Ambient light status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}