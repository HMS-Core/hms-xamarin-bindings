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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Text;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.Text
{
    [Activity(Label = "ImageTextAnalyseActivity")]
    public class ImageTextAnalyseActivity : Activity, View.IOnClickListener
    {
        private const string Tag = "ImageTextAnalyseActivity";

        private TextView mTextView;
        private MLTextAnalyzer analyzer;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_text_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.text_result);
            this.FindViewById(Resource.Id.text_detect).SetOnClickListener(this);
            this.FindViewById(Resource.Id.remote_text_detect).SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.text_detect:
                    LocalAnalyzer();
                    break;
                case Resource.Id.remote_text_detect:
                    RemoteAnalyzer();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Text recognition on the device
        /// </summary>
        private async void LocalAnalyzer()
        {
            // Create the text analyzer MLTextAnalyzer to recognize characters in images. You can set MLLocalTextSetting to
            // specify languages that can be recognized.
            // If you do not set the languages, only Romance languages can be recognized by default.
            // Use default parameter settings to configure the on-device text analyzer. Only Romance languages can be
            // recognized.
            // analyzer = MLAnalyzerFactory.Instance.LocalTextAnalyzer;
            // Use the customized parameter MLLocalTextSetting to configure the text analyzer on the device.
            MLLocalTextSetting setting = new MLLocalTextSetting.Factory()
                                            .SetOCRMode(MLLocalTextSetting.OcrDetectMode)
                                            .SetLanguage("en")
                                            .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetLocalTextAnalyzer(setting);
            // Create an MLFrame by using android.graphics.Bitmap.
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.text_image);
            MLFrame frame = MLFrame.FromBitmap(bitmap);

            Task<MLText> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    var result = task.Result;
                    this.DisplaySuccess(result);
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
                Log.Info(Tag, " Operation failure: " + e.Message);
                this.DisplayFailure(e);
            }
        }
        /// <summary>
        /// Text recognition on the cloud. If you want to use cloud text analyzer,
        /// you need to apply for an agconnect-services.json file 
        /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
        /// add agconnect-services.json to Assets folder in the project.
        /// </summary>
        private async void RemoteAnalyzer()
        {
            // Set the list of languages to be recognized.
            IList<string> languageList = new List<string>();
            languageList.Add("zh");
            languageList.Add("en");
            // Create an analyzer. You can customize the analyzer by creating MLRemoteTextSetting
            MLRemoteTextSetting setting =
                new MLRemoteTextSetting.Factory()
                        .SetTextDensityScene(MLRemoteTextSetting.OcrCompactScene)
                        .SetLanguageList(languageList)
                        .SetBorderType(MLRemoteTextSetting.Arc)
                        .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetRemoteTextAnalyzer(setting);
            // Use default parameter settings.
            //analyzer = MLAnalyzerFactory.Instance.RemoteTextAnalyzer;

            // Create an MLFrame by using Android.Graphics.Bitmap.
            Bitmap bitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.text_image);
            MLFrame frame = MLFrame.FromBitmap(bitmap);

            Task<MLText> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    var result = task.Result;
                    this.RemoteDisplaySuccess(result);
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
                Log.Info(Tag, " Operation failure: " + e.Message);
                this.DisplayFailure(e);
            }
        }

        private void DisplayFailure()
        {
            this.mTextView.Text = "Failure";
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
        /// for remote analyze
        /// </summary>
        private void RemoteDisplaySuccess(MLText mlTexts)
        {
            string result = "Remote Analyze:"+ "\n\n";
            IList<MLText.Block> blocks = mlTexts.Blocks;
            foreach (MLText.Block block in blocks)
            {
                IList<MLText.Base> lines = block.Contents;
                foreach (MLText.TextLine line in lines)
                {
                    IList<MLText.Base> words = line.Contents;
                    foreach (MLText.Base word in words)
                    {
                        result += word.StringValue + " ";
                    }
                }
                result += "\n";
            }
            this.mTextView.Text = result;
        }

        /// <summary>
        /// Display result in TextView
        /// for local analyze
        /// </summary>
        private void DisplaySuccess(MLText mlText)
        {
            string result = "Local Analyze:" + "\n\n";
            IList<MLText.Block> blocks = mlText.Blocks;
            foreach (MLText.Block block in blocks)
            {
                foreach (MLText.TextLine line in block.Contents)
                {
                    result += line.StringValue + "\n";
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
                Log.Info(ImageTextAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }

    }
}