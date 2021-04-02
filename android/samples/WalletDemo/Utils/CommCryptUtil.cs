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
using Java.Security;

namespace XamarinHmsWalletDemo.Utils
{
    public class CommCryptUtil
    {
        public CommCryptUtil() { }

        public static string Byte2HexStr(sbyte[] array)
        {
            return array == null ? null : new string(HwHex.EncodeHex(array, false));
        }
        public static sbyte[] HexStr2Byte(string hexStr)
        {
            return hexStr == null ? new sbyte[0] : HwHex.DecodeHex(hexStr.ToCharArray());
        }

        public static sbyte[] GenSecureRandomByte(int byteSize)
        {
            SecureRandom secureRandom = new SecureRandom();
            sbyte[] bytes = new sbyte[byteSize];
            secureRandom.NextBytes((byte[])(Array)bytes);
            return bytes;        }
    }
}