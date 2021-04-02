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
using Android.Util;
using Android.Views;
using AndroidX.ViewPager.Widget;

namespace XHms_Drive_Kit_Demo_Project.DriveKit.Views
{
    /// <summary>
    /// ViewPager for MainActivity.
    /// </summary>
    public class FileViewPager : ViewPager
    {

        private bool _enableScroll = true;
        public bool EnableScroll
        {
            get { return _enableScroll; }
            set { _enableScroll = value; }
        }

        public FileViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public FileViewPager(Context context) : base(context) 
        {

        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!_enableScroll)
            {
                return false;
            }
            return base.OnInterceptTouchEvent(ev);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (!_enableScroll)
            {
                return false;
            }
            return base.OnTouchEvent(e);
        }

        public override void SetCurrentItem(int item, bool smoothScroll)
        {
            base.SetCurrentItem(item, false);
        }
    }
}