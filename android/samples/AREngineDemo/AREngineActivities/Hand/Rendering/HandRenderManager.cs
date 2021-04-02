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
using Javax.Microedition.Khronos.Opengles;
using XamarinAREngineDemo.Common;
using static XamarinAREngineDemo.Common.TextDisplay;

namespace XamarinAREngineDemo.AREngineActivities.Hand.Rendering
{
    /// <summary>
    /// This class shows how to render data obtained from HUAWEI AR Engine.
    /// </summary>
    public class HandRenderManager : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private const string TAG = "HandRenderManager";

        private static readonly int PROJECTION_MATRIX_OFFSET = 0;

        private static readonly float PROJECTION_MATRIX_NEAR = 0.1f;

        private static readonly float PROJECTION_MATRIX_FAR = 100.0f;

        private static readonly float UPDATE_INTERVAL = 0.5f;

        private int frames = 0;

        private long lastInterval;

        private ARSession mSession;

        private float fps;

        private Activity mActivity;

        private TextView mTextView;

        private TextureDisplay mTextureDisplay = new TextureDisplay();

        private TextDisplay mTextDisplay = new TextDisplay();

        private List<HandRelatedDisplay> mHandRelatedDisplays = new List<HandRelatedDisplay>();

        private DisplayRotationManager mDisplayRotationManager;

        /// <summary>
        /// The constructor that passes context and activity.
        /// The method will be called when Activity's OnCreate.
        /// </summary>
        /// <param name="activity">Activity</param>
        public HandRenderManager(Activity activity)
        {
            mActivity = activity;
            HandRelatedDisplay handBoxDisplay = new HandBoxDisplay();
            HandRelatedDisplay mHandSkeletonDisplay = new HandSkeletonDisplay();
            HandRelatedDisplay mHandSkeletonLineDisplay = new HandSkeletonLineDisplay();
            mHandRelatedDisplays.Add(handBoxDisplay);
            mHandRelatedDisplays.Add(mHandSkeletonDisplay);
            mHandRelatedDisplays.Add(mHandSkeletonLineDisplay);
        }

        /// <summary>
        /// Set the ARSession object, which is used to obtain the latest data in the OnDrawFrame method.
        /// </summary>
        /// <param name="arSession">ARSession.</param>
        public void SetArSession(ARSession arSession)
        {
            if (arSession == null)
            {
                Log.Info(TAG, "set session error, arSession is null!");
                return;
            }
            mSession = arSession;
        }

        /// <summary>
        /// Set the DisplayRotationManage object, which is used in OnSurfaceChanged and OnDrawFrame.
        /// </summary>
        /// <param name="displayRotationManager">DisplayRotationManager.</param>
        public void SetDisplayRotationManager(DisplayRotationManager displayRotationManager)
        {
            if (displayRotationManager == null)
            {
                Log.Info(TAG, "SetDisplayRotationManage error, displayRotationManage is null!");
                return;
            }
            mDisplayRotationManager = displayRotationManager;
        }

        /// <summary>
        /// Set the TextView object, which is called in the UI thread to display text.
        /// </summary>
        /// <param name="textView">TextView.</param>
        public void SetTextView(TextView textView)
        {
            if (textView == null)
            {
                Log.Info(TAG, "Set text view error, textView is null!");
                return;
            }
            mTextView = textView;
        }


