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

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Opengl;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hiar;
using Java.Nio;
using XamarinAREngineDemo.Common;

namespace XamarinAREngineDemo.AREngineActivities.Face.Rendering
{
    /// <summary>
    /// Get the facial geometric data and render the data on the screen.
    /// </summary>
    public class FaceGeometryDisplay
    {
        private const string TAG = "FaceGeometryDisplay";

        private static readonly string LS = System.Environment.NewLine;

        //OpenGL Cull Face Mode constant
        private const int GL_CULL_FACE_CONSTANT = 0x0B44;

        private static readonly string FACE_GEOMETRY_VERTEX =
        "attribute vec2 inTexCoord;" + LS
        + "uniform mat4 inMVPMatrix;" + LS
        + "uniform float inPointSize;" + LS
        + "attribute vec4 inPosition;" + LS
        + "uniform vec4 inColor;" + LS
        + "varying vec4 varAmbient;" + LS
        + "varying vec4 varColor;" + LS
        + "varying vec2 varCoord;" + LS
        + "void main() {" + LS
        + "    varAmbient = vec4(1.0, 1.0, 1.0, 1.0);" + LS
        + "    gl_Position = inMVPMatrix * vec4(inPosition.xyz, 1.0);" + LS
        + "    varColor = inColor;" + LS
        + "    gl_PointSize = inPointSize;" + LS
        + "    varCoord = inTexCoord;" + LS
        + "}";

        private static readonly string FACE_GEOMETRY_FRAGMENT =
        "precision mediump float;" + LS
        + "uniform sampler2D inTexture;" + LS
        + "varying vec4 varColor;" + LS
        + "varying vec2 varCoord;" + LS
        + "varying vec4 varAmbient;" + LS
        + "void main() {" + LS
        + "    vec4 objectColor = texture2D(inTexture, vec2(varCoord.x, 1.0 - varCoord.y));" + LS
        + "    if(varColor.x != 0.0) {" + LS
        + "        gl_FragColor = varColor * varAmbient;" + LS
        + "    }" + LS
        + "    else {" + LS
        + "        gl_FragColor = objectColor * varAmbient;" + LS
        + "    }" + LS
        + "}";

    // Number of bytes occupied by each 3D coordinate point.
    // Each floating-point number occupies 4 bytes, and each point has three dimensions.
    private const int BYTES_PER_POINT = 4 * 3;

        // Number of bytes occupied by each 2D coordinate point.
        private const int BYTES_PER_COORD = 4 * 2;

        private const int BUFFER_OBJECT_NUMBER = 2;

        private const int POSITION_COMPONENTS_NUMBER = 4;

        private const int TEXCOORD_COMPONENTS_NUMBER = 2;

        private const float PROJECTION_MATRIX_NEAR = 0.1f;

        private const float PROJECTION_MATRIX_FAR = 100.0f;

        private int mVerticeId;

        private int mVerticeBufferSize = 8000; // Initialize the size of the vertex VBO.

        private int mTriangleId;

        private int mTriangleBufferSize = 5000; // Initialize the size of the triangle VBO.

        private int mProgram;

        private int mTextureName;

        private int mPositionAttribute;

        private int mColorUniform;

        private int mModelViewProjectionUniform;

        private int mPointSizeUniform;

        private int mTextureUniform;

        private int mTextureCoordAttribute;

        private int mPointsNum = 0;

        private int mTrianglesNum = 0;

        // The size of the MVP matrix is 4 x 4.
        private float[] mModelViewProjections = new float[16];

        /// <summary>
        /// Initialize the OpenGL ES rendering related to face geometry,
        /// including creating the shader program.
        /// This method is called when FaceRenderManager's OnSurfaceCreated method calling.
        /// </summary>
        /// <param name="context">Context.</param>
        public void Init(Context context)
        {
            ShaderUtil.CheckGlError(TAG, "Init start.");
            int[] texNames = new int[1];
            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlGenTextures(1, texNames, 0);
            mTextureName = texNames[0];

            int[] buffers = new int[BUFFER_OBJECT_NUMBER];
            GLES20.GlGenBuffers(BUFFER_OBJECT_NUMBER, buffers, 0);
            mVerticeId = buffers[0];
            mTriangleId = buffers[1];

            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVerticeId);
            GLES20.GlBufferData(GLES20.GlArrayBuffer, mVerticeBufferSize * BYTES_PER_POINT, null, GLES20.GlDynamicDraw);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, mTriangleId);

