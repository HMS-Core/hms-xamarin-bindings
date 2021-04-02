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
using Android.App;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Utils
{
    /// <summary>
    /// View util class.
    /// </summary>
    public class ViewUtil
    {
        private const string Tag = "ViewUtil";

        private ViewUtil()
        {

        }

        /// <summary>
        /// Look for a child view with the given id. If this view has the given id, return this view.
        /// </summary>
        /// <param name="activity">activity</param>
        /// <param name="id">The id to search for.</param>
        /// <returns>The view that has the given id in the hierarchy or null</returns>
        public static Android.Views.View FindViewById(Activity activity, int id)
        {
            if (null != activity)
            {
                return activity.FindViewById(id);
            }

            return null;
        }
    }
}