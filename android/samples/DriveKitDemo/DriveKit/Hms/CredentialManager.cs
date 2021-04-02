/*
        Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Content;
using Huawei.Cloud.Base.Auth;
using Huawei.Cloud.Client.Exception;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Hms
{
    /// <summary>
    /// Credential Manager.
    /// </summary>
    public class CredentialManager
    {      
        public static CredentialManager Instance
        {
            get { return InnerHolder.sInstance; }
        }

        //Drive Credential Info
        private DriveCredential mCredential;
        public DriveCredential Credential
        {
            get { return mCredential; }
        }

        private CredentialManager()
        {
        }

        /// <summary>
        /// Singleton CredentialManager instance.
        /// </summary>
        private static class InnerHolder
        {
            public static CredentialManager sInstance = new CredentialManager();
        }

        /// <summary>
        /// init the drive sdk witch application Context and HwId Account info(uid,countryCode,at),
        /// register a AccessMethod to get a new accessToken while accessToken is expired.
        /// </summary>
        /// <param name="unionId">unionID from HwID</param>
        /// <param name="at">access token</param>
        /// <param name="refreshAt">a callback to refresh AT</param>
        /// <returns>DriveCode</returns>
        public int Init(string unionId, string at, DriveCredential.IAccessMethod refreshAt)
        {
            if (String.IsNullOrEmpty(unionId) || String.IsNullOrEmpty(at))
            {
                return DriveCode.Error;
            }
            DriveCredential.Builder builder = new DriveCredential.Builder(unionId, refreshAt);
            mCredential = builder.Build().SetAccessToken(at);
            return DriveCode.Success;
        }

        /// <summary>
        /// Exit the Drive and clear all cache information during Drive usage.
        /// </summary>
        /// <param name="context">context</param>
        public void Exit(Context context)
        {
            // clear cache
            DeleteFile(context.CacheDir);
            DeleteFile(context.FilesDir);
        }

        /// <summary>
        /// Clear cache.
        /// </summary>
        /// <param name="file">Specified cache file</param>
        private static void DeleteFile(Java.IO.File file)
        {
            if (null == file || !file.Exists())
            {
                return;
            }

            if (file.IsDirectory)
            {
                Java.IO.File[] files = file.ListFiles();
                if (files != null)
                {
                    foreach (Java.IO.File f in files)
                    {
                        DeleteFile(f);
                    }
                }
            }
        }

    }
}