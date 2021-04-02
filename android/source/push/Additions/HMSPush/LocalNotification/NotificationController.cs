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
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Service.Notification;
using Android.Support.V4.App;
using Java.Lang;
using Org.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;
using Android.Util;
using Uri = Android.Net.Uri;

namespace Huawei.Hms.Push.LocalNotification
{

    public static class NotificationController
    {
        static readonly string TAG = "NotificationController";

        static readonly string Success = "Success";

        private static string CheckRequiredParams(Context context, Bundle bundle, string type)
        {
            if (GetActivityClass(context) == null)
                return "Activity is required!";
            else if (bundle.GetString(NotificationConstants.Message) == null)
                return "Notification Message is required!";
            else if (!bundle.ContainsKey(NotificationConstants.Id))
            {
                bundle.PutInt(NotificationConstants.Id, CoreUtils.GetRandomInt());
                return Success;
            }
            else if (type == CoreConstants.NotificationType.Scheduled && bundle.GetDouble(NotificationConstants.FireDate) == 0)
                return "Fire Date is required for schedule!";
            return Success;
        }

        private static object GetActivityClass(Context context)
        {
            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            try
            {
                return Class.ForName(launchIntent.Component.ClassName);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Pushes a local notification instantly.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task LocalNotificationNow(Context context, Bundle bundle)
        {
            try
            {
                string requiredParams = CheckRequiredParams(context, bundle, CoreConstants.NotificationType.Now);
                if (requiredParams != Success) throw new System.Exception(requiredParams);

                string channelId = bundle.GetString(NotificationConstants.ChannelId, CoreConstants.NotificationChannelId);
                int priority = bundle.GetPriority();
                int importance = bundle.GetImportance();
                int visibility = bundle.GetVisibility();

                channelId += '-' + importance;


                int smallIcon = bundle.GetInt(NotificationConstants.SmallIcon);
                bool isOngoing = bundle.GetBoolean(NotificationConstants.Ongoing, false);
                bool isOnlyAlertOnce = bundle.GetBoolean(NotificationConstants.OnlyAlertOnce, false);
                bool isAutoCancel = bundle.GetBoolean(NotificationConstants.AutoCancel, true);
                bool isGroupSummary = bundle.GetBoolean(NotificationConstants.GroupSummary, false);
                bool showWhen = bundle.GetBoolean(NotificationConstants.ShowWhen, true);
                string ticker = bundle.GetString(NotificationConstants.Ticker, "Optional Ticker");
                string channelDescription = bundle.GetString(NotificationConstants.ChannelDescription, string.Empty);
                string channelName = bundle.GetString(NotificationConstants.ChannelName, CoreConstants.ChannelName);
                string message = bundle.GetString(NotificationConstants.Message, string.Empty);
                string title = bundle.GetString(NotificationConstants.Title, string.Empty);
                string bigText = bundle.GetString(NotificationConstants.BigText, string.Empty);
                bigText ??= message;

                // Instantiate the builder and set notification elements:
                NotificationCompat.Builder builder = new NotificationCompat.Builder(context, "")
                    .SetVisibility(visibility)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetSmallIcon(smallIcon)
                    .SetPriority(priority)
                    .SetTicker(ticker)
                    .SetAutoCancel(isAutoCancel)
                    .SetOnlyAlertOnce(isOnlyAlertOnce)
                    .SetOngoing(isOngoing);

                if (bundle.ContainsKey(NotificationConstants.Number))
                {
                    builder.SetBadgeIconType(NotificationCompat.BadgeIconSmall);
                    builder.SetNumber(bundle.GetInt(NotificationConstants.Number));
                }

                //Big Picture Url
                if (bundle.ContainsKey(NotificationConstants.BigPictureUrl))
                {
                    Bitmap bitmap = await Utils.Utils.GetBitmapFromUrlAsync(bundle.GetString(NotificationConstants.BigPictureUrl));
                    NotificationCompat.BigPictureStyle style = new NotificationCompat.BigPictureStyle();
                    style.BigPicture(bitmap);
                    builder.SetStyle(style);
                }
                else
                {
                    NotificationCompat.Style style = new NotificationCompat.BigTextStyle().BigText(bigText);
                    builder.SetStyle(style);
                }

                //Large Icon Url
                if (bundle.GetString(NotificationConstants.LargeIconUrl) != null)
                {
                    Bitmap largeIconBitmap = await Utils.Utils.GetBitmapFromUrlAsync(bundle.GetString(NotificationConstants.LargeIconUrl));
                    builder.SetLargeIcon(largeIconBitmap);
                }
                else if (bundle.GetInt(NotificationConstants.LargeIcon, -1) != -1)
                {
                    builder.SetLargeIcon(BitmapFactory.DecodeResource(context.Resources, bundle.GetInt(NotificationConstants.LargeIcon)));
                }

                //Vibrate
                long[] vibratePattern = new long[] { 0 };
                if (bundle.GetBoolean(NotificationConstants.Vibrate, false))
                {
                    long vibrateDuration = bundle.ContainsKey(NotificationConstants.VibrateDuration) ?
                        bundle.GetLong(NotificationConstants.VibrateDuration) : CoreConstants.DefaultVibrateDuration;
                    vibratePattern = new long[] { 0, vibrateDuration };
                    builder.SetVibrate(vibratePattern);

                    channelId += '-' + vibrateDuration;
                }

                //SubText
                string subText = bundle.GetString(NotificationConstants.SubText);
                if (subText != string.Empty)
                    builder.SetSubText(subText);

                //Sound
                Uri soundUri = null;
                if (bundle.GetBoolean(NotificationConstants.PlaySound, false))
                {
                    soundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

                    string soundName = bundle.GetString(NotificationConstants.SoundName);
                    if (soundName != null)
                    {
                        int resId;
                        if (context.Resources.GetIdentifier(soundName, null, null) == 0)
                            soundName = soundName.Substring(0, soundName.LastIndexOf('.'));
                        resId = context.Resources.GetIdentifier(context.PackageName + ":drawable/" + soundName, null, null);
                        soundUri = Uri.Parse($"{ ContentResolver.SchemeAndroidResource}://{context.PackageName}/{resId}");
                    }

                    channelId += '-' + soundName;
                }

                //ShowWhen
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    builder.SetShowWhen(showWhen);
                }

                //Defaults
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    builder.SetDefaults(NotificationCompat.FlagShowLights);
                }

                //Group and GroupSummary
                if (Build.VERSION.SdkInt >= BuildVersionCodes.KitkatWatch)
                {
                    string group = bundle.GetString(NotificationConstants.Group);

                    if (group != null)
                    {
                        builder.SetGroup(group);
                    }

                    if (bundle.ContainsKey(NotificationConstants.GroupSummary) || isGroupSummary)
                    {
                        builder.SetGroupSummary(isGroupSummary);
                    }
                }

                //Category Color
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    builder.SetCategory(NotificationCompat.CategoryCall);

                    string color = bundle.GetString(NotificationConstants.Color);
                    if (color != null)
                    {
                        builder.SetColor(Color.ParseColor(color));
                    }
                }

