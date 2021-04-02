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
using System.IO;
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
using Huawei.Hms.Mlsdk.Dsc;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.DocumentSkewCorrection
{
    [Activity(Label = "DocumentSkewCorrectionActivity")]
    public class DocumentSkewCorrectionActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "DocumentSkewCorrectionActivity";

        private MLDocumentSkewCorrectionAnalyzer analyzer;
        private ImageView mImageView;
        private Bitmap bitmap;
        private MLDocumentSkewCorrectionCoordinateInput input;
        private MLFrame mlFrame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_document_skew_correction);
            this.FindViewById(Resource.Id.image_refine).SetOnClickListener(this);
            this.mImageView = (ImageView)this.FindViewById(Resource.Id.image_refine_result);
        }

        public void OnClick(View v)
        {
            this.Analyze();
        }

        /// <summary>
        /// Perform Document Skew Correction operation
        /// </summary>
        private async void Analyze()
        {
            // Create the setting.
            MLDocumentSkewCorrectionAnalyzerSetting setting = new MLDocumentSkewCorrectionAnalyzerSetting
                    .Factory()
                    .Create();

            // Get the analyzer.
            this.analyzer = MLDocumentSkewCorrectionAnalyzerFactory.Instance.GetDocumentSkewCorrectionAnalyzer(setting);

            // Create the bitmap.
            this.bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.document_correct_image);

            // Create a MLFrame by using the bitmap.
            this.mlFrame = new MLFrame.Creator().SetBitmap(this.bitmap).Create();

            Task<MLDocumentSkewDetectResult> detectTask = this.analyzer.DocumentSkewDetectAsync(this.mlFrame);

            try
            {
                await detectTask;

                if(detectTask.IsCompleted && detectTask.Result != null)
                {
                    // Analyze success.
                    var detectResult = detectTask.Result;
                    Point leftTop = detectResult.LeftTopPosition;
                    Point rightTop = detectResult.RightTopPosition;
                    Point leftBottom = detectResult.LeftBottomPosition;
                    Point rightBottom = detectResult.RightBottomPosition;

                    IList<Point> coordinates = new List<Point>();
                    coordinates.Add(leftTop);
                    coordinates.Add(rightTop);
                    coordinates.Add(leftBottom);
                    coordinates.Add(rightBottom);

                    this.SetDetectData(new MLDocumentSkewCorrectionCoordinateInput(coordinates));
                    this.RefineImg();
                }
                else
                {
                    // Analyze failure.
                    this.DisplayFailure();
                }
            }
            catch(Exception e)
            {
                // Operation failure.
                Log.Info(Tag, " Operation failure: " + e.Message );
                this.DisplayFailure();
            }
        }

        /// <summary>
        /// When analyze success,
        /// displays result.
        /// </summary>
        private void DisplaySuccess(MLDocumentSkewCorrectionResult refineResult)
        {
            if (this.bitmap == null)
            {
                this.DisplayFailure();
                return;
            }
            // Draw the portrait with a transparent background.
            Bitmap corrected = refineResult.Corrected;
            if (corrected != null)
            {
                this.mImageView.SetImageBitmap(corrected);
            }
            else
            {
                this.DisplayFailure();
            }
        }
        /// <summary>
        /// When analyze failed,
        /// displays exception message in TextView
        /// </summary>
        private void DisplayFailure()
        {
            Toast.MakeText(this.ApplicationContext, "Fail", ToastLength.Short).Show();
        }
        /// <summary>
        /// Set CoordinateInput object.
        /// </summary>
        /// <param name="input"></param>
        private void SetDetectData(MLDocumentSkewCorrectionCoordinateInput input)
        {
            this.input = input;
        }

        /// <summary>
        /// Refine image
        /// </summary>
        private async void RefineImg()
        {
            Task<MLDocumentSkewCorrectionResult> correctTask = this.analyzer.DocumentSkewCorrectAsync(this.mlFrame, this.input);

            try
            {
                await correctTask;

                if (correctTask.IsCompleted && correctTask.Result != null)
                {
                    // Analyze success.
                    var refineResult = correctTask.Result;
                    this.DisplaySuccess(refineResult);
                }
                else
                {
                    // Analyze failure.
                    this.DisplayFailure();
                }
            }
            catch (Exception e)
            {
                // Operation failure.
                Log.Info(Tag, " Refine operation failure: " + e.Message);
                this.DisplayFailure();
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer != null)
            {
                try
                {
                    this.analyzer.Stop();
                }
                catch (Exception e)
                {
                    Log.Info(DocumentSkewCorrectionActivity.Tag, "Stop failed: " + e.Message);
                }
            }
        }
    }
}