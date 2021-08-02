/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */

using Android.Content;
using Android.Content.PM;
using Android.Net;
using Huawei.Hms.Ml.Scan;
using Java.Lang;

namespace XamarinHmsScanKitDemo.Action
{
    class LocationAction
    {

        public static string GAODE_PKG = "com.autonavi.minimap";

        public static bool CheckMapAppExist(Context context)
        {
            PackageInfo packageInfo = null;
            try
            {
                packageInfo = context.PackageManager.GetPackageInfo(GAODE_PKG, 0);
            }
            catch (Exception e)
            {
               
            }
            if (packageInfo == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        public static Intent GetLocationInfo(HmsScan.LocationCoordinate locationCoordinate)
        {
            Intent intent = new Intent(Intent.ActionView, Uri.Parse("androidamap://viewMap?lat=" + locationCoordinate.Latitude + "&lon=" + locationCoordinate.Longitude));
            return intent;
        }
    }
}