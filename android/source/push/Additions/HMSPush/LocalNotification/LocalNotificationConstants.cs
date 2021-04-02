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
using Android.OS;
using Android.Support.V4.App;

namespace Huawei.Hms.Push.LocalNotification
{
    public static partial class LocalNotificationConstants
    {
        public static class Priority
        {
            public const string Max = "max";
            public const string High = "high";
            public const string Default = "default";
            public const string Low = "low";
            public const string Min = "min";
        }

        public static int GetPriority(this Bundle bundle)
        {
            string value = bundle.GetString(NotificationConstants.Priority);

            if (value == null) return NotificationCompat.PriorityHigh;

            return (value.ToLower()) switch
            {
                Priority.Max => NotificationCompat.PriorityMax,
                Priority.Low => NotificationCompat.PriorityLow,
                Priority.Min => NotificationCompat.PriorityMin,
                Priority.Default => NotificationCompat.PriorityDefault,
                _ => NotificationCompat.PriorityHigh,
            };
        }

        public static class Visibility
        {
            public const string Public = "public";
            public const string Secret = "secret";
            public const string Private = "private";
        }

        public static int GetVisibility(this Bundle bundle)
        {
            string value = bundle.GetString(NotificationConstants.Visibility);
            if (value == null) return NotificationCompat.VisibilityPublic;

            return (value.ToLower()) switch
            {
                Visibility.Public => NotificationCompat.VisibilityPublic,
                Visibility.Secret => NotificationCompat.VisibilitySecret,
                _ => NotificationCompat.VisibilityPrivate,
            };
        }

        public static class Importance
        {
            public const string Max = "max";
            public const string High = "high";
            public const string Default = "default";
            public const string Low = "low";
            public const string Min = "min";
            public const string None = "none";
            public const string Unspecified = "unspecified";
        }

        public static int GetImportance(this Bundle bundle)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
                return (int)NotificationImportance.High;
            string value = bundle.GetString(NotificationConstants.Importance);

            if (value == null) return (int)NotificationImportance.High;

            try
            {
                return (value.ToLower()) switch
                {
                    Importance.Max => (int)NotificationImportance.Max,
                    Importance.Low => (int)NotificationImportance.Low,
                    Importance.Min => (int)NotificationImportance.Min,
                    Importance.None => (int)NotificationImportance.None,
                    Importance.Unspecified => (int)NotificationImportance.Unspecified,
                    Importance.Default => (int)NotificationImportance.Default,
                    _ => (int)NotificationImportance.High,
                };
            }
            catch
            {
                return 4;
            }
        }

        public static class Repeat
        {
            public class Type
            {
                public const string Second = "second";
                public const string Minute = "minute";
                public const string Hour = "hour";
                public const string Day = "day";
                public const string Week = "week";
                public const string CustomTime = "custom_time";
            }
            public class Time
            {
                public const int OneSecond = 1000;
                public const int OneMinute = 60000;
                public const int OneHour = 3600000;
                public const int OneDay = 86400000;
                public const int OneWeek = 604800000;
            }

        }

    }
}