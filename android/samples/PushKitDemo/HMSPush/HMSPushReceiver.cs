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
using Huawei.Hms.Push;
using System;

namespace XamarinHmsPushDemo.HMSPush
{
    public class HMSPushReceiver : BroadcastReceiver
    {
        public const string Method = "method";
        public const string Message = "message";
        public const string MsgId = "msgId";
        public const string Exception = "exception";
        public const string ErrorCode = "errorCode";
        public const string ErrorMessage = "errorMessage";
        public const string Token = "token";
        public const string Bundle = "bundle";
        public override void OnReceive(Context context, Intent intent)
        {
            if (context.GetType() != typeof(MainActivity) || intent.Extras == null) return;

            if (context is IHmsPushEvent hmsPushEvent)//If IHmsPushEvent has implemented on Activity
            {
                Bundle bundle = intent.Extras;
                if (bundle.ContainsKey(Method))
                {
                    string method = intent.Extras.GetString(Method);
                    if (method == ((Action<string,Bundle>)hmsPushEvent.OnNewToken).Method.Name)
                        hmsPushEvent.OnNewToken(bundle.GetString(Token),bundle.GetBundle(Bundle));
                    else if (method == ((Action<RemoteMessage>)hmsPushEvent.OnMessageReceived).Method.Name)
                        hmsPushEvent.OnMessageReceived(bundle.Get(Message) as RemoteMessage);
                    else if (method == ((Action<string>)hmsPushEvent.OnMessageSent).Method.Name)
                        hmsPushEvent.OnMessageSent(bundle.GetString(MsgId));
                    else if (method == ((Action<string, int, string>)hmsPushEvent.OnSendError).Method.Name)
                        hmsPushEvent.OnSendError(
                            bundle.GetString(MsgId),
                            bundle.GetBundle(Exception).GetInt(ErrorCode),
                            bundle.GetBundle(Exception).GetString(ErrorMessage));
                    else if (method == ((Action<string, int, string>)hmsPushEvent.OnMessageDelivered).Method.Name)
                        hmsPushEvent.OnMessageDelivered(
                            bundle.GetString(MsgId),
                            bundle.GetBundle(Exception).GetInt(ErrorCode),
                            bundle.GetBundle(Exception).GetString(ErrorMessage));
                    else if (method == ((Action<int, string, Bundle>)hmsPushEvent.OnTokenError).Method.Name)
                        hmsPushEvent.OnTokenError(
                            bundle.GetBundle(Exception).GetInt(ErrorCode),
                            bundle.GetBundle(Exception).GetString(ErrorMessage),
                            bundle.GetBundle(Bundle));
                    else if (method == ((Action)hmsPushEvent.OnDeletedMessages).Method.Name)
                        hmsPushEvent.OnDeletedMessages();
                }
            }
        }
    }
}