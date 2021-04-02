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
using Javax.Crypto;
using Javax.Crypto.Spec;
using Java.Security.Spec;
using Java.Security;
using Java.Nio.Charset;

namespace XamarinHmsWalletDemo.Utils
{
    public class AESUtils
    {
        public AESUtils() { }
        /// <summary>
        /// AES-GCM encryption.
        /// </summary>
        /// <param name="plainData">the data to be encrypted.</param>
        /// <param name="secretKeyStr"> encryption secret key.</param>
        /// <param name="iv">encryption random iv.</param>
        /// <returns>the encrypted string.</returns>
        public static string EncryptByGcm(string plainData, string secretKeyStr,sbyte[] iv)
        {
            try
            {
                sbyte[] secretKeyByte = (sbyte[])(Array)Encoding.UTF8.GetBytes(secretKeyStr);
                sbyte[] plainByte = (sbyte[])(Array)Encoding.UTF8.GetBytes(plainData);
                SecretKeySpec secretKey = new SecretKeySpec((byte[])(Array)secretKeyByte, "AES");
                Cipher cipher = Cipher.GetInstance("AES/GCM/NoPadding");
                IAlgorithmParameterSpec spec = new GCMParameterSpec(128, (byte[])(Array)iv);
                cipher.Init((CipherMode)1, secretKey, spec);
                byte[] fBytes = cipher.DoFinal((byte[])(Array)plainByte);
                sbyte[] fSBytes = (sbyte[])(Array)fBytes;
                return new string(HwHex.EncodeHexString(fSBytes));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        /// <summary>
        /// encryption random iv.
        /// </summary>
        /// <param name="size">iv length</param>
        /// <returns>encryption the byte array.</returns>
        public static sbyte[] GetIvByte(int size)
        {
            try
            {
                SecureRandom secureRandom = SecureRandom.GetInstance("SHA1PRNG");
                sbyte[] bytes = new sbyte[size];
                secureRandom.NextBytes((byte[])(Array)bytes);
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}