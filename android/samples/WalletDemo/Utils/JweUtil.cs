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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Nio.Charset;
using Java.Util.Zip;
using Newtonsoft.Json.Linq;

namespace XamarinHmsWalletDemo.Utils
{
    public class JweUtil
    {
        public static string GenerateJwe(string issuerId, string dataJson)
        {
            string jwePrivateKey = Constant.PrivateKey;
            string sessionKeyPublicKey = Constant.SessionPublicKey;
            string sessionKey = RandomUtils.GenerateSecureRandomFactor(16);
            JObject jObject = JObject.Parse(dataJson);
            jObject.Add("iss", issuerId);

            // The first part: JWE Head
            JweHeader jweHeader = GetHeader();
            string jweHeaderEncode = GetEncodeHeader(jweHeader);

            // The Second part: JWE Encrypted Key
            string encryptedKeyEncode = GetEncryptedKey(sessionKey, sessionKeyPublicKey);

            // The third part: JWE IV
            sbyte[] iv = AESUtils.GetIvByte(12);
            string ivHexStr = new string(HwHex.EncodeHexString(iv));
            //Java.Lang.String ivHexString = (Java.Lang.String)ivHexStr;
            string ivEncode = Base64.EncodeToString(Encoding.UTF8.GetBytes(ivHexStr), Base64Flags.UrlSafe | Base64Flags.NoWrap);

            // The fourth part: JWE CipherText empty
            string cipherTextEncode = GetCipherText(jObject.ToString(), sessionKey, iv, jweHeader);

            // The fifth part: JWE Authentication Tag
            string authenticationTagEncode =
                GetAuthenticationTag(jwePrivateKey, sessionKey, jObject.ToString(), jweHeaderEncode, ivEncode);

            Java.Lang.StringBuilder stringBuilder = new Java.Lang.StringBuilder();
            return stringBuilder.Append(jweHeaderEncode)
                .Append(".")
                .Append(encryptedKeyEncode)
                .Append(".")
                .Append(ivEncode)
                .Append(".")
                .Append(cipherTextEncode)
                .Append(".")
                .Append(authenticationTagEncode)
                .ToString();
        }


        private static JweHeader GetHeader()
        {
            JweHeader jweHeader = new JweHeader();
            jweHeader.Alg = "RSA-OAEP";
            jweHeader.Enc = "A128GCM";
            jweHeader.Kid = "1";
            jweHeader.Zip = "gzip";
            return jweHeader;
        }
        private static string GetEncodeHeader(JweHeader jweHeader)
        {
            StringBuffer stringBuffer = new StringBuffer();
            string headerJson = stringBuffer.Append("alg=")
                .Append(jweHeader.Alg)
                .Append(", enc=")
                .Append(jweHeader.Enc)
                .Append(", kid=")
                .Append(jweHeader.Kid)
                .Append(", zip=")
                .Append(jweHeader.Zip)
                .ToString();

            return Base64.EncodeToString(Encoding.UTF8.GetBytes(headerJson), Base64Flags.UrlSafe | Base64Flags.NoWrap);
        }

        private static string GetEncryptedKey(string sessionKey, string sessionKeyPublicKey)
        {
            try
            {
                //Java.Lang.String sessionKeyStr = (Java.Lang.String)sessionKey;
                string encryptedSessionKey = RSA.Encrypt((sbyte[])(Array)Encoding.UTF8.GetBytes(sessionKey), sessionKeyPublicKey,
                    "RSA/NONE/OAEPwithSHA-256andMGF1Padding", "UTF-8");
                return Base64.EncodeToString(Encoding.UTF8.GetBytes(encryptedSessionKey), Base64Flags.UrlSafe | Base64Flags.NoWrap);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            return "";
        }

        private static string GetCipherText(string dataJson, string sessionKey, sbyte[] iv, JweHeader jweHeader)
        {
            if (!"A128GCM".Equals(jweHeader.Enc))
            {
                System.Console.WriteLine("enc only support A128GCM.");
                return "";
            }
            if (!"gzip".Equals(jweHeader.Zip))
            {
                System.Console.WriteLine("zip only support gzip.");
                return "";
            }
            string payLoadEncrypt = AESUtils.EncryptByGcm(dataJson, sessionKey, iv);
            sbyte[] payLoadEncryptCompressByte = Compress((sbyte[])(Array)Encoding.UTF8.GetBytes(payLoadEncrypt));
            string cipherTextEncode = Base64.EncodeToString((byte[])(Array)payLoadEncryptCompressByte, Base64Flags.UrlSafe | Base64Flags.NoWrap);
            return cipherTextEncode;
        }
        /// <summary>
        /// gzip Compress
        /// </summary>
        /// <param name="originalBytes">Data to be compressed</param>
        /// <returns>Compressed data</returns>
        private static sbyte[] Compress(sbyte[] originalBytes)
        {
            if (originalBytes == null || originalBytes.Length == 0)
            {
                return null;
            }
            try
            {
                //using (var result = new MemoryStream())
                //{
                //    var lengthBytes = BitConverter.GetBytes(originalBytes.Length);
                //    result.Write(lengthBytes, 0, 4);

                //    using (var compressionStream = new GZipStream(result,
                //        CompressionMode.Compress))
                //    {
                //        compressionStream.Write((byte[])(Array)originalBytes, 0, originalBytes.Length);
                //        compressionStream.Flush();

                //    }
                //    return (sbyte[])(Array)result.ToArray();
                //}

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    GZIPOutputStream outputStream = new GZIPOutputStream(memoryStream);
                    outputStream.Write((byte[])(Array)originalBytes);
                    outputStream.Finish();
                    return (sbyte[])(Array)memoryStream.ToArray();
                }


            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
                return null;
            }
        }

        private static string GetAuthenticationTag(string jwePrivateKey, string sessionKey, string payLoadJson, string jweHeaderEncode, string ivEncode)
        {
            StringBuffer stringBuffer = new StringBuffer();
            string signContent = stringBuffer.Append(jweHeaderEncode)
                .Append(".")
                .Append(sessionKey)
                .Append(".")
                .Append(ivEncode)
                .Append(".")
                .Append(payLoadJson)
                .ToString();
            return RSA.SignContent(signContent, jwePrivateKey);
        }



    }
}