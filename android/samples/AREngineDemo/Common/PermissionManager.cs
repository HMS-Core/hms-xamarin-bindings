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
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// Permission manager, which provides methods for determining and applying for camera permissions.
    /// </summary>
    public class PermissionManager
    {
        private static readonly int REQUEST_CODE_ASK_PERMISSIONS = 1;
        private static readonly string[] PERMISSIONS_ARRAYS = new string[]{
        Manifest.Permission.Camera
        };

        // List of permissions to be applied for.
        private static List<string> permissionsList = new List<string>();
        private static bool isHasPermission = true;

        private PermissionManager()
        {
        }

        /// <summary>
        /// Check whether the current app has the necessary permissions (by default, the camera permission is required).
        /// If not, apply for the permission. This method should be called in the onResume method of the main activity.
        /// </summary>
        /// <param name="activity"></param>
        public static void CheckPermission(Activity activity)
        {
            foreach (string permission in PERMISSIONS_ARRAYS)
            {
                if (ContextCompat.CheckSelfPermission(activity, permission) != Permission.Granted)
                {
                    isHasPermission = false;
                    break;
                }
            }

            if (!isHasPermission)
            {
                foreach (string permission in PERMISSIONS_ARRAYS)
                {
                    if (ContextCompat.CheckSelfPermission(activity, permission) != Permission.Granted)
                    {
                        permissionsList.Add(permission);
                    }
                }
                ActivityCompat.RequestPermissions(activity,
                    permissionsList.ToArray(), REQUEST_CODE_ASK_PERMISSIONS);
            }
        }

        /// <summary>
        /// Check whether the current app has the required permissions.
        /// </summary>
        /// <param name="activity">Activity.</param>
        /// <returns>Has permission or not.</returns>
        public static bool HasPermission(Activity activity)
        {
            foreach (string permission in PERMISSIONS_ARRAYS)
            {
                if (ContextCompat.CheckSelfPermission(activity, permission) != Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }
    }
}