                int notificationId = bundle.GetInt(NotificationConstants.Id, CoreUtils.GetRandomInt());
                bundle.PutInt(NotificationConstants.Id, notificationId);

                Class intentClass = Class.ForName(context.PackageManager.GetLaunchIntentForPackage(context.PackageName).Component.ClassName);

                Intent intent = new Intent(context, intentClass);
                intent.AddFlags(ActivityFlags.SingleTop);
                intent.PutExtra(NotificationConstants.Notification, bundle);

                PendingIntent pendingIntent = PendingIntent.GetActivity(context, notificationId, intent,
                        PendingIntentFlags.UpdateCurrent);

                CreateNotificationChannel(context, channelId, channelName, channelDescription, soundUri, (NotificationImportance)importance, vibratePattern);
                builder.SetChannelId(channelId);
                builder.SetContentIntent(pendingIntent);

                string[] actionArr = null;
                try
                {
                    if (bundle.ContainsKey(NotificationConstants.Actions))
                        actionArr = bundle.GetStringArray(NotificationConstants.Actions);
                }
                catch { }
                if (actionArr != null)
                {
                    int icon = 0;

                    for (int i = 0; i < actionArr.Length; i++)
                    {
                        string action;
                        try
                        {
                            action = actionArr[i];
                        }
                        catch
                        {
                            continue;
                        }
                        Class cls = Class.FromType(typeof(HmsLocalNotificationActionsReceiver));
                        Intent actionIntent = new Intent(context, cls);
                        actionIntent.SetAction(context.PackageName + ".ACTION_" + i);

                        actionIntent.AddFlags(ActivityFlags.SingleTop);

                        bundle.PutString(NotificationConstants.Action, action);
                        actionIntent.PutExtra(NotificationConstants.Notification, bundle);
                        actionIntent.SetPackage(context.PackageName);

                        PendingIntent pendingActionIntent = PendingIntent.GetBroadcast(context, notificationId, actionIntent, PendingIntentFlags.UpdateCurrent);


                        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                        {
                            builder.AddAction(new NotificationCompat.Action.Builder(icon, action, pendingActionIntent).Build());
                        }
                        else
                        {
                            builder.AddAction(icon, action, pendingActionIntent);
                        }
                    }
                }


