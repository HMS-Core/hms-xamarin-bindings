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
using System.Text;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities.Barrier
{
    [Activity(Label = "BeaconBarrierActivity")]
    public class BeaconBarrierActivity : Activity
    {
        #region Variables
        Logger log;

        const string DiscoverBarrierLabel = "discover beacon barrier label";
        const string KeepBarrierLabel = "keep beacon barrier label";
        const string MissedBarrierLabel = "missed beacon barrier label";
        const string ns = "sample namespace";
        const string type = "sample type";
        static byte[] content = Encoding.ASCII.GetBytes("sample");
        static BeaconStatusFilter filter = BeaconStatusFilter.Match(ns, type, content);
        PendingIntent mPendingIntent;
        BeaconBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_beacon_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}BEACON_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new BeaconBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));
            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_beaconBarrier_discover).Click += BeaconBarrierDiscover_Click; ;
            FindViewById(Resource.Id.add_beaconBarrier_keep).Click += BeaconBarrierKeep_Click;
            FindViewById(Resource.Id.add_beaconBarrier_missed).Click += BeaconBarrierMissed_Click; ;
            FindViewById(Resource.Id.delete_barrier).Click += BeaconBarrierDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void BeaconBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, mPendingIntent);
        }

        private void BeaconBarrierMissed_Click(object sender, EventArgs e)
        {
            AwarenessBarrier missedBeaconBarrier = BeaconBarrier.Missed(filter);
            BarrierUtils.AddBarrier(this, MissedBarrierLabel, missedBeaconBarrier, mPendingIntent);
        }

        private void BeaconBarrierKeep_Click(object sender, EventArgs e)
        {
            AwarenessBarrier keepBeaconBarrier = BeaconBarrier.Keep(filter);
            BarrierUtils.AddBarrier(this, KeepBarrierLabel, keepBeaconBarrier, mPendingIntent);
        }

        private void BeaconBarrierDiscover_Click(object sender, EventArgs e)
        {
            AwarenessBarrier discoverBeaconBarrier = BeaconBarrier.Discover(filter);
            BarrierUtils.AddBarrier(this, DiscoverBarrierLabel, discoverBeaconBarrier, mPendingIntent);
        }

        private class BeaconBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((BeaconBarrierActivity)context).log.ShowLog;
                switch (label)
                {
                    case DiscoverBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("A beacon matching the filters is found.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The discover beacon barrier status is false.");
                        }
                        else
                        {
                            ShowLog("The beacon status is unknown.");
                        }
                        break;

                    case KeepBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("A beacon matching the filters is found but not missed.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("No beacon matching the filters is found.");
                        }
                        else
                        {
                            ShowLog("The beacon status is unknown.");
                        }
                        break;

                    case MissedBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("A beacon matching the filters is missed.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The missed beacon barrier status is false.");
                        }
                        else
                        {
                            ShowLog("The beacon status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}