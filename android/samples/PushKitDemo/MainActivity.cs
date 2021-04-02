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
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using Huawei.Agconnect.Config;
using Huawei.Hms.Aaid;
using Huawei.Hms.Aaid.Entity;
using Huawei.Hms.Common;
using Huawei.Hms.Core;
using Huawei.Hms.Opendevice;
using Huawei.Hms.Push;
using Huawei.Hms.Push.LocalNotification;
using Huawei.Hms.Support.Api.Opendevice;
using XamarinHmsPushDemo.HMSPush;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using XamarinHmsPushDemo.Hmssample;
using XamarinHmsPushDemo.HMSSample;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;
using Uri = Android.Net.Uri;

namespace XamarinHmsPushDemo
{

    [Activity(Label = "@string/app_name", MainLauncher = true,
        WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.AdjustResize,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.FullSensor)]
    public class MainActivity : AppCompatActivity, IHmsPushEvent
    {
        LinearLayout tvLog;
        EditText txtTopic;
        public Logger log;

        //BroadcastReceiver to take data from HmsMessageService
        HMSPushReceiver HMSPushReceiver;

        //BroadcastReceiver to receive actions
        HMSPushActionReceiver HMSPushActionReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            InitializeViews();

            //OnCreate() method will be triggered if this activity is not in foreground or user clicks on actions
            var notificationData = RemoteMessageUtils.ExtractNotificationData(Intent);
            if (notificationData != null)
            {
                log.Info("NotificationData(OnCreate)", notificationData.DictionaryToString());
                if (notificationData.ContainsKey(NotificationConstants.Action))
                {
                    string action = notificationData[NotificationConstants.Action];
                    if (action == "Yes")//Custom action
                    {
                        //If user clicks on "Yes" action
                        log.Info("Notification", "Clicked \"Yes\"");
                    }
                    else if (action == "No")//Custom action
                    {
                        //If user clicks on "No" action
                        log.Info("Notification", "Clicked \"No\"");
                    }
                }
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            //OnNewIntent() method will be triggered if there is no action and this activity is in foreground.
            var notificationData = RemoteMessageUtils.ExtractNotificationData(intent);
            if (notificationData != null)
                log.Info("NotificationData(OnNewIntent)", notificationData.DictionaryToString());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnregisterReceiver(HMSPushReceiver);
            UnregisterReceiver(HMSPushActionReceiver);
        }

        public void InitializeViews()
        {
            tvLog = FindViewById<LinearLayout>(Resource.Id.tv_log);
            txtTopic = FindViewById<EditText>(Resource.Id.txt_topic);
            log = new Logger(tvLog, FindViewById<ScrollView>(Resource.Id.sv_log), this);

            //Registering Receiver
            string action = "PushReceiver";
            Intent intent = new Intent(action);
            HMSPushReceiver = new HMSPushReceiver();
            RegisterReceiver(HMSPushReceiver, new IntentFilter(action));

            action = "ActionReceiver";
            intent = new Intent(action);
            HMSPushActionReceiver = new HMSPushActionReceiver();
            RegisterReceiver(HMSPushActionReceiver,new IntentFilter(action));

            //Adding events for buttons
            FindViewById(Resource.Id.btn_local_notification).Click += OnClickLocalNotification;
            FindViewById(Resource.Id.btn_open_custom_intent).Click += OnClickCustomIntentUri;
            FindViewById(Resource.Id.btn_turn_off).Click += OnClickTurnOff;
            FindViewById(Resource.Id.btn_turn_on).Click += OnClickTurnOnPush;
            FindViewById(Resource.Id.btn_get_id).Click += OnClickGetId;
            FindViewById(Resource.Id.btn_get_aaid).Click += OnClickGetAAID;
            FindViewById(Resource.Id.btn_get_odid).Click += OnClickGetOdid;
            FindViewById(Resource.Id.btn_get_token).Click += OnClickGetToken;
            FindViewById(Resource.Id.btn_creation_time).Click += OnClickCreationTime;
            FindViewById(Resource.Id.btn_delete_aaid).Click += OnClickDeleteAAID;
            FindViewById(Resource.Id.btn_delete_token).Click += OnClickDeleteToken;
            FindViewById(Resource.Id.btn_subscribe).Click += OnClickSubscribe;
            FindViewById(Resource.Id.btn_unSubscribe).Click += OnClickUnsubscribe;
            FindViewById(Resource.Id.btn_disable_autoinit).Click += OnClickDisableAutoInit;
            FindViewById(Resource.Id.btn_enable_autoinit).Click += OnClickEnableAutoInit;
            FindViewById(Resource.Id.btn_is_autoinit_enabled).Click += OnClickIsAutoInitEnabled;
            FindViewById(Resource.Id.btn_sendRemoteMessage).Click += OnClickSendRemoteMessage;
            FindViewById(Resource.Id.btn_getInitialNotification).Click += OnClickGetInitialNotification;
        }
        /// <summary>
        /// Get last notification's bundle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetInitialNotification(object sender, EventArgs e)
        {
            var notificationData = RemoteMessageUtils.ExtractNotificationData(Intent);
            if (notificationData != null)
                log.Info("NotificationData", notificationData.DictionaryToString());
        }
        /// <summary>
        /// Sends uplink messages in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSendRemoteMessage(object sender, EventArgs e)
        {
            string Tag = "SendRemoteMessage";
            Random rnd = new Random();
            string messageId = rnd.Next().ToString();
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                ["key1"] = "test",
                ["message"] = "huawei-test",
            };

