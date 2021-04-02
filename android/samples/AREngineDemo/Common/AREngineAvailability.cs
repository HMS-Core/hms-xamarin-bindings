/*
*       Copyright 2020-2021 Huawei Technologies Co., Ltd. All rights reserved.

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
using Huawei.Hiar;

namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// This activity is used to redirect the user to AppGallery and install the AR Engine from server.
    /// This activity is called when the AR Engine is not installed.
    /// </summary>
    public class AREngineAvailability
    {
        private const string DOWNLOAD_APP_ACTION = "com.huawei.appmarket.intent.action.AppDetail";

        private const string HUAWEI_MARKET_NAME = "com.huawei.appmarket";

        private const string PACKAGE_NAME_KEY = "APP_PACKAGENAME";

        private const string PACKAGENAME_ARSERVICE = "com.huawei.arengine.service";

        private AREngineAvailability()
        {
        }

        public static bool IsArEngineServiceApkReady(Context context)
        {
            return AREnginesApk.IsAREngineApkReady(context);
        }

        public static void NavigateToAppMarketPage(Activity activity)
        {
            Intent intent = new Intent(DOWNLOAD_APP_ACTION);
            intent.PutExtra(PACKAGE_NAME_KEY, PACKAGENAME_ARSERVICE);
            intent.SetPackage(HUAWEI_MARKET_NAME);
            intent.SetFlags(ActivityFlags.NewTask);
            activity.StartActivity(intent);
        }
    }
}