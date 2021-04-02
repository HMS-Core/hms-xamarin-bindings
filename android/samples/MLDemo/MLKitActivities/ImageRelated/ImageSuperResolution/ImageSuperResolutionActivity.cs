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
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Imagesuperresolution;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSuperResolution
{
    [Activity(Label = "ImageSuperResolutionActivity")]
    public class ImageSuperResolutionActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "ImageSuperResolutionActivity";

        private static readonly int Index3x = 0;
        private static readonly int IndexOriginal = 1;
        private MLImageSuperResolutionAnalyzer analyzer;
        private ImageView imageView;
        private Bitmap srcBitmap;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_super_resolution);
            this.imageView = (ImageView)this.FindViewById(Resource.Id.image);
            FindViewById(Resource.Id.button_1x).SetOnClickListener(this);
            FindViewById(Resource.Id.button_original).SetOnClickListener(this);

            this.srcBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.superresolution_image);
            CreateAnalyzer();
            DetectImage(Index3x);
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.button_1x)
            {
                DetectImage(Index3x);
            }
            else if (v.Id == Resource.Id.button_original)
            {
                DetectImage(IndexOriginal);
            }
        }

        /// <summary>
        /// Detect Image
        /// </summary>
        private async void DetectImage(int type)
        {
            if (type == IndexOriginal)
            {
                imageView.SetImageBitmap(srcBitmap);
                return;
            }

            if (analyzer == null)
            {
                return;
            }

            // Create an MLFrame by using the bitmap.
            MLFrame frame = MLFrame.FromBitmap(srcBitmap);
            Task<MLImageSuperResolutionResult> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    Toast.MakeText(ApplicationContext, "Success", ToastLength.Short).Show();
                    var result = task.Result;
                    SetImage(result.Bitmap);

                }
                else
                {
                    // Analyze failure.
                    Toast.MakeText(ApplicationContext, "Failed", ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                // Operation failure.
                Log.Info(Tag, " Operation failure: " + e.Message);
                Toast.MakeText(ApplicationContext, "Failed：" + e.Message, ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Set image to imageView
        /// </summary>
        private void SetImage(Bitmap bitmap)
        {
            try
            {
                imageView.SetImageBitmap(bitmap);
            }
            catch(Exception e)
            {
                Log.Info(Tag, " setImage failure: " + e.Message);
            }
        }
        /// <summary>
        /// Create analyzer
        /// </summary>
        private void CreateAnalyzer()
        {
            // Method 1: use the default setting, that is, 1x image super resulotion.
            // analyzer = MLImageSuperResolutionAnalyzerFactory.Instance.ImageSuperResolutionAnalyzer;
            // Method 2: using the custom setting.
            MLImageSuperResolutionAnalyzerSetting settings = new MLImageSuperResolutionAnalyzerSetting.Factory()
                    // Set the scale of image super resolution to 1x.
                    .SetScale(MLImageSuperResolutionAnalyzerSetting.IsrScale3x)
                    .Create();
            analyzer = MLImageSuperResolutionAnalyzerFactory.Instance.GetImageSuperResolutionAnalyzer(settings);
        }

        /// <summary>
        /// Release analyzer sources.
        /// </summary>
        private void Release()
        {
            if (analyzer == null)
            {
                return;
            }
            analyzer.Stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (srcBitmap != null)
            {
                srcBitmap.Recycle();
            }
            Release();
        }
    }
}