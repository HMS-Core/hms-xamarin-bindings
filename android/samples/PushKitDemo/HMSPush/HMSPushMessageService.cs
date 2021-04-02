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
using Android.Util;
using Huawei.Hms.Core;
using Huawei.Hms.Push;
using Java.Lang;

namespace XamarinHmsPushDemo.HMSPush
{
    [Service]
    [IntentFilter(new[] { "com.huawei.push.action.MESSAGING_EVENT" })]
    public class HMSPushMessageService : HmsMessageService
    {
        static readonly string HMSPushAction = "PushReceiver";

        public override void OnNewToken(string token, Bundle bundle)
        {
            // Obtain a token.
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, token);
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            intent.PutExtra(HMSPushReceiver.Token, token);

            SendBroadcast(intent);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            //It get triggered when data message comes or a notification comes which as foreground attributes false
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, RemoteMessageUtils.FromMap(message).DictionaryToString());
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            intent.PutExtra(HMSPushReceiver.Message, message);

            SendBroadcast(intent);
        }

        public override void OnMessageSent(string msgId)
        {
            // Obtain the message ID.
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, msgId);
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            intent.PutExtra(HMSPushReceiver.MsgId, msgId);

            SendBroadcast(intent);
        }

        public override void OnSendError(string msgId, Exception exception)
        {
            // If the sending fails, obtain the error message.
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, $"msgId:{msgId}\nError/Exception:{exception.Message}");
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            intent.PutExtra(HMSPushReceiver.MsgId, msgId);
            Android.OS.Bundle bundle = new Android.OS.Bundle();
            bundle.PutString(HMSPushReceiver.Message, exception.Message);
            bundle.PutInt(HMSPushReceiver.ErrorCode, ((BaseException)exception).ErrorCode);
            intent.PutExtra(HMSPushReceiver.Exception, bundle);
            SendBroadcast(intent);
        }

        public override void OnMessageDelivered(string msgId, Exception exception)
        {
            // Obtain the error code and description.
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, $"msgId:{msgId}\nSituation:{exception.Message}");
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            intent.PutExtra(HMSPushReceiver.MsgId, msgId);
            Android.OS.Bundle bundle = new Android.OS.Bundle();
            bundle.PutString(HMSPushReceiver.Message, exception.Message);
            bundle.PutInt(HMSPushReceiver.ErrorCode, ((BaseException)exception).ErrorCode);
            intent.PutExtra(HMSPushReceiver.Exception, bundle);
            SendBroadcast(intent);
        }

        public override void OnTokenError(Exception exception, Bundle bundle)
        {
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, exception.Message);
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            Android.OS.Bundle bundleException= new Android.OS.Bundle();
            bundleException.PutString(HMSPushReceiver.Message, exception.Message);
            bundleException.PutInt(HMSPushReceiver.ErrorCode, ((BaseException)exception).ErrorCode);
            intent.PutExtra(HMSPushReceiver.Exception, bundleException);
            intent.PutExtra(HMSPushReceiver.Bundle, bundle);
            SendBroadcast(intent);
        }

        public override void OnDeletedMessages()
        {
            string MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            Log.Info(MethodName, $"{MethodName} has triggered.");
            Intent intent = new Intent();
            intent.SetAction(HMSPushAction);
            intent.PutExtra(HMSPushReceiver.Method, System.Reflection.MethodBase.GetCurrentMethod().Name);
            SendBroadcast(intent);
        }

    }

}