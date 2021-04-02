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
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Landmark;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.Landmark
{
    /// <summary>
    /// If you want to use landmark analyzer,
    /// you need to apply for an agconnect-services.json file 
    /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
    /// add agconnect-services.json to Assets folder in the project.
    /// </summary>
    [Activity(Label = "ImageLandmarkAnalyseActivity")]
    public class ImageLandmarkAnalyseActivity : Activity, View.IOnClickListener
    {
        private const string Tag = "ImageLandmarkAnalyseActivity";
        private readonly int PhotoRequestCode = 0;

        private TextView mTextView;
        private ImageView mImageView;
        private Bitmap mBitmap;
        private MLRemoteLandmarkAnalyzer analyzer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_landmark_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.landmark_result);
            this.mImageView = (ImageView)this.FindViewById(Resource.Id.landmark_image);
            this.FindViewById(Resource.Id.landmark_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.change_photo).SetOnClickListener(this);
            //Set default image
            this.mBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.landmark_image);
            mImageView.SetImageBitmap(mBitmap);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.landmark_detect:
                    this.RemoteAnalyzer();
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

        private async void RemoteAnalyzer()
        {
            /* Create a landmark analyzer.
             * For use default parameter settings.
             * analyzer = MLAnalyzerFactory.Instance.RemoteLandmarkAnalyzer;
             ////
             * For use customized parameter settings.
             * SetLargestNumOfReturns: maximum number of recognition results.
             * SetPatternType: analyzer mode.
             * MLRemoteLandmarkAnalyzerSetting.SteadyPattern: The value 1 indicates the stable mode.
             * MLRemoteLandmarkAnalyzerSetting.NewestPattern: The value 2 indicates the latest mode.
             */
            MLRemoteLandmarkAnalyzerSetting setting = new MLRemoteLandmarkAnalyzerSetting.Factory()
                                            .SetLargestNumOfReturns(1)
                                            .SetPatternType(MLRemoteLandmarkAnalyzerSetting.SteadyPattern)
                                            .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetRemoteLandmarkAnalyzer(setting);
            // Create an MLFrame by using android.graphics.Bitmap. Recommended image size: large than 640*640.
            MLFrame mlFrame = new MLFrame.Creator().SetBitmap(this.mBitmap).Create();
            Task<IList<MLRemoteLandmark>> task = this.analyzer.AnalyseFrameAsync(mlFrame);

            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    var landmark = task.Result;
                    DisplaySuccess(landmark);

                }
                else
                {
                    // Analyze failure.
                    Log.Info(Tag, " Analyze failure ");
                }
            }
            catch (Exception e)
            {
                // Operation failure.
                DisplayFailure(e);
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
                    this.mBitmap = (Bitmap)data.Extras.Get("data");
                    mImageView.SetImageBitmap(this.mBitmap);
                    //clean textView for new image analyzing
                    mTextView.Text = "";
                }
            }
        }

        /// <summary>
        /// When analyze failed,
        /// displays exception message in TextView
        /// </summary>
        private void DisplayFailure(Exception exception)
        {
            string error = "Failure. ";
            try
            {
                MLException mlException = (MLException)exception;
                error += "error code: " + mlException.ErrCode + "\n" + "error message: " + mlException.Message;
            }
            catch (Exception e)
            {
                error += e.Message;
            }
            this.mTextView.Text = error;
        }

        /// <summary>
        /// Display result in TextView
        /// </summary>
        private void DisplaySuccess(IList<MLRemoteLandmark> landmark)
        {
            string result = "";
            foreach (MLRemoteLandmark remoteLandmark in landmark)
            {
                if (remoteLandmark.Landmark != null)
                {
                    result = "Landmark: " + remoteLandmark.Landmark;
                }
                result += "\nPositions: ";
                if (remoteLandmark.PositionInfos != null)
                {
                    foreach (MLCoordinate coordinate in remoteLandmark.PositionInfos)
                    {
                        result += "\nLatitude:" + coordinate.Lat;
                        result += "\nLongitude:" + coordinate.Lng;
                    }
                }
            }

            this.mTextView.Text = result;
        }

        /// <summary>
        /// Stop analyzer on OnDestroy() event.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer == null)
            {
                return;
            }
            try
            {
                this.analyzer.Stop();
            }
            catch (Exception e)
            {
                Log.Info(ImageLandmarkAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }

    }
}