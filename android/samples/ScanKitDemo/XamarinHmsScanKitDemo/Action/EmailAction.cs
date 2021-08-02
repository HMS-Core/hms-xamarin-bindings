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
using Android.Net;
using Huawei.Hms.Ml.Scan;

namespace XamarinHmsScanKitDemo.Action
{
    class EmailAction
    {
        public static Intent GetEmailInfo(HmsScan.EmailContent emailContent)
        {
            Uri uri = Uri.Parse("mailto:" + emailContent.AddressInfo);
            string[] tos = { emailContent.AddressInfo };
            Intent intent = new Intent(Intent.ActionSendto, uri);
            intent.PutExtra(Intent.ExtraEmail, tos);
            intent.PutExtra(Intent.ExtraSubject, emailContent.SubjectInfo);
            intent.PutExtra(Intent.ExtraText, emailContent.BodyInfo);
            return intent;
        }
    }
}