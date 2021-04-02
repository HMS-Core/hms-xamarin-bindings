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
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Huawei.Hiar.Exceptions;
using Java.Util.Concurrent;
using XamarinAREngineDemo.AREngineActivities.World.Rendering;
using XamarinAREngineDemo.Common;


namespace XamarinAREngineDemo.AREngineActivities.World
{
    /// <summary>
    /// This AR example shows how to use the world AR scene of HUAWEI AR Engine,
    /// including how to identify planes, use the click function, and identify
    /// specific images.
    /// </summary>
    [Activity(Label = "WorldActivity")]
    public class WorldActivity : Activity
    {
        private const string TAG = "WorldActivity";

        private static readonly int MOTIONEVENT_QUEUE_CAPACITY = 2;

        private static readonly int OPENGLES_VERSION = 2;

        private ARSession mArSession;

        private GLSurfaceView mSurfaceView;

        private WorldRenderManager mWorldRenderManager;

        public GestureDetector mGestureDetector;

        private DisplayRotationManager mDisplayRotationManager;

        private ArrayBlockingQueue mQueuedSingleTaps = new ArrayBlockingQueue(MOTIONEVENT_QUEUE_CAPACITY);

        private string message = null;

        private bool isRemindInstall = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.world_activity_main);

            this.mSurfaceView = (GLSurfaceView)FindViewById(Resource.Id.surfaceview);
            mDisplayRotationManager = new DisplayRotationManager(this);
            InitGestureDetector();

            mSurfaceView.PreserveEGLContextOnPause = true;
            mSurfaceView.SetEGLContextClientVersion(OPENGLES_VERSION);

            // Set the EGL configuration chooser, including for the number of
            // bits of the color buffer and the number of depth bits.
            mSurfaceView.SetEGLConfigChooser(8, 8, 8, 8, 16, 0);

            mWorldRenderManager = new WorldRenderManager(this, this.ApplicationContext);
            mWorldRenderManager.SetDisplayRotationManager(mDisplayRotationManager);
            mWorldRenderManager.SetQueuedSingleTaps(mQueuedSingleTaps);

