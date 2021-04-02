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
using Android.Views;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Common;
using XamarinAccountKitDemo.Logger;

namespace XamarinAccountKitDemo.HmsSample.Common
{
    public class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        //Message when task is successful
        private string successMessage;
        //Message when task is failed
        private string failureMessage;

        public OnCompleteListener(string SuccessMessage, string FailureMessage)
        {
            this.successMessage = SuccessMessage;
            this.failureMessage = FailureMessage;
        }
        public void OnComplete(Task task)
        {
            if (task.IsSuccessful)
            {
                //do some thing while cancel success
                Log.InfoFunc(HuaweiIdActivity.TAG, successMessage);
            }
            else
            {
                //do some thing while cancel failed
                Exception exception = task.Exception;
                if (exception is ApiException)
                {
                    int statusCode = ((ApiException)exception).StatusCode;
                    Log.InfoFunc(HuaweiIdActivity.TAG, failureMessage + ": " + statusCode);
                }
            }
        }
    }
}