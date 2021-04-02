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
using Android.Util;
using Huawei.Hms.Push;
using Huawei.Hms.Push.LocalNotification;

namespace XamarinHmsPushDemo.HMSPush
{
    [Service]
    [IntentFilter(new string[] { "com.huawei.HMSActionService" })]
    public class HMSPushActionService : HMSActionService
    {
        readonly string Tag = "HMSPushActionService";
        readonly string HMSActionReceiver = "ActionReceiver";
        public override void OnClickAction(Intent notification)
        {
            var notificationData = RemoteMessageUtils.ExtractNotificationData(notification);
            if (notificationData != null)
            {
                Log.Info(Tag, notificationData.DictionaryToString());
                Intent intent = new Intent();
                intent.SetAction(HMSActionReceiver);
                intent.PutExtras(notification);

                SendBroadcast(intent);
            }
        }
    }
}