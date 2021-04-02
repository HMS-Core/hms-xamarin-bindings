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
    /// Draw hand skeleton connection line based on the coordinates of the hand skeleton points..
    /// </summary>
    public class HandSkeletonLineDisplay : HandRelatedDisplay
    {
        private const string TAG = "HandSkeletonLineDisplay";

        // Number of bytes occupied by each 3D coordinate.
        // Float data occupies 4 bytes. Each skeleton point represents a 3D coordinate
        private static readonly int BYTES_PER_POINT = 4 * 3;

        private static readonly int INITIAL_BUFFER_POINTS = 150;

        private static readonly float JOINT_POINT_SIZE = 100f;

        private int mVbo;

        private int mVboSize = INITIAL_BUFFER_POINTS * BYTES_PER_POINT;

        private int mProgram;

        private int mPosition;

        private int mModelViewProjectionMatrix;

        private int mColor;

        private int mPointSize;

        private int mPointsNum = 0;

        /// <summary>
        /// Create and build a shader for the hand skeleton line on the OpenGL thread,
        /// which is called when HandRenderManager's OnSurfaceCreated.
        /// </summary>
        public void Init()
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");

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
            ShaderUtil.CheckGlError(TAG, "program");
            mPosition = GLES20.GlGetAttribLocation(mProgram, "inPosition");
            mColor = GLES20.GlGetUniformLocation(mProgram, "inColor");
            mPointSize = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
            mModelViewProjectionMatrix = GLES20.GlGetUniformLocation(mProgram, "inMVPMatrix");
            ShaderUtil.CheckGlError(TAG, "Create program end.");
        }

        /// <summary>
        /// Draw hand skeleton connection line.
        /// This method is called when HandRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="hands">ARHand data collection.</param>
        /// <param name="projectionMatrix">ProjectionMatrix(4 * 4).</param>
        public void OnDrawFrame(ICollection hands, float[] projectionMatrix)
        {
            // Verify external input. If the hand data is empty, the projection matrix is empty,
            // or the projection matrix is not 4 * 4, rendering is not performed.
            if (hands.Count == 0 || projectionMatrix == null || projectionMatrix.Length != 16)
            {
                Log.Debug(TAG, "onDrawFrame Illegal external input!");
                return;
            }
            foreach (ARHand hand in hands)
            {
                float[] handSkeletons = hand.GetHandskeletonArray();
                int[] handSkeletonConnections = hand.GetHandSkeletonConnection();
                if (handSkeletons.Length == 0 || handSkeletonConnections.Length == 0)
                {
                    continue;
                }
                UpdateHandSkeletonLinesData(handSkeletons, handSkeletonConnections);
                DrawHandSkeletonLine(projectionMatrix);
            }
        }

        /// <summary>
        /// This method updates the connection data of skeleton points and is called when any frame is updated.
        /// </summary>
        /// <param name="handSkeletons">Bone point data of hand.</param>
        /// <param name="handSkeletonConnection">Data of connection between bone points of hand.</param>
        private void UpdateHandSkeletonLinesData(float[] handSkeletons, int[] handSkeletonConnection)
        {
            ShaderUtil.CheckGlError(TAG, "Update hand skeleton lines data start.");
            int pointsLineNum = 0;

            // Each point is a set of 3D coordinate. Each connection line consists of two points.
            float[] linePoint = new float[handSkeletonConnection.Length * 3 * 2];

            // The format of HandSkeletonConnection data is [p0,p1;p0,p3;p0,p5;p1,p2].
            // handSkeletonConnection saves the node indexes. Two indexes obtain a set
            // of connection point data. Therefore, j = j + 2. This loop obtains related
            // coordinates and saves them in linePoint.
            for (int j = 0; j < handSkeletonConnection.Length; j += 2)
            {
                linePoint[pointsLineNum * 3] = handSkeletons[3 * handSkeletonConnection[j]];
                linePoint[pointsLineNum * 3 + 1] = handSkeletons[3 * handSkeletonConnection[j] + 1];
                linePoint[pointsLineNum * 3 + 2] = handSkeletons[3 * handSkeletonConnection[j] + 2];
                linePoint[pointsLineNum * 3 + 3] = handSkeletons[3 * handSkeletonConnection[j + 1]];
                linePoint[pointsLineNum * 3 + 4] = handSkeletons[3 * handSkeletonConnection[j + 1] + 1];
                linePoint[pointsLineNum * 3 + 5] = handSkeletons[3 * handSkeletonConnection[j + 1] + 2];
                pointsLineNum += 2;
            }
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
            mPointsNum = pointsLineNum;

            // If the storage space is insufficient, apply for twice the memory each time.
            if (mVboSize < mPointsNum * BYTES_PER_POINT)
            {
                while (mVboSize < mPointsNum * BYTES_PER_POINT)
                {
                    mVboSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            }
            FloatBuffer linePoints = FloatBuffer.Wrap(linePoint);
            Log.Debug(TAG, "Skeleton skeleton line points num: " + mPointsNum);
            Log.Debug(TAG, "Skeleton line points: " + linePoints.ToString());
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mPointsNum * BYTES_PER_POINT,
                linePoints);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Update hand skeleton lines data end.");
        }

        /// <summary>
        /// Draw hand skeleton connection line.
        /// </summary>
        /// <param name="projectionMatrix">Projection matrix(4 * 4).</param>
        private void DrawHandSkeletonLine(float[] projectionMatrix)
        {
            ShaderUtil.CheckGlError(TAG, "Draw hand skeleton line start.");
            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlEnableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            // Set the width of the drawn line
            GLES20.GlLineWidth(18.0f);

            // Represented each point by 4D coordinates in the shader.
            GLES20.GlVertexAttribPointer(
                mPosition, 4, GLES20.GlFloat, false, BYTES_PER_POINT, 0);
            GLES20.GlUniform4f(mColor, 0.0f, 0.0f, 0.0f, 1.0f);
            GLES20.GlUniformMatrix4fv(mModelViewProjectionMatrix, 1, false, projectionMatrix, 0);

            GLES20.GlUniform1f(mPointSize, JOINT_POINT_SIZE);

            GLES20.GlDrawArrays(GLES20.GlLines, 0, mPointsNum);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlDisableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Draw hand skeleton line end.");
        }
    }
}