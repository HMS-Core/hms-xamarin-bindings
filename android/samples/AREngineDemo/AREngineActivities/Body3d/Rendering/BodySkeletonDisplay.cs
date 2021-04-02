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
    public class BodySkeletonDisplay : BodyRelatedDisplay
    {
        private const string TAG = "BodySkeletonDisplay";

        // Number of bytes occupied by each 3D coordinate. Float data occupies 4 bytes.
        // Each skeleton point represents a 3D coordinate.
        private static readonly int BYTES_PER_POINT = 4 * 3;

        private static readonly int INITIAL_POINTS_SIZE = 150;

        private static readonly float DRAW_COORDINATE = 2.0f;

        private int mVbo;

        private int mVboSize;

        private int mProgram;

        private int mPosition;

        private int mProjectionMatrix;

        private int mColor;

        private int mPointSize;

        private int mCoordinateSystem;

        private int mNumPoints = 0;

        private int mPointsNum = 0;

        private FloatBuffer mSkeletonPoints;

        /// <summary>
        /// Create a body skeleton shader on the GL thread.
        /// This method is called when BodyRenderManager's OnSurfaceCreated
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
            ShaderUtil.CheckGlError(TAG, "Before create gl program.");
            CreateProgram();
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "Create gl program start.");
            mProgram = BodyShaderUtil.CreateGlProgram();
            mColor = GLES20.GlGetUniformLocation(mProgram, "inColor");
            mPosition = GLES20.GlGetAttribLocation(mProgram, "inPosition");
            mPointSize = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
            mProjectionMatrix = GLES20.GlGetUniformLocation(mProgram, "inProjectionMatrix");
            mCoordinateSystem = GLES20.GlGetUniformLocation(mProgram, "inCoordinateSystem");
            ShaderUtil.CheckGlError(TAG, "Create gl program end.");
        }

        private void UpdateBodySkeleton()
        {
            ShaderUtil.CheckGlError(TAG, "Update Body Skeleton data start.");

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
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mNumPoints * BYTES_PER_POINT, mSkeletonPoints);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Update Body Skeleton data end.");
        }

        /// <summary>
        /// Update the node data and draw by using OpenGL.
        /// This method is called when BodyRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="bodies">Body data.</param>
        /// <param name="projectionMatrix">projection matrix.</param>
        public void OnDrawFrame(ICollection bodies, float[] projectionMatrix)
        {
            foreach (ARBody body in bodies)
            {
                if (body.TrackingState == ARTrackableTrackingState.Tracking)
                {
                    float coordinate = 1.0f;
                    if (body.CoordinateSystemType == ARCoordinateSystemType.CoordinateSystemType3dCamera)
                    {
                        coordinate = DRAW_COORDINATE;
                    }
                    FindValidSkeletonPoints(body);
                    UpdateBodySkeleton();
                    DrawBodySkeleton(coordinate, projectionMatrix);
                }
            }
        }

        private void DrawBodySkeleton(float coordinate, float[] projectionMatrix)
        {
            ShaderUtil.CheckGlError(TAG, "Draw body skeleton start.");

            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVbo);

            // The size of the vertex attribute is 4, and each vertex has four coordinate components.
            GLES20.GlVertexAttribPointer(
                mPosition, 4, GLES20.GlFloat, false, BYTES_PER_POINT, 0);
            GLES20.GlUniform4f(mColor, 0.0f, 0.0f, 1.0f, 1.0f);
            GLES20.GlUniformMatrix4fv(mProjectionMatrix, 1, false, projectionMatrix, 0);

            // Set the size of the skeleton points.
            GLES20.GlUniform1f(mPointSize, 30.0f);
            GLES20.GlUniform1f(mCoordinateSystem, coordinate);

            GLES20.GlDrawArrays(GLES20.GlPoints, 0, mNumPoints);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            ShaderUtil.CheckGlError(TAG, "Draw body skeleton end.");
        }

        private void FindValidSkeletonPoints(ARBody arBody)
        {
            int index = 0;
            int[] isExists;
            int validPointNum = 0;
            float[] points;
            float[] skeletonPoints;

            // Determine whether the data returned by the algorithm is 3D human
            // skeleton data or 2D human skeleton data, and obtain valid skeleton points.
            if (arBody.CoordinateSystemType == ARCoordinateSystemType.CoordinateSystemType3dCamera)
            {
                isExists = arBody.GetSkeletonPointIsExist3D();
                points = new float[isExists.Length * 3];
                skeletonPoints = arBody.GetSkeletonPoint3D();
            }
            else
            {
                isExists = arBody.GetSkeletonPointIsExist2D();
                points = new float[isExists.Length * 3];
                skeletonPoints = arBody.GetSkeletonPoint2D();
            }

            // Save the three coordinates of each joint point(each point has three coordinates).
            for (int i = 0; i < isExists.Length; i++)
            {
                if (isExists[i] != 0)
                {
                    points[index++] = skeletonPoints[3 * i];
                    points[index++] = skeletonPoints[3 * i + 1];
                    points[index++] = skeletonPoints[3 * i + 2];
                    validPointNum++;
                }
            }
            mSkeletonPoints = FloatBuffer.Wrap(points);
            mPointsNum = validPointNum;
        }
    }
}