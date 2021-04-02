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
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Imgseg;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSegmentation
{
    [Activity(Label = "ImageSegmentationStillAnalyseActivity")]
    public class ImageSegmentationStillAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "ImageSegmentationStillAnalyseActivity";
        private readonly int PhotoRequestCode = 0;

        private MLImageSegmentationAnalyzer analyzer;
        private ImageView mImageResultView;
        private ImageView mImageView;
        private Bitmap mBitmapTarget;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_segmentation_analyse);
            this.FindViewById(Resource.Id.segment_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.change_photo).SetOnClickListener(this);
            this.mImageView = (Android.Widget.ImageView)this.FindViewById(Resource.Id.image_foreground);
            this.mImageResultView = (Android.Widget.ImageView)this.FindViewById(Resource.Id.image_result);
            //Set default image
            this.mBitmapTarget = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.imgseg_foreground);
            mImageView.SetImageBitmap(mBitmapTarget);
        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.segment_detect:
                    this.Analyze();
                    break;
                case Resource.Id.change_photo:
                    this.ChangePhoto();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Change the target photo
        /// </summary>
        private void ChangePhoto()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            StartActivityForResult(intent, this.PhotoRequestCode);
        }

        private async void Analyze()
        {
            /**
             * Configure image segmentation analyzer with custom parameter MLImageSegmentationSetting.
             *
             * SetExact(): Set the segmentation fine mode, true is the fine segmentation mode,
             *     and false is the speed priority segmentation mode.
             * SetAnalyzerType(): Set the segmentation mode. When segmenting a static image, support setting
             *     MLImageSegmentationSetting.BodySeg (only segment human body and background)
             *     and MLImageSegmentationSetting.ImageSeg (segment 10 categories of scenes, including human bodies)
             * SetScene(): Set the type of the returned results. This configuration takes effect only in
             *     MLImageSegmentationSetting.BodySeg mode. In MLImageSegmentationSetting.ImageSeg mode,
             *     only pixel-level tagging information is returned.
             *     Supports setting MLImageSegmentationScene.All (returns all segmentation results,
             *     including: pixel-level tag information, portrait images with transparent backgrounds
             *     and portraits are white, gray background with black background),
             *     MLImageSegmentationScene.MaskOnly (returns only pixel-level tag information),
             *     MLImageSegmentationScene.ForegroundOnly (returns only portrait images with transparent background),
             *     MLImageSegmentationScene.GrayscaleOnly (returns only grayscale images with white portrait and black background).
             */
            MLImageSegmentationSetting setting = new MLImageSegmentationSetting.Factory()
                    .SetExact(false)
                    .SetAnalyzerType(MLImageSegmentationSetting.BodySeg)
                    .SetScene(MLImageSegmentationScene.All)
                    .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetImageSegmentationAnalyzer(setting);
            // Create an MLFrame by using android.graphics.Bitmap. Recommended image size: large than 224*224.
            MLFrame mlFrame = new MLFrame.Creator().SetBitmap(this.mBitmapTarget).Create();

            Task<MLImageSegmentation> task = this.analyzer.AnalyseFrameAsync(mlFrame);
            try
            {
                await task;
                if(task.IsCompleted && task.Result != null)
                {
                    // Analyze success
                    var imageSegmentationResult = task.Result;
                    this.DisplaySuccess(imageSegmentationResult);
                }
                else
                {
                    // Analyze failure
                    this.DisplayFailure();
                }
            }
            catch(Exception e)
            {
                // Operation failure
                this.DisplayFailure();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            //Take picture activity result
            if (requestCode == this.PhotoRequestCode)
            {
                if (data != null)
                {
                    this.mBitmapTarget = (Bitmap)data.Extras.Get("data");
                    mImageView.SetImageBitmap(this.mBitmapTarget);
                }
            }
        }

        /// <summary>
        /// Display result in TextView
        /// </summary>
        private void DisplaySuccess(MLImageSegmentation imageSegmentationResult)
        {
            if (this.mBitmapTarget == null)
            {
                this.DisplayFailure();
                return;
            }
            // Draw the portrait with a transparent background.
            Bitmap bitmapFore = imageSegmentationResult.Foreground;
            if (bitmapFore != null)
            {
                this.mImageResultView.SetImageBitmap(bitmapFore);
            }
            else
            {
                this.DisplayFailure();
            }
        }
        private void DisplayFailure()
        {
            Toast.MakeText(this.ApplicationContext, "Fail", ToastLength.Short).Show();
        }

        private int[] CutOutHuman(byte[] masks)
        {
            int[] results = new int[this.mBitmapTarget.Width * this.mBitmapTarget.Height];
            this.mBitmapTarget.GetPixels(results, 0, this.mBitmapTarget.Width, 0, 0, this.mBitmapTarget.Width,
                    this.mBitmapTarget.Height);
            for (int i = 0; i < masks.Length; i++)
            {
                if (masks[i] != MLImageSegmentationClassification.TypeHuman)
                {
                    results[i] = Color.White;
                }
            }
            return results;
        }
    }
}