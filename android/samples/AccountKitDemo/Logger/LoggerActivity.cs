﻿/*
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
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using XamarinAccountKitDemo.HmsSample;

namespace XamarinAccountKitDemo.Logger
{
    [Activity(Label = "LoggerActivity")]
    public class LoggerActivity : Activity
    {
        protected override void OnStart()
        {
            base.OnStart();
            InitalizeLogging();
        }

        /**
        * Initialize log info
        **/
        private void InitalizeLogging()
        {
            LogFragment logFragment = (LogFragment)FragmentManager.FindFragmentById(Resource.Id.framelog);
            LogCatWrapper logCat = new LogCatWrapper();
            logCat.SetNext(logFragment.GetLogView());
            Log.SetLogNode(logCat);
        }
    }
}