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
using Android.Util;
using Android.Views;
using Android.Widget;
using XamarinAccountKitDemo.Logger.Interface;
using Java.Lang;

namespace XamarinAccountKitDemo.Logger
{
    public class Log
    {
        public static readonly int Debug = (int)LogPriority.Debug;

        public static readonly int Info = (int)LogPriority.Info;

        public static readonly int Warn = (int)LogPriority.Warn;

        public static readonly int Error = (int)LogPriority.Error;

        private static ILogNode logNode;

        public static ILogNode GetLogNode() { return logNode; }

        public static void SetLogNode(ILogNode node) { logNode = node; }

        public static void DebugFunc(string tag, string msg, Throwable tr)
        {
            WriteLine(Debug, tag, msg, tr);
        }

        public static void DebugFunc(string tag, string msg)
        {
            DebugFunc(tag, msg, null);
        }

        public static void InfoFunc(string tag, string msg, Throwable tr)
        {
            WriteLine(Info, tag, msg, tr);
        }

        public static void InfoFunc(string tag, string msg)
        {
            InfoFunc(tag, msg, null);
        }

        public static void WarnFunc(string tag, string msg, Throwable tr)
        {
            WriteLine(Warn, tag, msg, tr);
        }

        public static void WarnFunc(string tag, string msg)
        {
            WarnFunc(tag, msg, null);
        }

        public static void WarnFunc(string tag, Throwable tr)
        {
            WarnFunc(tag, null, tr);
        }

        public static void ErrorFunc(string tag, string msg, Throwable tr)
        {
            WriteLine(Error, tag, msg, tr);
        }

        public static void ErrorFunc(string tag, string msg)
        {
            ErrorFunc(tag, msg, null);
        }

        public static void WriteLine(int priority, string tag, string msg, Throwable tr)
        {
            if (logNode != null)
            {
                logNode.WriteLine(priority, tag, msg, tr);
            }
        }

        public static void WriteLine(int priority, string tag, string msg)
        {
            WriteLine(priority, tag, msg, null);
        }
    }
}