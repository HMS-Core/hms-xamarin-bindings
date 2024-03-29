﻿/*
*       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face
{
    public class FaceAnalyzerTransactor : Java.Lang.Object, MLAnalyzer.IMLTransactor
    {
        private const string Tag = "FaceAnalyzerTransactor";

        private GraphicOverlay mGraphicOverlay;

        public FaceAnalyzerTransactor(GraphicOverlay ocrGraphicOverlay)
        {
            this.mGraphicOverlay = ocrGraphicOverlay;
        }
        /// <summary>
        /// Clear GraphicOverlay OnDestroy event.
        /// </summary>
        public void Destroy()
        {
            mGraphicOverlay.Clear();
        }

        public void TransactResult(MLAnalyzer.Result result)
        {
            this.mGraphicOverlay.Clear();
            SparseArray faceSparseArray = result.AnalyseList;
            for (int i = 0; i < faceSparseArray.Size(); i++)
            {
                MLFaceGraphic graphic = new MLFaceGraphic(this.mGraphicOverlay, (MLFace)faceSparseArray.ValueAt(i));
                this.mGraphicOverlay.Add(graphic);
            }
        }
    }
}