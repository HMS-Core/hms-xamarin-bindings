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

using Android.OS;
using Huawei.Hms.Push;

namespace XamarinHmsPushDemo.HMSPush
{
    public interface IHmsPushEvent
    {
        void OnDeletedMessages();
        /// <summary>
        ///  Called when a new token is received.
        /// </summary>
        /// <param name="token"></param>
        void OnNewToken(string token, Bundle bundle);
        /// <summary>
        /// Receives data messages pushed by the app server.
        /// </summary>
        /// <param name="message"></param>
        void OnMessageReceived(RemoteMessage message);
        /// <summary>
        /// Called after an uplink message is successfully sent.
        /// </summary>
        /// <param name="msgId"></param>
        void OnMessageSent(string msgId);
        /// <summary>
        /// Called after an uplink message fails to be sent.
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="errorInfo"></param>
        void OnSendError(string msgId, int errorCode ,string errorMessage);

        /// <summary>
        /// Sends the response from the app server to the app after an uplink message reaches the app server if the receipt is enabled.
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="errorCode"></param>
        /// <param name="errorMessage"></param>
        void OnMessageDelivered(string msgId, int errorCode, string errorMessage);
        /// <summary>
        /// Called when a token fails to be applied for.
        /// </summary>
        /// <param name="exp"></param>
        void OnTokenError(int errorCode, string errorMessage, Bundle bundle);
    }
}