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

namespace Huawei.Hms.Push
{
    public static class CoreConstants
    {
        public const string PreferenceName = "huawei_hms_xa_push";

        public const string ChannelName = "huawei-hms-xa-push-channel-id";


        public const string RemoteMessageUplinkTo = "push.hcm.upstream";


        public const string DefaultMessage = "HMS Push";
        public const long DefaultVibrateDuration = 250L;
        public const string NotificationChannelId = "huawei-hms-xa-push-channel-id";
        public const string NotificationChannelName = "huawei-hms-xa-push-channel";
        public const string NotificationChannelDesc = "Huawei HMS Push";
                
        public static class NotificationType
        {
            public const string Now = "NOW";
            public const string Scheduled = "SCHEDULED";
            public const string Remote = "REMOTE";
            public const string Data = "DATA";
        }

        public static class ScheduledPublisher
        {
            public const string NotificationId = "notificationId";
        }
    }
}