/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace XamarinAdsInstallReferrerDemo.InstallReferrer
{
    [Activity(Label = "InstallReferrerActivity")]
    public class InstallReferrerActivity : BaseActivity,InstallReferrerCallback
    {
        private static readonly string TAG = "InstallReferrerActivity";
        private TextView referrerTv;
        private TextView clickTimeTv;
        private TextView installTimeTv;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_install_referrer);
            Init();
        }

        protected void Init()
        {
            base.Init();
            referrerTv = FindViewById<TextView>(Resource.Id.install_referrer_tv);
            clickTimeTv = FindViewById<TextView>(Resource.Id.click_time_tv);
            installTimeTv = FindViewById<TextView>(Resource.Id.install_time_tv);

            ConnectThread connectThread = new ConnectThread(this);
            connectThread.Start();
        }
        /// <summary>
        /// Update install referrer from a UI thread.
        /// </summary>
        /// <param name="installReferrer"></param>
        /// <param name="clickTimestamp"></param>
        /// <param name="installTimestamp"></param>
        private void UpdateReferrerDetails(string installReferrer,long clickTimestamp,
            long installTimestamp)
        {
            if (TextUtils.IsEmpty(installReferrer))
            {
                Log.Warn(TAG, "installReferrer is empty");
                return;
            }
            Log.Info(TAG, "installReferrer: " + installReferrer +
                ", clickTimestamp: " + clickTimestamp +
                ", installTimestamp: " + installTimestamp);

            RunOnUiThread(new Runnable(this, installReferrer, clickTimestamp, installTimestamp));
          
        }

        protected class Runnable : Java.Lang.Object, IRunnable
        {
            InstallReferrerActivity installReferrerActivity;
            string installReferrer;
            long clickTimestamp;
            long installTimestamp;
            public Runnable(InstallReferrerActivity installReferrerActivity,string installReferrer,long clickTimestamp,long installTimestamp)
            {
                this.installReferrerActivity = installReferrerActivity;
                this.installReferrer = installReferrer;
                this.clickTimestamp = clickTimestamp;
                this.installTimestamp = installTimestamp;
            }
            public void Run()
            {
                installReferrerActivity.referrerTv.Text = installReferrer;
                installReferrerActivity.clickTimeTv.Text = clickTimestamp.ToString();
                installReferrerActivity.installTimeTv.Text = installTimestamp.ToString();
            }
        }

        /// <summary>
        /// Get install referrer from a non-UI thread.
        /// </summary>
        protected class ConnectThread : Thread
        {
            InstallReferrerActivity installReferrerActivity;
            public ConnectThread(InstallReferrerActivity installReferrerActivity)
            {
                this.installReferrerActivity = installReferrerActivity;
            }
            public override void Run()
            {

                installReferrerActivity.GetInstallReferrer();
            }
        }

        private void GetInstallReferrer()
        {
            int mode = GetIntExtra("mode", CallMode.Sdk);
            Log.Info(TAG, "InstallReferrer mode=" + mode);
            if (CallMode.Sdk == mode)
            {
                // Get install referrer by sdk mode.
                InstallReferrerSdkUtil sdkUtil = new InstallReferrerSdkUtil(this);
                sdkUtil.GetInstallReferrer(this);
            }else if(CallMode.Aidl == mode)
            {
                // Get install referrer by aidl mode.
                InstallReferrerAidUtil aidUtil = new InstallReferrerAidUtil(this);
                aidUtil.GetInstallReferrer(this);
            }
        }


        public void OnFail(string errMsg)
        {
            Log.Error(TAG, "OnFail: " + errMsg);
        }

        public void OnSuccess(string installReferrer, long clickTimeStamp, long installTimestamp)
        {
            Log.Info(TAG, "OnSuccess");
            UpdateReferrerDetails(installReferrer, clickTimeStamp, installTimestamp);
        }
    }
}