            // Each floating-point number occupies 4 bytes.
            GLES20.GlBufferData(GLES20.GlElementArrayBuffer, mTriangleBufferSize * 4, null,
                GLES20.GlDynamicDraw);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, 0);
            GLES20.GlBindTexture(GLES20.GlTexture2d, mTextureName);

            CreateProgram();

            //Add texture to facegeometry.
            Bitmap textureBitmap = null;
            AssetManager assets = context.Assets;
            try
            {
                Stream sr = assets.Open("face_geometry.png");
                textureBitmap = BitmapFactory.DecodeStream(sr);
            }
            catch(Exception e)
            {
                Log.Debug(TAG, " Open bitmap error!");
            }


            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureWrapS, GLES20.GlClampToEdge);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureWrapT, GLES20.GlClampToEdge);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMinFilter, GLES20.GlLinearMipmapLinear);
            GLES20.GlTexParameteri(GLES20.GlTexture2d, GLES20.GlTextureMagFilter, GLES20.GlLinear);
            GLUtils.TexImage2D(GLES20.GlTexture2d, 0, textureBitmap, 0);
            GLES20.GlGenerateMipmap(GLES20.GlTexture2d);
            GLES20.GlBindTexture(GLES20.GlTexture2d, 0);
            ShaderUtil.CheckGlError(TAG, "Init end.");
            }

        private void CreateProgram()
            {
                ShaderUtil.CheckGlError(TAG, "Create gl program start.");
                mProgram = CreateGlProgram();
                mPositionAttribute = GLES20.GlGetAttribLocation(mProgram, "inPosition");
                mColorUniform = GLES20.GlGetUniformLocation(mProgram, "inColor");
                mModelViewProjectionUniform = GLES20.GlGetUniformLocation(mProgram, "inMVPMatrix");
                mPointSizeUniform = GLES20.GlGetUniformLocation(mProgram, "inPointSize");
                mTextureUniform = GLES20.GlGetUniformLocation(mProgram, "inTexture");
                mTextureCoordAttribute = GLES20.GlGetAttribLocation(mProgram, "inTexCoord");
                ShaderUtil.CheckGlError(TAG, "Create gl program end.");
            }

        private static int CreateGlProgram()
        {
            int vertex = LoadShader(GLES20.GlVertexShader, FACE_GEOMETRY_VERTEX);
            if (vertex == 0)
            {
                return 0;
            }
            int fragment = LoadShader(GLES20.GlFragmentShader, FACE_GEOMETRY_FRAGMENT);
            if (fragment == 0)
            {
                return 0;
            }
            int program = GLES20.GlCreateProgram();
            if (program != 0)
            {
                GLES20.GlAttachShader(program, vertex);
                GLES20.GlAttachShader(program, fragment);
                GLES20.GlLinkProgram(program);
                int[] linkStatus = new int[1];
                GLES20.GlGetProgramiv(program, GLES20.GlLinkStatus, linkStatus, 0);
                if (linkStatus[0] != GLES20.GlTrue)
                {
                    Log.Debug(TAG, "Could not link program: " + GLES20.GlGetProgramInfoLog(program));
                    GLES20.GlDeleteProgram(program);
                    program = 0;
                }
            }
            return program;
        }

        private static int LoadShader(int shaderType, string source)
        {
            int shader = GLES20.GlCreateShader(shaderType);
            if (0 != shader)
            {
                GLES20.GlShaderSource(shader, source);
                GLES20.GlCompileShader(shader);
                int[] compiled = new int[1];
                GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compiled, 0);
                if (compiled[0] == 0)
                {
                    Log.Debug(TAG, "glError: Could not compile shader " + shaderType);
                    Log.Debug(TAG, "GLES20 Error: " + GLES20.GlGetShaderInfoLog(shader));
                    GLES20.GlDeleteShader(shader);
                    shader = 0;
                }
            }
            return shader;
        }

        /// <summary>
        /// Update the face geometric data in the buffer.
        /// This method is called when FaceRenderManager's OnDrawFrame method calling.
        /// </summary>
        /// <param name="camera">ARCamera</param>
        /// <param name="face">ARFace</param>
        public void OnDrawFrame(ARCamera camera, ARFace face)
        {
            ARFaceGeometry faceGeometry = face.FaceGeometry;
            UpdateFaceGeometryData(faceGeometry);
            UpdateModelViewProjectionData(camera, face);
            DrawFaceGeometry();
            faceGeometry.Release();
        }

        private void UpdateFaceGeometryData(ARFaceGeometry faceGeometry)
        {
            ShaderUtil.CheckGlError(TAG, "Before update data.");
            FloatBuffer faceVertices = faceGeometry.Vertices;

            // Obtain the number of geometric vertices of a face.
            mPointsNum = faceVertices.Limit() / 3;

            FloatBuffer textureCoordinates = faceGeometry.TextureCoordinates;

            // Obtain the number of geometric texture coordinates of the
            // face (the texture coordinates are two-dimensional).
            int texNum = textureCoordinates.Limit() / 2;
            Log.Debug(TAG, "Update face geometry data: texture coordinates size:" + texNum);

            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVerticeId);
            if (mVerticeBufferSize < (mPointsNum + texNum) * BYTES_PER_POINT)
            {
                while (mVerticeBufferSize < (mPointsNum + texNum) * BYTES_PER_POINT)
                {
                    // If the capacity of the vertex VBO buffer is insufficient, expand the capacity.
                    mVerticeBufferSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlArrayBuffer, mVerticeBufferSize, null, GLES20.GlDynamicDraw);
            }
            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, 0, mPointsNum * BYTES_PER_POINT, faceVertices);

            GLES20.GlBufferSubData(GLES20.GlArrayBuffer, mPointsNum * BYTES_PER_POINT, texNum * BYTES_PER_COORD,
                textureCoordinates);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);

            mTrianglesNum = faceGeometry.TriangleCount;
            IntBuffer faceTriangleIndices = faceGeometry.TriangleIndices;
            Log.Debug(TAG, "update face geometry data: faceTriangleIndices.size: " + faceTriangleIndices.Limit());

            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, mTriangleId);
            if (mTriangleBufferSize < mTrianglesNum * BYTES_PER_POINT)
            {
                while (mTriangleBufferSize < mTrianglesNum * BYTES_PER_POINT)
                {
                    // If the capacity of the vertex VBO buffer is insufficient, expand the capacity.
                    mTriangleBufferSize *= 2;
                }
                GLES20.GlBufferData(GLES20.GlElementArrayBuffer, mTriangleBufferSize, null, GLES20.GlDynamicDraw);
            }
            GLES20.GlBufferSubData(GLES20.GlElementArrayBuffer, 0, mTrianglesNum * BYTES_PER_POINT, faceTriangleIndices);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "After update data.");
        }

        private void UpdateModelViewProjectionData(ARCamera camera, ARFace face)
        {
            // The size of the projection matrix is 4 * 4.
            float[] projectionMatrix = new float[16];
            camera.GetProjectionMatrix(projectionMatrix, 0, PROJECTION_MATRIX_NEAR, PROJECTION_MATRIX_FAR);
            ARPose facePose = face.Pose;

            // The size of viewMatrix is 4 * 4.
            float[] facePoseViewMatrix = new float[16];

            facePose.ToMatrix(facePoseViewMatrix, 0);
            Android.Opengl.Matrix.MultiplyMM(mModelViewProjections, 0, projectionMatrix, 0, facePoseViewMatrix, 0);
        }

        /// <summary>
        /// Draw face geometrical features. This method is called on each frame.
        /// </summary>
        private void DrawFaceGeometry()
        {
            ShaderUtil.CheckGlError(TAG, "Before draw.");
            Log.Debug(TAG, "Draw face geometry: mPointsNum: " + mPointsNum + " mTrianglesNum: " + mTrianglesNum);

            GLES20.GlActiveTexture(GLES20.GlTexture0);
            GLES20.GlBindTexture(GLES20.GlTexture2d, mTextureName);
            GLES20.GlUniform1i(mTextureUniform, 0);
            ShaderUtil.CheckGlError(TAG, "Init texture.");

            GLES20.GlEnable(GLES20.GlDepthTest);
            GLES20.GlEnable(GL_CULL_FACE_CONSTANT);

            // Draw point.
            GLES20.GlUseProgram(mProgram);
            ShaderUtil.CheckGlError(TAG, "Draw point.");
            GLES20.GlEnableVertexAttribArray(mPositionAttribute);
            GLES20.GlEnableVertexAttribArray(mTextureCoordAttribute);
            GLES20.GlEnableVertexAttribArray(mColorUniform);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, mVerticeId);
            GLES20.GlVertexAttribPointer(mPositionAttribute, POSITION_COMPONENTS_NUMBER, GLES20.GlFloat, false,
                BYTES_PER_POINT, 0);
            GLES20.GlVertexAttribPointer(mTextureCoordAttribute, TEXCOORD_COMPONENTS_NUMBER, GLES20.GlFloat, false,
                BYTES_PER_COORD, 0);
            GLES20.GlUniform4f(mColorUniform, 1.0f, 0.0f, 0.0f, 1.0f);
            GLES20.GlUniformMatrix4fv(mModelViewProjectionUniform, 1, false, mModelViewProjections, 0);
            GLES20.GlUniform1f(mPointSizeUniform, 5.0f); // Set the size of Point to 5.
            GLES20.GlDrawArrays(GLES20.GlPoints, 0, mPointsNum);
            GLES20.GlDisableVertexAttribArray(mColorUniform);
            GLES20.GlBindBuffer(GLES20.GlArrayBuffer, 0);
            ShaderUtil.CheckGlError(TAG, "Draw point.");

            // Draw triangles.
            GLES20.GlEnableVertexAttribArray(mColorUniform);

            // Clear the color and use the texture color to draw triangles.
            GLES20.GlUniform4f(mColorUniform, 0.0f, 0.0f, 0.0f, 0.0f);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, mTriangleId);

            // The number of input triangle points
            GLES20.GlDrawElements(GLES20.GlTriangles, mTrianglesNum * 3, GLES20.GlUnsignedInt, 0);
            GLES20.GlBindBuffer(GLES20.GlElementArrayBuffer, 0);
            GLES20.GlDisableVertexAttribArray(mColorUniform);
            ShaderUtil.CheckGlError(TAG, "Draw triangles.");

            GLES20.GlDisableVertexAttribArray(mTextureCoordAttribute);
            GLES20.GlDisableVertexAttribArray(mPositionAttribute);
            GLES20.GlBindTexture(GLES20.GlTexture2d, 0);

            GLES20.GlDisable(GLES20.GlDepthTest);
            GLES20.GlDisable(GL_CULL_FACE_CONSTANT);
            ShaderUtil.CheckGlError(TAG, "Draw after.");
        }


    }
}