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
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Java.Nio;
using XamarinAREngineDemo.Common;

namespace XamarinAREngineDemo.AREngineActivities.Body3d.Rendering
{
    /// <summary>
    /// Gets the skeleton point connection data and pass it to OpenGL ES for rendering on the screen.
    /// </summary>
    public class BodySkeletonLineDisplay : BodyRelatedDisplay
    {
        private const string TAG = "BodySkeletonLineDisplay";

        // Number of bytes occupied by each 3D coordinate. Float data occupies 4 bytes.
        // Each skeleton point represents a 3D coordinate.
        private static readonly int BYTES_PER_POINT = 4 * 3;

        private static readonly int INITIAL_BUFFER_POINTS = 150;

        private static readonly float COORDINATE_SYSTEM_TYPE_3D_FLAG = 2.0f;

        private static readonly int LINE_POINT_RATIO = 6;

        private int mVbo;

        private int mVboSize = INITIAL_BUFFER_POINTS * BYTES_PER_POINT;

        private int mProgram;

        private int mPosition;

        private int mProjectionMatrix;

        private int mColor;

        private int mPointSize;

        private int mCoordinateSystem;

        private int mNumPoints = 0;

        private int mPointsLineNum = 0;

        private FloatBuffer mLinePoints;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BodySkeletonLineDisplay()
        {
        }

        /// <summary>
        /// Create a body skeleton line shader on the GL thread.
        /// This method is called when BodyRenderManager's OnSurfaceCreated.
        /// </summary>
        public void Init()
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");

            int[] buffers = new int[1];
            GLES20.GlGenBuffers(1, buffers, 0);
            mVbo = buffers[0];
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            ShaderUtil.CheckGlError(TAG, "Before create gl program.");
            CreateProgram();
            GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "Create gl program start.");
            mProgram = BodyShaderUtil.CreateGlProgram();
            mPosition = GLES20.GlGetAttribLocation(mProgram, "inPosition");
            mColor = GLES20.GlGetUniformLocation(mProgram, "inColor");
            mPointSize = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
            mProjectionMatrix = GLES20.GlGetUniformLocation(mProgram, "inProjectionMatrix");
            mCoordinateSystem = GLES20.GlGetUniformLocation(mProgram, "inCoordinateSystem");
            ShaderUtil.CheckGlError(TAG, "Create gl program end.");
        }

        private void DrawSkeletonLine(float coordinate, float[] projectionMatrix)
        {
            ShaderUtil.CheckGlError(TAG, "Draw skeleton line start.");
            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlEnableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            // Set the width of the rendered skeleton line.
            GLES20.GlLineWidth(18.0f);

            // The size of the vertex attribute is 4, and each vertex has four coordinate components.
            GLES20.GlVertexAttribPointer(
                mPosition, 4, GLES20.GlFloat, false, BYTES_PER_POINT, 0);
            GLES20.GlUniform4f(mColor, 1.0f, 0.0f, 0.0f, 1.0f);
            GLES20.GlUniformMatrix4fv(mProjectionMatrix, 1, false, projectionMatrix, 0);

            // Set the size of the points.
            GLES20.GlUniform1f(mPointSize, 100.0f);
            GLES20.GlUniform1f(mCoordinateSystem, coordinate);

            GLES20.GlDrawArrays(GLES20.GlLines, 0, mNumPoints);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlDisableVertexAttribArray(mColor);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Draw skeleton line end.");
        }

        /// <summary>
        /// Rendering lines between body bones.
        /// This method is called when calling BodyRenderManager's OnDrawFrame method.
        /// </summary>
        /// <param name="bodies">Bodies data.</param>
        /// <param name="projectionMatrix">Projection matrix.</param>
        public void OnDrawFrame(ICollection bodies, float[] projectionMatrix)
        {
            foreach (ARBody body in bodies)
            {
                if (body.TrackingState == ARTrackableTrackingState.Tracking)
                {
                    float coordinate = 1.0f;
                    if (body.CoordinateSystemType == ARCoordinateSystemType.CoordinateSystemType3dCamera)
                    {
                        coordinate = COORDINATE_SYSTEM_TYPE_3D_FLAG;
                    }
                    UpdateBodySkeletonLineData(body);
                    DrawSkeletonLine(coordinate, projectionMatrix);
                }
            }
        }

        /// <summary>
        /// Update body connection data.
        /// </summary>
        private void UpdateBodySkeletonLineData(ARBody body)
        {
            FindValidConnectionSkeletonLines(body);
            ShaderUtil.CheckGlError(TAG, "Update body skeleton line data start.");
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);
            mNumPoints = mPointsLineNum;
            if (mVboSize < mNumPoints * BYTES_PER_POINT)
            {
                while (mVboSize < mNumPoints * BYTES_PER_POINT)
                {
                    // If the storage space is insufficient, allocate double the space.
                    mVboSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlArrayBuffer, mVboSize, null, GLES20.GlDynamicDraw);
            }
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mNumPoints * BYTES_PER_POINT, mLinePoints);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Update body skeleton line data end.");
        }

        private void FindValidConnectionSkeletonLines(ARBody arBody)
        {
            mPointsLineNum = 0;
            int[] connections = arBody.GetBodySkeletonConnection();
            float[] linePoints = new float[LINE_POINT_RATIO * connections.Length];
            float[] coors;
            int[] isExists;

            if (arBody.CoordinateSystemType == ARCoordinateSystemType.CoordinateSystemType3dCamera)
            {
                coors = arBody.GetSkeletonPoint3D();
                isExists = arBody.GetSkeletonPointIsExist3D();
            }
            else
            {
                coors = arBody.GetSkeletonPoint2D();
                isExists = arBody.GetSkeletonPointIsExist2D();
            }

            // Filter out valid skeleton connection lines based on the returned results,
            // which consist of indexes of two ends, for example, [p0,p1;p0,p3;p0,p5;p1,p2].
            // The loop takes out the 3D coordinates of the end points of the valid connection
            // line and saves them in sequence.
            for (int j = 0; j < connections.Length; j += 2)
            {
                if (isExists[connections[j]] != 0 && isExists[connections[j + 1]] != 0)
                {
                    linePoints[mPointsLineNum * 3] = coors[3 * connections[j]];
                    linePoints[mPointsLineNum * 3 + 1] = coors[3 * connections[j] + 1];
                    linePoints[mPointsLineNum * 3 + 2] = coors[3 * connections[j] + 2];
                    linePoints[mPointsLineNum * 3 + 3] = coors[3 * connections[j + 1]];
                    linePoints[mPointsLineNum * 3 + 4] = coors[3 * connections[j + 1] + 1];
                    linePoints[mPointsLineNum * 3 + 5] = coors[3 * connections[j + 1] + 2];
                    mPointsLineNum += 2;
                }
            }
            mLinePoints = FloatBuffer.Wrap(linePoints);
        }
    }
}