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
using Android.Hardware.Display;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;

using static Android.Hardware.Display.DisplayManager;

namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// Device rotation manager, which is used by the demo to adapt to device rotations
    /// </summary>
    public class DisplayRotationManager : Java.Lang.Object, IDisplayListener
    {
        private const string TAG = "DisplayRotationManager";

        private bool mIsDeviceRotation;

        private readonly Context mContext;

        private readonly Display mDisplay;

        private int mViewPx;

        private int mViewPy;

        /// <summary>
        /// Construct DisplayRotationManage with the context.
        /// </summary>
        /// <param name="Context">context</param>
        public DisplayRotationManager(Context context)
        {
            mContext = context;
            IWindowManager systemService = mContext.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            if (systemService != null) {
                mDisplay = systemService.DefaultDisplay;
            } else {
                mDisplay = null;
            }
        }

        /// <summary>
        /// Register a listener on display changes. This method can be called when onResume is called for an activity.
        /// </summary>
        public void RegisterDisplayListener()
        {
            DisplayManager systemService = mContext.GetSystemService(Context.DisplayService).JavaCast<DisplayManager>();
            if (systemService != null) 
            {
            systemService.RegisterDisplayListener(this, null);
            }
        }

        /// <summary>
        /// Deregister a listener on display changes. 
        /// This method can be called when OnPause is called for an activity.
        /// </summary>
        public void UnregisterDisplayListener()
        {
            DisplayManager systemService = mContext.GetSystemService(Context.DisplayService).JavaCast<DisplayManager>();
            if (systemService != null) {
                systemService.UnregisterDisplayListener(this);
            }
        }

        /// <summary>
        /// When a device is rotated, the viewfinder size and whether the device is rotated
        /// should be updated to correctly display the geometric information returned by the
        /// AR Engine. This method should be called when onSurfaceChanged.
        /// </summary>
        /// <param name="width">Width of the surface updated by the device.</param>
        /// <param name="height">Height of the surface updated by the device.</param>
        public void UpdateViewportRotation(int width, int height)
        {
            mViewPx = width;
            mViewPy = height;
            mIsDeviceRotation = true;
        }

        /// <summary>
        /// Check whether the current device is rotated.
        /// </summary>
        /// <returns>The device rotation result.</returns>
        public bool GetDeviceRotation()
        {
            return mIsDeviceRotation;
        }

        /// <summary>
        /// If the device is rotated, update the device window of the current ARSession.
        /// This method can be called when onDrawFrame is called.
        /// </summary>
        /// <param name="session">ARSession object.</param>
        public void UpdateArSessionDisplayGeometry(ARSession session)
        {
            int displayRotation = 0;
            if (mDisplay != null)
            {
                displayRotation = (int)mDisplay.Rotation;
            }
            else
            {
                Log.Debug(TAG, "updateArSessionDisplayGeometry mDisplay null!");
            }
            session.SetDisplayGeometry(displayRotation, mViewPx, mViewPy);
            mIsDeviceRotation = false;
        }

        public void OnDisplayAdded(int displayId)
        {
            
        }

        public void OnDisplayChanged(int displayId)
        {
            mIsDeviceRotation = true;
        }

        public void OnDisplayRemoved(int displayId)
        {
            
        }
    }
}