                // Build the notification:
                Notification notification = builder.Build();

                ISharedPreferences sharedPreferences = context.GetSharedPreferences(CoreConstants.PreferenceName, FileCreationMode.Private);
                string id = bundle.GetInt(NotificationConstants.Id).ToString();
                if (sharedPreferences.GetString(id, null) != null)
                {
                    ISharedPreferencesEditor editor = sharedPreferences.Edit();
                    editor.Remove(bundle.GetInt(NotificationConstants.Id).ToString());
                    editor.Apply();
                }

                // Publish the notification:  
                if (!(Utils.Utils.IsApplicationInForeground(context) && bundle.GetBoolean(NotificationConstants.DontNotifyInForeground, false)))
                {
                    string tag = bundle.GetString(NotificationConstants.Tag);
                    if (tag != string.Empty && tag != null)
                    {
                        Utils.Utils.GetNotificationManager(context).Notify(tag, notificationId, notification);
                    }
                    else
                    {
                        Utils.Utils.GetNotificationManager(context).Notify(notificationId, notification);
                    }
                }
                LocalNotificationRepeat(context, bundle);

                Log.Info("LocalNotificationNow", Success + bundle.ToJsonObject().ToString());
                return;
            }
            catch (System.Exception e)
            {
                Log.Error("LocalNotificationNow", e.ToString());
                throw e;
            }
        }

