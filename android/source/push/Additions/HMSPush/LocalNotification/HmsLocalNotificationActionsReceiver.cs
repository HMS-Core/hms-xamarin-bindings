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
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace Huawei.Hms.Push.LocalNotification
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class HmsLocalNotificationActionsReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {

            string intentActionPrefix = context.PackageName + ".ACTION_";

            if (intent.Action == null || !intent.Action.StartsWith(intentActionPrefix))
                return;

            Bundle bundle = intent.GetBundleExtra(NotificationConstants.Notification);
            if (bundle == null) return;


            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            if (notificationManager == null) return;

            int id = bundle.GetInt(NotificationConstants.Id);


            if (bundle.GetBoolean(NotificationConstants.AutoCancel, true))
            {
                if (bundle.ContainsKey(NotificationConstants.Tag))
                {
                    string tag = bundle.GetString(NotificationConstants.Tag);
                    notificationManager.Cancel(tag, id);
                }
                else
                {
                    notificationManager.Cancel(id);
                }
            }
            if (bundle.GetBoolean(NotificationConstants.InvokeApp, true))
                NotificationController.InvokeApp(context, bundle);
            else
            {
                string Action = "com.huawei.HMSActionService";
                Intent sendIntent = new Intent(Action);
                sendIntent.SetPackage(context.PackageName);
                sendIntent.PutExtra(NotificationConstants.Notification,bundle);
                context.StartService(sendIntent);
            };

        }
    }
}

