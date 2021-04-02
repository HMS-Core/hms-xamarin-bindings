/*
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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Scd;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.SceneDetection
{
    public class MLSceneDetectionGraphic : Graphic
    {
        private readonly GraphicOverlay overlay;
        private readonly IList<MLSceneDetection> results;

        public MLSceneDetectionGraphic(GraphicOverlay overlay, IList<MLSceneDetection> results) :base(overlay)
        {
            this.overlay = overlay;
            this.results = results;
        }

        public override void Draw(Canvas canvas)
        {
            Paint paint = new Paint();
            paint.Color = Color.Red;
            paint.TextSize = 48;

            canvas.DrawText("SceneCount：" + results.Count, overlay.Width / 5, 50, paint);
            for (int i = 0; i < results.Count; i++)
            {
                canvas.DrawText("Scene：" + results.ElementAt(i).Result, overlay.Width / 5, 100 * (i + 1), paint);
                canvas.DrawText("Confidence：" + results.ElementAt(i).Confidence, overlay.Width / 5, (100 * (i + 1)) + 50, paint);
            }
        }
    }
}