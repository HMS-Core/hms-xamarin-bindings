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
using Java.Util;
using Javax.Microedition.Khronos.Opengles;
using XamarinAREngineDemo.Common;

using static XamarinAREngineDemo.Common.TextDisplay;
using StringBuilder = System.Text.StringBuilder;

namespace XamarinAREngineDemo.AREngineActivities.Face.Rendering
{
    /// <summary>
    /// This class manages rendering related to facial data.
    /// </summary>
    public class FaceRenderManager : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private const string TAG = "FaceRenderManager";

        private const float UPDATE_INTERVAL = 0.5f;

        private int frames = 0;

        private long lastInterval;

        private ARSession mArSession;

        private float fps;

        private Context mContext;

        private Activity mActivity;

        private TextView mTextView;

        private bool isOpenCameraOutside = true;

        private int mTextureId = -1; // Initialize the texture ID.

        private TextureDisplay mTextureDisplay = new TextureDisplay();

        private FaceGeometryDisplay mFaceGeometryDisplay = new FaceGeometryDisplay();

        private TextDisplay mTextDisplay = new TextDisplay();

        private DisplayRotationManager mDisplayRotationManager;

        /// <summary>
        /// The constructor initializes context and activity.
        /// This method will be called when Activity's OnCreate.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="activity">Activity</param>
        public FaceRenderManager(Context context, Activity activity)
        {
            mContext = context;
            mActivity = activity;
        }

        /// <summary>
        /// Set an ARSession. The input ARSession will be called in OnDrawFrame
        /// to obtain the latest data. This method is called when Activity's OnResume. 
        /// </summary>
        /// <param name="arSession">ARSession.</param>
        public void SetArSession(ARSession arSession)
        {
            if (arSession == null)
            {
                Log.Debug(TAG, "Set session error, arSession is null!");
                return;
            }
            mArSession = arSession;
        }

        /// <summary>
        /// Set the external camera open flag. If the value is true, the app opens the camera
        /// by itself and creates a texture ID during background rendering. Otherwise, the camera
        /// is opened by AR Engine.This method is called when Activity's OnResume.
        /// </summary>
        /// <param name="isOpenCameraOutsideFlag">Flag indicating the mode of opening the camera.</param>
        public void SetOpenCameraOutsideFlag(bool isOpenCameraOutsideFlag)
        {
            isOpenCameraOutside = isOpenCameraOutsideFlag;
        }

        /// <summary>
        /// Set the texture ID for background rendering. 
        /// This method will be called when Activity's OnResume
        /// </summary>
        /// <param name="textureId">Texture ID.</param>
        public void SetTextureId(int textureId)
        {
            mTextureId = textureId;
        }

        /// <summary>
        /// Set the displayRotationManage object, which will be used in OnSurfaceChanged
        /// and OnDrawFrame.This method is called when Activity's OnResume.
        /// </summary>
        /// <param name="displayRotationManager">DisplayRotationManager.</param>
        public void SetDisplayRotationManager(DisplayRotationManager displayRotationManager)
        {
            if (displayRotationManager == null)
            {
                Log.Debug(TAG, "Set display rotation manage error, displayRotationManage is null!");
                return;
            }
            mDisplayRotationManager = displayRotationManager;
        }

        /// <summary>
        /// Set TextView. This object will be used in the UI thread.
        /// This method is called when Activity's OnCreate.
        /// </summary>
        /// <param name="textView">TextView.</param>
        public void SetTextView(TextView textView)
        {
            if (textView == null)
            {
                Log.Debug(TAG, "Set text view error, textView is null!");
                return;
            }
            mTextView = textView;
        }
        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            GLES20.GlClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            if (isOpenCameraOutside)
            {
                mTextureDisplay.Init(mTextureId);
            }
            else
            {
                mTextureDisplay.Init();
            }
            Log.Debug(TAG, "On surface created textureId= " + mTextureId);

            mFaceGeometryDisplay.Init(mContext);

