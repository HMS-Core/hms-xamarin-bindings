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
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Speech;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlplugin.Asr;
using Huawei.Hms.Mlsdk.Asr;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Asr
{
    [Activity(Label = "AsrAnalyseActivity")]
    public class AsrAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "AsrAnalyseActivity";

        public static readonly int HandleCode = 0;
        private static readonly string HandleKey = "text";
        private static readonly int AudioPermissionCode = 1;
        private static readonly int MlAsrCaptureCode = 2;

        private TextView mTextView;
        private MLAsrRecognizer mSpeechRecognizer;
        HandlerCallBack handler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_voice_asr);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.textView);
            this.FindViewById(Resource.Id.voice_input).SetOnClickListener(this);
            this.FindViewById(Resource.Id.no_voice_input).SetOnClickListener(this);
            //initialize message handler.
            handler = new HandlerCallBack(this);
            //Checking Audio permission
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) == Android.Content.PM.Permission.Granted))
            {
                this.RequestAudioPermission();
            }
        }

        // Request permissions
        private void RequestAudioPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.RecordAudio };
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.RecordAudio))
            {
                ActivityCompat.RequestPermissions(this, permissions, AsrAnalyseActivity.AudioPermissionCode);
                return;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (requestCode != AsrAnalyseActivity.AudioPermissionCode)
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                return;
            }

        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                // If you want to use ASR, you need to apply for an agconnect-services.json file in the developer
                /// you need to apply for an agconnect-services.json file 
                /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
                /// add agconnect-services.json to Assets folder in the project.
                case Resource.Id.voice_input:
                    // Use Intent for recognition settings.
                    Intent intentPlugin = new Intent(this, typeof(MLAsrCaptureActivity))
                        // Set the language that can be recognized to English. If this parameter is not set,
                        // English is recognized by default. Example: "zh": Chinese or "en-US": English
                        .PutExtra(MLAsrCaptureConstants.Language, "en-US")
                        // Set whether to display text on the speech pickup UI. MLAsrCaptureConstants.FeatureAllinone: no;
                        // MLAsrCaptureConstants.FeatureWordflux: yes.
                        .PutExtra(MLAsrCaptureConstants.Feature, MLAsrCaptureConstants.FeatureWordflux);
                    // ML_ASR_CAPTURE_CODE: request code between the current activity and speech pickup UI activity.
                    // You can use this code to obtain the processing result of the speech pickup UI.
                    StartActivityForResult(intentPlugin, MlAsrCaptureCode);
                    break;
                case Resource.Id.no_voice_input:
                    // Call an API to create a speech recognizer.
                    mSpeechRecognizer = MLAsrRecognizer.CreateAsrRecognizer(this);
                    // Set the ASR result listener callback. You can obtain the ASR result or result code from the listener.
                    mSpeechRecognizer.SetAsrListener(new SpeechRecognitionListener(this));
                    // Set parameters and start the audio device.
                    Intent intentSdk = new Intent(RecognizerIntent.ActionRecognizeSpeech)
                        .PutExtra(MLAsrConstants.Language, "en-US")
                        // Set to return the recognition result along with the speech. If you ignore the setting, this mode is used by default. Options are as follows:
                        // MLAsrConstants.FeatureWordflux: Recognizes and returns texts through OnRecognizingResults.
                        // MLAsrConstants.FeatureAllinone: After the recognition is complete, texts are returned through onResults.
                        .PutExtra(MLAsrConstants.Feature, MLAsrConstants.FeatureAllinone);
                    // Start speech recognition.
                    mSpeechRecognizer.StartRecognizing(intentSdk);
                    mTextView.Text = "Ready to speak.";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Display result in TextView
        /// </summary>
        private void DisplayResult(string str)
        {
            Message msg = new Message();
            Bundle data = new Bundle();
            data.PutString(HandleKey, str);
            msg.Data = data;
            msg.What = HandleCode;
            handler.SendMessage(msg);
        }

        /// <summary>
        /// Set text in TextView
        /// </summary>
        public void SetText(string newText)
        {
            this.mTextView.Text = newText;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            string text = "";
            if (null == data)
            {
                DisplayResult("Intent data is null.");
            }
            // ML_ASR_CAPTURE_CODE: request code between the current activity and speech pickup UI activity.
            if (requestCode == MlAsrCaptureCode)
            {
                switch ((int)resultCode)
                {
                    // MLAsrCaptureConstants.AsrSuccess: Recognition is successful.
                    case MLAsrCaptureConstants.AsrSuccess:
                        if (data != null)
                        {
                            Bundle bundle = data.Extras;
                            // Obtain the text information recognized from speech.
                            if (bundle != null && bundle.ContainsKey(MLAsrCaptureConstants.AsrResult))
                            {
                                text = bundle.GetString(MLAsrCaptureConstants.AsrResult);
                            }
                            if (text == null || "".Equals(text))
                            {
                                text = "Result is null.";
                            }
                            // Process the recognized text information.
                            DisplayResult(text);
                        }
                        break;
                    // MLAsrCaptureConstants.AsrFailure: Recognition fails.
                    case MLAsrCaptureConstants.AsrFailure:
                        if (data != null)
                        {
                            Bundle bundle = data.Extras;
                            // Check whether a result code is contained.
                            if (bundle != null && bundle.ContainsKey(MLAsrCaptureConstants.AsrErrorCode))
                            {
                                text = text + bundle.GetInt(MLAsrCaptureConstants.AsrErrorCode);
                                // Perform troubleshooting based on the result code.
                            }
                            // Check whether error information is contained.
                            if (bundle != null && bundle.ContainsKey(MLAsrCaptureConstants.AsrErrorMessage))
                            {
                                string errorMsg = bundle.GetString(MLAsrCaptureConstants.AsrErrorMessage);
                                // Perform troubleshooting based on the error information.
                                if (errorMsg != null && !"".Equals(errorMsg))
                                {
                                    text = "[" + text + "]" + errorMsg;
                                }
                            }
                            // Check whether a sub-result code is contained.
                            if (bundle != null && bundle.ContainsKey(MLAsrCaptureConstants.AsrSubErrorCode))
                            {
                                int subErrorCode = bundle.GetInt(MLAsrCaptureConstants.AsrSubErrorCode);
                                // Process the sub-result code.
                                text = "[" + text + "]" + subErrorCode;
                            }
                        }
                        DisplayResult(text);
                        break;
                    default:
                        DisplayResult("Failure.");
                        break;
                }
            }
        }

        /// <summary>
        /// Stop recognizer on OnDestroy() event.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mSpeechRecognizer != null)
            {
                mSpeechRecognizer.Destroy();
            }
        }

        /// <summary>
        /// Message handler callback class
        /// </summary>
        public class HandlerCallBack : Android.OS.Handler
        {
            private AsrAnalyseActivity asrAnalyseActivity;

            public HandlerCallBack(AsrAnalyseActivity AsrAnalyseActivity)
            {
                this.asrAnalyseActivity = AsrAnalyseActivity;
            }

            public override void HandleMessage(Message msg)
            {
                base.HandleMessage(msg);
                switch (msg.What)
                {
                    case 0:
                        string text = msg.Data.GetString(HandleKey);
                        asrAnalyseActivity.SetText(text + "\n");
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Speech recognition listener class
        /// Use the callback to 
        /// implement the MLAsrListener API and methods in the API.
        /// </summary>
        public class SpeechRecognitionListener : Java.Lang.Object, IMLAsrListener
        {
            
            private AsrAnalyseActivity asrAnalyseActivity;
            public SpeechRecognitionListener(AsrAnalyseActivity AsrAnalyseActivity)
            {
                this.asrAnalyseActivity = AsrAnalyseActivity;
            }

            public void OnError(int error, string errorMessage)
            {
                // Called when an error occurs in recognition.
                asrAnalyseActivity.SetText(error.ToString() + " " + errorMessage);
            }

            public void OnRecognizingResults(Bundle partialResults)
            {
                // Receive the recognized text from MLAsrRecognizer.
                asrAnalyseActivity.SetText(partialResults.GetString(MLAsrRecognizer.ResultsRecognizing));
            }

            public void OnResults(Bundle results)
            {
                // Text data of ASR.
                asrAnalyseActivity.SetText(results.GetString(MLAsrRecognizer.ResultsRecognized));
            }

            public void OnStartingOfSpeech()
            {
                // The user starts to speak, that is, the speech recognizer detects that the user starts to speak.
                asrAnalyseActivity.SetText("Speaking...");
            }

            public void OnStartListening()
            {
                // The recorder starts to receive speech.
                asrAnalyseActivity.SetText("The recorder starts to receive speech.");
            }

            public void OnState(int i, Bundle bundle)
            {
                // Notify the app status change.
            }

            public void OnVoiceDataReceived(byte[] data, float energy, Bundle bundle)
            {
                // Return the original PCM stream and audio power to the user.
            }

        }
    }
}