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

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Huawei.Hms.Mlsdk.Speechrtt;
using Java.Lang;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.RealTimeTranscription
{
    [Activity(Label = "RealTimeTranscriptionActivity")]
    public class RealTimeTranscriptionActivity : AppCompatActivity, View.IOnClickListener
    {
        public const string Tag = "RealTimeTranscriptionActivity";

        private string languageItem = MLSpeechRealTimeTranscriptionConstants.LanEnUs;
        private TextView resultTv;

        public TextView ResultTv
        {
            get { return resultTv; }
            set { resultTv = value; }
        }

        private TextView errorTv;
        public TextView ErrorTv
        {
            get { return errorTv; }
            set { errorTv = value; }
        }

        private StringBuffer recognizerResult = new StringBuffer();
        public StringBuffer RecognizerResult
        {
            get { return recognizerResult; }
            set { recognizerResult = value; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_realtime_transcription);
            InitView();

            if (!AllPermissionsGranted())
            {
                GetRuntimePermissions();
            }
        }

        private void InitView()
        {
            this.FindViewById(Resource.Id.start_listen_btn).SetOnClickListener(this);
            this.FindViewById(Resource.Id.stop_listen_btn).SetOnClickListener(this);
            resultTv = (TextView) this.FindViewById(Resource.Id.result_tv);
            errorTv = (TextView) this.FindViewById(Resource.Id.error_tv);
        }

        public string GetPrompt(int errorCode)
        {
            Log.Debug(Tag, "ErrorCode： " + errorCode + " Other errors");
            string error_text;
            switch (errorCode)
            {
                case MLSpeechRealTimeTranscriptionConstants.ErrNoNetwork:
                    error_text = "The network is unavailable.，……Please try again.";
                    break;
                case MLSpeechRealTimeTranscriptionConstants.ErrServiceUnavailable:
                    error_text = "The service is unavailable.";
                    break;
                case MLSpeechRealTimeTranscriptionConstants.ErrInvalideToken:
                    error_text = "The unavailable is token.";
                    break;
                case MLSpeechRealTimeTranscriptionConstants.ErrServiceCredit:
                    error_text = "Insufficient balance. Please recharge.";
                    break;
                default:
                    error_text = "ErrorCode： " + errorCode + " Other errors";
                    break;
            }
            return error_text;
        }

        private void Start()
        {
            MLSpeechRealTimeTranscriptionConfig config = new MLSpeechRealTimeTranscriptionConfig.Factory()
                    // Set the language that can be recognized to English. If this parameter is not set,
                    // English is recognized by default.
                    // Example: MLSpeechRealTimeTranscriptionConstants.LanZhCn: Chinese,
                    // MLSpeechRealTimeTranscriptionConstants.LanEnUs: English,
                    // MLSpeechRealTimeTranscriptionConstants.LanFrFr: French.
                    .SetLanguage(languageItem)
                    // set punctuation support.
                    .EnablePunctuation(true)
                    // set sentence time offset.
                    .EnableSentenceTimeOffset(true)
                    // set word time offset.
                    .EnableWordTimeOffset(true)
                    // Set the usage scenario to shopping,Currently, only Chinese scenarios are supported.
                    //.SetScenes(MLSpeechRealTimeTranscriptionConstants.ScenesShopping)
                    .Create();
            MLSpeechRealTimeTranscription.Instance.StartRecognizing(config);

            MLSpeechRealTimeTranscription.Instance.SetRealTimeTranscriptionListener(new SpeechRecognitionListener(this));

            Log.Debug(Tag, "language  " + config.Language);
            Log.Debug(Tag, "isPunctuationEnable  " + config.IsPunctuationEnable);
            Log.Debug(Tag, "isWordTimeOffsetEnable  " + config.IsWordTimeOffsetEnable);
            Log.Debug(Tag, "isSentenceTimeOffsetEnable  " + config.IsSentenceTimeOffsetEnable);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.start_listen_btn:
                    Start();
                    break;
                case Resource.Id.stop_listen_btn:
                    DestroyRecognizer();
                    break;
                default:
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DestroyRecognizer();
        }

        public void DestroyRecognizer()
        {
            MLSpeechRealTimeTranscription.Instance.Destroy();
        }

        private bool AllPermissionsGranted()
        {
            foreach (string permission in GetRequiredPermissions())
            {
                if (!IsPermissionGranted(this, permission))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsPermissionGranted(Context context, string permission)
        {
            if (ContextCompat.CheckSelfPermission(context, permission) == Permission.Granted)
            {
                Log.Info(Tag, "Permission granted: " + permission);
                return true;
            }
            Log.Info(Tag, "Permission NOT granted: " + permission);
            return false;
        }

        private IList<string> GetRequiredPermissions()
        {
            try
            {
                PackageInfo info = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.Permissions);
                IList<string> ps = info.RequestedPermissions;
                if (ps != null && ps.Count > 0)
                {
                    return ps;
                }
                else
                {
                    return new List<string>();
                }
            }
            catch (RuntimeException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                return new List<string>();
            }
        }

        private void GetRuntimePermissions()
        {
            List<string> allNeededPermissions = new List<string>();
            foreach (string permission in GetRequiredPermissions())
            {
                if (!IsPermissionGranted(this, permission))
                {
                    allNeededPermissions.Add(permission);
                }
            }

            if (allNeededPermissions.Count != 0)
            {
                ActivityCompat.RequestPermissions(this, allNeededPermissions.ToArray(), 1);
            }
        }

    }

    public class SpeechRecognitionListener : Java.Lang.Object, IMLSpeechRealTimeTranscriptionListener
    {
        RealTimeTranscriptionActivity mActivity;

        public SpeechRecognitionListener(RealTimeTranscriptionActivity activity)
        {
            this.mActivity = activity;
        }
        public void OnError(int error, string errorMessage)
        {
            string tip = mActivity.GetPrompt(error);
            if (!TextUtils.IsEmpty(tip))
            {
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onError: " + error + " update: " + tip);
                tip = "ERROR: " + tip;
                mActivity.ErrorTv.Visibility = ViewStates.Visible;
                mActivity.ErrorTv.Text = tip;
                mActivity.ResultTv.Text = "";
            }
        }

        public void OnRecognizingResults(Bundle partialResults)
        {
            Log.Debug(RealTimeTranscriptionActivity.Tag, " sonuc OnRecognizingResults: ");
            if (partialResults == null)
            {
                Log.Debug(RealTimeTranscriptionActivity.Tag, " sonuc partialResults: ");
                return;
            }
            bool isFinal = partialResults.GetBoolean(MLSpeechRealTimeTranscriptionConstants.ResultsPartialfinal);
            string result = partialResults.GetString(MLSpeechRealTimeTranscriptionConstants.ResultsRecognizing);
            if (TextUtils.IsEmpty(result))
            {
                Log.Debug(RealTimeTranscriptionActivity.Tag, " sonuc IsEmpty: ");
                return;
            }
            if (isFinal)
            {

                IList<MLSpeechRealTimeTranscriptionResult> wordOffset = partialResults.GetParcelableArrayList(MLSpeechRealTimeTranscriptionConstants.ResultsWordOffset).Cast<MLSpeechRealTimeTranscriptionResult>().ToList();
                IList<MLSpeechRealTimeTranscriptionResult> sentenceOffset = partialResults.GetParcelableArrayList(MLSpeechRealTimeTranscriptionConstants.ResultsSentenceOffset).Cast<MLSpeechRealTimeTranscriptionResult>().ToList();

                if (wordOffset != null)
                {
                    for (int i = 0; i < wordOffset.Count; i++)
                    {
                        MLSpeechRealTimeTranscriptionResult remoteResult = wordOffset.ElementAt(i);
                        Log.Debug(RealTimeTranscriptionActivity.Tag, "onRecognizingResults word offset is " + i + " ---> " + remoteResult.ToString());
                    }
                }

                if (sentenceOffset != null)
                {
                    for (int i = 0; i < sentenceOffset.Count; i++)
                    {
                        MLSpeechRealTimeTranscriptionResult remoteResult = sentenceOffset.ElementAt(i);
                        Log.Debug(RealTimeTranscriptionActivity.Tag, "onRecognizingResults sentence offset is " + i + " ---> " + remoteResult.ToString());
                    }
                }
                mActivity.RecognizerResult.Append(result);
                mActivity.ErrorTv.Text = mActivity.RecognizerResult.ToString();

            }
            else
            {
                mActivity.ResultTv.Text = result;
            }
        }

        public void OnStartingOfSpeech()
        {
            Log.Debug(RealTimeTranscriptionActivity.Tag, "onStartingOfSpeech");
        }

        public void OnStartListening()
        {
            Log.Debug(RealTimeTranscriptionActivity.Tag, "onStartListening");
        }

        public void OnState(int state, Bundle bundle)
        {
            if (state == MLSpeechRealTimeTranscriptionConstants.StateServiceReconnecting)
            { // webSocket Reconnecting
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onState webSocket reconnect ");
            }
            else if (state == MLSpeechRealTimeTranscriptionConstants.StateServiceReconnected)
            { // webSocket Reconnection succeeded.
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onState webSocket reconnect success ");
            }
            else if (state == MLSpeechRealTimeTranscriptionConstants.StateListening)
            { // The recorder is ready.
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onState recorder is ready ");
            }
            else if (state == MLSpeechRealTimeTranscriptionConstants.StateNoUnderstand)
            { // Failed to detect the current frame.
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onState Failed to detect the current frame ");
            }
            else if (state == MLSpeechRealTimeTranscriptionConstants.StateNoNetwork)
            { // No network is available in the current environment.
                Log.Debug(RealTimeTranscriptionActivity.Tag, "onState No network ");
            }
        }

        public void OnVoiceDataReceived(byte[] data, float energy, Bundle bundle)
        {
            int length = data == null ? 0 : data.Length;
            Log.Debug(RealTimeTranscriptionActivity.Tag, "onVoiceDataReceived data.length=" + length);
        }
    }
}