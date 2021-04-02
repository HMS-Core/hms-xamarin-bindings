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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Kit.Awareness.Barrier;
using Huawei.Hms.Kit.Awareness.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AwarenessKitDemo.HMSSample
{
    public static class Constant
    {
        public static string GetCapabilityStatus(int code)
        {
            Type type = typeof(CapabilityStatus);
            FieldInfo[] fieldInfos = type.GetFields();

            string result = fieldInfos.ToList().Where(x => Convert.ToInt32(type.GetField(x.Name).GetValue(null)) == code).First().Name.Replace("AwaCapCode", "");

            return result;
        }
        public static string GetBehavior(int code)
        {
            Type type = typeof(BehaviorBarrier);
            FieldInfo[] fieldInfos = type.GetFields();

            string result = fieldInfos.ToList().Where(x => Convert.ToInt32(type.GetField(x.Name).GetValue(null)) == code).First().Name.Replace("Behavior", "");

            return result;
        }
        public static string GetTimeCategory(int code)
        {
            Type type = typeof(TimeBarrier);
            FieldInfo[] fieldInfos = type.GetFields();

            string result = fieldInfos.ToList().Where(x => Convert.ToInt32(type.GetField(x.Name).GetValue(null)) == code
            && x.Name.Contains("TimeCategory")).First().Name.Replace("TimeCategory", "");

            return result;
        }
    }
}