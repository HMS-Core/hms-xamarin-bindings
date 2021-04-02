﻿/*
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

using Android.Content;
using Android.OS;
using Android.Util;
using Huawei.Hms.Core;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace Huawei.Hms.Push.LocalNotification
{
    [BroadcastReceiver(Enabled = true)]
    public class HmsLocalNotificationScheduledPublisher : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.Extras;

            Log.Info("HmsLocalNotificationScheduledPublisher", bundle.ToJsonObject().ToString());

            ISharedPreferences sharedPreferences = context.GetSharedPreferences(CoreConstants.PreferenceName, FileCreationMode.Private);
            string id = bundle.GetInt(NotificationConstants.Id).ToString();
            if (sharedPreferences.GetString(id, null) != null)
            {
                ISharedPreferencesEditor editor = sharedPreferences.Edit();
                editor.Remove(bundle.GetInt(NotificationConstants.Id).ToString());
                editor.Apply();
                NotificationController.LocalNotificationNow(context, bundle);
            }

            
        }

    }


}