        public static void LocalNotificationRepeat(Context context, Bundle bundle)
        {
            string RepeatType = bundle.GetString(NotificationConstants.RepeatType);
            long RepeatTime = (long)bundle.GetDouble(NotificationConstants.RepeatTime);

            if (RepeatType == null) return;

            long fireDate = (long)bundle.GetDouble(NotificationConstants.FireDate);
            if (fireDate == 0) fireDate = DateTime.Now.Ticks;

            long newFireDate;
            switch (RepeatType)
            {
                case Repeat.Type.Second:
                    newFireDate = fireDate + Repeat.Time.OneSecond;
                    break;
                case Repeat.Type.Hour:
                    newFireDate = fireDate + Repeat.Time.OneHour;
                    break;
                case Repeat.Type.Minute:
                    newFireDate = fireDate + Repeat.Time.OneMinute;
                    break;
                case Repeat.Type.Day:
                    newFireDate = fireDate + Repeat.Time.OneDay;
                    break;
                case Repeat.Type.Week:
                    newFireDate = fireDate + Repeat.Time.OneWeek;
                    break;
                case Repeat.Type.CustomTime:
                    if (RepeatTime <= 0)
                    {
                        newFireDate = 1;
                        break;
                    }
                    newFireDate = fireDate + RepeatTime;
                    break;
                default:
                    newFireDate = 1;
                    break;
            }

            bundle.PutDouble(NotificationConstants.FireDate, newFireDate);
            LocalNotificationSchedule(context, bundle);
        }
        /// <summary>
        /// Schedules a local notification to be pushed at a further time.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static void LocalNotificationSchedule(Context context, Bundle bundle)
        {
            try
            {
                string requiredParams = CheckRequiredParams(context, bundle, CoreConstants.NotificationType.Scheduled);
                if (requiredParams != Success) throw new System.Exception(requiredParams);

                ISharedPreferences sharedPreferences = context.GetSharedPreferences(CoreConstants.PreferenceName, FileCreationMode.Private);
                ISharedPreferencesEditor editor = sharedPreferences.Edit();
                string id = bundle.GetInt(NotificationConstants.Id).ToString();
                string notificationAttributes = bundle.ToJsonObject().ToString();
                editor.Remove(bundle.GetInt(NotificationConstants.Id).ToString());
                editor.PutString(id, notificationAttributes);
                editor.Apply();

                PendingIntent pendingIntent = BuildScheduleNotificationIntent(context, bundle);

                if (pendingIntent == null) return;

                var fireTime = (long)bundle.GetDouble(NotificationConstants.FireDate);
                bool allowWhileIdle = bundle.GetBoolean(NotificationConstants.AllowWhileIdle);

                if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
                    Utils.Utils.GetAlarmManager(context).Set(AlarmType.RtcWakeup, fireTime, pendingIntent);
                else if (allowWhileIdle && Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    Utils.Utils.GetAlarmManager(context).SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, fireTime, pendingIntent);
                else
                    Utils.Utils.GetAlarmManager(context).SetExact(AlarmType.RtcWakeup, fireTime, pendingIntent);

            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public static PendingIntent BuildScheduleNotificationIntent(Context context, Bundle bundle)
        {
            PendingIntent result;

            int id = bundle.GetInt(NotificationConstants.Id);
            Intent intent = new Intent(context, typeof(HmsLocalNotificationScheduledPublisher));
            intent.PutExtra(CoreConstants.ScheduledPublisher.NotificationId, id);
            intent.PutExtras(bundle);
            intent.AddFlags(ActivityFlags.SingleTop);
            bundle.PutBoolean(NotificationConstants.UserInteraction, true);
            intent.PutExtra(NotificationConstants.Notification, bundle);
            result = PendingIntent.GetBroadcast(context, id, intent, PendingIntentFlags.UpdateCurrent);

            return result;
        }

        public static void CreateNotificationChannel(Context context, string channelId, string channelName, string channelDescription, Uri soundUri, NotificationImportance importance, long[] vibratePattern)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(channelId, channelName, importance)
            {
                Description = channelDescription
            };

            //Vibrate
            channel.SetVibrationPattern(vibratePattern);
            channel.EnableVibration(true);

            //Sound
            if (soundUri != null)
            {
                var audioAttributes = new AudioAttributes.Builder()
                    .SetContentType(AudioContentType.Sonification)
                    .SetUsage(AudioUsageKind.Notification).Build();
                channel.SetSound(soundUri, audioAttributes);
                channel.Importance = importance;
            }
            else channel.SetSound(null, null);

            Utils.Utils.GetNotificationManager(context).CreateNotificationChannel(channel);
            Log.Info(MethodBase.GetCurrentMethod().Name, $"ChannelId: {channelId}");
        }

        public static void InvokeApp(Context context, Bundle bundle)
        {

            string packageName = context.PackageName;
            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(packageName);
            if (launchIntent == null) return;

            try
            {
                string className = launchIntent.Component.ClassName;

                Class activityClass = Class.ForName(className);
                Intent activityIntent = new Intent(context, activityClass);

                if (bundle != null)
                {
                    activityIntent.PutExtra(NotificationConstants.Notification, bundle);
                }

                activityIntent.AddFlags(ActivityFlags.NewTask);

                context.StartActivity(activityIntent);
            }
            catch (System.Exception e)
            {
                Log.Error(TAG, "Class not found", e);
            }
        }

        /// <summary>
        /// Returns an List of all notifications.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetNotifications(Context context)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            StatusBarNotification[] activeNotifications = Utils.Utils.GetNotificationManager(context).GetActiveNotifications();
            foreach (StatusBarNotification statusBarNotification in activeNotifications)
            {
                if (statusBarNotification.Tag == "ranker_group") continue;
                Notification notification = statusBarNotification.Notification;
                Bundle extras = notification.Extras;

                Dictionary<string, string> notificationPrep = new Dictionary<string, string>
                {
                    [NotificationConstants.Identifier] = "" + statusBarNotification.Id,
                    [NotificationConstants.Title] = extras.GetString(Notification.ExtraTitle),
                    [NotificationConstants.Body] = extras.GetString(Notification.ExtraText),
                    [NotificationConstants.Tag] = statusBarNotification.Tag,
                    [NotificationConstants.Group] = notification.Group,
                };
                result.Add(notificationPrep);
            }
            Log.Info(MethodBase.GetCurrentMethod().Name, result.ToString());
            return result;
        }
        /// <summary>
        /// Returns a List of all pending scheduled notifications.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> GetScheduledNotifications(Context context)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            var scheduledNotifications = context.GetSharedPreferences(CoreConstants.PreferenceName, FileCreationMode.Private).All;

