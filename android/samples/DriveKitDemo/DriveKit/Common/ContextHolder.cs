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
using Android.Content;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Common
{
    /// <summary>
    /// Save Context.
    /// </summary>
    public class ContextHolder
    {
        private static Context _context;
        public  Context Context
        {
            get { return _context; }
        }

        /// <summary>
        /// init context
        /// </summary>
        /// <param name="context">context</param>
        public static void Init(Context context)
        {
            _context = context;
        }
    }
}