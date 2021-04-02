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
using Android.Views;
using Android.Widget;
using Android.Text;
using XamarinAccountKitDemo.Logger;
using Java.Lang;

namespace XamarinAccountKitDemo.HmsSample
{
    public class LogFragment : Fragment
    {
        private LogView logView;

        private ScrollView scrollView;

        public LogFragment()
        {

        }

        private View InflateViews()
        {
            scrollView = new ScrollView(this.Activity);

            scrollView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            logView = new LogView(Activity);
            logView.Clickable = true;

            scrollView.AddView(logView,
                 new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent));

            if (Constant.IsLog == 0)
            {
                scrollView.Visibility = ViewStates.Gone;
            }

            return scrollView;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View result = InflateViews();

            logView.AddTextChangedListener(new TextWatcherImpl(scrollView));

            /**
            * double click on the TextView,the application will clean the info window
            */
            GestureDetector gestureDetector = new GestureDetector(new GestureDoubleTapImpl(logView));

            logView.SetOnTouchListener(new OnTouchListenerImpl(gestureDetector));

            return result;
        }

        public LogView GetLogView()
        {
            return logView;
        }

        class TextWatcherImpl : Java.Lang.Object, ITextWatcher
        {
            private ScrollView scrollView;
            public TextWatcherImpl(ScrollView scrollView)
            {
                this.scrollView = scrollView;
            }

            public void AfterTextChanged(IEditable s)
            {
                scrollView.Post(() =>
                {
                    scrollView.FullScroll(FocusSearchDirection.Down);
                });
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
            }
        }

        class GestureDoubleTapImpl : GestureDetector.SimpleOnGestureListener
        {
            LogView logView;

            public GestureDoubleTapImpl(LogView logView)
            {
                this.logView = logView;
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                logView.SetText("", TextView.BufferType.Normal);
                return true;
            }
        }

        class OnTouchListenerImpl : Java.Lang.Object, View.IOnTouchListener
        {
            private GestureDetector gestureDetector;

            public OnTouchListenerImpl(GestureDetector gestureDetector)
            {
                this.gestureDetector = gestureDetector;
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                return gestureDetector.OnTouchEvent(e);
            }
        }
    }
}