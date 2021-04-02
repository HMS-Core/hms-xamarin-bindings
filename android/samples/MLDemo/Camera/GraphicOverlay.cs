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
using Huawei.Hms.Mlsdk.Common;

namespace HmsXamarinMLDemo.Camera
{
    /**
     * A view which renders a series of custom graphics to be overlayed on top of an associated preview
     * (i.e., the camera preview). The creator can add graphics objects, update the objects, and remove
     * them, triggering the appropriate drawing and invalidation within the view.
     * 
     * Supports scaling and mirroring of the graphics relative to the camera's preview properties. The
     * idea is that detection items are expressed in terms of a preview size, but need to be scaled up
     * to the full view size, and also mirrored in the case of the front-facing camera.
     * 
     * Associated {Graphic} items should use the following methods to convert to view coordinates
     * for the graphics that are drawn:
     * 
     * {Graphic#ScaleX(float)} and {Graphic#ScaleY(float)} adjust the size of the
     * supplied value from the preview scale to the view scale.
     * {Graphic#TranslateX(float)} and {Graphic#TranslateY(float)} adjust the coordinate
     * from the preview's coordinate system to the view coordinate system.
     */
    public class GraphicOverlay : View
    {
        private readonly object mLock = new object();
        public int mPreviewWidth;
        public float mWidthScaleFactor = 1.0f;
        public int mPreviewHeight;
        public float mHeightScaleFactor = 1.0f;
        public int mFacing = LensEngine.BackLens;
        private HashSet<Graphic> mGraphics = new HashSet<Graphic>();

        public GraphicOverlay(Context context, IAttributeSet attrs) : base(context,attrs)
        {
            
        }

        /// <summary>
        /// Removes all graphics from the overlay.
        /// </summary>
        public void Clear()
        {
            lock(mLock) {
                mGraphics.Clear();
            }
            PostInvalidate();
        }

        /// <summary>
        /// Adds a graphic to the overlay.
        /// </summary>
        public void Add(Graphic graphic)
        {
            lock(mLock) {
                mGraphics.Add(graphic);
            }
            PostInvalidate();
        }

        /// <summary>
        /// Removes a graphic from the overlay.
        /// </summary>
        public void Remove(Graphic graphic)
        {
            lock(mLock) 
            {
                mGraphics.Remove(graphic);
            }
            PostInvalidate();
        }

        /// <summary>
        /// Sets the camera attributes for size and facing direction, which informs how to transform image coordinates later.
        /// </summary>
        public void SetCameraInfo(int previewWidth, int previewHeight, int facing)
        {
            lock(mLock) {
                mPreviewWidth = previewWidth;
                mPreviewHeight = previewHeight;
                mFacing = facing;
            }
            PostInvalidate();
        }

        /// <summary>
        /// Draws the overlay with its associated graphic objects.
        /// </summary>
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            lock (mLock)
            {
                if ((mPreviewWidth != 0) && (mPreviewHeight != 0))
                {
                    mWidthScaleFactor = (float)canvas.Width / (float)mPreviewWidth;
                    mHeightScaleFactor = (float)canvas.Height / (float)mPreviewHeight;
                }

                foreach (Graphic graphic in mGraphics)
                {
                    graphic.Draw(canvas);
                }
            }
        }
        
    }
    /// <summary>
    /// Base class for a custom graphics object to be rendered within the graphic overlay. Subclass
    /// this and implement the {Graphic#Draw(Canvas)} method to define the
    /// graphics element. Add instances to the overlay using {GraphicOverlay#Add(Graphic)}.
    /// </summary>
    public abstract class Graphic
    {
        private GraphicOverlay mOverlay;

        public Graphic(GraphicOverlay overlay)
        {
            mOverlay = overlay;
        }

        /// <summary>
        /// Draw the graphic on the supplied canvas. Drawing should use the following methods to
        /// convert to view coordinates for the graphics that are drawn:
        /// <ol>
        /// <li>{Graphic#ScaleX(float)} and {Graphic#ScaleY(float)} adjust the size of
        /// the supplied value from the preview scale to the view scale.</li>
        /// <li>{Graphic#TranslateX(float)} and {Graphic#TranslateY(float)} adjust the
        /// coordinate from the preview's coordinate system to the view coordinate system.</li>
        /// </ ol >param canvas drawing canvas
        /// </summary>
        /// <param name="canvas"></param>
        public abstract void Draw(Canvas canvas);

        /// <summary>
        /// Adjusts a horizontal value of the supplied value from the preview scale to the view
        /// scale.
        /// </summary>
        public float ScaleX(float horizontal)
        {
            return horizontal * mOverlay.mWidthScaleFactor;
        }

        public float UnScaleX(float horizontal)
        {
            return horizontal / mOverlay.mWidthScaleFactor;
        }

        /// <summary>
        /// Adjusts a vertical value of the supplied value from the preview scale to the view scale.
        /// </summary>
        public float ScaleY(float vertical)
        {
            return vertical * mOverlay.mHeightScaleFactor;
        }

        public float UnScaleY(float vertical) { return vertical / mOverlay.mHeightScaleFactor; }

        /// <summary>
        /// Adjusts the x coordinate from the preview's coordinate system to the view coordinate system.
        /// </summary>
        public float TranslateX(float x)
        {
            if (mOverlay.mFacing == LensEngine.FrontLens)
            {
                return mOverlay.Width - ScaleX(x);
            }
            else
            {
                return ScaleX(x);
            }
        }

        /// <summary>
        /// Adjusts the y coordinate from the preview's coordinate system to the view coordinate system.
        /// </summary>
        public float TranslateY(float y)
        {
            return ScaleY(y);
        }

        public void PostInvalidate()
        {
            this.mOverlay.PostInvalidate();
        }
    }

}