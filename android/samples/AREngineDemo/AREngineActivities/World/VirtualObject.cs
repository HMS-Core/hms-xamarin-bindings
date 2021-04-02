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
using Huawei.Hiar;
using Java.Util;

namespace XamarinAREngineDemo.AREngineActivities.World
{
    /// <summary>
    /// This class provides attributes of the virtual object and necessary methods related to virtual object rendering.
    /// </summary>
    public class VirtualObject : Java.Lang.Object
    {

        private static readonly float ROTATION_ANGLE = 315.0f;

        private static readonly int MATRIX_SIZE = 16;

        private static readonly int COLOR_SIZE = 4;

        private static readonly float SCALE_FACTOR = 0.15f;

        private ARAnchor mArAnchor;

        private float[] mObjectColors = new float[COLOR_SIZE];

        private float[] mModelMatrix = new float[MATRIX_SIZE];

        private bool mIsSelectedFlag = false;

        /// <summary>
        /// The constructor initializes the pose of the virtual object in a space and the
        /// color of the virtual object with the input anchor point and color parameters.
        /// </summary>
        /// <param name="arAnchor">Data provided by AR Engine, describing the pose.</param>
        /// <param name="color4f">Color data in an array with a length of 4.</param>
        public VirtualObject(ARAnchor arAnchor, float[] color4f)
        {
            mObjectColors = Arrays.CopyOf(color4f, color4f.Length);
            mArAnchor = arAnchor;
            Init();
        }
        
        protected override void JavaFinalize()
        {
            // If the anchor object is destroyed, call the detach() method to instruct
            // the AR Engine to stop tracking the anchor.
            if (mArAnchor != null)
            {
                mArAnchor.Detach();
            }
            base.JavaFinalize();
        }

        private void Init()
        {
            // Set a scaling matrix, in which the elements of the principal diagonal is the scaling coefficient.
            Matrix.SetIdentityM(mModelMatrix, 0);
            mModelMatrix[0] = SCALE_FACTOR;
            mModelMatrix[5] = SCALE_FACTOR;
            mModelMatrix[10] = SCALE_FACTOR;

            // Rotate the camera along the Y axis by a certain angle.
            Matrix.RotateM(mModelMatrix, 0, ROTATION_ANGLE, 0f, 1f, 0f);
        }

        /// <summary>
        /// Update the anchor information in the virtual object corresponding to the class.
        /// </summary>
        /// <param name="arAnchor">Data provided by AR Engine, describing the pose.</param>
        public void SetAnchor(ARAnchor arAnchor)
        {
            if (mArAnchor != null)
            {
                mArAnchor.Detach();
            }
            mArAnchor = arAnchor;
        }

        /// <summary>
        /// Obtain the anchor information of a virtual object corresponding to the class.
        /// </summary>
        /// <returns>ARAnchor(provided by AREngine)</returns>
        public ARAnchor GetAnchor()
        {
            return mArAnchor;
        }

        /// <summary>
        /// Obtain the anchor information of a virtual object corresponding to the class.
        /// </summary>
        /// <returns>Color of the virtual object, returned in an array with a length of 4.</returns>
        public float[] GetColor()
        {
            float[] rets = new float[4];
            if (mIsSelectedFlag)
            {
                rets[0] = 255.0f - mObjectColors[0];
                rets[1] = 255.0f - mObjectColors[1];
                rets[2] = 255.0f - mObjectColors[2];
                rets[3] = mObjectColors[3];
                return rets;
            }
            else
            {
                rets = Arrays.CopyOf(mObjectColors, mObjectColors.Length);
                return rets;
            }
        }

        /// <summary>
        /// Set the color of the current virtual object.
        /// </summary>
        /// <param name="color">Color data in an array with a length of 4.</param>
        public void SetColor(float[] color)
        {
            if (color != null && color.Length == COLOR_SIZE)
            {
                mObjectColors = Arrays.CopyOf(color, color.Length);
            }
        }

        /// <summary>
        /// Obtain the anchor matrix data of the current virtual object.
        /// </summary>
        /// <returns>Anchor matrix data of the current virtual object.</returns>
        public float[] GetModelAnchorMatrix()
        {
            float[] modelMatrix = new float[MATRIX_SIZE];
            if (mArAnchor != null)
            {
                mArAnchor.Pose.ToMatrix(modelMatrix, 0);
            }
            else
            {
                Matrix.SetIdentityM(modelMatrix, 0);
            }
            float[] rets = new float[MATRIX_SIZE];
            Matrix.MultiplyMM(rets, 0, modelMatrix, 0, mModelMatrix, 0);
            return rets;
        }

        /// <summary>
        /// Determine whether the current virtual object is in a selected state.
        /// </summary>
        /// <returns>Check whether the object is selected.</returns>
        public bool GetIsSelectedFlag()
        {
            return mIsSelectedFlag;
        }

        /// <summary>
        /// Set the selection status of the current object by passing true or false,
        /// where true indicates that the object is selected, and false indicates not.
        /// </summary>
        /// <param name="isSelected">Whether the selection is successful.</param>
        public void SetIsSelected(bool isSelected)
        {
            mIsSelectedFlag = isSelected;
        }

    }
}