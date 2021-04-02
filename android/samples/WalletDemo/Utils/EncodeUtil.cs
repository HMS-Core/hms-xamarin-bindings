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
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Nio.Charset;

namespace XamarinHmsWalletDemo.Utils
{
    public class EncodeUtil
    {
        public static readonly Charset Utf8 = StandardCharsets.Utf8;
        public EncodeUtil() { }

        public static string Byte2Hex(sbyte[] array) { return CommCryptUtil.Byte2HexStr(array); }
        public static sbyte[] Hex2Byte(string hex)
        {
            try
            {
                return CommCryptUtil.HexStr2Byte(hex);
            }
            catch (Exception ex)
            {
                Log.Error("EncodeUtil", ex.Message);
                throw;
            }
        }
    }
}