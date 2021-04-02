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
    public class LensEnginePreview :ViewGroup
    {
        private const string Tag = "LensEnginePreview";

        private Context mContext;
        protected SurfaceView mSurfaceView;
        private bool mStartRequested;
        private bool mSurfaceAvailable;
        private LensEngine mLensEngine;
        private GraphicOverlay mOverlay;

        public LensEnginePreview(Context context, IAttributeSet attrs) : base(context,attrs)
        {
            this.mContext = context;
            this.mStartRequested = false;
            this.mSurfaceAvailable = false;

            this.mSurfaceView = new SurfaceView(context);
            this.mSurfaceView.Holder.AddCallback(new SurfaceCallback(this));
            this.AddView(this.mSurfaceView);
        }

        public void start(LensEngine lensEngine)
        {
                if (lensEngine == null) 
                {
                    this.stop();
                }

                this.mLensEngine = lensEngine;
                if (this.mLensEngine != null) 
                {
                this.mStartRequested = true;
                this.startIfReady();
                }
        }
        public void start(LensEngine lensEngine, GraphicOverlay overlay)
        {
            this.mOverlay = overlay;
            this.start(lensEngine);
        }

        public void stop()
        {
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Close();
            }
        }
        public void release()
        {
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Release();
                this.mLensEngine = null;
            }
        }
        private void startIfReady()
        {
            if (this.mStartRequested && this.mSurfaceAvailable) {
                this.mLensEngine.Run(this.mSurfaceView.Holder);
                if (this.mOverlay != null)
                {
                    Huawei.Hms.Common.Size.Size size = this.mLensEngine.DisplayDimension;
                    int min = Math.Min(640, 480);
                    int max = Math.Max(640, 480);
                    if (this.isPortraitMode())
                    {
                        // Swap width and height sizes when in portrait, since it will be rotated by 90 degrees.
                        this.mOverlay.SetCameraInfo(min, max, this.mLensEngine.LensType);
                    }
                    else
                    {
                        this.mOverlay.SetCameraInfo(max, min, this.mLensEngine.LensType);
                    }
                    this.mOverlay.Clear();
                }
                this.mStartRequested = false;
            }
        }
        private class SurfaceCallback : Java.Lang.Object, ISurfaceHolderCallback
        {
            private LensEnginePreview lensEnginePreview;
            public SurfaceCallback(LensEnginePreview LensEnginePreview)
            {
                this.lensEnginePreview = LensEnginePreview;
            }
            public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
            {
                
            }

            public void SurfaceCreated(ISurfaceHolder holder)
            {
                this.lensEnginePreview.mSurfaceAvailable = true;
                try
                {
                    this.lensEnginePreview.startIfReady();
                }
                catch (Exception e)
                {
                    Log.Info(LensEnginePreview.Tag, "Could not start camera source.", e);
                }
            }

            public void SurfaceDestroyed(ISurfaceHolder holder)
            {
                this.lensEnginePreview.mSurfaceAvailable = false;
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int previewWidth = 480;
            int previewHeight = 360;
            if (this.mLensEngine != null)
            {
                Huawei.Hms.Common.Size.Size size = this.mLensEngine.DisplayDimension;
                if (size != null)
                {
                    previewWidth = 640;
                    previewHeight = 480;
                }
            }

            // Swap width and height sizes when in portrait, since it will be rotated 90 degrees
            if (this.isPortraitMode())
            {
                int tmp = previewWidth;
                previewWidth = previewHeight;
                previewHeight = tmp;
            }

             int viewWidth = r - l;
             int viewHeight = b - t;

            int childWidth;
            int childHeight;
            int childXOffset = 0;
            int childYOffset = 0;
            float widthRatio = (float)viewWidth / (float)previewWidth;
            float heightRatio = (float)viewHeight / (float)previewHeight;

            // To fill the view with the camera preview, while also preserving the correct aspect ratio,
            // it is usually necessary to slightly oversize the child and to crop off portions along one
            // of the dimensions. We scale up based on the dimension requiring the most correction, and
            // compute a crop offset for the other dimension.
            if (widthRatio > heightRatio)
            {
                childWidth = viewWidth;
                childHeight = (int)((float)previewHeight * widthRatio);
                childYOffset = (childHeight - viewHeight) / 2;
            }
            else
            {
                childWidth = (int)((float)previewWidth * heightRatio);
                childHeight = viewHeight;
                childXOffset = (childWidth - viewWidth) / 2;
            }

            for (int i = 0; i < this.ChildCount; ++i)
            {
                // One dimension will be cropped. We shift child over or up by this offset and adjust
                // the size to maintain the proper aspect ratio.
                this.GetChildAt(i).Layout(-1 * childXOffset, -1 * childYOffset, childWidth - childXOffset,
                    childHeight - childYOffset);
            }

            try
            {
                this.startIfReady();
            }
            catch (Exception e)
            {
                Log.Info(LensEnginePreview.Tag, "Could not start camera source.", e);
            }
        }
        private bool isPortraitMode()
        {
            return true;
        }

    }
    
    
}