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
using System;

namespace AwarenessKitDemo.HMSSample
{
    public static class Utils
    {
        public static void OpenHMSCoreAppDetail()
        {
            try
            {
                var context = Application.Context;
                Android.Content.PM.PackageInfo packageInfo = context.PackageManager.GetPackageInfo("com.huawei.hwid", 0);
                Intent intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.AddCategory(Intent.CategoryDefault);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageInfo.PackageName));
                intent.AddFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}