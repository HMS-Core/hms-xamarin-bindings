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
using Java.IO;
using Java.Lang;
using Java.Nio;
using Java.Util;
using XamarinAREngineDemo.Common;
using Matrix = Android.Opengl.Matrix;

namespace XamarinAREngineDemo.AREngineActivities.World.Rendering
{
    /// <summary>
    /// This class demonstrates how to use ARPlane, including how to obtain the center point of a plane.
    /// If the plane type can be identified, it is also displayed at the center of the plane.Otherwise,
    /// "other" is displayed.
    /// </summary>
    public class LabelDisplay
    {
        private const string TAG = "LabelDisplay";

        private static readonly int COORDS_PER_VERTEX = 3;

        private static readonly float LABEL_WIDTH = 0.3f;

        private static readonly float LABEL_HEIGHT = 0.3f;

        private static readonly int TEXTURES_SIZE = 12;

        private static readonly int MATRIX_SIZE = 16;

        private static readonly int PLANE_ANGLE_MATRIX_SIZE = 4;

        private readonly int[] textures = new int[TEXTURES_SIZE];

        // Allocate a temporary list/matrix here to reduce the number of allocations per frame.
        private readonly float[] modelMatrix = new float[MATRIX_SIZE];

        private readonly float[] modelViewMatrix = new float[MATRIX_SIZE];

        private readonly float[] modelViewProjectionMatrix = new float[MATRIX_SIZE];

        // A 2 * 2 rotation matrix applied to the uv coordinates.
        private readonly float[] planeAngleUvMatrix = new float[PLANE_ANGLE_MATRIX_SIZE];

        private int mProgram;

        private int glPositionParameter;

        private int glModelViewProjectionMatrix;

        private int glTexture;

        private int glPlaneUvMatrix;