            mSurfaceView.SetRenderer(mWorldRenderManager);
            mSurfaceView.RenderMode = Android.Opengl.Rendermode.Continuously;
        }

        private void InitGestureDetector()
        {
            mGestureDetector = new GestureDetector(this, new SimpleGestureDetectorListener(this));

            mSurfaceView.SetOnTouchListener(new MyOnTouchListener(this));
        }

        public void OnGestureEvent(GestureEvent e)
        {
            bool offerResult = mQueuedSingleTaps.Offer(e);
            if (offerResult)
            {
                Log.Debug(TAG, "Successfully joined the queue.");
            }
            else
            {
                Log.Debug(TAG, "Failed to join queue.");
            }

        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();
            Exception exception = null;
            message = null;
            if (mArSession == null)
            {
                try
                {
                    if (!ArEngineAbilityCheck())
                    {
                        Finish();
                        return;
                    }
                    mArSession = new ARSession(this);
                    ARWorldTrackingConfig config = new ARWorldTrackingConfig(mArSession);
                    config.SetFocusMode(ARConfigBase.FocusMode.AutoFocus);
                    config.SemanticMode = (ARWorldTrackingConfig.SemanticPlane);
                    mArSession.Configure(config);
                    mWorldRenderManager.SetArSession(mArSession);
                }
                catch (Exception capturedException)
                {
                    exception = capturedException;
                    SetMessageWhenError(capturedException);
                }
                if (message != null)
                {
                    StopArSession(exception);
                    return;
                }
            }
            try
            {
                mArSession.Resume();
            }
            catch (ARCameraNotAvailableException e)
            {
                Toast.MakeText(this, "Camera open failed, please restart the app", ToastLength.Long).Show();
                mArSession = null;
                return;
            }
            mDisplayRotationManager.RegisterDisplayListener();
            mSurfaceView.OnResume();
        }

        /// <summary>
        /// Check whether HUAWEI AR Engine server (com.huawei.arengine.service) is installed on the current device.
        /// If not, redirect the user to HUAWEI AppGallery for installation.
        /// </summary>
        private bool ArEngineAbilityCheck()
        {
            bool isInstallArEngineApk = AREnginesApk.IsAREngineApkReady(this);
            if (!isInstallArEngineApk && isRemindInstall)
            {
                Toast.MakeText(this, "Please agree to install.", ToastLength.Long).Show();
                Finish();
            }
            Log.Debug(TAG, "Is Install AR Engine Apk: " + isInstallArEngineApk);
            if (!isInstallArEngineApk)
            {
                AREngineAvailability.NavigateToAppMarketPage(this);
                isRemindInstall = true;
            }
            return AREnginesApk.IsAREngineApkReady(this);
        }

        private void SetMessageWhenError(Exception catchException)
        {
            if (catchException is ARUnavailableServiceNotInstalledException) {
                AREngineAvailability.NavigateToAppMarketPage(this);
            } else if (catchException is ARUnavailableServiceApkTooOldException) {
                message = "Please update HuaweiARService.apk";
            } else if (catchException is ARUnavailableClientSdkTooOldException) {
                message = "Please update this app";
            } else if (catchException is ARUnSupportedConfigurationException) {
                message = "The configuration is not supported by the device!";
            } else
            {
                message = "exception throw";
            }
        }

        private void StopArSession(Exception exception)
        {
            Log.Debug(TAG, "stopArSession start.");
            Toast.MakeText(this, message, ToastLength.Long).Show();
            Log.Debug(TAG, "Creating session error" + exception.Message);
            if (mArSession != null)
            {
                mArSession.Stop();
                mArSession = null;
            }
            Log.Debug(TAG, "stopArSession end.");
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "onPause start.");
            base.OnPause();
            if (mArSession != null)
            {
                mDisplayRotationManager.UnregisterDisplayListener();
                mSurfaceView.OnPause();
                mArSession.Pause();
            }
            Log.Debug(TAG, "onPause end.");
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "onDestroy start.");
            if (mArSession != null)
            {
                mArSession.Stop();
                mArSession = null;
            }
            base.OnDestroy();
            Log.Debug(TAG, "onDestroy end.");
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            Log.Debug(TAG, "OnWindowFocusChanged");
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)
                    (SystemUiFlags.LayoutStable | SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen | SystemUiFlags.HideNavigation
                    | SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
            }
        }
    }

    /// <summary>
    /// Gesture detector listener class.
    /// </summary>
    public class SimpleGestureDetectorListener : Android.Views.GestureDetector.SimpleOnGestureListener
    {
        WorldActivity mActivity;
        public SimpleGestureDetectorListener(WorldActivity Activity)
        {
            this.mActivity = Activity;
        }
        public override bool OnDoubleTap(MotionEvent e)
        {
            mActivity.OnGestureEvent(GestureEvent.CreateDoubleTapEvent(e));
            return true;
        }

        public override bool OnSingleTapConfirmed(MotionEvent e)
        {
            mActivity.OnGestureEvent(GestureEvent.CreateSingleTapConfirmEvent(e));
            return true;
        }

        public override bool OnDown(MotionEvent e)
        {
            return true;
        }

        public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            mActivity.OnGestureEvent(GestureEvent.createScrollEvent(e1, e2, distanceX, distanceY));
            return true;
        }
    }

    /// <summary>
    /// On Touch listener class.
    /// </summary>
    public class MyOnTouchListener : Java.Lang.Object, View.IOnTouchListener
    {

        WorldActivity mActivity;
        public MyOnTouchListener(WorldActivity Activity)
        {
            this.mActivity = Activity;
        }
        public bool OnTouch(View v, MotionEvent e)
        {
            return mActivity.mGestureDetector.OnTouchEvent(e);
        }
    }
}