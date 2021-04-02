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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ProductVisionSearch
{
    public class BottomSheetGridView : GridView
    {
        public BottomSheetGridView(Context context) : base(context)
        {

        }

        public BottomSheetGridView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public BottomSheetGridView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            
        }

        public override bool OnInterceptHoverEvent(MotionEvent e)
        {
            return true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (CanScrollVertically(this))
            {
                Parent.RequestDisallowInterceptTouchEvent(true);
            }
            return base.OnTouchEvent(e);
        }

        private bool CanScrollVertically(AbsListView view)
        {
            bool canScroll = false;

            if (view != null && view.ChildCount > 0)
            {
                bool isOnTop = view.FirstVisiblePosition != 0 || view.GetChildAt(0).Top != 0;
                bool isAllItemsVisible = isOnTop && view.LastVisiblePosition == view.ChildCount;

                if (isOnTop || isAllItemsVisible)
                {
                    canScroll = true;
                }
            }

            return canScroll;
        }
    }
}