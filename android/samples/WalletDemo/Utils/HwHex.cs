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
using Java.Lang;
using Java.Nio.Charset;

namespace XamarinHmsWalletDemo.Utils
{
    public class HwHex
    {
        public static Charset DefaultCharset = StandardCharsets.Utf8;
        private static char[] DigitsLower = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        private static char[] DigitsUpper = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        public static char[] EncodeHex(sbyte[] data,bool toLowerCase)
        {
            return EncodeHex(data, toLowerCase ? DigitsLower : DigitsUpper);
        }
        private static char[] EncodeHex(sbyte[] data,char[] toDigits)
        {
            int l = data.Length;
            char[] outChr = new char[l << 1];
            int i = 0;

            for (int x = 0; i < l; ++i)
            {
                outChr[x++] = toDigits[(240 & data[i]) >> 4];
                outChr[x++] = toDigits[15 & data[i]];
            }
            return outChr;
        }

        public static sbyte[] DecodeHex(char[] data)
        {
            int len = data.Length;
            if((len & 1) != 0)
            {
                throw new System.Exception("Odd number of characters.");
            }
            else
            {
                sbyte[] outChr = new sbyte[len >> 1];
                int i = 0;
                for (int j = 0; j < len; ++i)
                {
                    int f = ToDigit(data[j], j) << 4;
                    ++j;
                    f |= ToDigit(data[j], j);
                    ++j;
                    outChr[i] = (sbyte)(f & 255);
                }
                return outChr;
            }
        }

        private static int ToDigit(char ch, int index)
        {
            int digit = Character.Digit(ch, 16);
            if(digit == -1)
            {
                throw new System.Exception("Illegal hexadecimal character " + ch + " at index " + index);
            }
            else
            {
                return digit;
            }
        }
        public static string EncodeHexString(sbyte[] data)
        {
            return new string(EncodeHex(data));
        }
        public static char[] EncodeHex(sbyte[] data)
        {
            return EncodeHex(data, true);
        }
    }
}