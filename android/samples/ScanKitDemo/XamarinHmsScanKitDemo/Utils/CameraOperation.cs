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
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Hardware;
using Android.OS;
using Android.Views;

namespace XamarinHmsScanKitDemo.Utils
{
    public class CameraOperation
    {
        private Camera camera = null;
        private Camera.Parameters parameters = null;
        private bool isPreview = false;
        private FrameCallback frameCallback = new FrameCallback();
        private int width = 1920;
        private int height = 1080;
        private double defaultZoom = 1.0;
        internal void Close()
        {
            if (camera != null)
            {
                camera.Release();
                camera = null;
            }
        }

        internal void Open(ISurfaceHolder surfaceHolder)
        {
            camera = Camera.Open();
            parameters = camera.GetParameters();
            parameters.SetPictureSize(width, height);
            parameters.FocusMode = Camera.Parameters.FocusModeContinuousPicture;
            parameters.PictureFormat = Android.Graphics.ImageFormatType.Nv21;
            camera.SetPreviewDisplay(surfaceHolder);
            camera.SetDisplayOrientation(90);
            camera.SetParameters(parameters);
        }

        public void StartPreview()
        {
            if (camera != null && !isPreview)
            {
                camera.StartPreview();
                isPreview = true;
            }
        }

        public void StopPreview()
        {
            if (camera != null && isPreview)
            {
                camera.StopPreview();
                frameCallback.SetProperties(null);
                isPreview = false;
            }
        }

        public void CallbackFrame(Handler handler, double zoomValue)
        {
            if (camera != null && isPreview)
            {
                frameCallback.SetProperties(handler);
                if (camera.GetParameters().IsZoomSupported && zoomValue != defaultZoom)
                {
                    //Auto zoom.
                    parameters.Zoom = ConvertZoomInt(zoomValue);
                    camera.SetParameters(parameters);
                }
                camera.SetOneShotPreviewCallback(frameCallback);
            }
        }

        public int ConvertZoomInt(double zoomValue)
        {
            List<Java.Lang.Integer> allZoomRatios = parameters.ZoomRatios.ToList();
            double maxZoom = Math.Round(Convert.ToDouble(allZoomRatios[allZoomRatios.Count - 1]) / 100);
            if (zoomValue >= maxZoom)
            {
                return allZoomRatios.Count - 1;
            }
            for (int i = 1; i < allZoomRatios.Count; i++)
            {
                if (Convert.ToInt32(allZoomRatios[i]) >= (zoomValue * 100) && Convert.ToInt32(allZoomRatios[i - 1]) <= (zoomValue * 100))
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public class FrameCallback : Java.Lang.Object, Camera.IPreviewCallback
    {
        private Handler handler;
        public void OnPreviewFrame(byte[] data, Camera camera)
        {
            if (handler != null)
            {
                Message message = handler.ObtainMessage(0, camera.GetParameters().PreviewSize.Width,
                        camera.GetParameters().PreviewSize.Height, data);
                message.SendToTarget();
                handler = null;
            }
        }

        internal void SetProperties(Handler handler)
        {
            this.handler = handler;
        }
    }
}