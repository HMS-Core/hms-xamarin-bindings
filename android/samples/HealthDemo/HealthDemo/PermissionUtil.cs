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

using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;


namespace XamarinHmsHealthDemo
{
    class PermissionUtil
    {
        public static bool HasPermission(Context context, String permission)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                if (context.CheckSelfPermission(permission) != Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }

        public static void RequestPermissions(Activity activity, String[] permissions, int requestCode)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                activity.RequestPermissions(permissions, requestCode);
            }
        }

        public static String[] GetDeniedPermissions(Context context, String[] permissions)
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                List<String> deniedPermissionList = new List<String>();
                foreach (String permission in permissions)
                {
                    if (context.CheckSelfPermission(permission) != Permission.Granted)
                    {
                        deniedPermissionList.Add(permission);
                    }
                }
                int size = deniedPermissionList.Count;
                if (size > 0)
                {
                    return deniedPermissionList.ToArray();
                }
            }
            return new string[0];
        }
    }
}