﻿/*
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


namespace XamarinAREngineDemo.AREngineActivities.Hand.Rendering
{
    /// <summary>
    /// This class provides code and programs for the shader related to hand rendering.
    /// </summary>
    public class HandShaderUtil
    {
        private const string TAG = "HandShaderUtil";

        //Newline character.
        public static readonly string LS = System.Environment.NewLine;

        
        //Code for the hand vertex shader.
        public static readonly string HAND_VERTEX =
            "uniform vec4 inColor;" + LS
            + "attribute vec4 inPosition;" + LS
            + "uniform float inPointSize;" + LS
            + "varying vec4 varColor;" + LS
            + "uniform mat4 inMVPMatrix;" + LS
            + "void main() {" + LS
            + "    gl_PointSize = inPointSize;" + LS
            + "    gl_Position = inMVPMatrix * vec4(inPosition.xyz, 1.0);" + LS
            + "    varColor = inColor;" + LS
            + "}";

        
        //Code for the hand fragment shader.
        public static readonly string HAND_FRAGMENT =
            "precision mediump float;" + LS
            + "varying vec4 varColor;" + LS
            + "void main() {" + LS
            + "    gl_FragColor = varColor;" + LS
            + "}";

        private HandShaderUtil()
        {
        }

        public static int CreateGlProgram()
        {
            int vertex = LoadShader(GLES20.GlVertexShader, HandShaderUtil.HAND_VERTEX);
            if (vertex == 0)
            {
                return 0;
            }
            int fragment = LoadShader(GLES20.GlFragmentShader, HandShaderUtil.HAND_FRAGMENT);
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
                    Log.Debug(TAG, "glError: Could not compile shader " + shaderType);
                    Log.Debug(TAG, "GLES20 Error: " + GLES20.GlGetShaderInfoLog(shader));
                    GLES20.GlDeleteShader(shader);
                    shader = 0;
                }
            }
            return shader;
        }
    }
}