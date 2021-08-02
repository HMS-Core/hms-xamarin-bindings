/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
using System;
using Android.Content;
using Android.Icu.Util;
using Android.OS;
using Android.Provider;
using Android.Support.Annotation;
using Huawei.Hms.Ml.Scan;

namespace XamarinHmsScanKitDemo.Action
{
    public static class CalendarEventAction
    {
        [RequiresApi(Api = (int)BuildVersionCodes.N)]
        public static Intent GetCalendarEventIntent(HmsScan.EventInfo calendarEvent)
        {
            Intent intent = new Intent(Intent.ActionInsert);
            try
            {
                intent.SetData(CalendarContract.Events.ContentUri)
                        .PutExtra(CalendarContract.ExtraEventBeginTime, GetTime(calendarEvent.BeginTime))
                        .PutExtra(CalendarContract.ExtraEventEndTime, GetTime(calendarEvent.CloseTime))
                        .PutExtra(CalendarContract.Events.InterfaceConsts.Title, calendarEvent.Theme)
                        .PutExtra(CalendarContract.Events.InterfaceConsts.Description, calendarEvent.AbstractInfo)
                        .PutExtra(CalendarContract.Events.InterfaceConsts.EventLocation, calendarEvent.PlaceInfo)
                        .PutExtra(CalendarContract.Events.InterfaceConsts.Organizer, calendarEvent.Sponsor)
                        .PutExtra(CalendarContract.Events.InterfaceConsts.Status, calendarEvent.Condition);
            }
            catch (Exception e)
            {
                Console.WriteLine("GetCalendarEventIntent", e);
            }
            return intent;
        }

        [RequiresApi(Api = (int)BuildVersionCodes.N)]
        private static long GetTime(HmsScan.EventTime calendarDateTime)
        {
            Calendar calendar = Calendar.Instance;
            calendar.Set(calendarDateTime.Year, calendarDateTime.Month - 1, calendarDateTime.Day,
                    calendarDateTime.Hours, calendarDateTime.Minutes, calendarDateTime.Seconds);
            return calendar.Time.Time;
        }
    }
}