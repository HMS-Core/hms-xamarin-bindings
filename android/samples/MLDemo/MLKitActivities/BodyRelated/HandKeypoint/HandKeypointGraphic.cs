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
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Handkeypoint;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.HandKeypoint
{
    /// <summary>
    ///  Graphic instance for rendering hand position, orientation, 
    ///  and landmarks within an associated graphic overlay view.
    /// </summary>
    public class HandKeypointGraphic : Graphic
    {
        private static readonly float BoxStrokeWidth = 5.0f;

        private readonly Paint rectPaint;
        private readonly Paint idPaintnew;
        private IList<MLHandKeypoints> handKeypoints;

        public HandKeypointGraphic(GraphicOverlay overlay, IList<MLHandKeypoints> handKeypoints) : base(overlay)
        {
            this.handKeypoints = handKeypoints;

            Color selectedColor = Color.White;

            idPaintnew = new Paint();
            idPaintnew.Color  = Color.Green;
            idPaintnew.TextSize = 32;

            rectPaint = new Paint();
            rectPaint.Color = selectedColor;
            rectPaint.SetStyle(Paint.Style.Stroke);
            rectPaint.StrokeWidth = BoxStrokeWidth;
        }

        public override void Draw(Canvas canvas)
        {
            for (int i = 0; i < handKeypoints.Count; i++)
            {
                MLHandKeypoints mHandKeypoints = handKeypoints.ElementAt(i);
                if (mHandKeypoints.HandKeypoints == null)
                {
                    continue;
                }

                Rect rect = TranslateRect(handKeypoints.ElementAt(i).Rect);
                canvas.DrawRect(rect, rectPaint);
                foreach(MLHandKeypoint handKeypoint in mHandKeypoints.HandKeypoints)
                {
                    if (!(Math.Abs(handKeypoint.PointX - 0f) == 0 && Math.Abs(handKeypoint.PointY - 0f) == 0))
                    {
                        canvas.DrawCircle(TranslateX(handKeypoint.PointX),
                                TranslateY(handKeypoint.PointY), 24f, idPaintnew);
                    }
                }
            }
        }
        public Rect TranslateRect(Rect rect)
        {
            float left = TranslateX(rect.Left);
            float right = TranslateX(rect.Right);
            float bottom = TranslateY(rect.Bottom);
            float top = TranslateY(rect.Top);
            return new Rect((int)left, (int)top, (int)right, (int)bottom);
        }
    }
}