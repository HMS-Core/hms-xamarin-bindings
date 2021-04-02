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

namespace XamarinHmsNearbyDemo
{
    class Constants
    {
     
     
        // Bluetooth error

        public static string BLUETOOTH_ERROR = "Bluetooth is invalid! Turn on Bluetooth and run this app again.";
    
        // Gps error

        public static string LOCATION_SWITCH_ERROR ="Location is invalid! Turn on Location and run this app again.";

        // Location error

        public static string LOCATION_ERROR = "Location permission is denied! Make sure you have Location permission and run this app again.";
       
        //The Code used in OnRequestPermissionsResult
        
        public static int REQ_CODE = 2020;
        
        //The Code used in OnActivityResult

        public static int REQ_CODE_FILE_PICKER = 2030;

    }
}