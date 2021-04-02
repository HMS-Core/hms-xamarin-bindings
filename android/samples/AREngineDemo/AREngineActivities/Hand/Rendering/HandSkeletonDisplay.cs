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
using XamarinAREngineDemo.Common;


namespace XamarinAREngineDemo.AREngineActivities.Hand.Rendering
{
    /// <summary>
    /// Draw hand skeleton points based on the coordinates of the hand skeleton points.
    /// </summary>
    public class HandSkeletonDisplay : HandRelatedDisplay
    {
        private const string TAG = "HandSkeletonDisplay";

        // Number of bytes occupied by each 3D coordinate.Float data occupies 4 bytes.
        // Each skeleton point represents a 3D coordinate
        private static readonly int BYTES_PER_POINT = 4 * 3;

        private static readonly int INITIAL_POINTS_SIZE = 150;

        private int mVbo;

        private int mVboSize;

        private int mProgram;

        private int mPosition;

        private int mModelViewProjectionMatrix;

        private int mColor;

        private int mPointSize;

        private int mNumPoints = 0;

        /// <summary>
        /// Create and build a shader for the hand skeleton points on the OpenGL thread.
        /// this is called when HandRenderManager's OnSurfaceCreated.
        /// </summary>
        public void Init()
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");
            int[] buffers = new int[1];
            GLES20.GlGenBuffers(1, buffers, 0);
            mVbo = buffers[0];
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
            mVboSize = INITIAL_POINTS_SIZE * BYTES_PER_POINT;
            GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            CreateProgram();
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "Create program start.");
            mProgram = HandShaderUtil.CreateGlProgram();
            ShaderUtil.CheckGlError(TAG, "program");
            mPosition = GLES20.GlGetAttribLocation(mProgram, "inPosition");
            mColor = GLES20.GlGetUniformLocation(mProgram, "inColor");
            mPointSize = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
            mModelViewProjectionMatrix = GLES20.GlGetUniformLocation(mProgram, "inMVPMatrix");
            ShaderUtil.CheckGlError(TAG, "Create program end.");
        }

        /// <summary>
        /// Draw hand skeleton points. This method is called when HandRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="hands">ARHand data collection.</param>
        /// <param name="projectionMatrix">Projection matrix(4 * 4).</param>
        public void OnDrawFrame(ICollection hands, float[] projectionMatrix)
        {
            // Verify external input. If the hand data is empty, the projection matrix is empty,
            // or the projection matrix is not 4 x 4, rendering is not performed.
            if (hands.Count == 0 || projectionMatrix == null || projectionMatrix.Length != 16)
            {
                Log.Debug(TAG, "onDrawFrame Illegal external input!");
                return;
            }
            foreach (ARHand hand in hands)
            {
                float[] handSkeletons = hand.GetHandskeletonArray();
                if (handSkeletons.Length == 0)
                {
                    continue;
                }
                UpdateHandSkeletonsData(handSkeletons);
                DrawHandSkeletons(projectionMatrix);
            }
        }

        /// <summary>
        /// Update the coordinates of hand skeleton points.
        /// </summary>
        private void UpdateHandSkeletonsData(float[] handSkeletons)
        {
            ShaderUtil.CheckGlError(TAG, "Update hand skeletons data start.");

            // Each point has a 3D coordinate. The total number of coordinates
            // is three times the number of skeleton points.
            int mPointsNum = handSkeletons.Length / 3;
            Log.Debug(TAG, "ARHand HandSkeletonNumber = " + mPointsNum);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
            mNumPoints = mPointsNum;
            if (mVboSize < mNumPoints * BYTES_PER_POINT)
            {
                while (mVboSize < mNumPoints * BYTES_PER_POINT)
                {
                    // If the size of VBO is insufficient to accommodate the new point cloud, resize the VBO.
                    mVboSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            }
            FloatBuffer mSkeletonPoints = FloatBuffer.Wrap(handSkeletons);
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mNumPoints * BYTES_PER_POINT,
                mSkeletonPoints);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Update hand skeletons data end.");
        }

        /// <summary>
        /// Draw hand skeleton points.
        /// </summary>
        /// <param name="projectionMatrix">Projection matrix.</param>
        private void DrawHandSkeletons(float[] projectionMatrix)
        {
            ShaderUtil.CheckGlError(TAG, "Draw hand skeletons start.");
            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            // The size of the vertex attribute is 4, and each vertex has four coordinate components
            GLES20.GlVertexAttribPointer(
                mPosition, 4, GLES20.GlFloat, false, BYTES_PER_POINT, 0);

            // Set the color of the skeleton points to blue.
            GLES20.GlUniform4f(mColor, 0.0f, 0.0f, 1.0f, 1.0f);
            GLES20.GlUniformMatrix4fv(mModelViewProjectionMatrix, 1, false, projectionMatrix, 0);

            // Set the size of the skeleton points.
            GLES20.GlUniform1f(mPointSize, 30.0f);

            GLES20.GlDrawArrays(GLES20.GlPoints, 0, mNumPoints);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Draw hand skeletons end.");
        }
    }
}