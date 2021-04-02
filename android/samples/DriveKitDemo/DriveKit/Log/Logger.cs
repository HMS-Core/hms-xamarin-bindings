/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using XHms_Drive_Kit_Demo_Project.DriveKit.Config;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Log
{
    public class Logger
    {
        private static readonly string TagPrefix = "CloudTest";

        private static string SetTag(string tag)
        {
            return String.Format("[{0}]{1}.{2}", Configure.VersionName, TagPrefix, tag);
        }

        /// <summary>
        /// Print info level log.
        /// </summary>
        /// <param name="tag">Class label</param>
        /// <param name="msg">message</param>
        public static void Info(string tag, string msg)
        {
            if (tag == null)
            {
                return;
            }

            Android.Util.Log.Info(SetTag(tag), msg);
        }

        /// <summary>
        /// Print debug log.
        /// </summary>
        /// <param name="tag">Class label</param>
        /// <param name="msg">message</param>
        public static void Debug(string tag, string msg)
        {
            if (tag == null)
            {
                return;
            }

            Android.Util.Log.Debug(SetTag(tag), msg);

        }

        /// <summary>
        /// Print warning log.
        /// </summary>
        /// <param name="tag">Class label</param>
        /// <param name="msg">message</param>
        public static void Warn(string tag, string msg)
        {
            if (tag == null)
            {
                return;
            }

            Android.Util.Log.Warn(SetTag(tag), msg);
        }

        /// <summary>
        /// Print verbose log.
        /// </summary>
        /// <param name="tag">Class label</param>
        /// <param name="msg">message</param>
        public static void Verbose(string tag, string msg)
        {
            if (tag == null)
            {
                return;
            }

            Android.Util.Log.Verbose(SetTag(tag), msg);
        }

        /// <summary>
        /// Print error log.
        /// </summary>
        /// <param name="tag">Class label</param>
        /// <param name="msg">message</param>
        public static void Error(string tag, string msg)
        {
            if (tag == null)
            {
                return;
            }

            Android.Util.Log.Error(SetTag(tag), msg);
        }
    }
}