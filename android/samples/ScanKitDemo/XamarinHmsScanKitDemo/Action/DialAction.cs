﻿/**
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
using Android.Net;
using Huawei.Hms.Ml.Scan;

namespace XamarinHmsScanKitDemo.Action
{
    class DialAction
    {
        public static Intent getDialIntent(HmsScan.TelPhoneNumber telPhoneNumber)
        {
            Uri uri = Uri.Parse("tel:" + telPhoneNumber.PhoneNumber);
            Intent intent = new Intent(Intent.ActionDial, uri);
            return intent;
        }
    }
}