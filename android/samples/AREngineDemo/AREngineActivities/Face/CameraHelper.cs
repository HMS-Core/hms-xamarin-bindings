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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Hardware.Camera2.Params;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Util;
using Java.Util.Concurrent;
using XamarinAREngineDemo.Common;

namespace XamarinAREngineDemo.AREngineActivities.Face
{
    /// <summary>
    ///  Provide services related to the camera, including starting and stopping
    ///  the camera thread, and opening and closing the camera.
    /// </summary>
    public class CameraHelper
    {
        private const string TAG = "CameraHelper";

        public CameraDevice mCameraDevice;

        public CameraCaptureSession mCameraCaptureSession;

        public string mCameraId;

        public Surface mVgaSurface;

        public Activity mActivity;

        public Size mPreviewSize;

        public HandlerThread mCameraThread;

        public Handler mCameraHandler;

        public SurfaceTexture mSurfaceTexture;

        public Surface mDepthSurface;

        public CaptureRequest.Builder mCaptureRequestBuilder;

        public Semaphore mCameraOpenCloseLock = new Semaphore(1);

        public Surface mPreViewSurface;

        public CameraDevice.StateCallback mStateCallback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="activity">Activity.</param>
        public CameraHelper(Activity activity)
        {
            mActivity = activity;
            mStateCallback = new CameraDeviceStateCallback(this);
            StartCameraThread();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">Device screen width, in pixels.</param>
        /// <param name="height">Device screen height, in pixels.</param>
        public void SetupCamera(int width, int height)
        {
            CameraManager cameraManager = (CameraManager)mActivity.GetSystemService(Context.CameraService);
            try
            {
                SurfaceTexture SurfaceTextureClassIntance = new SurfaceTexture(true);
                foreach (string id in cameraManager.GetCameraIdList())
                {
                    CameraCharacteristics characteristics = cameraManager.GetCameraCharacteristics(id);
                    int cameraLensFacing = (int)characteristics.Get(CameraCharacteristics.LensFacing);
                    if (cameraLensFacing == null)
                    {
                        continue;
                    }
                    if (cameraLensFacing != (int)LensFacing.Front)
                    {
                        continue;
                    }
                    StreamConfigurationMap maps =
                        (StreamConfigurationMap)characteristics.Get(CameraCharacteristics.ScalerStreamConfigurationMap);
                    if (maps == null || maps.GetOutputSizes(SurfaceTextureClassIntance.Class) == null) {
                    continue;
                }

                mPreviewSize = getOptimalSize(maps.GetOutputSizes(SurfaceTextureClassIntance.Class), width, height);
                mCameraId = id;
                Log.Debug(TAG, "Preview width = " + mPreviewSize.Width + ", height = "
                    + mPreviewSize.Height + ", cameraId = " + mCameraId);
                break;
            }
            } catch (CameraAccessException e)
            {
                Log.Debug(TAG, "Set upCamera error");
            }
        }

        /// <summary>
        /// Start the camera thread.
        /// </summary>
        private void StartCameraThread()
        {
            mCameraThread = new HandlerThread("CameraThread");
            mCameraThread.Start();
            if (mCameraThread.Looper != null)
            {
                Log.Debug(TAG, "startCameraThread mCameraThread.Looper null!");
                return;
            }
            mCameraHandler = new Handler(mCameraThread.Looper);
        }

        /// <summary>
        /// Close the camera thread.
        /// </summary>
        public void StopCameraThread()
        {
            mCameraThread.QuitSafely();
            try
            {
                mCameraThread.Join();
                mCameraThread = null;
                mCameraHandler = null;
            }
            catch (System.Exception e)
            {
                Log.Debug(TAG, "StopCameraThread error");
            }
        }

        /// <summary>
        /// Launch the camera.
        /// </summary>
        /// <returns>Open success or failure.</returns>
        public bool OpenCamera()
        {
                Log.Debug(TAG, "OpenCamera!");
                CameraManager cameraManager = null;
                if (mActivity.GetSystemService(Context.CameraService) is CameraManager) {
                    cameraManager = (CameraManager)mActivity.GetSystemService(Context.CameraService);
                } else
                {
                    return false;
                }
                try
                {
                    if (ActivityCompat.CheckSelfPermission(mActivity, Manifest.Permission.Camera)
                            != Android.Content.PM.Permission.Granted)
                    {
                        return false;
                    }

                    // 2500 is the maximum waiting time.
                    if (!mCameraOpenCloseLock.TryAcquire(2500, TimeUnit.Milliseconds))
                    {
                        throw new ArDemoRuntimeException("Time out waiting to lock camera opening.");
                    }
                    cameraManager.OpenCamera(mCameraId, mStateCallback, mCameraHandler);
                }
                catch (System.Exception e) {
                    Log.Debug(TAG, "OpenCamera error.");
                    return false;
                }
                return true;
        }

        /// <summary>
        /// Close the camera.
        /// </summary>
        public void CloseCamera()
        {
            try
            {
                mCameraOpenCloseLock.Acquire();
                Log.Debug(TAG, "Stop CameraCaptureSession begin!");
                StopPreview();
                Log.Debug(TAG, "Stop CameraCaptureSession stopped!");
                if (mCameraDevice != null)
                {
                    Log.Debug(TAG, "Stop Camera!");
                    mCameraDevice.Close();
                    mCameraDevice = null;
                    Log.Debug(TAG, "Stop Camera stopped!");
                }
            }
            catch (Java.Lang.Exception e)
            {
                throw new ArDemoRuntimeException("Interrupted while trying to lock camera closing.", e);
            }
            finally
            {
                mCameraOpenCloseLock.Release();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private Size getOptimalSize(Size[] sizeMap, int width, int height)
        {
            int max = Java.Lang.Math.Max(width, height);
            int min = Java.Lang.Math.Min(width, height);
            List<Size> sizeList = new List<Size>();
            foreach (Size option in sizeMap)
            {
                if (option.Width > max && option.Height > min)
                {
                    sizeList.Add(option);
                }
            }
            if (sizeList.Count == 0)
            {
                return sizeMap[0];
            }
            return (Size)Collections.Min(sizeList, new CalculatedAreaDifference());
            
        }

        /// <summary>
        /// Set the texture of the surface.
        /// </summary>
        /// <param name="surfaceTexture">Surface texture.</param>
        public void setPreviewTexture(SurfaceTexture surfaceTexture)
        {
            mSurfaceTexture = surfaceTexture;
        }

        /// <summary>
        /// Set the preview surface.
        /// </summary>
        /// <param name="surface">Surface</param>
        public void setPreViewSurface(Surface surface)
        {
            mPreViewSurface = surface;
        }

        /// <summary>
        /// Set the VGA surface.
        /// </summary>
        /// <param name="surface">Surface</param>
        public void setVgaSurface(Surface surface)
        {
            mVgaSurface = surface;
        }

        /// <summary>
        /// Set the depth surface.
        /// </summary>
        /// <param name="surface">Surface.</param>
        public void setDepthSurface(Surface surface)
        {
            mDepthSurface = surface;
        }

        /// <summary>
        /// Start preview
        /// </summary>
        public void StartPreview()
        {
            if (mSurfaceTexture == null)
            {
                Log.Debug(TAG, "mSurfaceTexture is null!");
                return;
            }
            Log.Debug(TAG, "StartPreview!");
            mSurfaceTexture.SetDefaultBufferSize(mPreviewSize.Width, mPreviewSize.Height);

            if (mCameraDevice == null)
            {
                Log.Debug(TAG, "mCameraDevice is null!");
                return;
            }
            try
            {
                mCaptureRequestBuilder = mCameraDevice.CreateCaptureRequest(CameraTemplate.Preview);
                List<Surface> surfaces = new List<Surface>();
                if (mPreViewSurface != null)
                {
                    surfaces.Add(mPreViewSurface);
                }
                if (mVgaSurface != null)
                {
                    surfaces.Add(mVgaSurface);
                }
                if (mDepthSurface != null)
                {
                    surfaces.Add(mDepthSurface);
                }
                CaptureSession(surfaces);
            }
            catch (CameraAccessException e)
            {
                Log.Debug(TAG, "StartPreview error");
            }
        }

        private void CaptureSession(List<Surface> surfaces)
        {
            try
            {
                mCameraDevice.CreateCaptureSession(surfaces,new CameraCaptureSessionStateCallback(this), mCameraHandler);
            }
            catch(System.Exception e)
            {
                Log.Debug(TAG, "CaptureSession error");
            }
        }

        private void StopPreview()
        {
            if (mCameraCaptureSession != null)
            {
                mCameraCaptureSession.Close();
                mCameraCaptureSession = null;
            }
            else
            {
                Log.Debug(TAG, "mCameraCaptureSession is null!");
            }
        }

        public void SetCameraDevice(CameraDevice cameraDevice)
        {
            this.mCameraDevice = cameraDevice;
        }

        public void ReleaseCameraOpenCloseLock()
        {
            this.mCameraOpenCloseLock.Release();
        }

        public CameraDevice GetCameraDevice()
        {
            return mCameraDevice;
        }



    }

    /// <summary>
    /// CameraDeviceStateCallback class
    /// </summary>
    public class CameraDeviceStateCallback : CameraDevice.StateCallback
    {
        private const string TAG = "CameraDeviceStateCallback";
        CameraHelper mCameraHelper;

        public CameraDeviceStateCallback(CameraHelper cameraHelper)
        {
            this.mCameraHelper = cameraHelper;
        }
        public override void OnDisconnected(CameraDevice camera)
        {
            mCameraHelper.ReleaseCameraOpenCloseLock();
            camera.Close();
            Log.Debug(TAG, "CameraDevice onDisconnected!");
            //Set camera devite as null
            mCameraHelper.SetCameraDevice(null);
        }

        public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
        {
            mCameraHelper.ReleaseCameraOpenCloseLock();
            camera.Close();
            Log.Debug(TAG, "CameraDevice onError!");
            //Set camera devite as null
            mCameraHelper.SetCameraDevice(null);
        }

        public override void OnOpened(CameraDevice camera)
        {
            mCameraHelper.SetCameraDevice(camera);
            Log.Debug(TAG, "CameraDevice onOpened!");
            mCameraHelper.StartPreview();
        }

    }

    /// <summary>
    /// CameraCaptureSession's State callback class.
    /// </summary>
    public class CameraCaptureSessionStateCallback : CameraCaptureSession.StateCallback
    {
        private const string TAG = "CameraCaptureSessionStateCallback";
        CameraHelper mCameraHelper;

        public CameraCaptureSessionStateCallback(CameraHelper cameraHelper)
        {
            this.mCameraHelper = cameraHelper;
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            try
            {
                if (mCameraHelper.GetCameraDevice() == null)
                {
                    Log.Debug(TAG, "CameraDevice stop!");
                    return;
                }
                if (mCameraHelper.mPreViewSurface != null)
                {
                    mCameraHelper.mCaptureRequestBuilder.AddTarget(mCameraHelper.mPreViewSurface);
                }
                if (mCameraHelper.mVgaSurface != null)
                {
                    mCameraHelper.mCaptureRequestBuilder.AddTarget(mCameraHelper.mVgaSurface);
                }
                if (mCameraHelper.mDepthSurface != null)
                {
                    mCameraHelper.mCaptureRequestBuilder.AddTarget(mCameraHelper.mDepthSurface);
                }

                // Set the number of frames to 30.
                Android.Util.Range fpsRange = new Android.Util.Range(30, 30);
                mCameraHelper.mCaptureRequestBuilder.Set(CaptureRequest.ControlAeTargetFpsRange, fpsRange);
                List<CaptureRequest> captureRequests = new List<CaptureRequest>();
                captureRequests.Add(mCameraHelper.mCaptureRequestBuilder.Build());
                mCameraHelper.mCameraCaptureSession = session;
                mCameraHelper.mCameraCaptureSession.SetRepeatingBurst(captureRequests, null, mCameraHelper.mCameraHandler);
                mCameraHelper.mCameraOpenCloseLock.Release();
            }
            catch (CameraAccessException e)
            {
                Log.Debug(TAG, "CaptureSession onConfigured error");
            }
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            Log.Debug(TAG, "CaptureSession OnConfiguredFailed error");
        }
    }

    /// <summary>
    /// Calculate the area difference.
    /// </summary>
    public class CalculatedAreaDifference : Java.Lang.Object, IComparator, ISerializable
    {
        private static readonly long serialVersionUID = 7710120461881073428L;
        public int Compare(Java.Lang.Object o1, Java.Lang.Object o2)
        {
            var lhs = o1.JavaCast<Size>();
            var rhs = o2.JavaCast<Size>();
            return Long.Signum(lhs.Width * lhs.Height - rhs.Width * rhs.Height);
        }
    }

}