        /// <summary>
        /// Create the shader program for label display in the openGL thread.
        /// This method will be called when WorldRenderManager's OnSurfaceCreated.
        /// </summary>
        /// <param name="labelBitmaps">View data indicating the plane type.</param>
        public void Init(List<Bitmap> labelBitmaps)
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");
            if (labelBitmaps.Count == 0)
            {
                Log.Debug(TAG, "No bitmap.");
            }
            CreateProgram();
            int idx = 0;
            GLES20.GlGenTextures(textures.Length, textures, 0);
            foreach (Bitmap labelBitmap in labelBitmaps)
            {
                // for semantic label plane
                GLES20.GlActiveTexture(GLES20.GlTexture0 + idx);
                GLES20.GlBindTexture(GLES20.GlTexture2d, textures[idx]);

                GLES20.GlTexParameteri(
                    GLES20.GlTexture2d, GLES20.GlTextureMinFilter, GLES20.GlLinearMipmapLinear);
                GLES20.GlTexParameteri(
                    GLES20.GlTexture2d, GLES20.GlTextureMagFilter, GLES20.GlLinear);
                GLUtils.TexImage2D(GLES20.GlTexture2d, 0, labelBitmap, 0);
                GLES20.GlGenerateMipmap(GLES20.GlTexture2d);
                GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
                idx++;
                ShaderUtil.CheckGlError(TAG, "Texture loading");
            }
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "program start.");
            mProgram = WorldShaderUtil.GetLabelProgram();
            glPositionParameter = GLES20.GlGetAttribLocation(mProgram, "inPosXZAlpha");
            glModelViewProjectionMatrix =
                GLES20.GlGetUniformLocation(mProgram, "inMVPMatrix");
            glTexture = GLES20.GlGetUniformLocation(mProgram, "inTexture");
            glPlaneUvMatrix = GLES20.GlGetUniformLocation(mProgram, "inPlanUVMatrix");
            ShaderUtil.CheckGlError(TAG, "program end.");
        }

        /// <summary>
        /// Render the plane type at the center of the currently identified plane.
        /// This method will be called when WorldRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="allPlanes">All identified planes.</param>
        /// <param name="cameraPose">Location and pose of the current camera.</param>
        /// <param name="cameraProjection">Projection matrix of the current camera.</param>
        public void OnDrawFrame(System.Collections.ICollection allPlanes, ARPose cameraPose, float[] cameraProjection)
        {
            if (allPlanes.Count != 0)
            {
                Log.Debug(TAG, "PlaneList count > 0");
            }
            List<ARPlane> sortedPlanes = GetSortedPlanes(allPlanes, cameraPose);
            float[] cameraViewMatrix = new float[MATRIX_SIZE];
            cameraPose.Inverse().ToMatrix(cameraViewMatrix, 0);
            DrawSortedPlans(sortedPlanes, cameraViewMatrix, cameraProjection);
        }

        private List<ARPlane> GetSortedPlanes(System.Collections.ICollection allPlanes, ARPose cameraPose)
        {
            // Planes must be sorted by the distance from the camera so that we can
            // first draw the closer planes, and have them block the further planes.
            Java.Util.ArrayList pairPlanes = new Java.Util.ArrayList();
            foreach (Java.Lang.Object item in allPlanes)
            {
                ARPlane plane = item.JavaCast<ARPlane>();
                if ((plane.Type == ARPlane.PlaneType.UnknownFacing)
                    || plane.TrackingState != ARTrackableTrackingState.Tracking
                    || plane.SubsumedBy != null)
                {
                    continue;
                }

                // Store the normal vector of the current plane.
                float[] planeNormalVector = new float[3];
                ARPose planeCenterPose = plane.CenterPose;
                planeCenterPose.GetTransformedAxis(1, 1.0f, planeNormalVector, 0);

                // Calculate the distance from the camera to the plane. If the value is negative,
                // it indicates that the camera is behind the plane (the normal vector distinguishes
                // the front side from the back side).
                float distanceBetweenPlaneAndCamera = (cameraPose.Tx() - planeCenterPose.Tx()) * planeNormalVector[0]
                    + (cameraPose.Ty() - planeCenterPose.Ty()) * planeNormalVector[1]
                    + (cameraPose.Tz() - planeCenterPose.Tz()) * planeNormalVector[2];
                pairPlanes.Add(new Pair(plane, distanceBetweenPlaneAndCamera));
                
            }
            //Get Pair object from ArrayList
            List<Pair> sortedList = new List<Pair>();
            IIterator myit = pairPlanes.Iterator();
            while (myit.HasNext)
            {
                sortedList.Add((Pair)(myit.Next()));
            }
            //Sort Pair objects by plane in list
            sortedList.Sort(new PlanCompare());

            //Obtain plane list.
            List<ARPlane> listOfPlane = new List<ARPlane>();
            
            foreach (Pair item in sortedList)
            {
                listOfPlane.Add((item.First).JavaCast<ARPlane>());
            }

            return listOfPlane;
        }

        private void DrawSortedPlans(List<ARPlane> sortedPlanes, float[] cameraViews, float[] cameraProjection)
        {
            ShaderUtil.CheckGlError(TAG, "Draw sorted plans start.");

            GLES20.GlDepthMask(false);
            GLES20.GlEnable(GLES20.GlBlend);
            GLES20.GlBlendFuncSeparate(
                GLES20.GlDstAlpha, GLES20.GlOne, GLES20.GlZero, GLES20.GlOneMinusSrcAlpha);
            GLES20.GlUseProgram(mProgram);
            GLES20.GlEnableVertexAttribArray(glPositionParameter);

            foreach (ARPlane plane in sortedPlanes)
            {
                float[] planeMatrix = new float[MATRIX_SIZE];
                plane.CenterPose.ToMatrix(planeMatrix, 0);

                Array.Copy(planeMatrix, modelMatrix, MATRIX_SIZE);
                float scaleU = 1.0f / LABEL_WIDTH;

                // Set the value of the plane angle uv matrix.
                planeAngleUvMatrix[0] = scaleU;
                planeAngleUvMatrix[1] = 0.0f;
                planeAngleUvMatrix[2] = 0.0f;
                float scaleV = 1.0f / LABEL_HEIGHT;
                planeAngleUvMatrix[3] = scaleV;

                int idx = plane.Label.Ordinal();
                Log.Debug(TAG, "Plane getLabel:" + idx);
                idx = Java.Lang.Math.Abs(idx);
                GLES20.GlActiveTexture(GLES20.GlTexture0 + idx);
                GLES20.GlBindTexture(GLES20.GlTexture2d, textures[idx]);
                GLES20.GlUniform1i(glTexture, idx);
                GLES20.GlUniformMatrix2fv(glPlaneUvMatrix, 1, false, planeAngleUvMatrix, 0);

                DrawLabel(cameraViews, cameraProjection);
            }

            GLES20.GlDisableVertexAttribArray(glPositionParameter);
            GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
            GLES20.GlDisable(GLES20.GlBlend);
            GLES20.GlDepthMask(true);
            ShaderUtil.CheckGlError(TAG, "Draw sorted plans end.");
        }

        private void DrawLabel(float[] cameraViews, float[] cameraProjection)
        {
            ShaderUtil.CheckGlError(TAG, "Draw label start.");
            Matrix.MultiplyMM(modelViewMatrix, 0, cameraViews, 0, modelMatrix, 0);
            Matrix.MultiplyMM(modelViewProjectionMatrix, 0, cameraProjection, 0, modelViewMatrix, 0);

            float halfWidth = LABEL_WIDTH / 2.0f;
            float halfHeight = LABEL_HEIGHT / 2.0f;
            float[] vertices = {
            -halfWidth, -halfHeight, 1,
            -halfWidth, halfHeight, 1,
            halfWidth, halfHeight, 1,
            halfWidth, -halfHeight, 1,
            };

            // The size of each floating point is 4 bits.
            FloatBuffer vetBuffer = ByteBuffer.AllocateDirect(4 * vertices.Length)
                .Order(ByteOrder.NativeOrder()).AsFloatBuffer();
            vetBuffer.Rewind();
            for (int i = 0; i < vertices.Length; ++i)
            {
                vetBuffer.Put(vertices[i]);
            }
            vetBuffer.Rewind();

            // The size of each floating point is 4 bits.
            GLES20.GlVertexAttribPointer(glPositionParameter, COORDS_PER_VERTEX, GLES20.GlFloat,
                false, 4 * COORDS_PER_VERTEX, vetBuffer);

            // Set the sequence of OpenGL drawing points to generate two triangles that form a plane.
            short[] indices = { 0, 1, 2, 0, 2, 3 };

            // Size of the allocated buffer.
            ShortBuffer idxBuffer = ByteBuffer.AllocateDirect(2 * indices.Length)
                .Order(ByteOrder.NativeOrder()).AsShortBuffer();
            idxBuffer.Rewind();
            for (int i = 0; i < indices.Length; ++i)
            {
                idxBuffer.Put(indices[i]);
            }
            idxBuffer.Rewind();

            GLES20.GlUniformMatrix4fv(glModelViewProjectionMatrix, 1, false, modelViewProjectionMatrix, 0);

            GLES20.GlDrawElements(GLES20.GlTriangleStrip, idxBuffer.Limit(), GLES20.GlUnsignedShort, idxBuffer);
            ShaderUtil.CheckGlError(TAG, "Draw label end.");
        }
    }

    /// <summary>
    /// Sort the planes.
    /// </summary>
    public class PlanCompare : IComparer<Pair>
    {
        public int Compare(Pair x, Pair y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            var xSecond = (float)x.Second;
            var ySecond = (float)y.Second;
            return xSecond.CompareTo(ySecond);
        }
    }

}