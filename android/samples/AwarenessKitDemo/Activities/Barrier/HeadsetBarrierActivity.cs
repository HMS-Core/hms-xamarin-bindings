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
    [Activity(Label = "HeadsetBarrierActivity")]
    public class HeadsetBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string KeepingBarrierLabel = "keep barrier label";
        private const string ConnectingBarrierLabel = "connecting barrier label";
        private const string DisconnectingBarrierLabel = "disconnecting barrier label";
        private PendingIntent mPendingIntent;
        private HeadsetBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_headset_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}HEADSET_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new HeadsetBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));

            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_headsetBarrier_keeping).Click += HeadsetBarrierKeeping_Click;
            FindViewById(Resource.Id.add_headsetBarrier_connecting).Click += HeadsetBarrierConnecting_Click;
            FindViewById(Resource.Id.add_headsetBarrier_disconnecting).Click += HeadsetBarrierDisconnecting_Click;
            FindViewById(Resource.Id.delete_barrier).Click += HeadsetBarrierDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void HeadsetBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, KeepingBarrierLabel, ConnectingBarrierLabel, DisconnectingBarrierLabel);
        }

        private void HeadsetBarrierDisconnecting_Click(object sender, EventArgs e)
        {
            AwarenessBarrier disconnectingBarrier = HeadsetBarrier.Disconnecting();
            BarrierUtils.AddBarrier(this, DisconnectingBarrierLabel, disconnectingBarrier, mPendingIntent);
        }

        private void HeadsetBarrierConnecting_Click(object sender, EventArgs e)
        {
            // Create a headset barrier. When the headset are connected, the barrier status changes to true temporarily for about 5 seconds.
            // After 5 seconds, the status changes to false. If headset are disconnected within 5 seconds, the status also changes to false.
            AwarenessBarrier connectingBarrier = HeadsetBarrier.Connecting();
            BarrierUtils.AddBarrier(this, ConnectingBarrierLabel, connectingBarrier, mPendingIntent);
        }

        private void HeadsetBarrierKeeping_Click(object sender, EventArgs e)
        {
            AwarenessBarrier keepingBarrier = HeadsetBarrier.Keeping(HeadsetStatus.Connected);
            BarrierUtils.AddBarrier(this, KeepingBarrierLabel, keepingBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class HeadsetBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((HeadsetBarrierActivity)context).log.ShowLog;
                switch (label)
                {

                    case KeepingBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The headset is connected.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The headset is disconnected.");
                        }
                        else
                        {
                            ShowLog("The headset status is unknown.");
                        }
                        break;

                    case ConnectingBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The headset is connecting.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The headset is not connecting.");
                        }
                        else
                        {
                            ShowLog("The headset status is unknown.");
                        }
                        break;

                    case DisconnectingBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The headset is disconnecting.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The headset is not disconnecting.");
                        }
                        else
                        {
                            ShowLog("The headset status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}