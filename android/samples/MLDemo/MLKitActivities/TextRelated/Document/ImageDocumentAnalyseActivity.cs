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
using System.Runtime.CompilerServices;
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
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Document;
using Huawei.Hms.Mlsdk.Text;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.Document
{
    /// <summary>
    /// If you want to use document analyzer,
    /// you need to apply for an agconnect-services.json file 
    /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
    /// add agconnect-services.json to Assets folder in the project.
    /// </summary>
    [Activity(Label = "ImageDocumentAnalyseActivity")]
    public class ImageDocumentAnalyseActivity : AppCompatActivity , View.IOnClickListener
    {
        private const string Tag = "ImageDocumentAnalyseActivity";

        private TextView mTextView;
        private MLDocumentAnalyzer analyzer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_document_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.document_result);
            this.FindViewById(Resource.Id.document_detect).SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            this.RemoteAnalyzer();
        }

        /// <summary>
        /// Performs remote analyze action
        /// </summary>
        private async void RemoteAnalyzer()
        {
            // Set the list of languages to be recognized.
            IList<string> languageList = new List<string>();
            languageList.Add("zh");
            languageList.Add("en");
            // Create a document analyzer. You can create an analyzer using the provided custom document recognition
            // parameter MLDocumentSetting
            MLDocumentSetting setting = new MLDocumentSetting.Factory()
                                            .SetBorderType(MLRemoteTextSetting.Arc)
                                            .SetLanguageList(languageList)
                                            .Create();           
            this.analyzer = MLAnalyzerFactory.Instance.GetRemoteDocumentAnalyzer(setting);
            // Create a document analyzer that uses the default configuration.
            // analyzer = MLAnalyzerFactory.Instance.RemoteDocumentAnalyzer;
           
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.document_image);
            // Create an MLFrame by using Android.Graphics.Bitmap.
            MLFrame frame = MLFrame.FromBitmap(bitmap);

            // Pass the MLFrame object to the AnalyseFrameAsync method for document recognition.
            Task<MLDocument> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;

                if (task.IsCompleted && task.Result != null)
                {
                    // Analyze success.
                    var document = task.Result;
                    this.DisplaySuccess(document);
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
        private void DisplaySuccess(MLDocument document)
        {
            string result = "";
            IList<MLDocument.Block> blocks = document.Blocks;
            foreach (MLDocument.Block block in blocks)
            {
                IList<MLDocument.Section> sections = block.Sections;
                foreach (MLDocument.Section section in sections)
                {
                    IList<MLDocument.Line> lines = section.LineList;
                    foreach (MLDocument.Line line in lines)
                    {
                        IList<MLDocument.Word> words = line.WordList;
                        foreach (MLDocument.Word word in words)
                        {
                            result += word.StringValue + " ";
                        }
                    }
                }
                result += "\n";
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
                Log.Info(ImageDocumentAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }
    }
}