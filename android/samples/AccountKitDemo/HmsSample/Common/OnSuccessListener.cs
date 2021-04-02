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
using XamarinAccountKitDemo.Logger;

namespace XamarinAccountKitDemo.HmsSample.Common
{
    public class OnSuccessListener : Java.Lang.Object, Huawei.Hmf.Tasks.IOnSuccessListener
    {
        //Message when task is successful
        private string successMessage;

        public OnSuccessListener(string SuccessMessage)
        {
            this.successMessage = SuccessMessage;
        }
        public void OnSuccess(Java.Lang.Object p0)
        {
            Log.InfoFunc(HuaweiIdActivity.TAG, successMessage);
        }
    }
}