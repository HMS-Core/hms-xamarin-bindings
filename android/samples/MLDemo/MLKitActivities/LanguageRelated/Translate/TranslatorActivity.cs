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
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Langdetect;
using Huawei.Hms.Mlsdk.Langdetect.Cloud;
using Huawei.Hms.Mlsdk.Langdetect.Local;
using Huawei.Hms.Mlsdk.Model.Download;
using Huawei.Hms.Mlsdk.Translate;
using Huawei.Hms.Mlsdk.Translate.Cloud;
using Huawei.Hms.Mlsdk.Translate.Local;
using Java.IO;

namespace HmsXamarinMLDemo.MLKitActivities.LanguageRelated.Translate
{
    [Activity(Label = "TranslatorActivity")]
    public class TranslatorActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "TranslatorActivity";

        private TextView mTextView;
        private EditText mEditText;

        private MLRemoteTranslator remoteTranslator;
        private MLRemoteLangDetector remoteLangDetector;
        private MLLocalLangDetector localLangDetector;
        private MLLocalTranslator localTranslator;
        private MLLocalModelManager manager;

        private static IDictionary<string, string> nationAndCode = new Dictionary<string, string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_language_detection_translation);
            this.mTextView = (Android.Widget.TextView)this.FindViewById(Resource.Id.tv_output);
            this.mEditText = (Android.Widget.EditText)this.FindViewById(Resource.Id.et_input);
            this.FindViewById(Resource.Id.btn_local_translator).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_local_detector).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_remote_translator).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_remote_detector).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_delete_model).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_download_model).SetOnClickListener(this);
            TranslatorActivity.nationAndCode = this.ReadNationAndCode();
            manager = MLLocalModelManager.Instance;
        }
        /// <summary>
        /// Translation on the cloud. If you want to use cloud remoteTranslator,
        /// you need to apply for an agconnect-services.json file 
        /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
        /// add agconnect-services.json to Assets folder in the project.
        /// </summary>
        private async void RemoteTranslator()
        {
            // Create an analyzer. You can customize the analyzer by creating MLRemoteTranslateSetting
            MLRemoteTranslateSetting setting =
                new MLRemoteTranslateSetting.Factory().SetTargetLangCode("zh").Create();
            this.remoteTranslator = MLTranslatorFactory.Instance.GetRemoteTranslator(setting);
            // Use default parameter settings.
            // analyzer = MLTranslatorFactory.Instance.RemoteTranslator;
            // Read text in edit box.
            string sourceText = this.mEditText.Text.ToString();
            Task<string> translateTask = this.remoteTranslator.TranslateAsync(sourceText);

            try
            {
                await translateTask;

                if (translateTask.IsCompleted && translateTask.Result != null)
                {
                    // Translate success.
                    var detectResult = translateTask.Result;
                    this.DisplaySuccess(detectResult, true);
                }
                else
                {
                    // Translate failure.
                    Log.Info(Tag, " Translate failure ");
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
        /// Text translation on the device.
        /// </summary>
        private async void LocalTranslator()
        {
            // Create an offline translator.
            MLLocalTranslateSetting setting = new MLLocalTranslateSetting
                    .Factory()
                    // Set the source language code. The ISO 639-1 standard is used. This parameter is mandatory. If this parameter is not set, an error may occur.
                    .SetSourceLangCode("en")
                    // Set the target language code. The ISO 639-1 standard is used. This parameter is mandatory. If this parameter is not set, an error may occur.
                    .SetTargetLangCode("zh")
                    .Create();
            this.localTranslator = MLTranslatorFactory.Instance.GetLocalTranslator(setting);

            // Download the offline model required for local offline translation.
            // Obtain the model manager.
            MLModelDownloadStrategy downloadStrategy = new MLModelDownloadStrategy.Factory()
                                                // It is recommended that you download the package in a Wi-Fi environment.
                                                .NeedWifi()
                                                .SetRegion(MLModelDownloadStrategy.RegionDrEurope)
                                                .Create();
            Task downloadTask = this.localTranslator.PreparedModelAsync(downloadStrategy);

            try
            {
                await downloadTask;

                if (downloadTask.IsCompleted)
                {
                    // Download success.
                    string input = mEditText.Text.ToString();
                    Task<string> translateTask = this.localTranslator.TranslateAsync(input);

                    try
                    {
                        await translateTask;

                        if (translateTask.IsCompleted && translateTask.Result != null)
                        {
                            // Translate success.
                            var detectResult = translateTask.Result;
                            this.DisplaySuccess(detectResult, true);
                        }
                        else
                        {
                            // Translate failure.
                            Log.Info(Tag, " Translate failure ");
                        }
                    }
                    catch (Exception e)
                    {
                        // Operation failure.
                        Log.Info(Tag, " Local translate operation failure: " + e.Message);
                        this.DisplayFailure(e);
                    }
                }
                else
                {
                    // Download failure.
                    Log.Info(Tag, " Download failure ");
                }
            }
            catch (Exception e)
            {
                // Operation failure.
                Log.Info(Tag, " Download operation failure: " + e.Message);
                this.DisplayFailure(e);
            }

        }

        /// <summary>
        /// Language detection on the cloud. If you want to use cloud language detector,
        /// you need to apply for an agconnect-services.json file 
        /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
        /// add agconnect-services.json to Assets folder in the project.
        /// </summary>
        private async void RemoteLangDetection()
        {
            // Create an analyzer. You can customize the analyzer by creating MLRemoteTextSetting
            MLRemoteLangDetectorSetting setting = new MLRemoteLangDetectorSetting.Factory().Create();
            this.remoteLangDetector = MLLangDetectorFactory.Instance.GetRemoteLangDetector(setting);
            // Use default parameter settings.
            // analyzer = MLLangDetectorFactory.Instance.RemoteLangDetector;
            // Read text in edit box.
            string sourceText = this.mEditText.Text.ToString();
            Task<IList<MLDetectedLang>> langDetectTask = this.remoteLangDetector.ProbabilityDetectAsync(sourceText);

            try
            {
                await langDetectTask;

                if (langDetectTask.IsCompleted && langDetectTask.Result != null)
                {
                    // Lang detect success.
                    var detectResult = langDetectTask.Result;
                    this.LangDetectionDisplaySuccess(detectResult);
                }
                else
                {
                    // Lang detect failure.
                    Log.Info(Tag, " Lang detect failure ");
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
        /// Language detection on the device.
        /// </summary>
        private async void LocalDetectLanguage()
        {
            // Create a local language detector.
            MLLangDetectorFactory factory = MLLangDetectorFactory.Instance;
            MLLocalLangDetectorSetting setting = new MLLocalLangDetectorSetting.Factory().SetTrustedThreshold(0.01f).Create();
            localLangDetector = factory.GetLocalLangDetector(setting);
            string input = mEditText.Text.ToString();
            Task<string> langDetectTask = this.localLangDetector.FirstBestDetectAsync(input);

            try
            {
                await langDetectTask;

                if (langDetectTask.IsCompleted && langDetectTask.Result != null)
                {
                    // Lang detect success.
                    var text = langDetectTask.Result;
                    this.DisplaySuccess(text, true);
                }
                else
                {
                    // Lang detect failure.
                    Log.Info(Tag, " Lang detect failure ");
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
        private void DisplaySuccess(string text, bool isTranslator)
        {
            if (isTranslator)
            {
                this.mTextView.Text = text;
            }
            else
            {
                this.mTextView.Text="Language=" + TranslatorActivity.nationAndCode[text] + "(" + text + ").";
            }
        }

        private void LangDetectionDisplaySuccess(IList<MLDetectedLang> result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (MLDetectedLang recognizedLang in result)
            {
                String langCode = recognizedLang.LangCode;
                float probability = recognizedLang.Probability;
                stringBuilder.Append("Language=" + nationAndCode[langCode] + "(" + langCode + "), score=" + probability + ".\n");
            }
            this.mTextView.Text = stringBuilder.ToString();
        }

        /// <summary>
        /// Stop analyzer on OnDestroy() event.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.remoteLangDetector != null)
            {
                this.remoteLangDetector.Stop();
            }
            if (this.remoteTranslator != null)
            {
                this.remoteTranslator.Stop();
            }
            if (this.localLangDetector != null)
            {
                this.localLangDetector.Stop();
            }
            if (this.localTranslator != null)
            {
                this.localTranslator.Stop();
            }
        }

        public void OnClick(View v)
            {
                switch (v.Id)
                {
                    case Resource.Id.btn_local_translator:
                        this.LocalTranslator();
                        break;
                    case Resource.Id.btn_local_detector:
                        this.LocalDetectLanguage();
                        break;
                    case Resource.Id.btn_remote_translator:
                        this.RemoteTranslator();
                        break;
                    case Resource.Id.btn_remote_detector:
                        this.RemoteLangDetection();
                        break;
                    case Resource.Id.btn_delete_model:
                        this.DeleteModel("zh");
                        break;
                    case Resource.Id.btn_download_model:
                        this.DownloadModel("zh");
                        break;
                    default:
                        break;
            }

        }

        /// <summary>
        /// Read the list of languages supported by language detection.
        /// </summary>
        /// <returns> Returns a dictionary that
        /// stores the country name and language code of the ISO 639-1.
        /// </returns>
        private Dictionary<string, string> ReadNationAndCode()
        {
            Dictionary<string, string> nationMap = new Dictionary<string, string>();
            InputStreamReader inputStreamReader = null;
            try
            {
                Stream inputStream = this.Assets.Open("Country_pair_new.txt");
                inputStreamReader = new InputStreamReader(inputStream, "utf-8");
            }
            catch (Exception e)
            {
                Log.Info(TranslatorActivity.Tag, "Read Country_pair_new.txt failed.");
            }
            BufferedReader reader = new BufferedReader(inputStreamReader);
            string line;
            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    string[] nationAndCodeList = line.Split(" ");
                    if (nationAndCodeList.Length == 2)
                    {
                        nationMap.Add(nationAndCodeList[1], nationAndCodeList[0]);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info(TranslatorActivity.Tag, "Read Country_pair_new.txt line by line failed.");
            }
            return nationMap;
        }

        /// <summary>
        /// Delete lang model
        /// </summary>
        /// <param name="langCode"></param>
        private async void DeleteModel(string langCode)
        {
            MLLocalTranslatorModel model = new MLLocalTranslatorModel.Factory(langCode).Create();

            Task deleteModelTask = manager.DeleteModelAsync(model);
            try
            {
                await deleteModelTask;
                if (deleteModelTask.IsCompleted)
                {
                    // Delete success.
                    this.DisplaySuccess("Delete success.", true);
                }
                else
                {
                    // Delete failure.
                    Log.Debug(Tag, " Delete failure.");
                }
            }
            catch(Exception e)
            {
                // Operation failure.
                DisplayFailure(e);
            }
        }
        /// <summary>
        /// Download lang model
        /// </summary>
        /// <param name="sourceLangCode"></param>
        private async void DownloadModel(String sourceLangCode)
        {
            MLLocalTranslatorModel model = new MLLocalTranslatorModel.Factory(sourceLangCode).Create();
            MLModelDownloadStrategy downloadStrategy = new MLModelDownloadStrategy.Factory()
                    .NeedWifi() //  It is recommended that you download the package in a Wi-Fi environment.
                    .Create();
            Task downloadModelTask = manager.DownloadModelAsync(model, downloadStrategy);
            try
            {
                await downloadModelTask;
                if (downloadModelTask.IsCompleted)
                {
                    // Delete success.
                    this.DisplaySuccess("Download success.", true);
                }
                else
                {
                    // Delete failure.
                    Log.Debug(Tag, " Download failure.");
                }
            }
            catch (Exception e)
            {
                // Operation failure.
                DisplayFailure(e);
            }
        }
    }
}