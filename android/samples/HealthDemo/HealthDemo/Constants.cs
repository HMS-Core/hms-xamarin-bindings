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


namespace XamarinHmsHealthDemo
{
    class Constants
    {
        //Request code for displaying the sign in authorization screen using the startActivityForResult method.
        public static int RequestSignInLogin = 1002;
        //Request code for displaying the HUAWEI Health authorization screen using the startActivityForResult method.
        public static int RequestHealthAuth = 1003;
        //Scheme of Huawei Health Authorization Activity
        public static string HEALTH_APP_SETTING_DATA_SHARE_HEALTHKIT_ACTIVITY_SCHEME = "huaweischeme://healthapp/achievement?module=kit";
        //Error Code: can not resolve HUAWEI Health Authorization Activity
        public static string AppHealthNotInstalled = "50033";
    }
}