            foreach (var statusBarNotification in scheduledNotifications)
            {
                JSONTokener tokener = new JSONTokener(statusBarNotification.Value.ToString());
                var notification = new JSONObject(tokener);

                try
                {
                    Dictionary<string, string> notificationMap = new Dictionary<string, string>
                    {
                        [NotificationConstants.Id] = notification.GetString(NotificationConstants.Id, null),
                        [NotificationConstants.Title] = notification.GetString(NotificationConstants.Title, null),
                        [NotificationConstants.Message] = notification.GetString(NotificationConstants.Message, null),
                        [NotificationConstants.Ticker] = notification.GetString(NotificationConstants.Ticker, null),
                        [NotificationConstants.Number] = notification.GetString(NotificationConstants.Number, null),
                        [NotificationConstants.FireDate] = notification.GetString(NotificationConstants.FireDate, null),
                        [NotificationConstants.ChannelId] = notification.GetString(NotificationConstants.ChannelId, null),
                        [NotificationConstants.ChannelName] = notification.GetString(NotificationConstants.ChannelName, null),
                        [NotificationConstants.Tag] = notification.GetString(NotificationConstants.Tag, null),
                        [NotificationConstants.RepeatInterval] = notification.GetString(NotificationConstants.RepeatInterval, null),
                        [NotificationConstants.SoundName] = notification.GetString(NotificationConstants.SoundName, null),
                    };
                    result.Add(notificationMap);
                }
                catch (System.Exception e)
                {
                    Log.Error($"{ MethodBase.GetCurrentMethod().Name}", e.ToString());
                }
            }
            Log.Info(MethodBase.GetCurrentMethod().Name, result.ToString());
            return result;
        }
        /// <summary>
        /// Returns a List<NotificationChannel> of all notification channels.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<NotificationChannel> GetChannels(Context context)
        {
            List<NotificationChannel> result = Utils.Utils.GetNotificationManager(context).NotificationChannels.ToList();
            Log.Info(MethodBase.GetCurrentMethod().Name, result.ToString());
            return result;
        }
        /// <summary>
        /// Returns true if the notification channel with the specified ID is blocked.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static bool ChannelBlocked(Context context, string channelId)
        {
            bool result;
            if (Build.VERSION.SdkInt < BuildVersionCodes.O || channelId == null)
                return false;
            try
            {
                NotificationChannel channel = Utils.Utils.GetNotificationManager(context).GetNotificationChannel(channelId);

                if (channel == null) return false;

                result = NotificationImportance.None == channel.Importance;
            }
            catch
            {
                result = false;
            }
            Log.Info(MethodBase.GetCurrentMethod().Name, $"Is {channelId} blocked: {result}");
            return result;
        }
        /// <summary>
        /// Returns true if channel with the specified ID exists .
        /// </summary>
        /// <param name="context"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static bool ChannelExists(Context context, string channelId)
        {
            bool result = false;
            if (Build.VERSION.SdkInt < BuildVersionCodes.O || channelId == null)
                return false;
            try
            {
                NotificationChannel channel = Utils.Utils.GetNotificationManager(context).GetNotificationChannel(channelId);

                result = channel != null;
            }
            catch
            {
                result = false;
            }
            Log.Info(MethodBase.GetCurrentMethod().Name, $"Does {channelId} exist: {result}");
            return result;
        }
        /// <summary>
        /// Cancel notifications with specified tag.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="tag"></param>
        public static void CancelNotificationsWith(Context context, string tag)
        {
            StatusBarNotification[] activeNotifications = Utils.Utils.GetNotificationManager(context).GetActiveNotifications();

            foreach (StatusBarNotification statusBarNotification in activeNotifications)
            {
                if (statusBarNotification.Tag == tag)
                    Utils.Utils.GetNotificationManager(context).Cancel(tag, statusBarNotification.Id);
            }
        }
        /// <summary>
        /// Cancels all pending notifications by an array of IDs.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ids"></param>
        public static void CancelNotificationsWith(Context context, int[] ids)
        {
            foreach (int id in ids)
                Utils.Utils.GetNotificationManager(context).Cancel(id);

        }
        /// <summary>
        /// Cancels all pending notifications by an array of CancelNotificationParameters which has properties "Id" and "tag", both of the properties values are strings.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="idtags"></param>
        public static void CancelNotificationsWith(Context context, CancelNotificationParameters[] idtags)
        {
            foreach (CancelNotificationParameters CancelNotificationParameters in idtags)
                Utils.Utils.GetNotificationManager(context).Cancel(CancelNotificationParameters.Tag, CancelNotificationParameters.Id);
        }
        /// <summary>
        /// Cancels all pending notifications by a JSONArray which has properties "Id" and "tag", both of the properties values are strings.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="idtags"></param>
        public static void CancelNotificationsWith(Context context, JSONArray idtags)
        {
            for (int i = 0; i < idtags.Length(); i++)
            {
                int id;
                string tag;
                try
                {
                    id = idtags.GetJSONObject(i).GetInt("id");
                    tag = idtags.GetJSONObject(i).GetString("tag");
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                Utils.Utils.GetNotificationManager(context).Cancel(tag, id);
            }
        }
        /// <summary>
        /// Cancels all pending notifications registered in the notification manager.
        /// </summary>
        /// <param name="context"></param>
        public static void CancelNotifications(Context context)
        {
            GetNotifications(context);
            Utils.Utils.GetNotificationManager(context).CancelAll();
            GetNotifications(context);
        }
        /// <summary>
        /// Delete channel with spesific channel Id.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="channelId"></param>
        public static void DeleteChannel(Context context, string channelId)
        {
            GetChannels(context);
            Utils.Utils.GetNotificationManager(context).DeleteNotificationChannel(channelId);
            GetChannels(context);
        }
        /// <summary>
        /// Cancels all pending scheduled notifications and the ones registered in the notification manager.
        /// </summary>
        /// <param name="context"></param>
        public static void CancelAllNotifications(Context context)
        {
            CancelScheduledNotifications(context); CancelNotifications(context);
        }
        /// <summary>
        /// Cancels all pending scheduled notifications at the same time by this way it also stops repeating notifications.
        /// </summary>
        /// <param name="context"></param>
        public static void CancelScheduledNotifications(Context context)
        {
            GetScheduledNotifications(context);
            ISharedPreferences sharedPreferences = context.GetSharedPreferences(CoreConstants.PreferenceName, FileCreationMode.Private);
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            foreach (string id in sharedPreferences.All.Keys.ToList())
            {
                editor.Remove(id);
                editor.Apply();
                Bundle bundle = new Bundle();
                bundle.PutString(NotificationConstants.Id, id);
                PendingIntent pendingIntent = BuildScheduleNotificationIntent(context, bundle);

                if (pendingIntent != null)
                    Utils.Utils.GetAlarmManager(context).Cancel(pendingIntent);
                try
                {
                    Utils.Utils.GetNotificationManager(context).Cancel(int.Parse(id));
                }
                catch
                {
                    Log.Error("NotificationController", "Could not Cancel.");
                }
            }
            GetScheduledNotifications(context);
        }
    }

}