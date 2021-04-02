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
using Android.Widget;
using Huawei.Hms.Common;
using Huawei.Hms.Core;
using Huawei.Hms.Push;
using Huawei.Hms.Push.LocalNotification;
using System;
using System.Collections.Generic;
using XamarinHmsPushDemo.HMSPush;
using XamarinHmsPushDemo.HMSSample;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace XamarinHmsPushDemo
{
    [Activity(Label = "Push Kit Demo - LocalNotification", WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.AdjustResize,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.FullSensor)]
    public class LocalNotificationActivity : Activity
    {
        private LinearLayout tvLog;

        HMSPushActionReceiver HMSPushActionReceiver;

        Logger log;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_local_notification);
            InitializeViews();
        }

        public void InitializeViews()
        {
            tvLog = FindViewById<LinearLayout>(Resource.Id.tv_log);
            log = new Logger(tvLog, FindViewById<ScrollView>(Resource.Id.sv_log), this);

            //Adding events for buttons
            FindViewById(Resource.Id.btn_local_notification_default).Click += OnClickDefaultNotification;
            FindViewById(Resource.Id.btn_ongoing).Click += OnClickOnGoing;
            FindViewById(Resource.Id.btn_sound).Click += OnClickSound;
            FindViewById(Resource.Id.btn_vibrate).Click += OnClickVibrate;
            FindViewById(Resource.Id.btn_bigimage).Click += OnClickBigImage;
            FindViewById(Resource.Id.btn_repeat).Click += OnClickRepeate;
            FindViewById(Resource.Id.btn_scheduled).Click += OnClickScheduled;
            FindViewById(Resource.Id.btn_cancelAllNotifications).Click += OnClickCancelAllNotifications;
            FindViewById(Resource.Id.btn_getNotifications).Click += OnClickGetNotifications;
            FindViewById(Resource.Id.btn_cancelScheduledNotifications).Click += OnClickCancelScheduledNotifications;
            FindViewById(Resource.Id.btn_getScheduledLocalNotifications).Click += OnClickGetScheduledNotifications;
            FindViewById(Resource.Id.btn_cancelNotificationsWithTag).Click += OnClickCancelNotificationsWith;
            FindViewById(Resource.Id.btn_getChannels).Click += OnClickGetChannels;
            FindViewById(Resource.Id.btn_cancelNotifications).Click += OnClickCancelNotifications;
            FindViewById(Resource.Id.btn_deleteChannel).Click += OnClickDeleteChannel;
            FindViewById(Resource.Id.btn_channelBlocked).Click += OnClickIsChannelBlocked;
            FindViewById(Resource.Id.btn_channelExists).Click += OnClickChannelExists;
        }
        /// <summary>
        /// Checks whether a notification channel with the given ID exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickChannelExists(object sender, System.EventArgs e)
        {
            string Tag = "ChannelExists";
            try
            {
                bool isExist = NotificationController.ChannelExists(this, CoreConstants.NotificationChannelId + 49);
                if (isExist)
                    log.Info(Tag, "The channel exists!");
                else log.Info(Tag, "The channel does not exist!");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Checks whether a notification channel with the given ID is blocked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickIsChannelBlocked(object sender, System.EventArgs e)
        {
            string Tag = "IsChannelBlocked";
            try
            {
                bool isChannelBlocked = NotificationController.ChannelBlocked(this, CoreConstants.NotificationChannelId + 45);
                if (isChannelBlocked)
                    log.Info(Tag, "The channel is blocked!");
                else log.Info(Tag, "The channel is not blocked!");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Deletes a notification channel with the given ID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDeleteChannel(object sender, System.EventArgs e)
        {
            string Tag = "DeleteChannel";
            try
            {
                NotificationController.DeleteChannel(this, CoreConstants.NotificationChannelId + 49);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Cancels all pending notification messages registered in the notification manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelNotifications(object sender, System.EventArgs e)
        {
            string Tag = "CancelNotifications";
            try
            {
                NotificationController.CancelNotifications(this);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Obtains a list of all notification messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetChannels(object sender, System.EventArgs e)
        {
            string Tag = "GetChannels";
            try
            {
                List<NotificationChannel> channels = NotificationController.GetChannels(this);

                log.Info(Tag, $"There are {channels.Count} channel(s).");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Cancels all pending notification messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelNotificationsWith(object sender, System.EventArgs e)
        {
            string Tag = "CancelNotificationsWith";
            try
            {
                NotificationController.CancelNotificationsWith(this, "tag");
                NotificationController.CancelNotificationsWith(this, new int[] { 1, 2 });
                NotificationController.CancelNotificationsWith(this,
                    new CancelNotificationParameters[] {
                        new CancelNotificationParameters { Id = 5, Tag = "tag" },
                        new CancelNotificationParameters { Id = 6, Tag = "tag2" } });

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Obtains a list of all pending scheduled notification messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetScheduledNotifications(object sender, System.EventArgs e)
        {
            string Tag = "ScheduledNotifications";
            try
            {
                List<Dictionary<string, string>> notifications = NotificationController.GetScheduledNotifications(this);

                log.Info(Tag, $"There are {notifications.Count} scheduled notification(s).");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Cancels all pending scheduled notification messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelScheduledNotifications(object sender, System.EventArgs e)
        {
            string Tag = "CancelScheduledNotifications";
            try
            {
                NotificationController.CancelScheduledNotifications(this);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Obtains a list of all notification messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetNotifications(object sender, System.EventArgs e)
        {
            string Tag = "Notifications";
            try
            {
                List<Dictionary<string, string>> notifications = NotificationController.GetNotifications(this);

                log.Info(Tag, $"There are {notifications.Count} active notifications.");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }
        /// <summary>
        /// Cancels all pending scheduled notification messages and the ones registered in the notification manager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCancelAllNotifications(object sender, System.EventArgs e)
        {
            string Tag = "CancelAllNotifications";
            try
            {
                NotificationController.CancelAllNotifications(this);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private void OnClickScheduled(object sender, System.EventArgs e)
        {
            string Tag = "Scheduled";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                notification.PutDouble(NotificationConstants.FireDate, 10000L);//Time when a notification message will be pushed.
                NotificationController.LocalNotificationSchedule(this, notification);//Schedules a local notification to be pushed at a future time.

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private async void OnClickRepeate(object sender, System.EventArgs e)
        {
            string Tag = "Repeate";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                //notification.PutString(NotificationConstants.RepeatType, Repeat.Type.Second);
                //or
                notification.PutString(NotificationConstants.RepeatType, Repeat.Type.CustomTime);//Time when a scheduled notification message is pushed repeatedly.
                notification.PutLong(NotificationConstants.RepeatTime, 5 * Repeat.Time.OneSecond);//Time when the next scheduled notification message is pushed repeatedly, in milliseconds.

                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        public async void OnClickBigImage(object sender, System.EventArgs e)
        {
            string Tag = "BigImage";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                notification.PutString(NotificationConstants.BigPictureUrl, "https://www-file.huawei.com/-/media/corp/home/image/logo_400x200.png"); //URL of the large image for notification messages.
                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private async void OnClickVibrate(object sender, System.EventArgs e)
        {
            string Tag = "Vibrate";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                notification.PutBoolean(NotificationConstants.Vibrate, true);//Indicates whether to enable the vibration mode when a notification message is pushed.
                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private async void OnClickSound(object sender, System.EventArgs e)
        {
            string Tag = "Sound";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                notification.PutBoolean(NotificationConstants.PlaySound, true);//Indicates whether a sound is played when a notification message is pushed.
                notification.PutString(NotificationConstants.SoundName, "huawei_bounce.mp3");//Name of the sound file to be played in the raw directory when a notification message is pushed.
                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private async void OnClickOnGoing(object sender, System.EventArgs e)
        {
            string Tag = "OnGoing";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                //Indicates whether a notification message to be pushed is an ongoing message. 
                //This message cannot be canceled by users through swiping, but can be canceled by the app.
                notification.PutBoolean(NotificationConstants.Ongoing, true);
                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private async void OnClickDefaultNotification(object sender, System.EventArgs e)
        {
            string Tag = "LocalNotification Default";
            try
            {
                var notification = new Bundle();
                FillCommonParts(notification);
                FillBundle(notification);
                await NotificationController.LocalNotificationNow(this, notification);

                log.Info(Tag, "Success");
            }
            catch (ApiException exception)
            {
                log.Error(Tag, "Error/Exception: " + exception);
            }
        }

        private void FillBundle(Bundle notification)
        {
            //For details Push Kit Cross-Platform Xamarin APIs https://developer.huawei.com/consumer/en/doc/ 
            //notification.PutString(NotificationConstants.Ticker, "Optional Ticker");
            notification.PutString(NotificationConstants.ChannelId, CoreConstants.NotificationChannelId);
            //notification.PutString(NotificationConstants.ChannelName, CoreConstants.ChannelName);
            //notification.PutString(NotificationConstants.ChannelDescription, "");
            //notification.PutString(NotificationConstants.RepeatInterval, "");
            //notification.PutString(NotificationConstants.SoundName, "");
            //notification.PutString(NotificationConstants.Color, "white");
            notification.PutStringArray(NotificationConstants.Actions, new string[] { "Yes", "No" });
            //notification.PutString(NotificationConstants.Group, "group");
            notification.PutString(NotificationConstants.Importance, LocalNotificationConstants.Importance.Max);
            notification.PutString(NotificationConstants.Priority, Priority.Max);
            notification.PutString(NotificationConstants.Visibility, Visibility.Public);
            //notification.PutString(NotificationConstants.LargeIconUrl, "https://developer.huawei.com/Enexport/sites/default/images/en/Develop/hms/push/push2-tuidedao.png");
            //notification.PutString(NotificationConstants.Number, "3");
            //notification.PutInt(NotificationConstants.Id, 0);
            //notification.PutInt(NotificationConstants.SmallIcon, Resource.Mipmap.ic_launcher);
            //notification.PutInt(NotificationConstants.LargeIcon, Resource.Mipmap.ic_launcher_foreground);
            //notification.PutDouble(NotificationConstants.VibrateDuration, 2500L);
            //notification.PutBoolean(NotificationConstants.Vibrate, false);
            //notification.PutBoolean(NotificationConstants.Ongoing, false);
            //notification.PutBoolean(NotificationConstants.PlaySound, false);
            //notification.PutBoolean(NotificationConstants.AllowWhileIdle, false);
            notification.PutBoolean(NotificationConstants.InvokeApp, false);
            //notification.PutBoolean(NotificationConstants.DontNotifyInForeground, false);
            //notification.PutBoolean(NotificationConstants.AutoCancel, false);
            //notification.PutBoolean(NotificationConstants.GroupSummary, false);
            //notification.PutBoolean(NotificationConstants.OnlyAlertOnce, false);
            //notification.PutBoolean(NotificationConstants.ShowWhen, true);
        }

        private void FillCommonParts(Bundle bundle)
        {
            bundle.PutString(NotificationConstants.Title, FindViewById<EditText>(Resource.Id.txt_title).Text);
            bundle.PutString(NotificationConstants.Message, FindViewById<EditText>(Resource.Id.txt_message).Text);
            bundle.PutString(NotificationConstants.BigText, FindViewById<EditText>(Resource.Id.txt_bigText).Text);
            bundle.PutString(NotificationConstants.SubText, FindViewById<EditText>(Resource.Id.txt_subText).Text);
            bundle.PutInt(NotificationConstants.SmallIcon, Resource.Mipmap.ic_launcher);
            bundle.PutString(NotificationConstants.Tag, FindViewById<EditText>(Resource.Id.txt_tag).Text);
        }
    }
}