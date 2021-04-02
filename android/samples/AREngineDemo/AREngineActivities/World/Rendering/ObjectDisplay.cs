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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Nio;
using Java.Util;
using ObjLoader.Loader.Loaders;
using XamarinAREngineDemo.Common;

using File = System.IO.File;
using Matrix = Android.Opengl.Matrix;
using Path = System.IO.Path;

namespace XamarinAREngineDemo.AREngineActivities.World.Rendering
{
    /// <summary>
    /// Draw a virtual object based on the specified parameters.
    /// </summary>
    public class ObjectDisplay
    {
        private const string TAG = "ObjectDisplay";

        // The name of the Wavefront file(.obj) that in Assets folder.
        private const string objFileName = "AR_logo.obj";

        // Set the default light direction.
        private static readonly float[] LIGHT_DIRECTIONS = new float[] { 0.0f, 1.0f, 0.0f, 0.0f };

        private static readonly int FLOAT_BYTE_SIZE = 4;

        private static readonly int INDEX_COUNT_RATIO = 2;

        private static readonly int MATRIX_SIZE = 16;

        // Light direction (x, y, z, w).
        private float[] mViewLightDirections = new float[4];

        private int mTexCoordsBaseAddress;

        private int mNormalsBaseAddress;

        private int mVertexBufferId;

        private int mIndexCount;

        private int mGlProgram;

        private int mIndexBufferId;

        private int[] mTextures = new int[1];

        private int mModelViewUniform;

        private int mModelViewProjectionUniform;

        private int mPositionAttribute;

        private int mNormalAttribute;

        private int mTexCoordAttribute;

        private int mTextureUniform;

        private int mLightingParametersUniform;

        private int mColorUniform;

        private float[] mModelMatrixs = new float[MATRIX_SIZE];

        private float[] mModelViewMatrixs = new float[MATRIX_SIZE];

        private float[] mModelViewProjectionMatrixs = new float[MATRIX_SIZE];

        // The largest bounding box of a virtual object, represented by two diagonals of a cube.
        private float[] mBoundingBoxs = new float[6];

        private float mWidth;

        private float mHeight;

        /// <summary>
        /// If the surface size is changed, update the changed size of the record synchronously.
        /// This method is called when WorldRenderManager's OnSurfaceChanged.
        /// </summary>
        /// <param name="width">Surface's width.</param>
        /// <param name="height">Surface's height.</param>
        public void SetSize(float width, float height)
        {
            mWidth = width;
            mHeight = height;
        }

        /// <summary>
        /// Create a shader program to read the data of the virtual object.
        /// This method is called when WorldRenderManager's OnSurfaceCreated.
        /// </summary>
        /// <param name="context">Context.</param>
        public void Init(Context context)
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");
            CreateProgram();

