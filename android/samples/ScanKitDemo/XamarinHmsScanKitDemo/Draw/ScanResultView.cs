/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Huawei.Hms.Ml.Scan;

namespace XamarinHmsScanKitDemo.Draw
{
    public class ScanResultView : View
    {
        private readonly object syncLock = new object();
        protected float widthScaleFactor = 1.0f;
        protected float heightScaleFactor = 1.0f;
        protected float previewWidth;
        protected float previewHeight;

        public static List<HmsScanGraphic> hmsScanGraphics = new List<HmsScanGraphic>();

        public ScanResultView(Context context) : base(context)
        {

        }

        public ScanResultView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        public void Clear()
        {
            lock (syncLock)
            {
                hmsScanGraphics.Clear();
                PostInvalidate();
            }
        }

        public void Add(HmsScanGraphic graphic)
        {
            lock (syncLock)
            {
                hmsScanGraphics.Add(graphic);
            }
        }

        public void SetCameraInfo(int previewWidth, int previewHeight)
        {
            lock (syncLock)
            {
                this.previewWidth = previewWidth;
                this.previewHeight = previewHeight;
            }
            PostInvalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            lock (syncLock)
            {
                if ((previewWidth != 0) && (previewHeight != 0))
                {
                    widthScaleFactor = (float)canvas.Width / (float)previewWidth;
                    heightScaleFactor = (float)canvas.Height / (float)previewHeight;
                }

                foreach (HmsScanGraphic graphic in hmsScanGraphics)
                {
                    graphic.DrawGraphic(canvas);
                }
            }
        }
        public class HmsScanGraphic
        {

            private static Color TextColor = Color.White;
            private static float TextSize = 35.0f;
            private static float StrokeWidth = 4.0f;

            private readonly Paint rectPaint;
            private readonly Paint hmsScanResult;
            private readonly HmsScan hmsScan;
            private ScanResultView scanResultView;


            public HmsScanGraphic(ScanResultView scanResultView, HmsScan hmsScan, Color? color)
            {
                this.scanResultView = scanResultView;
                this.hmsScan = hmsScan;

                rectPaint = new Paint();
                if (color.HasValue)
                    rectPaint.Color = color.Value;
                rectPaint.SetStyle(Paint.Style.Stroke);
                rectPaint.StrokeWidth = StrokeWidth;

                hmsScanResult = new Paint();
                hmsScanResult.Color = TextColor;
                hmsScanResult.TextSize = TextSize;
            }


            public void DrawGraphic(Canvas canvas)
            {
                if (hmsScan == null)
                {
                    return;
                }

                RectF rect = new RectF(hmsScan.BorderRect);
                RectF other = new RectF();
                other.Left = canvas.Width - ScaleX(rect.Top);
                other.Top = ScaleY(rect.Left);
                other.Right = canvas.Width - ScaleX(rect.Bottom);
                other.Bottom = ScaleY(rect.Right);
                canvas.DrawRect(other, rectPaint);

                canvas.DrawText(hmsScan.OriginalValue, other.Right, other.Bottom, hmsScanResult);
            }

            public float ScaleX(float horizontal)
            {
                return horizontal * scanResultView.widthScaleFactor;
            }

            public float ScaleY(float vertical)
            {
                return vertical * scanResultView.heightScaleFactor;
            }

        }
    }


}