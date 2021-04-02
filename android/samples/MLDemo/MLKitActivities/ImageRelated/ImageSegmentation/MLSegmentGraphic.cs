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
using Huawei.Hms.Mlsdk.Imgseg;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSegmentation
{
    public class MLSegmentGraphic : Graphic
    {
        private const string Tag = "MLSegmentGraphic";

        private Rect mDestRect;
        private readonly Paint resultPaint;
        private Bitmap bitmapForeground;
        private Bitmap bitmapBackground;
        private bool isFront;
        public MLSegmentGraphic(LensEnginePreview preview, GraphicOverlay overlay, MLImageSegmentation segmentation, Boolean isFront)
            :base(overlay)
        {
            this.bitmapForeground = segmentation.Foreground;
            this.isFront = isFront;

            int width = bitmapForeground.Width;
            int height = bitmapForeground.Height;
            int div = overlay.Width - preview.Width;
            int left = overlay.Width - width + div / 2;
            // Set the image display area.
            // Partial display.
            mDestRect = new Rect(left, 0, overlay.Width - div / 2, height / 2);
            // All display.
            // mDestRect = new Rect(0, 0, overlay.Width, overlay.Height);
            this.resultPaint = new Paint(PaintFlags.AntiAlias);
            this.resultPaint.FilterBitmap = true;
            this.resultPaint.Dither = true;
        }
        public override void Draw(Canvas canvas)
        {
            if (isFront)
            {
                bitmapForeground = Convert(bitmapForeground);
            }
            this.bitmapBackground = Bitmap.CreateBitmap(bitmapForeground.Width, bitmapForeground.Height, Bitmap.Config.Argb8888);
            this.bitmapBackground.EraseColor(Color.ParseColor("#ffffff"));

            canvas.DrawBitmap(bitmapForeground, null, mDestRect, resultPaint);
        }

        public static Bitmap JoinBitmap(Bitmap background, Bitmap foreground)
        {
            if (background == null || foreground == null)
            {
                Log.Info(Tag, "bitmap is null.");
                return null;
            }
            if (background.Height != foreground.Height || background.Width != foreground.Width)
            {
                Log.Info(Tag, "bitmap size is not match.");
                return null;
            }
            Bitmap newmap = Bitmap.CreateBitmap(foreground.Width, foreground.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(newmap);
            canvas.DrawBitmap(background, 0, 0, null);
            canvas.DrawBitmap(foreground, 0, 0, null);
            canvas.Save();
            canvas.Restore();
            return newmap;
        }

        private Bitmap Convert(Bitmap bitmap)
        {
            Matrix m = new Matrix();
            m.SetScale(-1, 1);
            Bitmap reverseBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, m, true);
            return reverseBitmap;
        }
    }
}