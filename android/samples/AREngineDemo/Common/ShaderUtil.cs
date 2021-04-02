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


namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// This class is used to read shader code and compile links.
    /// </summary>
    public class ShaderUtil
    {
        private ShaderUtil()
        {
        }

        /// <summary>
        /// Check OpenGL ES running exceptions and throw them when necessary.
        /// </summary>
        /// <param name="tag">Exception information.</param>
        /// <param name="label">Program label.</param>
        public static void CheckGlError(string tag, string label)
        {
            int lastError = GLES20.GlNoError;
            int error = GLES20.GlGetError();
            while (error != GLES20.GlNoError)
            {
                Log.Debug(tag, label + ": glError " + error);
                lastError = error;
                error = GLES20.GlGetError();
            }
            if (lastError != GLES20.GlNoError)
            {
                
                throw new ArDemoRuntimeException(label + ": glError " + lastError);
            }
        }
    }
}