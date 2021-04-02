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

namespace XamarinHmsWalletDemo.Utils
{
    public class Base64Util
    {
        
        private static string Encoding = "iso8859-1";
        public static string UsAscii = "US-ASCII";

        private static char[] Base64EncodeChars = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
        'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
        'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3',
        '4', '5', '6', '7', '8', '9', '+', '/'};

        private static sbyte[] Base64DecodeChars = new sbyte[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        62, -1, -1, -1, 63, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7,
        8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29,
        30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1};

        private Base64Util() { }

        public static string Encode(byte[] var0)
        {
            StringBuffer var1 = new StringBuffer();
            int var2 = var0.Length;
            int var3 = 0;

            while (var3 < var2)
            {
                int var4 = var0[var3++] & 255;
                if (var3 == var2)
                {
                    var1.Append(Base64EncodeChars[var4 >> 2]);
                    var1.Append(Base64EncodeChars[(var4 & 3) << 4]);
                    var1.Append("==");
                    break;
                }

                int var5 = var0[var3++] & 255;
                if (var3 == var2)
                {
                    var1.Append(Base64EncodeChars[var4 >> 2]);
                    var1.Append(Base64EncodeChars[(var4 & 3) << 4 | (var5 & 240) >> 4]);
                    var1.Append(Base64EncodeChars[(var5 & 15) << 2]);
                    var1.Append("=");
                    break;
                }

                int var6 = var0[var3++] & 255;
                var1.Append(Base64EncodeChars[var4 >> 2]);
                var1.Append(Base64EncodeChars[(var4 & 3) << 4 | (var5 & 240) >> 4]);
                var1.Append(Base64EncodeChars[(var5 & 15) << 2 | (var6 & 192) >> 6]);
                var1.Append(Base64EncodeChars[var6 & 63]);
            }

            return var1.ToString();
        }

        public static byte[] decode(string var0)
        {
            StringBuffer var1 = new StringBuffer();
            Java.Lang.String var0Str = (Java.Lang.String)var0.ToString();
            sbyte[] var2 = (sbyte[])(Array)var0Str.GetBytes("US-ASCII");
            int var3 = var2.Length;
            int[] var4 = new int[] { 0 };

            while (var4[0] < var3)
            {
                sbyte var5 = GetValue(var2, var3, var4);

                if (var5 == -1)
                {
                    break;
                }

                sbyte var6 = GetValue(var2, var3, var4);

                if (var6 == -1)
                {
                    break;
                }

                var1.Append((char)(var5 << 2 | (var6 & 48) >> 4));

                sbyte var7;
                do
                {
                    var7 = var2[var4[0]++];
                    if (var7 == 61)
                    {
                        Java.Lang.String var1Str = (Java.Lang.String)var1.ToString();
                        return  var1Str.GetBytes("iso8859-1");
                    }

                    var7 = Base64DecodeChars[var7];
                } while (var4[0] < var3 && var7 == -1);

                if (var7 == -1)
                {
                    break;
                }

                var1.Append((char)((var6 & 15) << 4 | (var7 & 60) >> 2));

                sbyte var8;
                do
                {
                    var8 = var2[var4[0]++];
                    if (var8 == 61)
                    {
                        Java.Lang.String var1Str = (Java.Lang.String)var1.ToString();
                        return var1Str.GetBytes("iso8859-1");
                    }

                    var8 = Base64DecodeChars[var8];
                } while (var4[0] < var3 && var8 == -1);

                if (var8 == -1)
                {
                    break;
                }

                var1.Append((char)((var7 & 3) << 6 | var8));
            }
            Java.Lang.String var1Strmain = (Java.Lang.String)var1.ToString();
            return var1Strmain.GetBytes("iso8859-1");
        }

        private static sbyte GetValue(sbyte[] var2, int var3, int[] var4)
        {
            sbyte value;
            do
            {
              value = Base64DecodeChars[var2[var4[0]++]];
            } while (var4[0] < var3 && value == -1);

            return value;
        }
    }
}