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
    [Activity(Label = "BluetoothBarrierActivity")]
    public class BluetoothBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string KeepBarrierLabel = "bluetooth keep barrier label";
        private const string ConnectiongBarrierLabel = "bluetooth connecting barrier label";
        private const string DisconnectiongBarrierLabel = "bluetooth disconnecting barrier label";
        private PendingIntent mPendingIntent;
        private BluetoothBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_bluetooth_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}BLUETOOTH_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new BluetoothBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));
            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_bluetoothBarrier_keeping).Click += BluetoothBarrierKeeping_Click;
            FindViewById(Resource.Id.add_bluetoothBarrier_connecting).Click += BluetoothBarrierConnecting_Click;
            FindViewById(Resource.Id.add_bluetoothBarrier_disconnecting).Click += BluetoothBarrierDisconnecting_Click;
            FindViewById(Resource.Id.delete_barrier).Click += BluetoothBarrierDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void BluetoothBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, mPendingIntent);
        }

        private void BluetoothBarrierDisconnecting_Click(object sender, EventArgs e)
        {
            AwarenessBarrier disconnectingBarrier = BluetoothBarrier.Disconnecting(BluetoothStatus.DeviceCar);
            BarrierUtils.AddBarrier(this, DisconnectiongBarrierLabel, disconnectingBarrier, mPendingIntent);
        }

        private void BluetoothBarrierConnecting_Click(object sender, EventArgs e)
        {
            AwarenessBarrier connectingBarrier = BluetoothBarrier.Connecting(BluetoothStatus.DeviceCar);
            BarrierUtils.AddBarrier(this, ConnectiongBarrierLabel, connectingBarrier, mPendingIntent);
        }

        private void BluetoothBarrierKeeping_Click(object sender, EventArgs e)
        {
            AwarenessBarrier keepingBarrier = BluetoothBarrier.Keep(BluetoothStatus.DeviceCar, BluetoothStatus.Connected);
            BarrierUtils.AddBarrier(this, KeepBarrierLabel, keepingBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class BluetoothBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((BluetoothBarrierActivity)context).log.ShowLog;
                switch (label)
                {

                    case KeepBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The Bluetooth car stereo is connected.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The Bluetooth car stereo is disconnected.");
                        }
                        else
                        {
                            ShowLog("The Bluetooth car stereo status is unknown.");
                        }
                        break;

                    case ConnectiongBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The Bluetooth car stereo is connecting.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The Bluetooth car stereo is not connecting.");
                        }
                        else
                        {
                            ShowLog("The Bluetooth car stereo status is unknown.");
                        }
                        break;

                    case DisconnectiongBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The Bluetooth car stereo is disconnecting.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("The Bluetooth car stereo is not disconnecting.");
                        }
                        else
                        {
                            ShowLog("The Bluetooth car stereo status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}