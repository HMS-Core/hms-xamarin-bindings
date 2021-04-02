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
using Android.OS;
using Huawei.Hms.Core;
using System.Collections.Generic;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace Huawei.Hms.Push
{
    public static class RemoteMessageUtils
    {
        public static Dictionary<string, string> ExtractNotificationData(Intent intent)
        {
            Bundle bundle = intent.Extras;
            if (bundle == null) return null;
            if (!(bundle.Get(NotificationConstants.Notification) is Bundle extras)) return null;
            return extras.ToDictionary();
        }
        public static Dictionary<string, string> FromMap(this RemoteMessage message)
        {
            if (message == null) return null;
            Dictionary<string, string> result = new Dictionary<string, string>();
            
            result[RemoteMessageAttributes.CollapseKey] = message.CollapseKey;
            result[RemoteMessageAttributes.Data] = message.Data;
            result[RemoteMessageAttributes.DataOfMap] = CoreUtils.DictionaryToString(message.DataOfMap);
            result[RemoteMessageAttributes.MessageId] = message.MessageId;
            result[RemoteMessageAttributes.MessageType] = message.MessageType;
            result[RemoteMessageAttributes.OriginalUrgency] = message.OriginalUrgency.ToString();
            result[RemoteMessageAttributes.Urgency] = message.Urgency.ToString();
            result[RemoteMessageAttributes.Ttl] = message.Ttl.ToString();
            result[RemoteMessageAttributes.SentTime] = message.SentTime.ToString();
            result[RemoteMessageAttributes.To] = message.To;
            result[RemoteMessageAttributes.From] = message.From;
            result[RemoteMessageAttributes.Token] = message.Token;
            result[RemoteMessageAttributes.ReceiptMode]= message.ReceiptMode.ToString();
            result[RemoteMessageAttributes.SendMode]= message.SendMode.ToString();
            result[RemoteMessageAttributes.Contents] = message.DescribeContents().ToString();

            if (message.GetNotification() != null)
            {
                RemoteMessage.Notification notification = message.GetNotification();

                result[RemoteMessageAttributes.Title] = notification.Title;
                result[RemoteMessageAttributes.TitleLocalizationKey] = notification.TitleLocalizationKey;
                result[RemoteMessageAttributes.TitleLocalizationArgs] = notification.GetTitleLocalizationArgs().CheckNString();
                result[RemoteMessageAttributes.BodyLocalizationKey] = notification.BodyLocalizationKey;
                result[RemoteMessageAttributes.BodyLocalizationArgs] = notification.GetBodyLocalizationArgs().CheckNString();
                result[RemoteMessageAttributes.Body] = notification.Body;
                result[RemoteMessageAttributes.Icon] = notification.Icon;
                result[RemoteMessageAttributes.Sound] = notification.Sound;
                result[RemoteMessageAttributes.Tag] = notification.Tag;
                result[RemoteMessageAttributes.Color] = notification.Color;
                result[RemoteMessageAttributes.ClickAction] = notification.ClickAction;
                result[RemoteMessageAttributes.ChannelId] = notification.ChannelId;
                result[RemoteMessageAttributes.ImageUrl] = notification.ImageUrl+"";
                result[RemoteMessageAttributes.Link] = notification.Link+"";
                result[RemoteMessageAttributes.NotifyId] = notification.NotifyId+"";
                result[RemoteMessageAttributes.When] = notification.When + "";
                result[RemoteMessageAttributes.LightSettings] = notification.GetLightSettings().CheckNString();
                result[RemoteMessageAttributes.BadgeNumber] = notification.BadgeNumber + "";
                result[RemoteMessageAttributes.Importance] = notification.Importance + "";
                result[RemoteMessageAttributes.Ticker] = notification.Ticker;
                result[RemoteMessageAttributes.VibrateConfig] = notification.GetVibrateConfig().CheckNString();
                result[RemoteMessageAttributes.Visibility] = notification.Visibility + "";
                result[RemoteMessageAttributes.IntentUri] = notification.IntentUri;
                result[RemoteMessageAttributes.IsAutoCancel] = notification.IsAutoCancel + "";
                result[RemoteMessageAttributes.IsLocalOnly] = notification.IsLocalOnly+"";
                result[RemoteMessageAttributes.IsDefaultLight] = notification.IsDefaultLight+"";
                result[RemoteMessageAttributes.IsDefaultSound] = notification.IsDefaultSound+"";
                result[RemoteMessageAttributes.IsDefaultVibrate] = notification.IsDefaultVibrate+"";
            }

            return result;
        }

        public static string CheckNString(this string[] array)
        {
            if (array == null) return null;
            return array.ToString();
        }
        public static string CheckNString(this int[] array)
        {
            if (array == null) return null;
            return array.ToString();
        }
        public static string CheckNString(this long[] array)
        {
            if (array == null) return null;
            return array.ToString();
        }
        
    }
}