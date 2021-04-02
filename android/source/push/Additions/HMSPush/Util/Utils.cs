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
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;
using Importance = Android.App.Importance;

namespace Huawei.Hms.Push.Utils
{
    public static class Utils
    {
        public static AlarmManager GetAlarmManager(Context context) => (AlarmManager)context.GetSystemService(Context.AlarmService);

        public static NotificationManager GetNotificationManager(Context context) => (NotificationManager)context.GetSystemService(Context.NotificationService);

        public static NotificationManagerCompat GetNotificationManager2(Context context) => NotificationManagerCompat.From(context);

        public static async System.Threading.Tasks.Task<Bitmap> GetBitmapFromUrlAsync(string url)
        {
            if (url == null) return null;
            using (WebClient webClient = new WebClient())
            {
                byte[] bytes = await webClient.DownloadDataTaskAsync(new Uri(url));
                if (bytes != null && bytes.Length > 0)
                {
                    return BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                }
            }
            return null;
        }

        public static bool IsApplicationInForeground(Context context)
        {
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            List<ActivityManager.RunningAppProcessInfo> processInfos = activityManager.RunningAppProcesses.ToList();

            if (processInfos == null) return false;

            foreach (ActivityManager.RunningAppProcessInfo processInfo in processInfos)
            {
                if (processInfo.PkgList.Count < 1) return false;
                if (processInfo.ProcessName.Equals(context.PackageName) && processInfo.Importance == Importance.Foreground)
                    return true;
            }
            return false;
        }

        public static void SendIntent(Context context, Bundle bundle)
        {
            string Tag = "Utils.SendIntent";
            string packageName = context.PackageName;
            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(packageName);
            if (launchIntent == null) return;

            try
            {
                string className = launchIntent.Component.ClassName;

                Class activityClass = Class.ForName(className);
                Intent activityIntent = new Intent(context, activityClass);

                if (bundle != null)
                {
                    activityIntent.PutExtra(NotificationConstants.Notification, bundle);
                }

                activityIntent.AddFlags(ActivityFlags.NewTask);
                activityIntent.AddFlags(ActivityFlags.NoUserAction);

                context.StartActivity(activityIntent);
            }
            catch (System.Exception e)
            {
                Log.Error(Tag, "Class not found", e);
            }
        }
    }
}