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
using System.Collections.Generic;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Constants
{
    public sealed class MimeType
    {
        // Huawei Drive Folder Type
        public const string Folder = "application/vnd.huawei-apps.folder";

        // Default File Type
        public const string Default = "*/*";

        // All cached MimeType types based on file suffixes
        public static readonly IDictionary<string, string> MimeTypeMap = new Dictionary<string, string>();

        static MimeType()
        {
            MimeTypeMap.Add(".3gp", "video/3gpp");
            MimeTypeMap.Add(".apk", "application/vnd.android.package-archive");
            MimeTypeMap.Add(".asf", "video/x-ms-asf");
            MimeTypeMap.Add(".avi", "video/x-msvideo");
            MimeTypeMap.Add(".bin", "application/octet-stream");
            MimeTypeMap.Add(".bmp", "image/bmp");
            MimeTypeMap.Add(".c", "text/plain");
            MimeTypeMap.Add(".class", "application/octet-stream");
            MimeTypeMap.Add(".conf", "text/plain");
            MimeTypeMap.Add(".cpp", "text/plain");
            MimeTypeMap.Add(".doc", "application/msword");
            MimeTypeMap.Add(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            MimeTypeMap.Add(".xls", "application/vnd.ms-excel");
            MimeTypeMap.Add(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            MimeTypeMap.Add(".exe", "application/octet-stream");
            MimeTypeMap.Add(".gif", "image/gif");
            MimeTypeMap.Add(".gtar", "application/x-gtar");
            MimeTypeMap.Add(".gz", "application/x-gzip");
            MimeTypeMap.Add(".h", "text/plain");
            MimeTypeMap.Add(".htm", "text/html");
            MimeTypeMap.Add(".html", "text/html");
            MimeTypeMap.Add(".jar", "application/java-archive");
            MimeTypeMap.Add(".java", "text/plain");
            MimeTypeMap.Add(".jpeg", "image/jpeg");
            MimeTypeMap.Add(".jpg", "image/jpeg");
            MimeTypeMap.Add(".js", "application/x-javascript");
            MimeTypeMap.Add(".log", "text/plain");
            MimeTypeMap.Add(".m3u", "audio/x-mpegurl");
            MimeTypeMap.Add(".m4a", "audio/mp4a-latm");
            MimeTypeMap.Add(".m4b", "audio/mp4a-latm");
            MimeTypeMap.Add(".m4p", "audio/mp4a-latm");
            MimeTypeMap.Add(".m4u", "video/vnd.mpegurl");
            MimeTypeMap.Add(".m4v", "video/x-m4v");
            MimeTypeMap.Add(".mov", "video/quicktime");
            MimeTypeMap.Add(".mp2", "audio/x-mpeg");
            MimeTypeMap.Add(".mp3", "audio/x-mpeg");
            MimeTypeMap.Add(".mp4", "video/mp4");
            MimeTypeMap.Add(".mpc", "application/vnd.mpohun.certificate");
            MimeTypeMap.Add(".mpe", "video/mpeg");
            MimeTypeMap.Add(".mpeg", "video/mpeg");
            MimeTypeMap.Add(".mpg", "video/mpeg");
            MimeTypeMap.Add(".mpg4", "video/mp4");
            MimeTypeMap.Add(".mpga", "audio/mpeg");
            MimeTypeMap.Add(".msg", "application/vnd.ms-outlook");
            MimeTypeMap.Add(".ogg", "audio/ogg");
            MimeTypeMap.Add(".pdf", "application/pdf");
            MimeTypeMap.Add(".png", "image/png");
            MimeTypeMap.Add(".pps", "application/vnd.ms-powerpoint");
            MimeTypeMap.Add(".ppt", "application/vnd.ms-powerpoint");
            MimeTypeMap.Add(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            MimeTypeMap.Add(".prop", "text/plain");
            MimeTypeMap.Add(".rc", "text/plain");
            MimeTypeMap.Add(".rmvb", "audio/x-pn-realaudio");
            MimeTypeMap.Add(".rtf", "application/rtf");
            MimeTypeMap.Add(".sh", "text/plain");
            MimeTypeMap.Add(".tar", "application/x-tar");
            MimeTypeMap.Add(".tgz", "application/x-compressed");
            MimeTypeMap.Add(".txt", "text/plain");
            MimeTypeMap.Add(".wav", "audio/x-wav");
            MimeTypeMap.Add(".wma", "audio/x-ms-wma");
            MimeTypeMap.Add(".wmv", "audio/x-ms-wmv");
            MimeTypeMap.Add(".wps", "application/vnd.ms-works");
            MimeTypeMap.Add(".xml", "text/plain");
            MimeTypeMap.Add(".z", "application/x-compress");
            MimeTypeMap.Add(".zip", "application/x-zip-compressed");
        }

        /// <summary>
        /// Match the MIMETYPE based on the file name suffix.
        /// </summary>
        /// <param name="suffix">suffix File Name suffix</param>
        /// <returns>MimeType</returns>
        public static string GetMimeType(string suffix)
        {
            if (MimeTypeMap.ContainsKey(suffix))
            {
                string mimeTypeValue = null;
                MimeTypeMap.TryGetValue(suffix, out mimeTypeValue);
                return mimeTypeValue;
            }
            else
            {
                return Default;
            }
        }

        /// <summary>
        /// Match the MimeType based on the file type.
        /// </summary>
        /// <param name="file">java.io.File object</param>
        /// <returns>MimeType</returns>
        public static string GetMimeType(Java.IO.File file)
        {
            if (file == null || !file.Exists())
            {
                return Default;
            }

            string fileName = file.Name;
            string suffix = fileName.Substring(fileName.LastIndexOf("."));
            return GetMimeType(suffix);
        }
    }
}