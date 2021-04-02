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
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Com.Huawei.Android.Hms.Ppskit;
using System.IO;


namespace XamarinAdsInstallReferrerDemo
{
    [Service(Exported = true, Name = "com.huawei.pps.hms.test.PpsChannelInfoService")]
    [IntentFilter(new string[] { "com.huawei.android.hms.CHANNEL_SERVICE" })]
    public class PpsChannelInfoService : Service
    {
        private static readonly string TAG = "PpsChannelInfoService";


        public override IBinder OnBind(Intent intent)
        {
            Log.Info(TAG, "OnBind");
            InfoServiceStub binder = new InfoServiceStub(this);
            return binder;
        }


        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate");
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Info(TAG, "OnStartCommand");
            return base.OnStartCommand(intent, flags, startId);
        }

        public override bool OnUnbind(Intent intent)
        {
            Log.Info(TAG, "OnUnbind");
            return base.OnUnbind(intent);

        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Info(TAG, "OnDestroy");
        }

        public static string GetCallerPkgSafe(PackageManager packageManager, int uid)
        {
            if (packageManager == null)
            {
                return "";
            }

            string pkg = "";
            try
            {
                pkg = packageManager.GetNameForUid(uid);
            }
            catch (RuntimeException rex)
            {
                Log.Warn(TAG, "Get name for uid error: " + rex.Message);
            }
            catch (Java.Lang.Exception ex)
            {
                Log.Warn(TAG, "Get name for uid error: " + ex.Message);
            }
            catch (Throwable e)
            {
                Log.Warn(TAG, "Get name for uid error: " + e.Message);
            }
            return pkg;
        }


        private class InfoServiceStub : IPPSChannelInfoServiceStub
        {
            PpsChannelInfoService ppsChannelInfoService;
            public InfoServiceStub(PpsChannelInfoService ppsChannelInfoService)
            {
                this.ppsChannelInfoService = ppsChannelInfoService;
            }
            public override string GetChannelInfo()
            {
                PackageManager packageManager = ppsChannelInfoService.PackageManager;
                string callerPkg = GetCallerPkgSafe(packageManager, Binder.CallingUid);
                Log.Info(TAG, "callerPkg= " + callerPkg);
                ISharedPreferences sharedPreferences = ppsChannelInfoService.GetSharedPreferences(Constants.InstallReferrerFile, FileCreationMode.Private);
                string installReferrer = sharedPreferences.GetString(callerPkg, "");
                Log.Info(TAG, "installReferrer= " + installReferrer);
                return installReferrer;
            }
        }
    }
}