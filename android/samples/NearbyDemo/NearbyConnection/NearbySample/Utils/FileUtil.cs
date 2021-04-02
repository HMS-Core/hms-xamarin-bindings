/*
       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Support.V4.Provider;
using Java.Math;
using Java.Text;

namespace XamarinHmsNearbyDemo
{
    class FileUtil
    {
        private static  int CONVERSION_BASE = 1024;
        private static  string[] BYTES_UNIT_LIST = {"B", "KB", "MB", "GB", "TB"};
        private static  long BASE_KB = CONVERSION_BASE;
        private static  long BASE_MB = CONVERSION_BASE * BASE_KB;
        private static  long BASE_GB = CONVERSION_BASE * BASE_MB;
        private static  long BASE_TB = CONVERSION_BASE * BASE_GB;
        private static  long[] BASE_LIST = { 1, BASE_KB, BASE_MB, BASE_GB, BASE_TB };

        public static String FormatBytes(long bytes)
        {
            long inputBytes = bytes;
            BigDecimal bigBytes = new BigDecimal(bytes);
            NumberFormat numberFormat = new DecimalFormat();
            numberFormat.MaximumFractionDigits=1;
            int index;
            for (index = 0; index < BYTES_UNIT_LIST.Length; index++)
            {
                if (inputBytes < CONVERSION_BASE)
                {
                    break;
                }
                inputBytes /= CONVERSION_BASE;
            }
            return numberFormat.Format(bigBytes.Divide(new BigDecimal(BASE_LIST[index]))) + " " + BYTES_UNIT_LIST[index];
        }


        public static String getFileRealNameFromUri(Context context, Android.Net.Uri fileUri)
        {
            if (context == null || fileUri == null)
            {
                return null;
            }
            DocumentFile documentFile = DocumentFile.FromSingleUri(context, fileUri);
            if (documentFile == null)
            {
                return null;
            }
            return documentFile.Name;
        }

    }
}