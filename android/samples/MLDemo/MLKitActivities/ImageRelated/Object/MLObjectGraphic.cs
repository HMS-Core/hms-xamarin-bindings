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
using Huawei.Hms.Mlsdk.Objects;
using HmsXamarinMLDemo.Camera;
using static Android.Graphics.Paint;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.Object
{
    public class MLObjectGraphic : Graphic
    {
        private static readonly float TextSize = 54.0f;
        private static readonly float StrokeWidth = 4.0f;
        private readonly MLObject mObject;
        private readonly Paint boxPaint;
        private readonly Paint textPaint;

        public MLObjectGraphic(GraphicOverlay overlay, MLObject MObject) : base(overlay)
        {

            this.mObject = MObject;

            this.boxPaint = new Paint();
            this.boxPaint.Color = Color.White;
            this.boxPaint.SetStyle(Style.Stroke);
            this.boxPaint.StrokeWidth = MLObjectGraphic.StrokeWidth;

            this.textPaint = new Paint();
            this.textPaint.Color = Color.White;
            this.textPaint.TextSize = MLObjectGraphic.TextSize;
        }

        public override void Draw(Canvas canvas)
        {
            // draw the object border.
            RectF rect = new RectF(this.mObject.Border);
            rect.Left = this.TranslateX(rect.Left);
            rect.Top = this.TranslateY(rect.Top);
            rect.Right = this.TranslateX(rect.Right);
            rect.Bottom = this.TranslateY(rect.Bottom);
            canvas.DrawRect(rect, this.boxPaint);

            // draw other object info.
            canvas.DrawText(MLObjectGraphic.GetCategoryName(this.mObject.TypeIdentity), rect.Left, rect.Bottom, this.textPaint);
            canvas.DrawText("trackingId: " + this.mObject.TracingIdentity, rect.Left, rect.Top, this.textPaint);
            if (this.mObject.TypePossibility != null)
            {
                canvas.DrawText("confidence: " + this.mObject.TypePossibility, rect.Right, rect.Bottom, this.textPaint); 
            }
        }
        private static string GetCategoryName(int category)
        {
            switch (category)
            {
                case MLObject.TypeOther:
                    return "Unknown";
                case MLObject.TypeFurniture:
                    return "Home good";
                case MLObject.TypeGoods:
                    return "Fashion good";
                case MLObject.TypePlace:
                    return "Place";
                case MLObject.TypePlant:
                    return "Plant";
                case MLObject.TypeFood:
                    return "Food";
                case MLObject.TypeFace:
                    return "Face";
                default:
                    return "";
            }
            return "";
        }
    }
}