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
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face
{
    /// <summary>
    /// GraphicOverlay for live face analyzer.
    /// </summary>
    public class MLFaceGraphic : Graphic
    {
        private static readonly float BoxStrokeWidth = 8.0f;

        private static readonly float LineWidth = 5.0f;

        private readonly GraphicOverlay overlay;

        private readonly Paint facePositionPaint;

        private readonly Paint landmarkPaint;

        private readonly Paint boxPaint;

        private readonly Paint facePaint;

        private readonly Paint eyePaint;

        private readonly Paint eyebrowPaint;

        private readonly Paint lipPaint;

        private readonly Paint nosePaint;

        private readonly Paint noseBasePaint;

        private readonly Paint textPaint;

        private readonly Paint probilityPaint;

        private volatile MLFace mFace;

        public MLFaceGraphic(GraphicOverlay overlay, MLFace face) : base(overlay)
        {

            this.mFace = face;
            this.overlay = overlay;
            Color selectedColor = Color.White;

            this.facePositionPaint = new Paint();
            this.facePositionPaint.Color = selectedColor;

            this.textPaint = new Paint();
            this.textPaint.Color = Color.White;
            this.textPaint.TextSize = (24);
            this.textPaint.SetTypeface(Typeface.Default);

            this.probilityPaint = new Paint();
            this.probilityPaint.Color = Color.White;
            this.probilityPaint.TextSize = (35);
            this.probilityPaint.SetTypeface(Typeface.Default);

            this.landmarkPaint = new Paint();
            this.landmarkPaint.Color = Color.Red;
            this.landmarkPaint.SetStyle(Paint.Style.Fill);
            this.landmarkPaint.StrokeWidth = (10f);

            this.boxPaint = new Paint();
            this.boxPaint.Color = Color.White;
            this.boxPaint.SetStyle(Paint.Style.Stroke);
            this.boxPaint.StrokeWidth = (MLFaceGraphic.BoxStrokeWidth);

            this.facePaint = new Paint();
            this.facePaint.Color = (Color.ParseColor("#ffcc66"));
            this.facePaint.SetStyle(Paint.Style.Stroke);
            this.facePaint.StrokeWidth = (MLFaceGraphic.LineWidth);

            this.eyePaint = new Paint();
            this.eyePaint.Color = (Color.ParseColor("#00ccff"));
            this.eyePaint.SetStyle(Paint.Style.Stroke);
            this.eyePaint.StrokeWidth = (MLFaceGraphic.LineWidth);

            this.eyebrowPaint = new Paint();
            this.eyebrowPaint.Color = (Color.ParseColor("#006666"));
            this.eyebrowPaint.SetStyle(Paint.Style.Stroke);
            this.eyebrowPaint.StrokeWidth = (MLFaceGraphic.LineWidth);

            this.nosePaint = new Paint();
            this.nosePaint.Color = (Color.ParseColor("#ffff00"));
            this.nosePaint.SetStyle(Paint.Style.Stroke);
            this.nosePaint.StrokeWidth = (MLFaceGraphic.LineWidth);

            this.noseBasePaint = new Paint();
            this.noseBasePaint.Color = (Color.ParseColor("#ff6699"));
            this.noseBasePaint.SetStyle(Paint.Style.Stroke);
            this.noseBasePaint.StrokeWidth = (MLFaceGraphic.LineWidth);

            this.lipPaint = new Paint();
            this.lipPaint.Color = (Color.ParseColor("#990000"));
            this.lipPaint.SetStyle(Paint.Style.Stroke);
            this.lipPaint.StrokeWidth = (MLFaceGraphic.LineWidth);
        }

        public override void Draw(Canvas canvas)
        {
            if (this.mFace == null)
            {
                return;
            }
            float start = 350f;
            float x = start;
            float width = 500f;
            float y = this.overlay.Height - 300.0f;
            Dictionary<string, float> emotions = new Dictionary<string, float>();
            emotions.Add("Smiling", this.mFace.Emotions.SmilingProbability);
            emotions.Add("Neutral", this.mFace.Emotions.NeutralProbability);
            emotions.Add("Angry", this.mFace.Emotions.AngryProbability);
            emotions.Add("Fear", this.mFace.Emotions.FearProbability);
            emotions.Add("Sad", this.mFace.Emotions.SadProbability);
            emotions.Add("Disgust", this.mFace.Emotions.DisgustProbability);
            emotions.Add("Surprise", this.mFace.Emotions.SurpriseProbability);
            //List<string> result = emotions.ToList<string>();

            DecimalFormat decimalFormat = new DecimalFormat("0.000");
            // Draw the facial feature value.
            canvas.DrawText("Left eye: " + decimalFormat.Format(this.mFace.Features.LeftEyeOpenProbability), x, y,
                    this.probilityPaint);
            x = x + width;
            canvas.DrawText("Right eye: " + decimalFormat.Format(this.mFace.Features.RightEyeOpenProbability), x, y,
                    this.probilityPaint);
            y = y - 40.0f;
            x = start;
            canvas.DrawText("Moustache Probability: " + decimalFormat.Format(this.mFace.Features.MoustacheProbability),
                x, y, this.probilityPaint);
            x = x + width;
            canvas.DrawText("Glass Probability: " + decimalFormat.Format(this.mFace.Features.SunGlassProbability), x,
                y, this.probilityPaint);
            y = y - 40.0f;
            x = start;
            canvas.DrawText("Hat Probability: " + decimalFormat.Format(this.mFace.Features.HatProbability), x, y,
                    this.probilityPaint);
            x = x + width;
            canvas.DrawText("Age: " + this.mFace.Features.Age, x, y, this.probilityPaint);
            y = y - 40.0f;
            x = start;
            String sex = (this.mFace.Features.SexProbability > 0.5f) ? "Female" : "Male";
            canvas.DrawText("Gender: " + sex, x, y, this.probilityPaint);
            x = x + width;
            canvas.DrawText("RotationAngleY: " + decimalFormat.Format(this.mFace.RotationAngleY), x, y, this.probilityPaint);
            y = y - 40.0f;
            x = start;
            canvas.DrawText("RotationAngleZ: " + decimalFormat.Format(this.mFace.RotationAngleZ), x, y, this.probilityPaint);
            x = x + width;
            canvas.DrawText("RotationAngleX: " + decimalFormat.Format(this.mFace.RotationAngleX), x, y, this.probilityPaint);
            y = y - 40.0f;
            x = start;
            var sortedDict = from entry in emotions orderby entry.Value descending select entry;
            canvas.DrawText(sortedDict.ElementAt(0).Key, x, y, this.probilityPaint);

            // Draw a face contour.
            if (this.mFace.FaceShapeList != null)
            {
                foreach (MLFaceShape faceShape in this.mFace.FaceShapeList)
                {
                    if (faceShape == null)
                    {
                        continue;
                    }
                    IList<MLPosition> points = faceShape.Points;
                    for (int i = 0; i < points.Count; i++)
                    {
                        MLPosition point = points.ElementAt(i);
                        canvas.DrawPoint(this.TranslateX(point.GetX().FloatValue()), this.TranslateY(point.GetY().FloatValue()),
                                this.boxPaint);
                        if (i != (points.Count - 1))
                        {
                            MLPosition next = points.ElementAt(i + 1); ;
                            if (point != null && point.GetX() != null && point.GetY() != null)
                            {
                                if (i % 3 == 0)
                                {
                                    canvas.DrawText(i + 1 + "", this.TranslateX(point.GetX().FloatValue()),
                                            this.TranslateY(point.GetY().FloatValue()), this.textPaint);
                                }
                                canvas.DrawLines(new float[] {this.TranslateX(point.GetX().FloatValue()),
                                    this.TranslateY(point.GetY().FloatValue()), this.TranslateX(next.GetX().FloatValue()),
                                    this.TranslateY(next.GetY().FloatValue())}, this.GetPaint(faceShape));
                            }
                        }
                    }
                }
            }
            // Face Key Points
            foreach (MLFaceKeyPoint keyPoint in this.mFace.FaceKeyPoints)
            {
                if (keyPoint != null)
                {
                    MLPosition point = keyPoint.Point;
                    canvas.DrawCircle(this.TranslateX((float)point.GetX()), this.TranslateY((float)point.GetY()), 10f, this.landmarkPaint);
                }
            }
        }
        private Paint GetPaint(MLFaceShape faceShape)
        {
            switch (faceShape.FaceShapeType)
            {
                case MLFaceShape.TypeLeftEye:
                case MLFaceShape.TypeRightEye:
                    return this.eyePaint;
                case MLFaceShape.TypeBottomOfLeftEyebrow:

                case MLFaceShape.TypeBottomOfRightEyebrow:
                case MLFaceShape.TypeTopOfLeftEyebrow:
                case MLFaceShape.TypeTopOfRightEyebrow:
                    return this.eyebrowPaint;
                case MLFaceShape.TypeBottomOfLowerLip:
                case MLFaceShape.TypeTopOfLowerLip:
                case MLFaceShape.TypeBottomOfUpperLip:
                case MLFaceShape.TypeTopOfUpperLip:
                    return this.lipPaint;
                case MLFaceShape.TypeBottomOfNose:
                    return this.noseBasePaint;
                case MLFaceShape.TypeBridgeOfNose:
                    return this.nosePaint;
                default:
                    return this.facePaint;
            }
        }
    }
}