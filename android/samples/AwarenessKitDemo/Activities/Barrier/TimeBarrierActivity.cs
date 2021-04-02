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
using Android.Views;
using Android.Widget;
using Huawei.Hms.Kit.Awareness.Barrier;
using System;
using AwarenessKitDemo.HMSSample;

namespace AwarenessKitDemo.Activities.Barrier
{
    [Activity(Label = "TimeBarrierActivity")]
    public class TimeBarrierActivity : Activity
    {
        #region Variables
        public Logger log;

        private const string InSunriseOrSunsetPeriodBarrierLabel = "sunset barrier label";
        private const string DuringPeriodOfDayBarrierLabel = "period of day barrier label";
        private const string DuringTimePeriodBarrierLabel = "time period barrier label";
        private const string DuringPeriodOfWeekBarrierLabel = "period of week barrier label";
        private const string InTimeCategoryBarrierLabel = "in time category label";
        private const long oneHourMilliSecond = 60 * 60 * 1000L;
        private static Java.Util.TimeZone timeZone = Java.Util.TimeZone.Default;
        private PendingIntent mPendingIntent;
        private TimeBarrierReceiver mBarrierReceiver;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_time_barrier);
            log = new Logger(FindViewById<LinearLayout>(Resource.Id.ll_log), FindViewById<ScrollView>(Resource.Id.sv_log), this);

            string barrierReceiverAction = $"{Application.PackageName}TIME_BARRIER_RECEIVER_ACTION";
            Intent intent = new Intent(barrierReceiverAction);
            // You can also create PendingIntent with Activity or Service.
            // This depends on what action you want Awareness Kit to trigger when the barrier status changes.
            mPendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Register a broadcast receiver to receive the broadcast sent by Awareness Kit when the barrier status changes.
            mBarrierReceiver = new TimeBarrierReceiver();
            RegisterReceiver(mBarrierReceiver, new IntentFilter(barrierReceiverAction));

            InitializeButtons();
        }

        public void InitializeButtons()
        {
            FindViewById(Resource.Id.add_timeBarrier_inSunriseOrSunsetPeriod).Click += TimeBarrierInSunriseOrSunsetPeriod_Click;
            FindViewById(Resource.Id.add_timeBarrier_duringPeriodOfDay).Click += TimeBarrierDuringPeriodOfDay_Click;
            FindViewById(Resource.Id.add_timeBarrier_duringTimePeriod).Click += TimeBarrierDuringTimePeriod_Click;
            FindViewById(Resource.Id.add_timeBarrier_duringPeriodOfWeek).Click += TimeBarrierDuringPeriodOfWeek_Click;
            FindViewById(Resource.Id.add_timeBarrier_inTimeCategory).Click += TimeBarrierInTimeCategory_Click;
            FindViewById(Resource.Id.delete_barrier).Click += TimeBarrierDelete_Click;
            FindViewById(Resource.Id.clear_log).Click += log.CleanEvent;
        }

        private void TimeBarrierDelete_Click(object sender, EventArgs e)
        {
            BarrierUtils.DeleteBarrier(this, mPendingIntent);
        }

        private void TimeBarrierInTimeCategory_Click(object sender, EventArgs e)
        {
            AwarenessBarrier inTimeCategoryBarrier = TimeBarrier.InTimeCategory(TimeBarrier.TimeCategoryWeekend);
            BarrierUtils.AddBarrier(this, InTimeCategoryBarrierLabel, inTimeCategoryBarrier, mPendingIntent);
        }

        private void TimeBarrierDuringPeriodOfWeek_Click(object sender, EventArgs e)
        {
            AwarenessBarrier periodOfWeekBarrier = TimeBarrier.DuringPeriodOfWeek(TimeBarrier.MondayCode,
                        timeZone, 9 * oneHourMilliSecond, 10 * oneHourMilliSecond);
            BarrierUtils.AddBarrier(this, DuringPeriodOfWeekBarrierLabel, periodOfWeekBarrier, mPendingIntent);
        }

        private void TimeBarrierDuringTimePeriod_Click(object sender, EventArgs e)
        {
            long currentTimeStamp = DateTime.Now.Ticks;
            long tenSecondsMillis = 10 * 1000L;
            AwarenessBarrier timePeriodBarrierr = TimeBarrier.DuringTimePeriod(currentTimeStamp,
                currentTimeStamp + tenSecondsMillis);
            BarrierUtils.AddBarrier(this, DuringTimePeriodBarrierLabel, timePeriodBarrierr, mPendingIntent);
        }

        private void TimeBarrierDuringPeriodOfDay_Click(object sender, EventArgs e)
        {
            AwarenessBarrier periodOfDayBarrier = TimeBarrier.DuringPeriodOfDay(timeZone,
                        11 * oneHourMilliSecond, 12 * oneHourMilliSecond);
            BarrierUtils.AddBarrier(this, DuringPeriodOfDayBarrierLabel, periodOfDayBarrier, mPendingIntent);
        }

        private void TimeBarrierInSunriseOrSunsetPeriod_Click(object sender, EventArgs e)
        {
            AwarenessBarrier sunsetBarrier = TimeBarrier.InSunriseOrSunsetPeriod(TimeBarrier.SunsetCode,
                        -oneHourMilliSecond, oneHourMilliSecond);
            BarrierUtils.AddBarrier(this, InSunriseOrSunsetPeriodBarrierLabel, sunsetBarrier, mPendingIntent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mBarrierReceiver != null)
            {
                UnregisterReceiver(mBarrierReceiver);
            }
        }

        private class TimeBarrierReceiver : BroadcastReceiver
        {
            public override void OnReceive(Context context, Intent intent)
            {
                BarrierStatus barrierStatus = BarrierStatus.Extract(intent);
                string label = barrierStatus.BarrierLabel;
                int barrierPresentStatus = barrierStatus.PresentStatus;
                Action<string> ShowLog = ((TimeBarrierActivity)context).log.ShowLog;
                switch (label)
                {

                    case InSunriseOrSunsetPeriodBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("It's around sunset time.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("It's not around sunset time.");
                        }
                        else
                        {
                            ShowLog("The time status is unknown.");
                        }
                        break;

                    case DuringPeriodOfDayBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("It's between 11 am and 12 am.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("It's not between 11 am and 12 am.");
                        }
                        else
                        {
                            ShowLog("The time status is unknown.");
                        }
                        break;

                    case DuringTimePeriodBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("The barrier was added no more than 10 seconds ago.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("10 seconds have passed after adding the duringTimePeriod barrier.");
                        }
                        else
                        {
                            ShowLog("The time status is unknown.");
                        }
                        break;
                    case DuringPeriodOfWeekBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("It's between 9 am and 10 am on Monday.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("It's not between 9 am and 10 am on Monday.");
                        }
                        else
                        {
                            ShowLog("The time status is unknown.");
                        }
                        break;

                    case InTimeCategoryBarrierLabel:
                        if (barrierPresentStatus == BarrierStatus.True)
                        {
                            ShowLog("Today is a weekend.");
                        }
                        else if (barrierPresentStatus == BarrierStatus.False)
                        {
                            ShowLog("Today is not a weekend.");
                        }
                        else
                        {
                            ShowLog("The time status is unknown.");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}