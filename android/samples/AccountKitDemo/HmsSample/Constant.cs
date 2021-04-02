/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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

namespace XamarinAccountKitDemo.HmsSample
{
    public class Constant
    {

        public static readonly int IsLog = 1;
        //login
        public static readonly int RequestSignInLogin = 1002;
        //login by code
        public static readonly int RequestSignInLoginCode = 1003;

        /**
         * your app’s client ID,please replace it of yours
         */
        public static readonly String ClientId = "488188123478492160";

        public static readonly String TokenVerificationUrl = "https://oauth-login.cloud.huawei.com/oauth2/v3/tokeninfo?id_token=";
        /**
         *  Id Token issue
         */
        public static readonly String IdTokenIssue = "https://accounts.huawei.com";

    }
    
}