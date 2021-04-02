/*
        Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.

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

namespace XamarinHmsWalletDemo.Utils
{
    public abstract class CommonUtil
    {
        /// <summary>
        /// Check if a string is null or empty.
        /// </summary>
        /// <param name="str">the string to be checked.</param>
        /// <returns>if the string is null or empty.</returns>
        public static bool IsNull(string str) { return null == str || "".Equals(str.Trim()); }
        /// <summary>
        /// Check if an object is null.
        /// </summary>
        /// <param name="obj">the object to be checked.</param>
        /// <returns>if the object is null.</returns>
        public static bool IsNull(Object obj) { return obj == null; }

    }
}