            // The input parameter of the RemoteMessage.Builder method is push.hcm.upstream, which cannot be changed.
            RemoteMessage.Builder builder = new RemoteMessage.Builder("push.hcm.upstream")
                .SetCollapseKey("-1")
                .SetMessageId(messageId)
                .SetMessageType("hms")
                .SetTtl(120)
                .SetData(data)
                .SetSendMode(1)
                .SetReceiptMode(1);

            RemoteMessage message = builder.Build();

            try
            {
                //Get Token before send RemoteMessage
                HmsMessaging.GetInstance(this).Send(message);
            }
            catch (ApiException exception)
            {
                log.Error(Tag, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Checks whether automatic initialization is enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickIsAutoInitEnabled(object sender, EventArgs e)
        {
            string MethodName = "IsAutoInitEnabled";
            try
            {
                bool isAutoInitEnabled = HmsMessaging.GetInstance(this).AutoInitEnabled;
                log.Info(MethodName, isAutoInitEnabled);
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Enable automatic initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickEnableAutoInit(object sender, EventArgs e)
        {
            string MethodName = "EnableAutoInit";
            try
            {
                HmsMessaging.GetInstance(this).AutoInitEnabled = true;
                log.Info(MethodName, "Success");
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// disable automatic initialization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDisableAutoInit(object sender, EventArgs e)
        {
            string MethodName = "DisableAutoInit";
            try
            {
                HmsMessaging.GetInstance(this).AutoInitEnabled = false;
                log.Info(MethodName, "Success");
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Unsubscribes from topics in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickUnsubscribe(object sender, EventArgs e)
        {
            string MethodName = "Unsubscribe";
            string topic = txtTopic.Text;
            var task = HmsMessaging.GetInstance(this).UnsubscribeAsync(topic);
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    log.Info(MethodName, $"Success");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Subscribes to topics in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickSubscribe(object sender, EventArgs e)
        {
            string MethodName = "Subscribe";
            string topic = txtTopic.Text;
            var task = HmsMessaging.GetInstance(this).SubscribeAsync(topic);
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    log.Info(MethodName, $"Success");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Deletes a token.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDeleteToken(object sender, EventArgs e)
        {
            string MethodName = "DeleteToken";
            Thread thread = new Thread(() =>
            {
                try
                {
                    string appid = AGConnectServicesConfig.FromContext(this).GetString("client/app_id");
                    HmsInstanceId.GetInstance(this).DeleteToken(appid, HmsMessaging.DefaultTokenScope);

                    log.Info(MethodName, "Success");
                }
                catch (ApiException exception)
                {
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            });
            thread.Start();
        }
        /// <summary>
        /// Deletes a local AAID and its generation timestamp.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickDeleteAAID(object sender, EventArgs e)
        {
            string MethodName = "DeleteAAID";
            Thread thread = new Thread(() =>
            {
                try
                {
                    HmsInstanceId.GetInstance(this).DeleteAAID();
                    log.Info(MethodName, "Success");
                }
                catch (Exception exception)
                {
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            });
            thread.Start();    
        }
        /// <summary>
        /// Obtains the generation timestamp of an AAID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCreationTime(object sender, EventArgs e)
        {
            string MethodName = "CreationTime";
            try
            {
                long creationTime = HmsInstanceId.GetInstance(this).CreationTime;
                log.Info(MethodName, creationTime);
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Obtains a token required for accessing Push Kit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetToken(object sender, EventArgs e)
        {
            string MethodName = "GetToken";
            Thread thread = new Thread(() =>
            {
                try
                {
                    string appid = AGConnectServicesConfig.FromContext(this).GetString("client/app_id");
                    string token = HmsInstanceId.GetInstance(this).GetToken(appid, HmsMessaging.DefaultTokenScope);

                    log.Info(MethodName, token);
                }
                catch (ApiException exception)
                {
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            });
            thread.Start();
        }
        /// <summary>
        /// Obtains an ODID in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGetOdid(object sender, EventArgs e)
        {
            string MethodName = "GetOdid";
            var task = OpenDevice.GetInstance(this).GetOdidAsync();
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    OdidResult odid = task.Result;
                    log.Info(MethodName, $"{odid.Id}");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Obtains an AAID in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickGetAAID(object sender, EventArgs e)
        {
            string MethodName = "GetAAID";
            var task = HmsInstanceId.GetInstance(this).GetAAIDAsync();
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    AAIDResult aaid = task.Result;
                    log.Info(MethodName, $"{aaid.Id}");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Obtains an AAID in synchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGetId(object sender, EventArgs e)
        {
            string MethodName = "Id";
            try
            {
                string Id = HmsInstanceId.GetInstance(this).Id;
                log.Info(MethodName, Id);
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Disables the function of receiving notification messages in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickTurnOff(object sender, EventArgs e)
        {
            string MethodName = "TurnOnOff";
            var task = HmsMessaging.GetInstance(this).TurnOffPushAsync();
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    log.Info(MethodName, $"Success");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }
        /// <summary>
        /// Enables the function of receiving notification messages in asynchronous mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickTurnOnPush(object sender, EventArgs e)
        {
            string MethodName = "TurnOnPush";
            var task = HmsMessaging.GetInstance(this).TurnOnPushAsync();
            try
            {
                await task;
                if (task.IsCompleted)
                {
                    log.Info(MethodName, $"Success");
                }
                else
                {
                    var exception = task.Exception;
                    log.Error(MethodName, $"Error/Exception: {exception.Message}");
                }
            }
            catch (Exception exception)
            {
                log.Error(MethodName, $"Error/Exception: {exception.Message}");
            }
        }

        private void OnClickCustomIntentUri(object sender, EventArgs e)
        {
            string Tag = "GenerateIntentUri";
            Intent intent = new Intent(Intent.ActionView);

            // You can add parameters in either of the following ways:
            // Define a scheme protocol, for example, pushscheme://com.huawei.pushkitdemo/deeplink?.
            // way 1 start: Use ampersands (&) to separate key-value pairs. The following is an example:
            intent.SetData(Uri.Parse("pushscheme://com.huawei.pushkitdemo/deeplink?name=abc&age=180"));
            //pushscheme://com.huawei.pushkitdemo/deeplink?name=abc&age=180

            // way 1 end. In this example, name=abc and age=180 are two key-value pairs separated by an ampersand (&).

            // way 2 start: Directly add parameters to the Intent.
            // intent.SetData(Uri.Parse("pushscheme://com.huawei.pushkitdemo/deeplink?"));
            // intent.PutExtra("name", "abc");
            // intent.PutExtra("age", 180);
            // way 2 end.

            // The following flag is mandatory. If it is not added, duplicate messages may be displayed.
            intent.AddFlags(ActivityFlags.ClearTop);
            string intentUri = intent.ToUri(IntentUriType.Scheme);

            // The value of intentUri will be assigned to the intent parameter in the message to be sent.
            log.Info(Tag, intentUri);

            // You can start the deep link activity with the following code.
            intent.SetClass(this, typeof(DeeplinkActivity));
            StartActivity(intent);
        }

        private void OnClickLocalNotification(object sender, EventArgs e)
        {
            StartActivity(typeof(LocalNotificationActivity));
        }

        public void OnNewToken(string token, Bundle bundle)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, token);
        }

        public void OnMessageReceived(RemoteMessage message)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            if (message.Data != null)
                log.Info(MethodName, message.Data);
        }

        public void OnMessageSent(string msgId)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, $"MessageId: {msgId}");
        }

        public void OnDeletedMessages()
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, $"Success");
        }

        public void OnSendError(string msgId, int errorCode, string errorMessage)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, $"msgId: {msgId} errorCode: {errorCode} errorMessage: {errorMessage}");
        }

        public void OnMessageDelivered(string msgId, int errorCode, string errorMessage)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, $"msgId: {msgId} status: {errorMessage}");
        }

        public void OnTokenError(int errorCode, string errorMessage, Bundle bundle)
        {
            string MethodName = MethodBase.GetCurrentMethod().Name;
            log.Info(MethodName, $"errorCode: {errorCode} errorMessage: {errorMessage}");
        }
    }
}


