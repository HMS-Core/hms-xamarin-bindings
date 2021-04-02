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

using Android.Content;
using Android.Net;
using Android.Util;


namespace XamarinHmsNearbyDemoMessage
{
    class NetCheckUtil
    {
        private static string TAG = "NetCheckUtil";
        public static bool IsNetworkAvailable(Context context)
        {
            bool mobileConnection = IsMobileConnection(context);
            bool wifiConnection = IsWifiConnection(context);
            if (!mobileConnection && !wifiConnection)
            {
                Log.Info(TAG, "No network available");
                return false;
            }
            return true;
        }

       
       //Is Mobile Connection
       
       //@param context Context
       //@return true:Mobile is connection

        public static bool IsMobileConnection(Context context)
        {
            Object obj = context.GetSystemService(Context.ConnectivityService);
            if (!(obj.GetType()== typeof(ConnectivityManager))) {
                return false;
            }
            ConnectivityManager manager = (ConnectivityManager)obj;
            NetworkInfo networkInfo = manager.GetNetworkInfo(ConnectivityType.Mobile);

            if (networkInfo != null && networkInfo.IsConnected)
            {
                return true;
            }
            return false;
        }

   
          //Is WIFI Connection
              
          //@param context Context
          //@return true:wifi is connection

        public static bool IsWifiConnection(Context context)
        {
            Object obj = context.GetSystemService(Context.ConnectivityService);
            if (!(obj.GetType() == typeof(ConnectivityManager)))
            {
                return false;
            }
            ConnectivityManager manager = (ConnectivityManager)obj;
            NetworkInfo networkInfo = manager.GetNetworkInfo(ConnectivityType.Wifi);

            if (networkInfo != null && networkInfo.IsConnected)
            {
                return true;
            }
            return false;
        }
    }
}