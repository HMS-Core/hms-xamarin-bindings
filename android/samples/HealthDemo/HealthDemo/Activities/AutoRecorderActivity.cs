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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Huawei.Hms.Hihealth;
using Android.Provider;
using Uri = Android.Net.Uri;
using Android.Util;
using Android.Text.Method;
using Huawei.Hms.Support.Hwid.Result;
using Huawei.Hms.Support.Hwid;
using Huawei.Hms.Hihealth.Data;
using System.Threading.Tasks;

namespace XamarinHmsHealthDemo
{
    [Activity(Label = "AutoRecorderActivity")]
    public class AutoRecorderActivity : Activity
    {
        private static string TAG = "AutoRecorderTest";

        private static string Split = "*******************************" + System.Environment.NewLine;

        // HMS Health AutoRecorderController
        private AutoRecorderController MyAutoRecorderController;

        // Text control that displays action information on the page
        private static TextView LogInfoView;

        private Intent intent;

        // Defining a Dynamic Broadcast Receiver
        private MyReceiver Receiver = null;

        // Record Start Times
        int Count = 0;

        // WakeLock
        private PowerManager.WakeLock wakeLock;

        private Button StartRecord;

        private Button StopRecord;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AutoRecorder_layout);
            LogInfoView = (TextView) FindViewById(Resource.Id.auto_recorder_log_info);
            LogInfoView.MovementMethod = ScrollingMovementMethod.Instance;

            InitData();

            PowerManager PM = (PowerManager) this.GetSystemService(PowerService);
            wakeLock = PM.NewWakeLock(WakeLockFlags.Partial, TAG);
            wakeLock.Acquire();
            Log.Info(TAG, " wakelock wakeLock.Acquire(); ");

            StartRecord = (Button)FindViewById(Resource.Id.Start_auto_recorder_button);
            StopRecord = (Button)FindViewById(Resource.Id.Stop_auto_recorder_button);

            StartRecord.Click += delegate { StartRecordByType(); };
            StopRecord.Click += delegate { StopRecordByType(); };
        }

        public void IgnoreBatteryOptimization(Activity activity)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                try
                {
                    PowerManager powerManager = (PowerManager)GetSystemService(PowerService);
                    bool HasIgnored = powerManager.IsIgnoringBatteryOptimizations(activity.PackageName);
                 
                     //Check whether the current app is added to the battery optimization trust list,
                     //If not, a dialog box is displayed for you to add a battery optimization trust list.
                    
                    if (!HasIgnored)
                    {
                        Intent intent = new Intent(Settings.ActionRequestIgnoreBatteryOptimizations);
                        intent.SetData(Uri.Parse("package:" + activity.PackageName));
                        StartActivity(intent);
                    }
                }
                catch (System.Exception e)
                {
                   Log.Error(TAG,e.Message);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            IgnoreBatteryOptimization(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            wakeLock.Release();
            wakeLock = null;
            Log.Info(TAG, " wakelock wakeLock.Release(); ");
        }

        private void InitData()
        {
            intent = new Intent();
            intent.SetPackage(this.PackageName);
            intent.SetAction("HealthKitService");
        }

        protected override void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
            AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
            MyAutoRecorderController = HuaweiHiHealth.GetAutoRecorderController(this, signInHuaweiId);
        }

        public void StartRecordByType()
        {
            if (Count < 1)
            {
                StartService(intent);
                // Registering a Broadcast Receiver
                Receiver = new MyReceiver();
                IntentFilter filter = new IntentFilter();
                filter.AddAction("HealthKitService");
                this.RegisterReceiver(Receiver, filter);
                Count++;
            }
            else
            {
                this.UnregisterReceiver(Receiver);
                Count--;
                StartService(intent);
                // Registering a Broadcast Receiver
                Receiver = new MyReceiver();
                IntentFilter filter = new IntentFilter();
                filter.AddAction("HealthKitService");
                this.RegisterReceiver(Receiver, filter);
                Count++;
            }
        }

        public async void StopRecordByType()
        {
            Logger("StopRecordByType");

            if (MyAutoRecorderController == null)
            {
                HiHealthOptions hiHealthOptions = HiHealthOptions.HiHealthOptionsBulider().Build();
                AuthHuaweiId signInHuaweiId = HuaweiIdAuthManager.GetExtendedAuthResult(hiHealthOptions);
                MyAutoRecorderController = HuaweiHiHealth.GetAutoRecorderController(this, signInHuaweiId);
            }

            Task StopTask = MyAutoRecorderController.StopRecordAsync(Huawei.Hms.Hihealth.Data.DataType.DtContinuousStepsTotal, new MySamplePointListener());
            try
            {
                await StopTask;

                if (StopTask.IsCompleted)
                {
                    if (StopTask.Exception == null)
                    {
                        Logger("StopRecordByType Successful");
                        Logger(Split);
                    }
                    else
                    {
                        Logger("StopRecordByType Failed: " + StopTask.Exception);
                        Logger(Split);
                    }
                }

            }
            catch (System.Exception ex)
            {
                Logger("StopRecordByType Failed: " + ex.Message);
                Logger(Split);
            }


            if (Count > 0)
            {
                StopService(intent);
                this.UnregisterReceiver(Receiver);
                Count--;
            }
        }
        
        // Returns the callback data in SamplePoint mode.
        // @param samplePoint Reported data

        public static void ShowSamplePoint(SamplePoint samplePoint)
        {
            if (samplePoint != null)
            {
                Logger("Sample point type: " + samplePoint.DataType.Name);
                foreach (Field field in samplePoint.DataType.Fields)
                {
                    Logger("Field: " + field.Name + " Value: " + samplePoint.GetFieldValue(field));
                    Logger(StampToData(DateTime.Now.ToString()));
                }
            }
            else
            {
                Logger("samplePoint is null!! ");
                Logger(Split);
            }
        }

        
        //Timestamp conversion function
        //@param timeStr Timestamp
        //@return Time in date format
        
        public static string StampToData(string timeStr)
        {
            string res;
            DateTime date = DateTime.Parse(timeStr);
            res = date.ToString("yyyy-MM-dd HH:mm:ss");
            return res;
        }

        public static void Logger(string Str)
        {
            Log.Info(TAG, Str);
            LogInfoView.Append(Str + System.Environment.NewLine);
            int offset = LogInfoView.LineCount * LogInfoView.LineHeight;
            if (offset > LogInfoView.Height)
            {
                LogInfoView.ScrollTo(0, offset - LogInfoView.Height);
            }
        }

        public class MyReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                Bundle bundle = intent.Extras;
                SamplePoint samplePoint = (SamplePoint) bundle.Get("SamplePoint");
                AutoRecorderActivity.ShowSamplePoint(samplePoint);
            }
        }
}

}