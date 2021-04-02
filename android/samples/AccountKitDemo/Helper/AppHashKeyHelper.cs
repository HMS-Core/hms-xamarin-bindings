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
using Android.Content;
using Android.Content.PM;
using Android.Util;
using Java.Security;
using Java.Util;
using XamarinAccountKitDemo.HmsSample;
using Signature = Android.Content.PM.Signature;

namespace XamarinAccountKitDemo.Helper
{
    public class AppHashKeyHelper
    {
        private static string HashType = "SHA-256";
        private static int HashedBytes = 9;
        private static int Base64Char = 11;

        /// <summary>
        /// Retrieve the app signed package signature
        /// known as signed keystore file hex string
        /// </summary>
        /// <param name="context">Android app Context.</param>
        /// <returns>Package signatures</returns>
        private static string GetPackageSignature(Context context)
        {
            PackageManager packageManager = context.PackageManager;
            IList<Signature> signatures;
            try 
            {
                signatures = packageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Signatures).Signatures;
            }
            catch(Exception e)
            {
                Log.Info(HuaweiIdActivity.TAG, "Package name inexistent. Exception:" + e.Message);
                return "";
            }

            if (null == signatures || 0 == signatures.Count)
            {
                Log.Info(HuaweiIdActivity.TAG, "signature is null.");
                return "";
            }

            return signatures.First().ToCharsString();
        }

        /// <summary>
        /// Gets the app hash key.
        /// </summary>
        /// <returns>The app hash key.</returns>
        /// <param name="context">Android app Context.</param>
        public static string GetAppHashKey(Context context)
        {
            string keystoreHexSignature = GetPackageSignature(context);

            String appInfo = context.PackageName + " " + keystoreHexSignature;

            try
            {
                MessageDigest messageDigest = MessageDigest.GetInstance(HashType);
                messageDigest.Update(Encoding.UTF8.GetBytes(appInfo));
                byte[] hashSignature = messageDigest.Digest();

                hashSignature = Arrays.CopyOfRange(hashSignature, 0, HashedBytes);
                String base64Hash = Android.Util.Base64.EncodeToString(hashSignature, Base64Flags.NoPadding | Base64Flags.NoWrap);
                base64Hash = base64Hash.Substring(0, Base64Char);

                return base64Hash;
            }
            catch (NoSuchAlgorithmException e)
            {
                return null;
            }
        }
    }
}