            mTextDisplay.SetListener(new FaceOnTextInfoChangeListenerClass(this));
        }

        /// <summary>
        /// Create a thread for text display on the UI. The method for displaying texts is called back in TextureDisplay.
        /// </summary>
        /// <param name="text">Information displayed on the screen.</param>
        /// <param name="positionX">X coordinate of a point.</param>
        /// <param name="positionY">Y coordinate of a point.</param>
        public void ShowTextViewOnUiThread(string text, float positionX, float positionY)
        {

            mActivity.RunOnUiThread(() => {
                mTextView.SetTextColor(Color.White);

                // Set the size of the text displayed on the screen.
                mTextView.TextSize = 10f;
                if (text != null)
                {
                    mTextView.Text = text;
                    mTextView.SetPadding((int)positionX, (int)positionY, 0, 0);
                }
                else
                {
                    mTextView.Text = "";
                }
            });

        }

    public void OnDrawFrame(IGL10 gl)
        {
            GLES20.GlClear(GLES20.GlColorBufferBit | GLES20.GlDepthBufferBit);

            if (mArSession == null)
            {
                return;
            }
            if (mDisplayRotationManager.GetDeviceRotation())
            {
                mDisplayRotationManager.UpdateArSessionDisplayGeometry(mArSession);
            }

            try
            {
                mArSession.SetCameraTextureName(mTextureDisplay.GetExternalTextureId());
                ARFrame frame = mArSession.Update();
                mTextureDisplay.OnDrawFrame(frame);
                float fpsResult = DoFpsCalculate();

                System.Collections.ICollection faces = (System.Collections.ICollection)mArSession.GetAllTrackables(Java.Lang.Class.FromType(typeof(ARFace)));

                if (faces.Count == 0) {
                    mTextDisplay.OnDrawFrame(null);
                    return;
                }
                Log.Debug(TAG, "Face number: " + faces.Count);
                        ARCamera camera = frame.Camera;
                        foreach (ARFace face in faces) {
                            if (face.TrackingState == ARTrackableTrackingState.Tracking) {
                                mFaceGeometryDisplay.OnDrawFrame(camera, face);
                                StringBuilder sb = new StringBuilder();
                                UpdateMessageData(sb, fpsResult, face);
                                mTextDisplay.OnDrawFrame(sb);
                            }
                        }
            } 
            catch (ArDemoRuntimeException e)
            {
                Log.Debug(TAG, "Exception on the ArDemoRuntimeException!");
            }
            catch (Throwable t)
            {
                // This prevents the app from crashing due to unhandled exceptions.
                Log.Debug(TAG, "Exception on the OpenGL thread", t);
            }
        }

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            mTextureDisplay.OnSurfaceChanged(width, height);
            GLES20.GlViewport(0, 0, width, height);
            mDisplayRotationManager.UpdateViewportRotation(width, height);
        }

        private void UpdateMessageData(StringBuilder sb, float fpsResult, ARFace face)
        {
            sb.Append("FPS= ").Append(fpsResult).Append(System.Environment.NewLine);
            ARPose pose = face.Pose;
            if (pose != null)
            {
                sb.Append("face pose information:");
                sb.Append("face pose tx:[").Append(pose.Tx()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose ty:[").Append(pose.Ty()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose tz:[").Append(pose.Tz()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose qx:[").Append(pose.Qx()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose qy:[").Append(pose.Qy()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose qz:[").Append(pose.Qz()).Append("]").Append(System.Environment.NewLine);
                sb.Append("face pose qw:[").Append(pose.Qw()).Append("]").Append(System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);

            float[] textureCoordinates = new float[face.FaceGeometry.TextureCoordinates.Capacity()];
            for(int i = 0; i == face.FaceGeometry.TextureCoordinates.Capacity(); i++)
            {
                textureCoordinates[i] = face.FaceGeometry.TextureCoordinates.Get(i);
            }
            sb.Append("textureCoordinates length:[ ").Append(textureCoordinates.Length).Append(" ]");
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

    /// <summary>
    /// This class used for handle text to display on screen
    /// </summary>
    public class FaceOnTextInfoChangeListenerClass : OnTextInfoChangeListener
    {
        FaceRenderManager mRenderManager;

        public FaceOnTextInfoChangeListenerClass(FaceRenderManager mRenderManager)
        {
            this.mRenderManager = mRenderManager;
        }
        public void TextInfoChanged(string text, float positionX, float positionY)
        {
            mRenderManager.ShowTextViewOnUiThread(text, positionX, positionY);
        }
    }
}