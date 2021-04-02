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
using Android;
using Android.App;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Huawei.Hms.Kit.Awareness;
using Huawei.Hms.Kit.Awareness.Base;
using Huawei.Hms.Kit.Awareness.Capture;
using Huawei.Hms.Kit.Awareness.Quickapp;
using Huawei.Hms.Kit.Awareness.Status;
using Huawei.Hms.Kit.Awareness.Status.Weather;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwarenessKitDemo.Activities;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Logger log;

        private static string[] mPermissionsOnHigherVersion = new string[]{Manifest.Permission.AccessFineLocation,
            "android.permission.ACTIVITY_RECOGNITION", "android.permission.ACCESS_BACKGROUND_LOCATION" };
        private static string[] mPermissionsOnLowerVersion = new string[]{Manifest.Permission.AccessFineLocation,
            "com.huawei.hms.permission.ACTIVITY_RECOGNITION"};

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitializeButtons();

            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            Awareness.GetBarrierClient(this).EnableUpdateWindow(true);
            QueryDeviceSupportingCapabilities();
            CheckAndRequestPermissions();
        }

        private void CheckAndRequestPermissions()
        {
            List<string> permissionsDoNotGrant = new List<string>();
            if (Build.VERSION.SdkInt >= (BuildVersionCodes)29)
            {
                foreach (string permission in mPermissionsOnHigherVersion)
                    if (ActivityCompat.CheckSelfPermission(this, permission) != Permission.Granted)
                        permissionsDoNotGrant.Add(permission);
            }
            else foreach (string permission in mPermissionsOnLowerVersion)
                    if (ActivityCompat.CheckSelfPermission(this, permission) != Permission.Granted)
                        permissionsDoNotGrant.Add(permission);

            if (permissionsDoNotGrant.Count > 0)
            {
                ActivityCompat.RequestPermissions(this, permissionsDoNotGrant.ToArray(), 940);
            }
        }

        private async void QueryDeviceSupportingCapabilities()
        {
            Task<CapabilityResponse> querySupportingCapabilities = Awareness.GetCaptureClient(this).QuerySupportingCapabilitiesAsync();
            try
            {
                await querySupportingCapabilities;
                if (querySupportingCapabilities.IsCompleted && querySupportingCapabilities.Result != null)
                {
                    ICapabilityStatus capabilityStatus = querySupportingCapabilities.Result.CapabilityStatus;
                    int[] capabilities = capabilityStatus.GetCapabilities();
                    Log.Info("QueryDeviceSupportingCapabilities", $"capabilities code : {string.Join(" ", capabilities)}");
                    string logMessage = "This device supports the following awareness capabilities:\n";
                    foreach (int capability in capabilities)
                    {
                        logMessage += Constant.GetCapabilityStatus(capability) + "\n";
                    }
                    log.ShowLog(logMessage);
                }
                else
                {
                    Log.Error("QueryDeviceSupportingCapabilities", "Failed to get supported capabilities." + querySupportingCapabilities.Exception);
                    log.ShowLog("Failed to get supported capabilities.");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error("QueryDeviceSupportingCapabilities", "Failed to get supported capabilities." + ex);
                log.ShowLog("Failed to get supported capabilities.");
            }

        }

        private void RequestPermissions()
        {
            if (Build.VERSION.SdkInt >= (BuildVersionCodes)29)
            {
                if (ContextCompat.CheckSelfPermission(this, mPermissionsOnHigherVersion[0]) != (int)Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, mPermissionsOnHigherVersion[1]) != (int)Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, mPermissionsOnHigherVersion[2]) != (int)Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this,
                        new string[]
                        {
                            mPermissionsOnHigherVersion[0],
                            mPermissionsOnHigherVersion[1],
                            mPermissionsOnHigherVersion[2]
                        },
                        100);
                }
            }
            else
            {
                if (ContextCompat.CheckSelfPermission(this, mPermissionsOnLowerVersion[0]) != (int)Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, mPermissionsOnLowerVersion[1]) != (int)Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this,
                        new string[]
                        {
                            mPermissionsOnLowerVersion[0],
                            mPermissionsOnLowerVersion[1]
                        },
                        100);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode != 100)
            {
                foreach (var item in permissions)
                {
                    if (ContextCompat.CheckSelfPermission(this, item) == Permission.Denied)
                    {
                        if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permissions[0]) || ActivityCompat.ShouldShowRequestPermissionRationale(this, permissions[1]))
                            Snackbar.Make(FindViewById<RelativeLayout>(Resource.Id.rl_main), "You need to grant permission to use awareness services.", Snackbar.LengthLong).SetAction("Ask again", v => RequestPermissions()).Show();
                        else
                            Toast.MakeText(this, "You need to grant location permissions in settings.", ToastLength.Long).Show();
                    }
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }

        }

        public void InitializeButtons()
        {
            FindViewById<Button>(Resource.Id.btn_capture_sample).Click += StartCaptureActivity;
            FindViewById<Button>(Resource.Id.btn_barrier_sample).Click += StartBarrierActivity;
        }

        private void StartBarrierActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(BarrierActivity));
        }

        private void StartCaptureActivity(object sender, EventArgs e)
        {
            StartActivity(typeof(CaptureActivity));
        }
    }
}
