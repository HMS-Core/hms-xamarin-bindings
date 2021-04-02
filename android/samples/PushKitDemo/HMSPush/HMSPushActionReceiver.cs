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
using Android.Content;
using Android.Util;
using Huawei.Hms.Push;
using Huawei.Hms.Push.LocalNotification;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace XamarinHmsPushDemo.HMSPush
{
    public class HMSPushActionReceiver : BroadcastReceiver
    {
        readonly string Tag = "HMSPushActionReceiver";
        public override void OnReceive(Context context, Intent intent)
        {
            var notificationData = RemoteMessageUtils.ExtractNotificationData(intent);
            if (notificationData != null)
            {
                Log.Info(Tag, notificationData.DictionaryToString());
                string action = notificationData[NotificationConstants.Action];
                if (action == "Yes")//Custom action
                {
                    //If user clicks on "Yes" action
                    NotificationController.InvokeApp(context, intent.Extras.GetBundle(NotificationConstants.Notification));
                }
            }
            
        }
    }
}