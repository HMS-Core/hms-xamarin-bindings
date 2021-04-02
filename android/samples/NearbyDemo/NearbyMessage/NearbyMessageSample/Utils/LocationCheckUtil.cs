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

using Android.Content;
using Android.Locations;
namespace XamarinHmsNearbyDemoMessage
{
    class LocationCheckUtil
    {
        public static bool IsLocationEnabled(Context context)
        {
           Java.Lang.Object obj = context.GetSystemService(Context.LocationService);
            if (!(obj.GetType()== typeof(LocationManager))) {
                return false;
            }
            LocationManager locationManager = (LocationManager)obj;
            return locationManager.IsProviderEnabled(LocationManager.GpsProvider);
        }
    }
}