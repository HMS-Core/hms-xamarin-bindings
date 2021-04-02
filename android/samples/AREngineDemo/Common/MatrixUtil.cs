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
using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = Java.Lang.Math;

namespace XamarinAREngineDemo.Common
{
    /// <summary>
    /// Matrix utility class.
    /// </summary>
    public class MatrixUtil
    {
        private const int MATRIX_SIZE = 16;

        private MatrixUtil()
        {
        }

        /// <summary>
        /// Get the matrix of a specified type.
        /// </summary>
        /// <param name="matrix">Results of matrix obtained.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static void GetProjectionMatrix(float[] matrix, int width, int height)
        {
            if (height > 0 && width > 0)
            {
                float[] projection = new float[MATRIX_SIZE];
                float[] camera = new float[MATRIX_SIZE];

                // Calculate the orthographic projection matrix.
                Matrix.OrthoM(projection, 0, -1, 1, -1, 1, 1, 3);
                Matrix.SetLookAtM(camera, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0);
                Matrix.MultiplyMM(matrix, 0, projection, 0, camera, 0);
            }
        }

        /// <summary>
        /// Three-dimensional data standardization method, which divides each
        /// number by the root of the sum of squares of all numbers.
        /// </summary>
        /// <param name="vector">vector.</param>
        public static void NormalizeVec3(float[] vector)
        {
            // This data has three dimensions(0,1,2)
            float length = 1.0f / (float)Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
            vector[0] *= length;
            vector[1] *= length;
            vector[2] *= length;
        }

        /// <summary>
        /// Provide a 4 * 4 unit matrix.
        /// </summary>
        /// <returns>Returns matrix as an array.</returns>
        public static float[] GetOriginalMatrix()
        {
            return new float[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
        }

    }
}