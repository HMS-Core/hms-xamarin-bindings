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
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Java.Nio;
using Java.Util;
using XamarinAREngineDemo.Common;


namespace XamarinAREngineDemo.AREngineActivities.Hand.Rendering
{
    /// <summary>
    /// This class shows how to use the hand bounding box. With this class,
    /// a rectangular box bounding the hand can be displayed on the screen.
    /// </summary>
    public class HandBoxDisplay : HandRelatedDisplay
    {
        private const string TAG = "HandBoxDisplay";

        // Number of bytes occupied by each 3D coordinate. Float data occupies 4 bytes.
        // Each skeleton point represents a 3D coordinate.
        private static readonly int BYTES_PER_POINT = 4 * 3;
        private static readonly int INITIAL_BUFFER_POINTS = 150;
        private static readonly int COORDINATE_DIMENSION = 3;

        private int mVbo;

        private int mVboSize = INITIAL_BUFFER_POINTS * BYTES_PER_POINT;

        private int mProgram;

        private int mPosition;

        private int mColor;

        private int mModelViewProjectionMatrix;

        private int mPointSize;

        private int mNumPoints = 0;

        private float[] mMVPMatrix;

        /// <summary>
        /// Create and build a shader for the hand gestures on the OpenGL thread.
        /// This method is called when HandRenderManager's OnSurfaceCreated.
        /// </summary>
        public void Init()
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");
            mMVPMatrix = MatrixUtil.GetOriginalMatrix();
            int[] buffers = new int[1];
            GLES20.GlGenBuffers(1, buffers, 0);
            mVbo = buffers[0];
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            CreateProgram();
            GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "Create program start.");
            mProgram = HandShaderUtil.CreateGlProgram();
            mPosition = GLES20.GlGetAttribLocation(mProgram, "inPosition");
            mColor = GLES20.GlGetUniformLocation(mProgram, "inColor");
            mPointSize = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
            mModelViewProjectionMatrix = GLES20.GlGetUniformLocation(mProgram, "inMVPMatrix");
            ShaderUtil.CheckGlError(TAG, "Create program start.");
        }

        /// <summary>
        /// Render the hand bounding box and hand information.
        /// This method is called when HandRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="hands">Hand data.</param>
        /// <param name="projectionMatrix">ARCamera projection matrix.</param>
        public void OnDrawFrame(System.Collections.ICollection hands, float[] projectionMatrix)
        {
            if (hands.Count == 0)
            {
                return;
            }
            if (projectionMatrix != null)
            {
                Log.Debug(TAG, "Camera projection matrix: " + Arrays.ToString(projectionMatrix));
            }
            foreach (ARHand hand in hands)
            {
                float[] gestureHandBoxPoints = hand.GetGestureHandBox();
                if (hand.TrackingState == ARTrackableTrackingState.Tracking)
                {
                    UpdateHandBoxData(gestureHandBoxPoints);
                    DrawHandBox();
                }
            }
        }

        /// <summary>
        /// Update the coordinates of the hand bounding box.
        /// </summary>
        /// <param name="gesturePoints">Gesture hand box data.</param>
        private void UpdateHandBoxData(float[] gesturePoints)
        {
            ShaderUtil.CheckGlError(TAG, "Update hand box data start.");
            float[] glGesturePoints = {
                // Get the four coordinates of a rectangular box bounding the hand.
                gesturePoints[0], gesturePoints[1], gesturePoints[2],
                gesturePoints[3], gesturePoints[1], gesturePoints[2],
                gesturePoints[3], gesturePoints[4], gesturePoints[5],
                gesturePoints[0], gesturePoints[4], gesturePoints[5],
            };
            int gesturePointsNum = glGesturePoints.Length / COORDINATE_DIMENSION;

            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            mNumPoints = gesturePointsNum;
            if (mVboSize < mNumPoints * BYTES_PER_POINT)
            {
                while (mVboSize < mNumPoints * BYTES_PER_POINT)
                {
                    // If the size of VBO is insufficient to accommodate the new point cloud, resize the VBO.
                    mVboSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            }
            Log.Debug(TAG, "gesture.getGestureHandPointsNum()" + mNumPoints);
            FloatBuffer mVertices = FloatBuffer.Wrap(glGesturePoints);
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mNumPoints * BYTES_PER_POINT,
                mVertices);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Update hand box data end.");
        }

        /// <summary>
        /// Render the hand bounding box.
        /// </summary>
        private void DrawHandBox()
        {
            ShaderUtil.CheckGlError(TAG, "Draw hand box start.");
            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlEnableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
            GLES20.GlVertexAttribPointer(
                mPosition, COORDINATE_DIMENSION, GLES20.GlFloat, false, BYTES_PER_POINT, 0);
            GLES20.GlUniform4f(mColor, 1.0f, 0.0f, 0.0f, 1.0f);

            GLES20.GlUniformMatrix4fv(mModelViewProjectionMatrix, 1, false, mMVPMatrix, 0);

            // Set the size of the rendering vertex.
            GLES20.GlUniform1f(mPointSize, 50.0f);

            // Set the width of a rendering stroke.
            GLES20.GlLineWidth(18.0f);
            GLES20.GlDrawArrays(GLES20.GlLineLoop, 0, mNumPoints);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlDisableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Draw hand box end.");
        }
    }
}