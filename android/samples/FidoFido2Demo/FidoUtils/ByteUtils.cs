/*
 * Copyright 2021. Huawei Technologies Co., Ltd. All rights reserved.

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


using Android.Util;
using Java.Lang;
/**
* Byte Utilities
*
* @author Huawei HMS
* @since 2020-03-08
*/
namespace XFido_Fido2_Demo.FidoUtils
{
    public class ByteUtils
    {
        private const string Tag = "ByteUtils";

        private ByteUtils()
        {
        }

        public static byte[] Base64ToByte(string base64)
        {
            try
            {
                return Base64.Decode(base64, Base64Flags.UrlSafe | Base64Flags.NoPadding | Base64Flags.NoWrap);
            }
            catch (IllegalArgumentException e)
            {
                Log.Error(Tag, e.Message, e);
                return null;
            }
        }

        public static string ByteToBase64(byte[] raw)
        {
            return Base64.EncodeToString(raw, Base64Flags.UrlSafe | Base64Flags.NoPadding | Base64Flags.NoWrap);
        }
    }

}