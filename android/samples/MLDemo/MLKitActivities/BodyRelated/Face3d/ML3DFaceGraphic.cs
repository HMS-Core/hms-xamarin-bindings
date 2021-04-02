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
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face.Face3d;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face3d
{
    public class ML3DFaceGraphic : Graphic
    {
        private const string Tag = "ML3DFaceGraphic";

        private readonly GraphicOverlay overlay;
        private readonly Paint keypointPaint;
        private readonly Paint boxPaint;
        private volatile ML3DFace mLFace;
        private Context mContext;
        private static float LineWidth;

        public ML3DFaceGraphic(GraphicOverlay overlay, ML3DFace face, Context context) : base(overlay)
        {

            this.mContext = context;
            this.mLFace = face;
            this.overlay = overlay;
            LineWidth = Dp2px(this.mContext, 3);

            this.keypointPaint = new Paint();
            this.keypointPaint.Color = (Color.Red);
            this.keypointPaint.SetStyle(Paint.Style.Fill);
            this.keypointPaint.TextSize = Dp2px(context, 2);

            this.boxPaint = new Paint();
            this.boxPaint.Color = Color.Blue;
            this.boxPaint.SetStyle(Paint.Style.Stroke);
            this.boxPaint.StrokeWidth = ML3DFaceGraphic.LineWidth;

        }

        public static float Dp2px(Context context, float dipValue)
        {
            return dipValue * context.Resources.DisplayMetrics.Density + 0.5f;
        }

        public override void Draw(Canvas canvas)
        {
            if (this.mLFace == null)
            {
                return;
            }
            IList<MLPosition> face3dPoints = mLFace.Get3DKeyPoints(ML3DFace.LandmarkFive);

            float[] projectionMatrix = new float[4 * 4];
            float[] viewMatrix = new float[4 * 4];
            mLFace.Get3DProjectionMatrix(projectionMatrix, 1, 10);
            mLFace.Get3DViewMatrix(viewMatrix);

            int frameHeight = (int)UnScaleX(overlay.Height);// Image height
            int frameWidth = (int)UnScaleY(overlay.Width);// Image Width

            float[] adaptMatrix = { frameWidth / 2, 0, frameWidth / 2, 0, -frameHeight / 2, frameHeight / 2, 0, 0, 1 };
            IList<MLPosition> face2dPoints = TranslateTo2D(face3dPoints, projectionMatrix, viewMatrix, adaptMatrix);

            StringBuilder sb = new StringBuilder();
            // Draw 2D points
            Paint numPaint;
            numPaint = new Paint();
            numPaint.Color = Color.Red;
            numPaint.TextSize = frameHeight / 80;
            for (int i = 0; i < face2dPoints.Count; i++)
            {
                MLPosition point = face2dPoints.ElementAt(i);
                canvas.DrawPoint(TranslateX(point.GetX().FloatValue()), TranslateY(point.GetY().FloatValue()), boxPaint);
                canvas.DrawText("" + i, TranslateX(point.GetX().FloatValue()), TranslateY(point.GetY().FloatValue()), numPaint);
                sb.Append(point.GetX() + " " + point.GetY() + "\n");

            }
        }

        private IList<MLPosition> TranslateTo2D(IList<MLPosition> face3dPoints, float[] projectionMatrix, float[] viewMatrix, float[] adaptMatrix)
        {
            IList<MLPosition> face2dPoints = new List<MLPosition>();
            for (int i = 0; i < face3dPoints.Count; i++)
            {
                MLPosition curPoint = face3dPoints.ElementAt(i);
                float[] curVec = { (float)curPoint.GetX(), (float)curPoint.GetY(), (float)curPoint.GetZ(), 1 };
                //1 V*Vec
                float[] temp1 = MatrixMulti(viewMatrix, 4, 4, curVec);
                //2 P*(V*Vec)
                float[] temp2 = MatrixMulti(projectionMatrix, 4, 4, temp1);
                //3 calculations x’ y'
                float[] temp3 = { temp2[0] / temp2[3], temp2[1] / temp2[3], 1 };
                //4 calculations X Y coordinates
                float[] point = MatrixMulti(adaptMatrix, 3, 3, temp3);
                face2dPoints.Add(new MLPosition((Java.Lang.Float)point[0], (Java.Lang.Float)point[1]));
            }
            return face2dPoints;
        }

        private float[] MatrixMulti(float[] V, int m, int n, float[] vec)
        {
            float[] result = new float[n];
            for (int i = 0; i < n; i++)
            {
                float temp = 0;
                for (int j = 0; j < m; j++)
                {
                    temp += V[i * m + j] * vec[j];
                }
                result[i] = temp;
            }
            return result;
        }
    }
}