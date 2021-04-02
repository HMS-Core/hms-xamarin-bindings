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
using System.Collections.ObjectModel;
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
using Java.Lang;
using Javax.Microedition.Khronos.Opengles;
using XamarinAREngineDemo.Common;
using static XamarinAREngineDemo.Common.TextDisplay;
using StringBuilder = System.Text.StringBuilder;

namespace XamarinAREngineDemo.AREngineActivities.Body3d.Rendering
{
    /// <summary>
    /// This class renders personal data obtained by the AR Engine.
    /// </summary>
    public class BodyRenderManager : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private const string TAG = "BodyRenderManager";

        private const int PROJECTION_MATRIX_OFFSET = 0;

        private const float PROJECTION_MATRIX_NEAR = 0.1f;

        private const float PROJECTION_MATRIX_FAR = 100.0f;

        private const float UPDATE_INTERVAL = 0.5f;

        private int frames = 0;

        private long lastInterval;

        private ARSession mSession;

        private float fps;

        private Activity mActivity;

        private TextView mTextView;

        private TextureDisplay mTextureDisplay = new TextureDisplay();

        private TextDisplay mTextDisplay = new TextDisplay();

        private List<BodyRelatedDisplay> mBodyRelatedDisplays = new List<BodyRelatedDisplay>();

        private DisplayRotationManager mDisplayRotationManager;

        /// <summary>
        /// The constructor passes activity.
        /// </summary>
        /// <param name="activity">Activity</param>
        public BodyRenderManager(Activity activity)
        {
            mActivity = activity;
            BodyRelatedDisplay bodySkeletonDisplay = new BodySkeletonDisplay();
            BodyRelatedDisplay bodySkeletonLineDisplay = new BodySkeletonLineDisplay();
            mBodyRelatedDisplays.Add(bodySkeletonDisplay);
            mBodyRelatedDisplays.Add(bodySkeletonLineDisplay);
        }

        /// <summary>
        /// Set the AR session to be updated in OnDrawFrame to obtain the latest data.
        /// </summary>
        /// <param name="arSession">ARSession</param>
        public void SetArSession(ARSession arSession)
        {
            if (arSession == null)
            {
                Log.Info(TAG, "Set session error, arSession is null!");
                return;
            }
            mSession = arSession;
        }

        /// <summary>
        /// Set displayRotationManage, which is used in OnSurfaceChanged and OnDrawFrame.
        /// </summary>
        /// <param name="displayRotationManager">DisplayRotationManage</param>
        public void SetDisplayRotationManager(DisplayRotationManager displayRotationManager)
        {
            if (displayRotationManager == null)
            {
                Log.Info(TAG, "Set display rotation manage error, display rotation manage is null!");
                return;
            }
            mDisplayRotationManager = displayRotationManager;
        }

        /// <summary>
        /// Set TextView, which is called in the UI thread to display data correctly.
        /// </summary>
        /// <param name="textView">TextView.</param>
        public void SetTextView(TextView textView)
        {
            if (textView == null)
            {
                Log.Info(TAG, "Set textView error, text view is null!");
                return;
            }
            mTextView = textView;
        }

        public void OnDrawFrame(IGL10 gl)
        {
            // Clear the screen to notify the driver not to load pixels of the previous frame.
            GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

            if (mSession == null)
            {
                return;
            }
            if (mDisplayRotationManager.GetDeviceRotation())
            {
                mDisplayRotationManager.UpdateArSessionDisplayGeometry(mSession);
            }

            try
            {
                mSession.SetCameraTextureName(mTextureDisplay.GetExternalTextureId());
                ARFrame frame = mSession.Update();

                // The size of the projection matrix is 4 * 4.
                float[] projectionMatrix = new float[16];
                ARCamera camera = frame.Camera;

                // Obtain the projection matrix of ARCamera.
                camera.GetProjectionMatrix(projectionMatrix, PROJECTION_MATRIX_OFFSET, PROJECTION_MATRIX_NEAR,
                    PROJECTION_MATRIX_FAR);
                mTextureDisplay.OnDrawFrame(frame);
                ICollection bodies = mSession.GetAllTrackables(Java.Lang.Class.FromType(typeof(ARBody)));
                if (bodies.Count == 0) {
                    mTextDisplay.OnDrawFrame(null);
                    return;
                }
                foreach (ARBody body in bodies)
                {
                    if (body.TrackingState != ARTrackableTrackingState.Tracking)
                    {
                        continue;
                    }

                    // Update the body recognition information to be displayed on the screen.
                    StringBuilder sb = new StringBuilder();
                    UpdateMessageData(sb, body);

                    // Display the updated body information on the screen.
                    mTextDisplay.OnDrawFrame(sb);
                }
                foreach (BodyRelatedDisplay bodyRelatedDisplay in mBodyRelatedDisplays)
                {
                    bodyRelatedDisplay.OnDrawFrame(bodies, projectionMatrix);
                }
            }
            catch(ArDemoRuntimeException e)
            {
                Log.Info(TAG, "Exception on the ArDemoRuntimeException!");
            }
            catch(Throwable t)
            {
                // This prevents the app from crashing due to unhandled exceptions.
                Log.Info(TAG, "Exception on the OpenGL thread");
            }
        }

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            mTextureDisplay.OnSurfaceChanged(width, height);
            GLES20.GlViewport(0, 0, width, height);
            mDisplayRotationManager.UpdateViewportRotation(width, height);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            // Clear the color and set the window color.
            GLES20.GlClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            foreach (BodyRelatedDisplay bodyRelatedDisplay in mBodyRelatedDisplays)
            {
                bodyRelatedDisplay.Init();
            }
            mTextureDisplay.Init();
            mTextDisplay.SetListener(new BodyOnTextInfoChangeListenerClass(this));
        }

        public void ShowTextViewOnUiThread(string text, float positionX, float positionY)
        {

            mActivity.RunOnUiThread(() => {
                mTextView.SetTextColor(Color.White);

                // Set the font size.
                mTextView.TextSize = 10f;
                if (text != null)
                {
                    mTextView.Text = text;
                    mTextView.SetPadding((int)positionX, (int)positionY, 0, 0);
                }
                else
                {
                    mTextView.Text = ("");
                }
            });

        }

        /// <summary>
        /// Update gesture-related data for display.
        /// </summary>
        /// <param name="sb">String buffer.</param>
        /// <param name="body">ARBody</param>
        private void UpdateMessageData(StringBuilder sb, ARBody body)
        {
            float fpsResult = DoFpsCalculate();
            sb.Append("FPS=").Append(fpsResult).Append(System.Environment.NewLine);
            int bodyAction = body.BodyAction;
            sb.Append("bodyAction=").Append(bodyAction).Append(System.Environment.NewLine);
        }

        private float DoFpsCalculate()
        {
            ++frames;
            long timeNow = Java.Lang.JavaSystem.CurrentTimeMillis();

            // Convert millisecond to second.
            if (((timeNow - lastInterval) / 1000.0f) > UPDATE_INTERVAL)
            {
                fps = frames / ((timeNow - lastInterval) / 1000.0f);
                frames = 0;
                lastInterval = timeNow;
            }
            return fps;
        }
    }

    public class BodyOnTextInfoChangeListenerClass : OnTextInfoChangeListener
    {
        BodyRenderManager mRenderManager;

        public BodyOnTextInfoChangeListenerClass(BodyRenderManager mRenderManager)
        {
            this.mRenderManager = mRenderManager;
        }
        public void TextInfoChanged(string text, float positionX, float positionY)
        {
            mRenderManager.ShowTextViewOnUiThread(text, positionX, positionY);
        }
    }
}