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
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Classification;
using Huawei.Hms.Mlsdk.Common;
using Exception = System.Exception;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.Classification
{
    [Activity(Label = "ImageClassificationAnalyseActivity")]
    public class ImageClassificationAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "ImageClassificationAnalyseActivity";

        private TextView mTextView;
        private MLImageClassificationAnalyzer analyzer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_classification_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.classification_result);
            this.FindViewById(Resource.Id.classification_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.remote_classification_detect).SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.remote_classification_detect:
                    RemoteAnalyzer();
                    break;
                case Resource.Id.classification_detect:
                    LocalAnalyzer();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Image classification on the device
        /// </summary>
        private async void LocalAnalyzer()
        {
            // Use customized parameter settings for device-based recognition.
            MLLocalClassificationAnalyzerSetting deviceSetting = 
                new MLLocalClassificationAnalyzerSetting.Factory().SetMinAcceptablePossibility(0.8f).Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetLocalImageClassificationAnalyzer(deviceSetting);
            // Create an MLFrame by using Android.Graphics.Bitmap.
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.classification_image);
            MLFrame frame = MLFrame.FromBitmap(bitmap);

            Task<IList<MLImageClassification>> taskLocalAnalyzer = this.analyzer.AnalyseFrameAsync(frame);

            try
            {
                await taskLocalAnalyzer;

                if(taskLocalAnalyzer.IsCompleted && taskLocalAnalyzer.Result != null)
                {
                    //Analyze success
                    var classifications = taskLocalAnalyzer.Result;
                    DisplaySuccess(classifications);
                }
                else
                {
                    //Analyze failure
                    Log.Debug(Tag, " Local analyze failed");
                }
            }
            catch(Exception e)
            {
                //Operation failed
                DisplayFailure(e);
            }
        }

        /// <summary>
        /// Image classification analyzer on the cloud. If you want to use cloud image classification analyzer,
        /// you need to apply for an agconnect-services.json file 
        /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
        /// add agconnect-services.json to Assets folder in the project.
        /// </summary>
        private async void RemoteAnalyzer()
        {
            // Use customized parameter settings for device-based recognition.
            MLRemoteClassificationAnalyzerSetting cloudSetting =
                new MLRemoteClassificationAnalyzerSetting.Factory().SetMinAcceptablePossibility(0.8f).Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetRemoteImageClassificationAnalyzer(cloudSetting);
            // Create an MLFrame by using the bitmap.
            Bitmap bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.classification_image);
            MLFrame frame = MLFrame.FromBitmap(bitmap);
            Task<IList<MLImageClassification>> taskRemoteAnalyzer = this.analyzer.AnalyseFrameAsync(frame);

            try
            {
                await taskRemoteAnalyzer;

                if (taskRemoteAnalyzer.IsCompleted && taskRemoteAnalyzer.Result != null)
                {
                    //Analyze success
                    var classifications = taskRemoteAnalyzer.Result;
                    DisplaySuccess(classifications);
                }
                else
                {
                    //Analyze failure
                    Log.Debug(Tag, " Local analyze failed");
                }
            }
            catch (Exception e)
            {
                //Operation failed
                DisplayFailure(e);
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
            catch (System.Exception e)
            {
                error += e.Message;
            }
            this.mTextView.Text = error;
        }

        /// <summary>
        /// Displays result in TextView
        /// for remote analyze
        /// </summary>
        private void DisplaySuccess(IList<MLImageClassification> classifications)
        {
            string result = "";
            int count = 0;
            foreach (MLImageClassification classification in classifications)
            {
                count++;
                if (count % 3 == 0)
                {
                    result += classification.Name + "\n";
                }
                else
                {
                    result += classification.Name + "\t\t\t\t\t\t";
                }
            }
            this.mTextView.Text = result;
        }

        /// <summary>
        /// Stop recognizer on OnDestroy() event.
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
                Log.Info(ImageClassificationAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }
    }
}