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

using Android.OS;
using Android.Util;
using Java.Util;
using Org.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using static Huawei.Hms.Push.LocalNotification.LocalNotificationConstants;

namespace Huawei.Hms.Push
{
    public static class CoreUtils
    {
        public static int GetRandomInt()
        {
            System.Random rnd = new System.Random();
            int result = rnd.Next();
            return result;
        }

        public static JSONObject ToJsonObject(this Bundle bundle)
        {
            if (bundle == null) return null;
            JSONObject result = new JSONObject();
            List<string> keys = bundle.KeySet().ToList();
            foreach (string key in keys)
            {
                try
                {
                    object value = bundle.Get(key);
                    Log.Info(key, value.GetType().ToString());
                    if (value == null) continue;
                    Type t = value.GetType();
                    if (value is Java.Lang.Boolean boolean)
                        result.Put(key, boolean);
                    else if (value is Java.Lang.String @string)
                        result.Put(key, @string);
                    else if (value is Java.Lang.Double @double)
                        result.Put(key, @double);
                    else if (value is Java.Lang.Integer integer)
                        result.Put(key, integer);
                    else
                    {
                        if (key == NotificationConstants.Actions)
                        {
                            var stringArray = bundle.GetStringArray(key);
                            result.Put(key, StringArrayToString(stringArray));
                        }
                        else
                            result.Put(key, value.ToString());
                    }
                }
                catch (Exception exception)
                {
                }
            }
            return result;
        }

        public static Dictionary<string, string> ToDictionary(this Bundle bundle)
        {
            if (bundle == null) return null;
            return GetData(bundle.ToJsonObject().ToString());
        }

        public static Dictionary<string, string> GetData(string json)
        {
            if (json == null) return null;

            Dictionary<string, string> result = new Dictionary<string, string>();
            JSONObject jObj = new JSONObject(json);

            IIterator itr = jObj.Keys();
            while (itr.HasNext)
            {
                string key = itr.Next().ToString();
                result[key] = jObj.GetString(key);
            }

            return result;
        }

        public static string DictionaryToString(this IDictionary<string, string> dict)
        {
            if (dict == null) return null;
            string result = string.Empty;
            foreach (var item in dict)
            {
                if (item.Value == null || item.Value == string.Empty) continue;
                result += item.Key + ": " + item.Value + "\n";
            }
            return result;
        }

        public static string GetString(this JSONObject jSONObject, string name, string defaultValue)
        {
            try
            {
                return jSONObject.GetString(name);
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string StringArrayToString(this string[] list)
        {
            string result = string.Empty;

            foreach (var item in list)
            {
                result += $", {item}";
            }

            return result[2..];
        }
    }
}