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

namespace XHms_Map_Kit_Demo_Project.Utils
{
    public class CheckUtils
    {
        /// <summary>
        /// Check whether the character string is a number.
        /// </summary>
        public static bool IsNumber(string value)
        {
            return IsInteger(value) || IsDouble(value);
        }

        /// <summary>
        /// Check whether the character string is an integer.
        /// </summary>
        public static bool IsInteger(string value)
        {
            int i = 0;
            bool result = int.TryParse(value, out i);

            return result;
        }

        /// <summary>
        /// Check whether the character string is a double.
        /// </summary>
        public static bool IsDouble(string value)
        {
            double i = 0;
            bool result = double.TryParse(value, out i);

            return result;
        }

        public static bool CheckIsEdit(string str)
        {
            return ((str.Length == 0) || (str == null) || ("".Equals(str)));
        }

        public static bool CheckIsRight(string str)
        {
            return IsNumber(str);
        }
    }
}