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


namespace XamarinAREngineDemo.AREngineActivities.World.Rendering
{
    /// <summary>
    /// This class provides code and program for the rendering shader related to the world scene.
    /// </summary>
    public class WorldShaderUtil
    {
        private const string TAG = "WorldShaderUtil";

        private static readonly string LS = System.Environment.NewLine;

        
        //Vertex shader for label rendering.
        private static readonly string LABEL_VERTEX =
            "uniform mat2 inPlanUVMatrix;" + LS
            + "uniform mat4 inMVPMatrix;" + LS
            + "attribute vec3 inPosXZAlpha;" + LS
            + "varying vec3 varTexCoordAlpha;" + LS
            + "void main() {" + LS
            + "    vec4 tempPosition = vec4(inPosXZAlpha.x, 0.0, inPosXZAlpha.y, 1.0);" + LS
            + "    vec2 tempUV = inPlanUVMatrix * inPosXZAlpha.xy;" + LS
            + "    varTexCoordAlpha = vec3(tempUV.x + 0.5, tempUV.y + 0.5, inPosXZAlpha.z);" + LS
            + "    gl_Position = inMVPMatrix * tempPosition;" + LS
            + "}";

        
        //Fragment shader for label rendering.
        private static readonly string LABEL_FRAGMENT =
            "precision highp float;" + LS
            + "uniform sampler2D inTexture;" + LS
            + "varying vec3 varTexCoordAlpha;" + LS
            + "void main() {" + LS
            + "    vec4 control = texture2D(inTexture, varTexCoordAlpha.xy);" + LS
            + "    gl_FragColor = vec4(control.rgb, 1.0);" + LS
            + "}";

        private static readonly string OBJECT_VERTEX =
            "uniform mat4 inMVPMatrix;" + LS
            + "uniform mat4 inViewMatrix;" + LS
            + "attribute vec3 inObjectNormalVector;" + LS
            + "attribute vec4 inObjectPosition;" + LS
            + "attribute vec2 inTexCoordinate;" + LS
            + "varying vec3 varCameraNormalVector;" + LS
            + "varying vec2 varTexCoordinate;" + LS
            + "varying vec3 varCameraPos;" + LS
            + "void main() {" + LS
            + "    gl_Position = inMVPMatrix * inObjectPosition;" + LS
            + "    varCameraNormalVector = (inViewMatrix * vec4(inObjectNormalVector, 0.0)).xyz;" + LS
            + "    varTexCoordinate = inTexCoordinate;" + LS
            + "    varCameraPos = (inViewMatrix * inObjectPosition).xyz;" + LS
            + "}";

        private static readonly string OBJECT_FRAGMENT =
            "precision mediump float;" + LS
            + " uniform vec4 inLight;" + LS
            + "uniform vec4 inObjectColor;" + LS
            + "uniform sampler2D inObjectTexture;" + LS
            + "varying vec3 varCameraPos;" + LS
            + "varying vec3 varCameraNormalVector;" + LS
            + "varying vec2 varTexCoordinate;" + LS
            + "void main() {" + LS
            + "    vec4 objectColor = texture2D(inObjectTexture, vec2(varTexCoordinate.x, 1.0 - varTexCoordinate.y));" + LS
            + "    objectColor.rgb = inObjectColor.rgb / 255.0;" + LS
            + "    vec3 viewNormal = normalize(varCameraNormalVector);" + LS
            + "    vec3 reflectedLightDirection = reflect(inLight.xyz, viewNormal);" + LS
            + "    vec3 normalCameraPos = normalize(varCameraPos);" + LS
            + "    float specularStrength = max(0.0, dot(normalCameraPos, reflectedLightDirection));" + LS
            + "    gl_FragColor.a = objectColor.a;" + LS
            + "    float diffuse = inLight.w * 3.5 *" + LS
            + "        0.5 * (dot(viewNormal, inLight.xyz) + 1.0);" + LS
            + "    float specular = inLight.w *" + LS
            + "        pow(specularStrength, 6.0);" + LS
            + "    gl_FragColor.rgb = objectColor.rgb * + diffuse + specular;" + LS
            + "}";

        private WorldShaderUtil()
        {
        }

        public static int GetLabelProgram()
        {
            return CreateGlProgram(LABEL_VERTEX, LABEL_FRAGMENT);
        }

        public static int GetObjectProgram()
        {
            return CreateGlProgram(OBJECT_VERTEX, OBJECT_FRAGMENT);
        }

        private static int CreateGlProgram(string vertexCode, string fragmentCode)
        {
            int vertex = LoadShader(GLES20.GlVertexShader, vertexCode);
            if (vertex == 0)
            {
                return 0;
            }
            int fragment = LoadShader(GLES20.GlFragmentShader, fragmentCode);
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
                    Log.Debug(TAG, "Could not link program " + GLES20.GlGetProgramInfoLog(program));
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
    }
}