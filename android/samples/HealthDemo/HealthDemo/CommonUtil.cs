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

using System.Text.RegularExpressions;
using Android.Util;
using Android.Widget;
using Huawei.Hms.Common;
using Huawei.Hms.Hihealth;
using Java.Lang;

namespace XamarinHmsHealthDemo
{
    class CommonUtil
    {
        // Line separators for the display on the UI
        private static string Split = "*******************************" + System.Environment.NewLine;


        //Printout failure exception error code and error message
        //
        //@param  tag activity log tag
        //@param e   Exception object
        //@param api Interface name
        //@param logInfoView  Text View object

        public static void PrintFailureMessage(string tag, System.Exception e, string api, TextView logInfoView)
        {
            string errorCode = e.Message;
            if (e.GetType() == typeof(ApiException))
            {
                int eCode = ((ApiException)e).StatusCode;
                string errorMsg = HiHealthStatusCodes.GetStatusCodeMessage(eCode);
                Logger(api + " failure " + eCode + ":" + errorMsg, tag, logInfoView);
                return;
            }
            else if (Regex.IsMatch(errorCode, @"^\d+$"))
            {
                string errorMsg = HiHealthStatusCodes.GetStatusCodeMessage(Integer.ParseInt(errorCode));
                Logger(api + " failure " + errorCode + ":" + errorMsg, tag, logInfoView);
                return;
            }
            else
            {
                Logger(api + " failure " + errorCode, tag, logInfoView);
            }
            Logger(Split, tag, logInfoView);
        }

        
        //Send the operation result logs to the logcat and TextView control on the UI
        //
        //@param string indicating the log string
        //@param  tag activity log tag
        //@param logInfoView  Text View object
        
        public static void Logger(string str, string tag, TextView logInfoView)
        {
            Log.Info(tag, str);
            logInfoView.Append(str + System.Environment.NewLine);
            int offset = logInfoView.LineCount * logInfoView.LineHeight;
            if (offset > logInfoView.Height)
            {
                logInfoView.ScrollTo(0, offset - logInfoView.Height);
            }
        }
    }
}