        public void OnDrawFrame(IGL10 gl)
        {
            // Clear the color buffer and notify the driver not to load the data of the previous frame.
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
                ARFrame arFrame = mSession.Update();
                ARCamera arCamera = arFrame.Camera;

                // The size of the projection matrix is 4 * 4.
                float[] projectionMatrix = new float[16];

                // Obtain the projection matrix through ARCamera.
                arCamera.GetProjectionMatrix(projectionMatrix, PROJECTION_MATRIX_OFFSET, PROJECTION_MATRIX_NEAR,
                    PROJECTION_MATRIX_FAR);
                mTextureDisplay.OnDrawFrame(arFrame);
                ICollection hands = mSession.GetAllTrackables(Java.Lang.Class.FromType(typeof(ARHand)));
                if (hands.Count == 0)
                {
                    mTextDisplay.OnDrawFrame(null);
                    return;
                }
                foreach (ARHand hand in hands)
                {
                    // Update the hand recognition information to be displayed on the screen.
                    StringBuilder sb = new StringBuilder();
                    UpdateMessageData(sb, hand);

                    // Display hand recognition information on the screen.
                    mTextDisplay.OnDrawFrame(sb);
                }
                foreach (HandRelatedDisplay handRelatedDisplay in mHandRelatedDisplays)
                {
                    handRelatedDisplay.OnDrawFrame(hands, projectionMatrix);
                }
            }
            catch(ArDemoRuntimeException e)
            {
                Log.Info(TAG, "Exception on the ArDemoRuntimeException!");
            }
            catch (Exception t)
            {
                // This prevents the app from crashing due to unhandled exceptions.
                Log.Info(TAG, "Exception on the OpenGL thread " + t.Message);
            }
        }

        /// <summary>
        /// Update gesture-related information.
        /// </summary>
        /// <param name="sb">String buffer.</param>
        /// <param name="hand">ARHand.</param>
        private void UpdateMessageData(StringBuilder sb, ARHand hand)
        {
            float fpsResult = DoFpsCalculate();
            sb.Append("FPS=").Append(fpsResult).Append(System.Environment.NewLine);
            AddHandNormalStringBuffer(sb, hand);
            AddGestureActionStringBuffer(sb, hand);
            AddGestureCenterStringBuffer(sb, hand);
            float[] gestureHandBoxPoints = hand.GetGestureHandBox();

            sb.Append("GestureHandBox length:[").Append(gestureHandBoxPoints.Length).Append("]")
                .Append(System.Environment.NewLine);
            for (int i = 0; i < gestureHandBoxPoints.Length; i++)
            {
                Log.Info(TAG, "gesturePoints:" + gestureHandBoxPoints[i]);
                sb.Append("gesturePoints[").Append(i).Append("]:[").Append(gestureHandBoxPoints[i]).Append("]")
                    .Append(System.Environment.NewLine);
            }
            AddHandSkeletonStringBuffer(sb, hand);
        }

        private void AddHandNormalStringBuffer(StringBuilder sb, ARHand hand)
        {
            sb.Append("GestureType=").Append(hand.GestureType).Append(System.Environment.NewLine);
            sb.Append("GestureCoordinateSystem=").Append(hand.GestureCoordinateSystem).Append(System.Environment.NewLine);
            float[] gestureOrientation = hand.GetGestureOrientation();
            sb.Append("gestureOrientation length:[").Append(gestureOrientation.Length).Append("]")
                .Append(System.Environment.NewLine);
            for (int i = 0; i < gestureOrientation.Length; i++)
            {
                Log.Info(TAG, "gestureOrientation:" + gestureOrientation[i]);
                sb.Append("gestureOrientation[").Append(i).Append("]:[").Append(gestureOrientation[i])
                    .Append("]").Append(System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);
        }

        private void AddGestureActionStringBuffer(StringBuilder sb, ARHand hand)
        {
            int[] gestureAction = hand.GetGestureAction();
            sb.Append("gestureAction length:[").Append(gestureAction.Length).Append("]").Append(System.Environment.NewLine);
            for (int i = 0; i < gestureAction.Length; i++)
            {
                Log.Info(TAG, "GestureAction:" + gestureAction[i]);
                sb.Append("gestureAction[").Append(i).Append("]:[").Append(gestureAction[i])
                    .Append("]").Append(System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);
        }

        private void AddGestureCenterStringBuffer(StringBuilder sb, ARHand hand)
        {
            float[] gestureCenter = hand.GetGestureCenter();
            sb.Append("gestureCenter length:[").Append(gestureCenter.Length).Append("]").Append(System.Environment.NewLine);
            for (int i = 0; i < gestureCenter.Length; i++)
            {
                Log.Info(TAG, "GestureCenter:" + gestureCenter[i]);
                sb.Append("gestureCenter[").Append(i).Append("]:[").Append(gestureCenter[i])
                    .Append("]").Append(System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);
        }

        private void AddHandSkeletonStringBuffer(StringBuilder sb, ARHand hand)
        {
            sb.Append(System.Environment.NewLine).Append("Handtype=").Append(hand.Handtype)
                .Append(System.Environment.NewLine);
            sb.Append("SkeletonCoordinateSystem=").Append(hand.SkeletonCoordinateSystem);
            sb.Append(System.Environment.NewLine);
            float[] skeletonArray = hand.GetHandskeletonArray();
            sb.Append("HandskeletonArray length:[").Append(skeletonArray.Length).Append("]")
                .Append(System.Environment.NewLine);
            Log.Info(TAG, "SkeletonArray.length:" + skeletonArray.Length);
            for (int i = 0; i < skeletonArray.Length; i++)
            {
                Log.Info(TAG, "SkeletonArray:" + skeletonArray[i]);
            }
            sb.Append(System.Environment.NewLine);
            int[] handSkeletonConnection = hand.GetHandSkeletonConnection();
            sb.Append("HandSkeletonConnection length:[").Append(handSkeletonConnection.Length)
                .Append("]").Append(System.Environment.NewLine);
            Log.Info(TAG, "handSkeletonConnection.length:" + handSkeletonConnection.Length);
            for (int i = 0; i < handSkeletonConnection.Length; i++)
            {
                Log.Info(TAG, "handSkeletonConnection:" + handSkeletonConnection[i]);
            }
            sb.Append(System.Environment.NewLine).Append("-----------------------------------------------------");
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

        public void OnSurfaceChanged(IGL10 gl, int width, int height)
        {
            mTextureDisplay.OnSurfaceChanged(width, height);
            GLES20.GlViewport(0, 0, width, height);
            mDisplayRotationManager.UpdateViewportRotation(width, height);
        }

        public void OnSurfaceCreated(IGL10 gl, Javax.Microedition.Khronos.Egl.EGLConfig config)
        {
            // Clear the original color and set a new color.
            GLES20.GlClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            foreach (HandRelatedDisplay handRelatedDisplay in mHandRelatedDisplays)
            {
                handRelatedDisplay.Init();
            }
            mTextureDisplay.Init();
            mTextDisplay.SetListener(new HandOnTextInfoChangeListenerClass(this));
        }

        /// <summary>
        /// Create a text display thread that is used for text update tasks.
        /// </summary>
        /// <param name="text">Gesture information displayed on the screen</param>
        /// <param name="positionX">The left padding in pixels.</param>
        /// <param name="positionY">The right padding in pixels.</param>
        public void ShowHandTypeTextView(string text, float positionX, float positionY)
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
    }

    public class HandOnTextInfoChangeListenerClass : OnTextInfoChangeListener
    {
        HandRenderManager mRenderManager;

        public HandOnTextInfoChangeListenerClass(HandRenderManager mRenderManager)
        {
            this.mRenderManager = mRenderManager;
        }
        public void TextInfoChanged(string text, float positionX, float positionY)
        {
            mRenderManager.ShowHandTypeTextView(text, positionX, positionY);
        }
    }
}