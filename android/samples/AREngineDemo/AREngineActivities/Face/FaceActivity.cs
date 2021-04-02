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
using Android.Graphics;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Huawei.Hiar.Exceptions;
using XamarinAREngineDemo.AREngineActivities.Face.Rendering;
using XamarinAREngineDemo.Common;

namespace XamarinAREngineDemo.AREngineActivities.Face
{
    /// <summary>
    /// This demo shows the capabilities of HUAWEI AR Engine to recognize faces, including facial
    /// features and facial expressions.In addition, this demo shows how an app can open the camera
    /// to display preview.Currently, only apps of the ARface type can open the camera. If you want
    /// to the app to open the camera, set isOpenCameraOutside = true in this file.
    /// </summary>
    [Activity(Label = "FaceActivity")]
    public class FaceActivity : Activity
    {
        private const string TAG = "FaceActivity";

        private ARSession mArSession;

        private GLSurfaceView glSurfaceView;

        private FaceRenderManager mFaceRenderManager;

        private DisplayRotationManager mDisplayRotationManager;

        private bool isOpenCameraOutside = false;

        private CameraHelper mCamera;

        private Surface mPreViewSurface;

        private Surface mVgaSurface;

        private Surface mMetaDataSurface;

        private Surface mDepthSurface;

        private ARConfigBase mArConfig;

        private TextView mTextView;

        private String message = null;

        private bool isRemindInstall = false;

        // The initial texture ID is -1.
        private int textureId = -1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.face_activity_main);
            mTextView = (TextView)FindViewById(Resource.Id.faceTextView);
            glSurfaceView = (GLSurfaceView)FindViewById(Resource.Id.faceSurfaceview);
               
            mDisplayRotationManager = new DisplayRotationManager(this);

            glSurfaceView.PreserveEGLContextOnPause = true;

            // Set the OpenGLES version.
            glSurfaceView.SetEGLContextClientVersion(2);

            // Set the EGL configuration chooser, including for the
            // number of bits of the color buffer and the number of depth bits.
            glSurfaceView.SetEGLConfigChooser(8, 8, 8, 8, 16, 0);

            mFaceRenderManager = new FaceRenderManager(this, this);
            mFaceRenderManager.SetDisplayRotationManager(mDisplayRotationManager);
            mFaceRenderManager.SetTextView(mTextView);

            glSurfaceView.SetRenderer(mFaceRenderManager);
            glSurfaceView.RenderMode = Android.Opengl.Rendermode.Continuously;
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();
            mDisplayRotationManager.RegisterDisplayListener();
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
                    mArConfig = new ARFaceTrackingConfig(mArSession);

                    mArConfig.SetPowerMode(ARConfigBase.PowerMode.PowerSaving);

                    if (isOpenCameraOutside)
                    {
                        mArConfig.SetImageInputMode(ARConfigBase.ImageInputMode.ExternalInputAll);
                    }
                    mArSession.Configure(mArConfig);
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
                Toast.MakeText(this, "Camera open failed, please restart the app", ToastLength.Short).Show();
                mArSession = null;
                return;
            }
            mDisplayRotationManager.RegisterDisplayListener();
            SetCamera();
            mFaceRenderManager.SetArSession(mArSession);
            mFaceRenderManager.SetOpenCameraOutsideFlag(isOpenCameraOutside);
            mFaceRenderManager.SetTextureId(textureId);
            glSurfaceView.OnResume();
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

        private void StopArSession(Exception exception)
        {
            Log.Debug(TAG, "Stop session start.");
            Toast.MakeText(this, message, ToastLength.Long).Show();
            Log.Debug(TAG, "Creating session error ", exception);
            if (mArSession != null)
            {
                mArSession.Stop();
                mArSession = null;
            }
            Log.Debug(TAG, "Stop session end.");
        }

        private void SetCamera()
        {
            if (isOpenCameraOutside && mCamera == null)
            {
                Log.Debug(TAG, "new Camera");
                DisplayMetrics dm = new DisplayMetrics();
                mCamera = new CameraHelper(this);
                mCamera.SetupCamera(dm.WidthPixels, dm.HeightPixels);
            }

            // Check whether setCamera is called for the first time.
            if (isOpenCameraOutside)
            {
                if (textureId != -1)
                {
                    mArSession.SetCameraTextureName(textureId);
                    InitSurface();
                }
                else
                {
                    int[] textureIds = new int[1];
                    GLES20.GlGenTextures(1, textureIds, 0);
                    textureId = textureIds[0];
                    mArSession.SetCameraTextureName(textureId);
                    InitSurface();
                }

                SurfaceTexture surfaceTexture = new SurfaceTexture(textureId);
                mCamera.setPreviewTexture(surfaceTexture);
                mCamera.setPreViewSurface(mPreViewSurface);
                mCamera.setVgaSurface(mVgaSurface);
                mCamera.setDepthSurface(mDepthSurface);
                if (!mCamera.OpenCamera())
                {
                    string showMessage = "Open camera filed!";
                    Log.Debug(TAG, showMessage);
                    Toast.MakeText(this, showMessage, ToastLength.Long).Show();
                    Finish();
                }
            }
        }

        private void InitSurface()
        {
            IList<ARConfigBase.SurfaceType> surfaceTypeList = mArConfig.ImageInputSurfaceTypes;
            IList<Surface> surfaceList = mArConfig.ImageInputSurfaces;

            Log.Debug(TAG, "surfaceList size : " + surfaceList.Count);
            int size = surfaceTypeList.Count;
            for (int i = 0; i < size; i++)
            {
                ARConfigBase.SurfaceType type = surfaceTypeList.ElementAt(i);
                Surface surface = surfaceList.ElementAt(i);
                if (ARConfigBase.SurfaceType.Preview.Equals(type))
                {
                    mPreViewSurface = surface;
                }
                else if (ARConfigBase.SurfaceType.Vga.Equals(type))
                {
                    mVgaSurface = surface;
                }
                else if (ARConfigBase.SurfaceType.Metadata.Equals(type))
                {
                    mMetaDataSurface = surface;
                }
                else if (ARConfigBase.SurfaceType.Depth.Equals(type))
                {
                    mDepthSurface = surface;
                }
                else
                {
                    Log.Debug(TAG, "Unknown type.");
                }
                Log.Debug(TAG, "list[" + i + "] get surface : " + surface + ", type : " + type);
            }
        }

        protected override void OnPause()
        {
            Log.Debug(TAG, "OnPause Start.");
            base.OnPause();
            if (isOpenCameraOutside)
            {
                if (mCamera != null)
                {
                    mCamera.CloseCamera();
                    mCamera.StopCameraThread();
                    mCamera = null;
                }
            }

            if (mArSession != null)
            {
                mDisplayRotationManager.UnregisterDisplayListener();
                glSurfaceView.OnPause();
                mArSession.Pause();
                Log.Debug(TAG, "Session paused!");
            }
            Log.Debug(TAG, "OnPause end.");
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy Start.");
            base.OnDestroy();
            if (mArSession != null)
            {
                Log.Debug(TAG, "Session OnDestroy!");
                mArSession.Stop();
                mArSession = null;
                Log.Debug(TAG, "Session stop!");
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
                    |SystemUiFlags.Fullscreen | SystemUiFlags.ImmersiveSticky);
            }
        }

    }
}