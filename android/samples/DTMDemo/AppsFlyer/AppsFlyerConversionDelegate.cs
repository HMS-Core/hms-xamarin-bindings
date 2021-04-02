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
using Android.Support.V7.App;
using Com.Appsflyer;
using System;
using System.Collections.Generic;

namespace XamarinHmsDTMDemo.AppsFlyer
{
    public class AppsFlyerConversionDelegate : Java.Lang.Object, IAppsFlyerConversionListener
    {
        AppCompatActivity activity;

        public AppsFlyerConversionDelegate(MainActivity activity)
        {
            this.activity = activity;
            Console.WriteLine("AppsFlyerConversionDelegate called");
        }

        public void OnAppOpenAttribution(IDictionary<string, string> p0)
        {
            string message = "OnAppOpenAttribution:\n";
            foreach (var kvp in p0)
            {
                message = message + kvp.Key.ToString() + " = " + kvp.Value.ToString() + "\n";
            }
            Console.WriteLine(message);
        }

        public void OnAttributionFailure(string p0)
        {
            string message = "OnAttributionFailure = " + p0;
            Console.WriteLine(message);
        }

        public void OnConversionDataFail(string p0)
        {
            string message = "OnInstallConversionFailure = " + p0;
            Console.WriteLine(message);
        }

        public void OnConversionDataSuccess(IDictionary<string, Java.Lang.Object> p0)
        {
            string message = "OnConversionDataSuccess:\n";
            foreach (var kvp in p0)
            {
                if (kvp.Value != null)
                {
                    message = message + kvp.Key.ToString() + " = " + kvp.Value.ToString() + "\n";
                }
            }
            message = message + "Timestamp:" + DateTime.Now;
            Console.WriteLine(message);
        }
    }
}