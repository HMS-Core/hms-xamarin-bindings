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


namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// This is a common class for drawing camera textures that you can use to display camera images on the screen.
    /// </summary>
    public class TextureDisplay
    {
        private const string TAG = "TextureDisplay";

        private static readonly string LS = System.Environment.NewLine;

        private static readonly string BASE_FRAGMENT =
            "#extension GL_OES_EGL_image_external : require" + LS
            + "precision mediump float;" + LS
            + "varying vec2 textureCoordinate;" + LS
            + "uniform samplerExternalOES vTexture;" + LS
            + "void main() {" + LS
            + "    gl_FragColor = texture2D(vTexture, textureCoordinate );" + LS
            + "}";

        private static readonly string BASE_VERTEX =
            "attribute vec4 vPosition;" + LS
            + "attribute vec2 vCoord;" + LS
            + "uniform mat4 vMatrix;" + LS
            + "uniform mat4 vCoordMatrix;" + LS
            + "varying vec2 textureCoordinate;" + LS
            + "void main(){" + LS
            + "    gl_Position = vMatrix*vPosition;" + LS
            + "    textureCoordinate = (vCoordMatrix*vec4(vCoord,0,1)).xy;" + LS
            + "}";

        // Coordinates of a vertex.
        private static readonly float[] POS = { -1.0f, 1.0f, -1.0f, -1.0f, 1.0f, 1.0f, 1.0f, -1.0f };

        // Texture coordinates.
        private static readonly float[] COORD = { 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f };

        private const int MATRIX_SIZE = 16;

        private const float RGB_CLEAR_VALUE = 0.8157f;

        private int mExternalTextureId;

        private int mProgram;

        private int mPosition;

        private int mCoord;

        private int mMatrix;

        private int mTexture;

        private int mCoordMatrix;

        private FloatBuffer mVerBuffer;

        private FloatBuffer mTexTransformedBuffer;

        private FloatBuffer mTexBuffer;

        private float[] mProjectionMatrix = new float[MATRIX_SIZE];

        private float[] coordMatrixs;

        /// <summary>
        /// The constructor is a texture rendering utility class, 
        /// used to create a texture rendering object.
        /// </summary>
        public TextureDisplay()
        {
            coordMatrixs = MatrixUtil.GetOriginalMatrix();
            InitBuffers();
        }

        /// <summary>
        /// This method is called when Android.Opengl.GLSurfaceView.IRenderer's OnSurfaceChanged
        /// to update the projection matrix.
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void OnSurfaceChanged(int width, int height)
        {
            MatrixUtil.GetProjectionMatrix(mProjectionMatrix, width, height);
        }

        /// <summary>
        /// This method is called when Android.Opengl.GLSurfaceView.IRenderer's OnSurfaceCreated
        /// to initialize the texture ID and create the OpenGL ES shader program.
        /// </summary>
        public void Init()
        {
            int[] textures = new int[1];
            GLES20.GlGenTextures(1, textures, 0);
            mExternalTextureId = textures[0];
            GenerateExternalTexture();
            CreateProgram();
        }

        /// <summary>
        /// If the texture ID has been created externally, this method is called when
        /// Android.Opengl.GLSurfaceView.IRenderer's OnSurfaceCreated.
        /// </summary>
        /// <param name="textureId">Texture id.</param>
        public void Init(int textureId)
        {
            mExternalTextureId = textureId;
            GenerateExternalTexture();
            CreateProgram();
        }

        /// <summary>
        /// Obtain the texture ID.
        /// </summary>
        /// <returns>Texture id.</returns>
        public int GetExternalTextureId()
        {
            return mExternalTextureId;
        }

        /// <summary>
        /// Render each frame. 
        /// This method is called when Android.Opengl.GLSurfaceView.IRenderer's OnDrawFrame.
        /// </summary>
        /// <param name="frame">ARFrame</param>
        public void OnDrawFrame(ARFrame frame)
        {
            ShaderUtil.CheckGlError(TAG, "On draw frame start.");
            if (frame == null)
            {
                return;
            }
            if (frame.HasDisplayGeometryChanged)
            {
                frame.TransformDisplayUvCoords(mTexBuffer, mTexTransformedBuffer);
            }
            Clear();

            GLES20.GlDisable(GLES20.GlDepthTest);
            GLES20.GlDepthMask(false);

            GLES20.GlUseProgram(mProgram);

            // Set the texture ID.
            GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, mExternalTextureId);

            // Set the projection matrix.
            GLES20.GlUniformMatrix4fv(mMatrix, 1, false, mProjectionMatrix, 0);

            GLES20.GlUniformMatrix4fv(mCoordMatrix, 1, false, coordMatrixs, 0);

            // Set the vertex.
            GLES20.GlEnableVertexAttribArray(mPosition);
            GLES20.GlVertexAttribPointer(mPosition, 2, GLES20.GlFloat, false, 0, mVerBuffer);

            // Set the texture coordinates.
            GLES20.GlEnableVertexAttribArray(mCoord);
            GLES20.GlVertexAttribPointer(mCoord, 2, GLES20.GlFloat, false, 0, mTexTransformedBuffer);

            // Number of vertices.
            GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);
            GLES20.GlDisableVertexAttribArray(mPosition);
            GLES20.GlDisableVertexAttribArray(mCoord);

            GLES20.GlDepthMask(true);
            GLES20.GlEnable(GLES20.GlDepthTest);
            ShaderUtil.CheckGlError(TAG, "On draw frame end.");
        }

        private void GenerateExternalTexture()
        {
            GLES20.GlBindTexture(GLES11Ext.GlTextureExternalOes, mExternalTextureId);
            GLES20.GlTexParameteri(GLES11Ext.GlTextureExternalOes, GLES10.GlTextureWrapS, GLES10.GlClampToEdge);
            GLES20.GlTexParameteri(GLES11Ext.GlTextureExternalOes, GLES10.GlTextureWrapT, GLES10.GlClampToEdge);
            GLES20.GlTexParameterf(GLES11Ext.GlTextureExternalOes, GLES10.GlTextureMinFilter, GLES10.GlNearest);
            GLES20.GlTexParameterf(GLES11Ext.GlTextureExternalOes, GLES10.GlTextureMagFilter, GLES10.GlNearest);
        }

        private void CreateProgram()
        {
            mProgram = CreateGlProgram();
            mPosition = GLES20.GlGetAttribLocation(mProgram, "vPosition");
            mCoord = GLES20.GlGetAttribLocation(mProgram, "vCoord");
            mMatrix = GLES20.GlGetUniformLocation(mProgram, "vMatrix");
            mTexture = GLES20.GlGetUniformLocation(mProgram, "vTexture");
            mCoordMatrix = GLES20.GlGetUniformLocation(mProgram, "vCoordMatrix");
        }

        private static int CreateGlProgram()
        {
            int vertex = LoadShader(GLES20.GlVertexShader, BASE_VERTEX);
            if (vertex == 0)
            {
                return 0;
            }
            int fragment = LoadShader(GLES20.GlFragmentShader, BASE_FRAGMENT);
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
                    GlError("Could not link program:" + GLES20.GlGetProgramInfoLog(program));
                    GLES20.GlDeleteProgram(program);
                    program = 0;
                }
            }
            return program;
        }

        private static int LoadShader(int shaderType, String source)
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
                    GlError("Could not compile shader:" + shaderType);
                    GlError("GLES20 Error:" + GLES20.GlGetShaderInfoLog(shader));
                    GLES20.GlDeleteShader(shader);
                    shader = 0;
                }
            }
            return shader;
        }

        private static void GlError(Object index)
        {
            Log.Debug(TAG, "glError: " + index);
        }

        private void InitBuffers()
        {
            // Initialize the size of the vertex buffer.
            ByteBuffer byteBufferForVer = ByteBuffer.AllocateDirect(32);
            byteBufferForVer.Order(ByteOrder.NativeOrder());
            mVerBuffer = byteBufferForVer.AsFloatBuffer();
            mVerBuffer.Put(POS);
            mVerBuffer.Position(0);

            // Initialize the size of the texture buffer.
            ByteBuffer byteBufferForTex = ByteBuffer.AllocateDirect(32);
            byteBufferForTex.Order(ByteOrder.NativeOrder());
            mTexBuffer = byteBufferForTex.AsFloatBuffer();
            mTexBuffer.Put(COORD);
            mTexBuffer.Position(0);

            // Initialize the size of the transformed texture buffer.
            ByteBuffer byteBufferForTransformedTex = ByteBuffer.AllocateDirect(32);
            byteBufferForTransformedTex.Order(ByteOrder.NativeOrder());
            mTexTransformedBuffer = byteBufferForTransformedTex.AsFloatBuffer();
        }

        /// <summary>
        /// Clear canvas.
        /// </summary>
        private void Clear()
        {
            GLES20.GlClearColor(RGB_CLEAR_VALUE, RGB_CLEAR_VALUE, RGB_CLEAR_VALUE, 1.0f);
            GLES20.GlClear(GLES20.GlDepthBufferBit);
        }
    }
}