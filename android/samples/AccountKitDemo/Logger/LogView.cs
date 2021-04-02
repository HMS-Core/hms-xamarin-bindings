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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using XamarinAccountKitDemo.Logger.Interface;
using Java.Lang;
using Java.Text;
using Java.Util;

namespace XamarinAccountKitDemo.Logger
{
    public class LogView : TextView, ILogNode
    {
        private ILogNode next;

        public LogView(Context context) :
            base(context)
        {
            Initialize();
        }

        public LogView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public LogView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
        }

        public ILogNode GetNext() { return next; }

        public void SetNext(ILogNode node) { next = node; }

        public void WriteLine(int priority, string tag, string msg, Throwable tr)
        {
            string priorityStr = null;
            switch (priority)
            {
                // Debug
                case 3:
                    priorityStr = "D";
                    break;
                // Info
                case 4:
                    priorityStr = "I";
                    break;
                // Warn
                case 5:
                    priorityStr = "W";
                    break;
                // Error
                case 6:
                    priorityStr = "E";
                    break;
                default:
                    break;
            }

            string exceptionStr = null;
            if (tr != null)
            {
                exceptionStr = Android.Util.Log.GetStackTraceString(tr);
            }
            System.Text.StringBuilder outputBuilder = new System.Text.StringBuilder();

            CultureInfo culture = new CultureInfo("tr-TR");
            // Get current UTC time.   
            DateTime utcDate = DateTime.UtcNow;
            // Change time to match GMT + 1.
            DateTime gmt1Date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcDate, TimeZoneInfo.Local.Id);
            // Output the GMT+1 time in our specified format using the TR-culture. 
            string str = gmt1Date.ToString(" HH:mm:ss", culture);

            outputBuilder.Append(str);
            outputBuilder.Append(" ");
            outputBuilder.Append(msg);
            outputBuilder.Append("\r\n");

            ((Activity)Context).RunOnUiThread(() =>
            {
                AppendToLog(outputBuilder.ToString());
            });

            if (next != null)
            {
                next.WriteLine(priority, tag, msg, tr);
            }
        }

        public void AppendToLog(string s)
        {
            Append("\n" + s);
        }

        private System.Text.StringBuilder AppendIfNotNull(System.Text.StringBuilder source, string addStr, string delimiter)
        {
            if (addStr != null)
            {
                if (addStr.Length == 0)
                {
                    delimiter = "";
                }

                return source.Append(addStr).Append(delimiter);
            }
            return source;
        }
    }
}