/*
*       Copyright 2020-2021 Huawei Technologies Co., Ltd. All rights reserved.

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

namespace XamarinAREngineDemo.AREngineActivities.World
{
    /// <summary>
    /// Gesture event management class for storing and creating gestures.
    /// </summary>
    public class GestureEvent : Java.Lang.Object
    {
        /**
         * Define the constant 0, indicating an unknown gesture type.
         */
        public const int GESTURE_EVENT_TYPE_UNKNOW = 0;

        /**
         * Define the constant 1, indicating that the gesture type is DOWN.
         */
        public const int GESTURE_EVENT_TYPE_DOWN = 1;

        /**
         * Define the constant 2, indicating that the gesture type is SINGLETAPUP.
         */
        public const int GESTURE_EVENT_TYPE_SINGLETAPUP = 2;

        /**
         * Define the constant 3, indicating that the gesture type is SCROLL.
         */
        public const int GESTURE_EVENT_TYPE_SCROLL = 3;

        /**
         * Define the constant 4, indicating that the gesture type is SINGLETAPCONFIRMED.
         */
        public const int GESTURE_EVENT_TYPE_SINGLETAPCONFIRMED = 4;

        /**
         * Define the constant 5, indicating that the gesture type is DOUBLETAP.
         */
        public const int GESTURE_EVENT_TYPE_DOUBLETAP = 5;

        private int type;

        private MotionEvent eventFirst;

        private MotionEvent eventSecond;

        private float distanceX;

        private float distanceY;

        private GestureEvent()
        {
        }

        public float GetDistanceX()
        {
            return distanceX;
        }

        public float GetDistanceY()
        {
            return distanceY;
        }

        public int GetType()
        {
            return type;
        }

        public MotionEvent GetEventFirst()
        {
            return eventFirst;
        }

        public MotionEvent GetEventSecond()
        {
            return eventSecond;
        }

        /// <summary>
        /// Create a gesture type: DOWN.
        /// </summary>
        /// <param name="motionEvent">The gesture motion event: DOWN.</param>
        /// <returns>GestureEvent.</returns>
        public static GestureEvent CreateDownEvent(MotionEvent motionEvent)
        {
            GestureEvent ret = new GestureEvent();
            ret.type = GESTURE_EVENT_TYPE_DOWN;
            ret.eventFirst = motionEvent;
            return ret;
        }

        /// <summary>
        /// Create a gesture type: SINGLETAPUP.
        /// </summary>
        /// <param name="motionEvent">The gesture motion event: SINGLETAPUP.</param>
        /// <returns>GestureEvent(SINGLETAPUP).</returns>
        public static GestureEvent CreateSingleTapUpEvent(MotionEvent motionEvent)
        {
            GestureEvent ret = new GestureEvent();
            ret.type = GESTURE_EVENT_TYPE_SINGLETAPUP;
            ret.eventFirst = motionEvent;
            return ret;
        }

        /// <summary>
        /// Create a gesture type: SINGLETAPCONFIRM.
        /// </summary>
        /// <param name="motionEvent">The gesture motion event: SINGLETAPCONFIRM.</param>
        /// <returns>GestureEvent(SINGLETAPCONFIRM).</returns>
        public static GestureEvent CreateSingleTapConfirmEvent(MotionEvent motionEvent)
        {
            GestureEvent ret = new GestureEvent();
            ret.type = GESTURE_EVENT_TYPE_SINGLETAPCONFIRMED;
            ret.eventFirst = motionEvent;
            return ret;
        }

        /// <summary>
        /// Create a gesture type: DOUBLETAP.
        /// </summary>
        /// <param name="motionEvent">The gesture motion event: DOUBLETAP.</param>
        /// <returns>GestureEvent(DOUBLETAP).</returns>
        public static GestureEvent CreateDoubleTapEvent(MotionEvent motionEvent)
        {
            GestureEvent ret = new GestureEvent();
            ret.type = GESTURE_EVENT_TYPE_DOUBLETAP;
            ret.eventFirst = motionEvent;
            return ret;
        }

        /// <summary>
        /// Create a gesture type: SCROLL.
        /// </summary>
        /// <param name="e1">The first down motion event that started the scrolling.</param>
        /// <param name="e2">The second down motion event that ended the scrolling.</param>
        /// <param name="distanceX">The distance along the X axis that has been scrolled since the last call to onScroll.</param>
        /// <param name="distanceY">The distance along the Y axis that has been scrolled since the last call to onScroll.</param>
        /// <returns>GestureEvent(SCROLL).</returns>
        public static GestureEvent createScrollEvent(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            GestureEvent ret = new GestureEvent();
            ret.type = GESTURE_EVENT_TYPE_SCROLL;
            ret.eventFirst = e1;
            ret.eventSecond = e2;
            ret.distanceX = distanceX;
            ret.distanceY = distanceY;
            return ret;
        }
    }
}