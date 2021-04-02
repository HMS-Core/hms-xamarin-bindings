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

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.BankCard
{
    /// <summary>
    /// Custom control, responsible for the display of the scanning interface
    /// </summary>
    public class ViewfinderView : View
    {
        private const string Tag = "ViewfinderView";

        private  Paint paint;
        private Rect frameRect;
        private Context context;
        private bool isLandscape;

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewfinderView(Context context, Rect frameRect) : base(context)
        {
            this.context = context;
            this.frameRect = frameRect;
            this.isLandscape = IsLandscape(context);
            this.paint = new Paint(Android.Graphics.PaintFlags.AntiAlias);
        }

        /// <summary>
        /// Obtain Orientation
        /// </summary>
        public static bool IsLandscape(Context context)
        {
            Android.Content.Res.Orientation orientation = context.Resources.Configuration.Orientation;
            return orientation == Android.Content.Res.Orientation.Landscape;
        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="canvas"></param>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            Log.Info(Tag, "onDraw frameRect = " + frameRect);
            // Scanning border drawing
            // You can also draw other such as scan lines, masks, and draw prompts and other buttons according to your needs
            DrawBoarderFrame(canvas, frameRect);
        }

        /// <summary>
        /// Return false when touch event perform.
        /// </summary>
        public override bool OnTouchEvent(MotionEvent e)
        {
            return false;
        }

        /// <summary>
        /// Draw frame
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="frame"></param>
        private void DrawBoarderFrame(Canvas canvas, Rect frame)
        {
            int lindDrawWidth = context.Resources.GetDimensionPixelSize(Resource.Dimension.mlkit_bcr_line_draw_width);
            int lineWidth = isLandscape ? lindDrawWidth : lindDrawWidth >> 1;
            int roundWidth = (frame.Right - frame.Left) / 12;

            canvas.Save();
            canvas.DrawRect((float)frame.Left, (float)frame.Top, (float)(frame.Left + roundWidth + 1),
                    (float)(frame.Top + lineWidth + 1), this.paint);
            canvas.DrawRect((float)frame.Left, (float)frame.Top, (float)(frame.Left + lineWidth + 1),
                    (float)(frame.Top + roundWidth + 1), this.paint);
            canvas.DrawRect((float)(frame.Right - roundWidth), (float)frame.Top, (float)(frame.Right + 1),
                    (float)(frame.Top + lineWidth + 1), this.paint);
            canvas.DrawRect((float)(frame.Right - lineWidth), (float)frame.Top, (float)(frame.Right + 1),
                    (float)(frame.Top + roundWidth + 1), this.paint);

            canvas.DrawRect((float)frame.Left, (float)(frame.Bottom - lineWidth), (float)(frame.Left + roundWidth + 1),
                    (float)(frame.Bottom + 1), this.paint);
            canvas.DrawRect((float)frame.Left, (float)(frame.Bottom - roundWidth), (float)(frame.Left + lineWidth + 1),
                    (float)(frame.Bottom + 1), this.paint);
            canvas.DrawRect((float)(frame.Right - roundWidth), (float)(frame.Bottom - lineWidth),
                    (float)(frame.Right + 1), (float)(frame.Bottom + 1), this.paint);
            canvas.DrawRect((float)(frame.Right - lineWidth), (float)(frame.Bottom - roundWidth),
                    (float)(frame.Right + 1), (float)(frame.Bottom + 1), this.paint);
            canvas.Restore();
        }

    }
}