            // Coordinate and index.
            int[] buffers = new int[2];
            GLES20.GlGenBuffers(2, buffers, 0);
            mVertexBufferId = buffers[0];
            mIndexBufferId = buffers[1];
            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlGenTextures(mTextures.Length, mTextures, 0);
            GLES20.GlBindTexture(GLES20.GlTexture2d, mTextures[0]);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMinFilter, GLES20.GlLinearMipmapLinear);
            InitGlTextureData(context);
            InitializeGlObjectData(context);
            ShaderUtil.CheckGlError(TAG, "Init end.");
        }

        private void CreateProgram()
        {
            ShaderUtil.CheckGlError(TAG, "Create program start.");
            mGlProgram = WorldShaderUtil.GetObjectProgram();
            mModelViewUniform = GLES20.GlGetUniformLocation(mGlProgram, "inViewMatrix");
            mModelViewProjectionUniform = GLES20.GlGetUniformLocation(mGlProgram, "inMVPMatrix");
            mPositionAttribute = GLES20.GlGetAttribLocation(mGlProgram, "inObjectPosition");
            mNormalAttribute = GLES20.GlGetAttribLocation(mGlProgram, "inObjectNormalVector");
            mTexCoordAttribute = GLES20.GlGetAttribLocation(mGlProgram, "inTexCoordinate");
            mTextureUniform = GLES20.GlGetUniformLocation(mGlProgram, "inObjectTexture");
            mLightingParametersUniform = GLES20.GlGetUniformLocation(mGlProgram, "inLight");
            mColorUniform = GLES20.GlGetUniformLocation(mGlProgram, "inObjectColor");
            Android.Opengl.Matrix.SetIdentityM(mModelMatrixs, 0);
            ShaderUtil.CheckGlError(TAG, "Create program end.");
        }

        private void InitGlTextureData(Context context)
        {
            ShaderUtil.CheckGlError(TAG, "Init gl texture data start.");
            Bitmap textureBitmap = null;
            AssetManager assets = context.Assets;
            try
            {
                var sr = assets.Open("AR_logo.png");
                textureBitmap = BitmapFactory.DecodeStream(sr);
            }
            catch(Exception e)
            {
                Log.Debug(TAG, " Open Bitmap Error! ");
                return;
            }

            ShaderUtil.CheckGlError(TAG, "bitmap error.");
            GLUtils.TexImage2D(GLES20.GlTexture2d, 0, textureBitmap, 0);
            GLES20.GlGenerateMipmap(GLES20.GlTexture2d);
            GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
            textureBitmap.Recycle();
            ShaderUtil.CheckGlError(TAG, "Init gl texture data end.");
        }

        private void InitializeGlObjectData(Context context)
        {
            ObjectData objectData = null;
            Optional objectDataOptional = ReadObject(context);
            if (objectDataOptional.IsPresent)
            {
                objectData = objectDataOptional.Get().JavaCast<ObjectData>();
            }
            else
            {
                Log.Debug(TAG, "Read object error.");
                return;
            }

            mTexCoordsBaseAddress = FLOAT_BYTE_SIZE * objectData.objectIndices.Limit();
            mNormalsBaseAddress = mTexCoordsBaseAddress + FLOAT_BYTE_SIZE * objectData.texCoords.Limit();
            int totalBytes = mNormalsBaseAddress + FLOAT_BYTE_SIZE * objectData.normals.Limit();
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVertexBufferId);
            GLES20.GlBufferData(GLES20.GlArrayBuffer, totalBytes, null, GLES20.GlStaticDraw);
            GLES20.GlBufferSubData(
                GLES20.GlArrayBuffer, 0, FLOAT_BYTE_SIZE * objectData.objectVertices.Limit(), objectData.objectVertices);
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, mTexCoordsBaseAddress,
                FLOAT_BYTE_SIZE * objectData.texCoords.Limit(), objectData.texCoords);
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, mNormalsBaseAddress,
                FLOAT_BYTE_SIZE * objectData.normals.Limit(), objectData.normals);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, mIndexBufferId);
            mIndexCount = objectData.indices.Limit();
            GLES20.GlBufferData(
                GLES20.GlElementArrayBuffer, INDEX_COUNT_RATIO * mIndexCount, objectData.indices, GLES20.GlStaticDraw);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "obj buffer load");
        }

        private LoadResult Load(string fileString)
        {
            var objectStream = CreateMemoryStreamFromString(fileString);
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create(new MaterialNullStreamProvider());
            var _loadResult = objLoader.Load(objectStream);

            return _loadResult;
        }

        private Stream CreateMemoryStreamFromString(string str)
        {
            var data = Encoding.ASCII.GetBytes(str);
            return new MemoryStream(data);
        }

        /// <summary>
        /// Read and parse the Wavefront file(.obj) that in Assets folder.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns></returns>
        private Optional ReadObject(Context context)
        {
            LoadResult obj = null;
            AssetManager assets = context.Assets;

            using (StreamReader sr = new StreamReader(assets.Open(objFileName)))
            {
                obj = Load(sr.ReadToEnd());
            }

            int numVerticesPerFace = 3;
            IntBuffer objectIndices = ObjDataBufferHelper.GetFaceVerticesIntBuffer(obj.Groups, numVerticesPerFace);

            FloatBuffer objectVertices = ObjDataBufferHelper.GetVerticesFloatBuffer(obj.Vertices);

            CalculateBoundingBox(objectVertices);

            // Size of the allocated buffer.
            ShortBuffer indices = ByteBuffer.AllocateDirect(2 * objectIndices.Limit())
                .Order(ByteOrder.NativeOrder()).AsShortBuffer();

            while (objectIndices.HasRemaining)
            {
                indices.Put((short)objectIndices.Get());
            }
            indices.Rewind();

            int dimensionOfTextures = 2;
            FloatBuffer texCoordinates = ObjDataBufferHelper.GetTexturesFloatBuffer(obj.Textures, dimensionOfTextures);
            FloatBuffer normals = ObjDataBufferHelper.GetNormalsFloatBuffer(obj.Normals);

            return Optional.Of(new ObjectData(objectIndices, objectVertices, indices, texCoordinates, normals));
        }


        /// <summary>
        /// Draw a virtual object at a specific location on a specified plane.
        /// This method is called when WorldRenderManager's OnDrawFrame.
        /// </summary>
        /// <param name="cameraView">The viewMatrix is a 4 * 4 matrix.</param>
        /// <param name="cameraProjection">The ProjectionMatrix is a 4 * 4 matrix.</param>
        /// <param name="lightIntensity">The lighting intensity.</param>
        /// <param name="obj">The virtual object.</param>
        public void OnDrawFrame(float[] cameraView, float[] cameraProjection, float lightIntensity, VirtualObject obj)
        {
            ShaderUtil.CheckGlError(TAG, "onDrawFrame start.");
            mModelMatrixs = obj.GetModelAnchorMatrix();
            Matrix.MultiplyMM(mModelViewMatrixs, 0, cameraView, 0, mModelMatrixs, 0);
            Matrix.MultiplyMM(mModelViewProjectionMatrixs, 0, cameraProjection, 0, mModelViewMatrixs, 0);
            GLES20.GlUseProgram(mGlProgram);
            Matrix.MultiplyMV(mViewLightDirections, 0, mModelViewMatrixs, 0, LIGHT_DIRECTIONS, 0);
            MatrixUtil.NormalizeVec3(mViewLightDirections);

            // Light direction.
            GLES20.GlUniform4f(mLightingParametersUniform,
                mViewLightDirections[0], mViewLightDirections[1], mViewLightDirections[2], lightIntensity);
            float[] objColors = obj.GetColor();

            GLES20.GlUniform4fv(mColorUniform, 1, objColors, 0);
            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlBindTexture(GLES20.GlTexture2d, mTextures[0]);
            GLES20.GlUniform1i(mTextureUniform, 0);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVertexBufferId);

            // The coordinate dimension of the read virtual object is 3.
            GLES20.GlVertexAttribPointer(
                mPositionAttribute, 3, GLES20.GlFloat, false, 0, 0);

            // The dimension of the normal vector is 3.
            GLES20.GlVertexAttribPointer(
                mNormalAttribute, 3, GLES20.GlFloat, false, 0, mNormalsBaseAddress);

            // The dimension of the texture coordinate is 2.
            GLES20.GlVertexAttribPointer(
                mTexCoordAttribute, 2, GLES20.GlFloat, false, 0, mTexCoordsBaseAddress);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            GLES20.GlUniformMatrix4fv(
                mModelViewUniform, 1, false, mModelViewMatrixs, 0);
            GLES20.GlUniformMatrix4fv(
                mModelViewProjectionUniform, 1, false, mModelViewProjectionMatrixs, 0);
            GLES20.GlEnableVertexAttribArray(mPositionAttribute);
            GLES20.GlEnableVertexAttribArray(mNormalAttribute);
            GLES20.GlEnableVertexAttribArray(mTexCoordAttribute);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, mIndexBufferId);
            GLES20.GlDrawElements(GLES20.GlTriangles, mIndexCount, GLES20.GlUnsignedShort, 0);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, 0);
            GLES20.GlDisableVertexAttribArray(mPositionAttribute);
            GLES20.GlDisableVertexAttribArray(mNormalAttribute);
            GLES20.GlDisableVertexAttribArray(mTexCoordAttribute);
            GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
            ShaderUtil.CheckGlError(TAG, "onDrawFrame end.");
        }

        /// <summary>
        /// Check whether the virtual object is clicked.
        /// </summary>
        /// <param name="cameraView">The viewMatrix 4 * 4.</param>
        /// <param name="cameraPerspective">The ProjectionMatrix 4 * 4.</param>
        /// <param name="obj">The virtual object data.</param>
        /// <param name="mEvent">The gesture event.</param>
        /// <returns>Return the click result for determining whether the input virtual object is clicked</returns>
        public bool HitTest(float[] cameraView, float[] cameraPerspective, VirtualObject obj, MotionEvent mEvent)
        {
            mModelMatrixs = obj.GetModelAnchorMatrix();
            Matrix.MultiplyMM(mModelViewMatrixs, 0, cameraView, 0, mModelMatrixs, 0);
            Matrix.MultiplyMM(mModelViewProjectionMatrixs, 0, cameraPerspective, 0, mModelViewMatrixs, 0);
            // Calculate the coordinates of the smallest bounding box in the coordinate system of the device screen.
            float[] screenPos = CalculateScreenPos(mBoundingBoxs[0], mBoundingBoxs[1], mBoundingBoxs[2]);

            // Record the largest bounding rectangle of an object (minX/minY/maxX/maxY).
            float[] boundarys = new float[4];
            boundarys[0] = screenPos[0];
            boundarys[1] = screenPos[0];
            boundarys[2] = screenPos[1];
            boundarys[3] = screenPos[1];
            // Determine whether a screen position corresponding to (maxX, maxY, maxZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 3, 4, 5 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }
            // Determine whether a screen position corresponding to (minX, minY, maxZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 0, 1, 5 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }

            // Determine whether a screen position corresponding to (minX, maxY, minZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 0, 4, 2 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }

            // Determine whether a screen position corresponding to (minX, maxY, maxZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 0, 4, 5 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }
            // Determine whether a screen position corresponding to (maxX, minY, minZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 3, 1, 2 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }

            // Determine whether a screen position corresponding to (maxX, minY, maxZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 3, 1, 5 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }

            // Determine whether a screen position corresponding to (maxX, maxY, maxZ) is clicked.
            boundarys = FindMaximum(boundarys, new int[] { 3, 4, 2 });
            if (((mEvent.GetX() > boundarys[0]) && (mEvent.GetX() < boundarys[1]))
            && ((mEvent.GetY() > boundarys[2]) && (mEvent.GetY() < boundarys[3]))) 
            {
            return true;
            }

            return false;
        }

        // The size of minXmaxXminYmaxY is 4, and the size of index is 3.
        private float[] FindMaximum(float[] minXmaxXminYmaxY, int[] index)
        {
            float[] screenPos = CalculateScreenPos(mBoundingBoxs[index[0]],
                    mBoundingBoxs[index[1]], mBoundingBoxs[index[2]]);
            if (screenPos[0] < minXmaxXminYmaxY[0])
            {
                minXmaxXminYmaxY[0] = screenPos[0];
            }
            if (screenPos[0] > minXmaxXminYmaxY[1])
            {
                minXmaxXminYmaxY[1] = screenPos[0];
            }
            if (screenPos[1] < minXmaxXminYmaxY[2])
            {
                minXmaxXminYmaxY[2] = screenPos[1];
            }
            if (screenPos[1] > minXmaxXminYmaxY[3])
            {
                minXmaxXminYmaxY[3] = screenPos[1];
            }
            return minXmaxXminYmaxY;
        }

        // Convert the input coordinates to the plane coordinate system.
        private float[] CalculateScreenPos(float coordinateX, float coordinateY, float coordinateZ)
        {
            // The coordinates of the point are four-dimensional (x, y, z, w).
            float[] vecs = new float[4];
            vecs[0] = coordinateX;
            vecs[1] = coordinateY;
            vecs[2] = coordinateZ;
            vecs[3] = 1.0f;

            // Store the coordinate values in the clip coordinate system.
            float[] rets = new float[4];
            Matrix.MultiplyMV(rets, 0, mModelViewProjectionMatrixs, 0, vecs, 0);

            // Divide by the w component of the coordinates.
            rets[0] /= rets[3];
            rets[1] /= rets[3];
            rets[2] /= rets[3];

            // In the current coordinate system, left is negative, right is positive, downward
            // is positive, and upward is negative.Adding 1 to the left of the X coordinate is
            // equivalent to moving the coordinate system leftwards. Such an operation on the Y
            // axis is equivalent to moving the coordinate system upwards.
            rets[0] += 1.0f;
            rets[1] = 1.0f - rets[1];

            // Convert to pixel coordinates.
            rets[0] *= mWidth;
            rets[1] *= mHeight;

            // When the w component is set to 1, the xy component caused by coordinate system
            // movement is eliminated and doubled.
            rets[3] = 1.0f;
            rets[0] /= 2.0f;
            rets[1] /= 2.0f;
            return rets;
        }

        // Bounding box [minX, minY, minZ, maxX, maxY, maxZ].
        private void CalculateBoundingBox(FloatBuffer vertices)
        {
            if (vertices.Limit() < 3)
            {
                mBoundingBoxs[0] = 0.0f;
                mBoundingBoxs[1] = 0.0f;
                mBoundingBoxs[2] = 0.0f;
                mBoundingBoxs[3] = 0.0f;
                mBoundingBoxs[4] = 0.0f;
                mBoundingBoxs[5] = 0.0f;
                return;
            }
            else
            {
                mBoundingBoxs[0] = vertices.Get(0);
                mBoundingBoxs[1] = vertices.Get(1);
                mBoundingBoxs[2] = vertices.Get(2);
                mBoundingBoxs[3] = vertices.Get(0);
                mBoundingBoxs[4] = vertices.Get(1);
                mBoundingBoxs[5] = vertices.Get(2);
            }

            // Use the first three pairs as the initial variables and get the three
            // maximum values and three minimum values.
            int index = 3;
            while (index < vertices.Limit() - 2)
            {
                if (vertices.Get(index) < mBoundingBoxs[0])
                {
                    mBoundingBoxs[0] = vertices.Get(index);
                }
                if (vertices.Get(index) > mBoundingBoxs[3])
                {
                    mBoundingBoxs[3] = vertices.Get(index);
                }
                index++;

                if (vertices.Get(index) < mBoundingBoxs[1])
                {
                    mBoundingBoxs[1] = vertices.Get(index);
                }
                if (vertices.Get(index) > mBoundingBoxs[4])
                {
                    mBoundingBoxs[4] = vertices.Get(index);
                }
                index++;

                if (vertices.Get(index) < mBoundingBoxs[2])
                {
                    mBoundingBoxs[2] = vertices.Get(index);
                }
                if (vertices.Get(index) > mBoundingBoxs[5])
                {
                    mBoundingBoxs[5] = vertices.Get(index);
                }
                index++;
            }
        }

    }

    /// <summary>
    /// The virtual object data class.
    /// </summary>
    public class ObjectData : Java.Lang.Object
    {
        public IntBuffer objectIndices;
        public FloatBuffer objectVertices;
        public ShortBuffer indices;
        public FloatBuffer texCoords;
        public FloatBuffer normals;

        public ObjectData(IntBuffer objectIndices,
            FloatBuffer objectVertices,
            ShortBuffer indices,
            FloatBuffer texCoords,
            FloatBuffer normals)
        {
            this.objectIndices = objectIndices;
            this.objectVertices = objectVertices;
            this.indices = indices;
            this.texCoords = texCoords;
            this.normals = normals;
        }
    }
}