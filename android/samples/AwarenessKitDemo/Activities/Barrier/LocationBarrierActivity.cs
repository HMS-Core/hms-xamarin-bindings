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
using Huawei.Hms.Kit.Awareness.Capture;
using Huawei.Hms.Kit.Awareness.Status;
using System;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities.Barrier
{
    [Activity(Label = "LocationBarrierActivity")]
    public class LocationBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string EnterBarrierLabel = "enter barrier label";
        private const string StayBarrierLabel = "stay barrier label";
        private const string ExitBarrierLabel = "exit barrier label";
        private const long mTimeOfDuration = 1000L;
        private const double latitude = 40.9779846;
        private const double longitude = 29.0841426;
        private const double radius = 2000;
        private PendingIntent mPendingIntent;
        private LocationBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_location_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}LOCATION_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new LocationBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));

            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_locationBarrier_enter).Click += LocationBarrierEnter; ;
            FindViewById(Resource.Id.add_locationBarrier_stay).Click += LocationBarrierStay;
            FindViewById(Resource.Id.add_locationBarrier_exit).Click += LocationBarrierExit;
            FindViewById(Resource.Id.delete_barrier).Click += LocationBarrierDelete;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void LocationBarrierDelete(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, EnterBarrierLabel, StayBarrierLabel, ExitBarrierLabel);
        }

        private void LocationBarrierExit(object sender, EventArgs e)
        {
            AwarenessBarrier exitBarrier = LocationBarrier.Exit(latitude, longitude, radius);
            BarrierUtils.AddBarrier(this, ExitBarrierLabel, exitBarrier, mPendingIntent);
        }

        private void LocationBarrierStay(object sender, EventArgs e)
        {
            AwarenessBarrier stayBarrier = LocationBarrier.Stay(latitude, longitude, radius, mTimeOfDuration);
            BarrierUtils.AddBarrier(this, StayBarrierLabel, stayBarrier, mPendingIntent);
        }

        private void LocationBarrierEnter(object sender, EventArgs e)
        {
            AwarenessBarrier enterBarrier = LocationBarrier.Enter(latitude, longitude, radius);
            BarrierUtils.AddBarrier(this, EnterBarrierLabel, enterBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class LocationBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((LocationBarrierActivity)context).log.ShowLog;
                switch (label)
                {
                    case EnterBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("You are entering the area set by locationBarrier.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("You are not entering the area set by locationBarrier.");
                        }
                        else
                        {
                            ShowLog("The location status is unknown.");
                        }
                        break;

                    case StayBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog($"You have stayed in the area set by locationBarrier for {mTimeOfDuration}ms");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("You are not staying in the area set by locationBarrier or the time of duration is not enough.");
                        }
                        else
                        {
                            ShowLog("The location status is unknown.");
                        }
                        break;

                    case ExitBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("You are exiting the area set by locationBarrier.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("You are not exiting the area set by locationBarrier.");
                        }
                        else
                        {
                            ShowLog("The location status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}