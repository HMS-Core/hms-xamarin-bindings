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
using XamarinAREngineDemo.AREngineActivities.Body3d.Rendering;
using XamarinAREngineDemo.Common;

namespace XamarinAREngineDemo.AREngineActivities.Body3d
{
    /// <summary>
    /// The sample code demonstrates the capability of HUAWEI AR Engine to identify
    /// body skeleton points and output human body features such as limb endpoints,
    /// body posture, and skeleton.
    /// </summary>
    [Activity(Label = "BodyActivity")]
    public class BodyActivity : Activity
    {
        private const string TAG = "BodyActivity";

        private ARSession mArSession;

        private GLSurfaceView mSurfaceView;

        private BodyRenderManager mBodyRenderManager;

        private DisplayRotationManager mDisplayRotationManager;

        // Used for the display of recognition data.
        private TextView mTextView;

        private string message = null;

        private bool isRemindInstall = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.body3d_activity_main);
            this.mTextView = (TextView)FindViewById(Resource.Id.bodyTextView);
            this.mSurfaceView = (GLSurfaceView)FindViewById(Resource.Id.bodySurfaceview);
            mDisplayRotationManager = new DisplayRotationManager(this);

            // Keep the OpenGL ES running context.
            mSurfaceView.PreserveEGLContextOnPause = true;

            // Set the OpenGLES version.
            mSurfaceView.SetEGLContextClientVersion(2);

            // Set the EGL configuration chooser, including for the
            // number of bits of the color buffer and the number of depth bits.
            mSurfaceView.SetEGLConfigChooser(8, 8, 8, 8, 16, 0);

            mBodyRenderManager = new BodyRenderManager(this);
            mBodyRenderManager.SetDisplayRotationManager(mDisplayRotationManager);
            mBodyRenderManager.SetTextView(mTextView);

            mSurfaceView.SetRenderer(mBodyRenderManager);
            mSurfaceView.RenderMode = Android.Opengl.Rendermode.Continuously;
        }

        protected override void OnResume()
        {

            Log.Debug(TAG, "onResume");
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
                    ARBodyTrackingConfig config = new ARBodyTrackingConfig(mArSession);
                    config.EnableItem = ARConfigBase.EnableDepth | ARConfigBase.EnableMask;
                    mArSession.Configure(config);
                    mBodyRenderManager.SetArSession(mArSession);
                }
                catch (Exception capturedException)
                {
                    exception = capturedException;
                    SetMessageWhenError(capturedException);
                }
                if (message != null)
                {
                    Toast.MakeText(this, message, ToastLength.Long).Show();
                    Log.Debug(TAG, "Creating session" + exception.Message);
                    if (mArSession != null)
                    {
                        mArSession.Stop();
                        mArSession = null;
                    }
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
            mSurfaceView.OnResume();
            mDisplayRotationManager.RegisterDisplayListener();
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
            if (catchException is ARUnavailableServiceNotInstalledException)
            {
                AREngineAvailability.NavigateToAppMarketPage(this);
            }
            else if (catchException is ARUnavailableServiceApkTooOldException)
            {
                message = "Please update HuaweiARService.apk";
            }
            else if (catchException is ARUnavailableClientSdkTooOldException)
            {
                message = "Please update this app";
            }
            else if (catchException is ARUnSupportedConfigurationException)
            {
                message = "The configuration is not supported by the device!";
            }
            else
            {
                message = "exception throw";
            }
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause start.");
            base.OnPause();
            if (mArSession != null)
            {
                mDisplayRotationManager.UnregisterDisplayListener();
                mSurfaceView.OnPause();
                mArSession.Pause();
            }
            Log.Debug(TAG, "OnPause end.");
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy start.");
            base.OnDestroy();
            if (mArSession != null)
            {
                mArSession.Stop();
                mArSession = null;
            }
            Log.Debug(TAG, "OnDestroy end.");
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
}