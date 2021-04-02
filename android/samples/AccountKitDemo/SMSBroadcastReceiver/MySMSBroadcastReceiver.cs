/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Common.Api;
using Huawei.Hms.Support.Api.Client;
using Huawei.Hms.Support.Sms.Common;
using XamarinAccountKitDemo.HmsSample;

namespace XamarinAccountKitDemo.SMSBroadcastReceiver
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { ReadSmsConstant.ReadSmsBroadcastAction })]
    public class MySMSBroadcastReceiver :  BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Bundle bundle = intent.Extras;
            if(bundle != null)
            {
                Status status = (Status) bundle.GetParcelable(ReadSmsConstant.ExtraStatus);
                if (status.StatusCode == (int)CommonStatusCodes.Timeout)
                {
                    // Service has timed out and no SMS message that meets the requirement is read. Service ended.

                    //DoSomethingWhenTimeOut();
                }
                else if(status.StatusCode == CommonStatusCodes.Success)
                {
                    if (bundle.ContainsKey(ReadSmsConstant.ExtraSmsMessage))
                    {
                        // An SMS message that meets the requirement is read. Service ended.

                        //doSomethingWhenGetMessage(bundle.GetString(ReadSmsConstant.ExtraSmsMessage));

                        var smsMessage = (string)bundle.GetString(ReadSmsConstant.ExtraSmsMessage);
                        Log.Info(HuaweiIdActivity.TAG, smsMessage);
                    }
                }
            }
        }
    }
}