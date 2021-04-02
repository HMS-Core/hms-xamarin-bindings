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
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Model.Download;
using Huawei.Hms.Mlsdk.Tts;
using static HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts.BaseActivity;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts
{
    [Activity(Label = "TtsOfflineModeActivity")]
    public class TtsOfflineModeActivity : BaseActivity , DisplayResultCallback, View.IOnClickListener , IMLModelDownloadListener
    {
        private new const string Tag = "TtsOfflineModeActivity";
        private static readonly long M = 1024 * 1024;

        public EditText mEditText;
        public TextView mTextView;
        public TextView tv_download_progress;

        MLLocalModelManager manager;
        MLTtsEngine mlTtsEngine;
        MLTtsConfig mlTtsConfigs;
        Handler handler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_tts_offline_mode);
            this.FindViewById(Resource.Id.btn_speak).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_stop_speak).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_download_model).SetOnClickListener(this);
            this.FindViewById(Resource.Id.close).SetOnClickListener(this);
            mEditText = (EditText) this.FindViewById(Resource.Id.edit_input);
            mTextView = (TextView) this.FindViewById(Resource.Id.textView);
            tv_download_progress = (TextView) this.FindViewById(Resource.Id.tv_download_progress);
            // Use customized parameter settings to create a offline TTS engine.
            mlTtsConfigs = new MLTtsConfig()
                    // Setting the language for synthesis.
                    .SetLanguage(MLTtsConstants.TtsEnUs)
                    // Set the timbre.
                    .SetPerson(MLTtsConstants.TtsSpeakerOfflineEnUsMaleEagle)
                    // Set the speech speed. Range: 0.2–2.0 1.0 indicates 1x speed.
                    .SetSpeed(1.0f)
                    // Set the volume. Range: 0.2–2.0 1.0 indicates 1x volume.
                    .SetVolume(1.0f)
                    // set the synthesis mode.
                    .SetSynthesizeMode(MLTtsConstants.TtsOfflineMode);
            mlTtsEngine = new MLTtsEngine(mlTtsConfigs);
            // Pass the TTS callback to the TTS engine.
            mlTtsEngine.SetTtsCallback(this);
            manager = MLLocalModelManager.Instance;
            this.SetOnResultCallback(this);
            handler = new Handler(new TtsOfflineHandlerCallBack(this));
        }

        public async void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.close:
                    mEditText.Text = "";
                    break;
                case Resource.Id.btn_speak:
                    string text = mEditText.Text.ToString();
                    //Check whether the offline model corresponding to the language has been downloaded.
                    MLTtsLocalModel model = new MLTtsLocalModel.Factory(MLTtsConstants.TtsSpeakerOfflineEnUsMaleEagle).Create();
                    Task<bool> checkModelTask = manager.IsModelExistAsync(model);

                    try
                    {
                        await checkModelTask;

                        if (checkModelTask.IsCompleted && checkModelTask.Result == true)
                        {
                            Speak(text);
                        }
                        else
                        {
                            Log.Error(Tag, "isModelDownload== " + checkModelTask.Result);
                            ShowToast("The offline model has not been downloaded!");
                            DownloadModel(MLTtsConstants.TtsSpeakerOfflineEnUsMaleEagle);
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(Tag, "downloadModel failed: " + e.Message);
                        ShowToast(e.Message);
                    }
                    break;
                case Resource.Id.btn_download_model:
                    DownloadModel(MLTtsConstants.TtsSpeakerOfflineEnUsMaleEagle);
                    break;
                case Resource.Id.btn_stop_speak:
                    mlTtsEngine.Stop();
                    break;
                default:
                    break;
            }
        }

        private void Speak(string text)
        {
               /**
                *First parameter sourceText: text information to be synthesized. The value can contain a maximum of 500 characters.
                *Second parameter indicating the synthesis mode: The format is configA | configB | configC.
                *configA：
                *    MLTtsEngine.QueueAppend：After an audio synthesis task is generated, the audio synthesis task is processed as follows: If playback is going on, the task is added to the queue for execution in sequence; if playback pauses, the playback is resumed and the task is added to the queue for execution in sequence; if there is no playback, the audio synthesis task is executed immediately.
                *    MLTtsEngine.QueueFlush：The ongoing audio synthesis task and playback are stopped immediately, all audio synthesis tasks in the queue are cleared, and the current audio synthesis task is executed immediately and played.
                *configB：
                *    MLTtsEngine.OpenStream：The synthesized audio data is output through onAudioAvailable.
                *configC：
                *    MLTtsEngine.ExternalPlayback：external playback mode. The player provided by the SDK is shielded. You need to process the audio output by the onAudioAvailable callback API. In this case, the playback-related APIs in the callback APIs become invalid, and only the callback APIs related to audio synthesis can be listened.
                */
               // Use the built-in player of the SDK to play speech in queuing mode.
               string id = mlTtsEngine.Speak(text, MLTtsEngine.QueueAppend);
            // In queuing mode, the synthesized audio stream is output through OnAudioAvailable, and the built-in player of the SDK is used to play the speech.
            // string id = mlTtsEngine.Speak(text, MLTtsEngine.QueueAppend | MLTtsEngine.OpenStream);
            // In queuing mode, the synthesized audio stream is output through OnAudioAvailable, and the audio stream is not played, but controlled by you.
            // string id = mlTtsEngine.Speak(text, MLTtsEngine.QueueAppend | MLTtsEngine.OpenStream | MLTtsEngine.ExternalPlayback);
        }

        private async void DownloadModel(string person)
        {
            MLTtsLocalModel model = new MLTtsLocalModel.Factory(person).Create();
            MLModelDownloadStrategy request = new MLModelDownloadStrategy.Factory()
                .NeedWifi()
                .SetRegion(MLModelDownloadStrategy.RegionDrEurope)
                .Create();

            Task downloadTask = manager.DownloadModelAsync(model, request, this);

            try
            {
                await downloadTask;

                if (downloadTask.IsCompleted)
                {
                    mlTtsEngine.UpdateConfig(mlTtsConfigs);
                    Log.Info(Tag, "downloadModel: " + model.ModelName + " success");
                    ShowToast("downloadModel Success");
                    Speak(mEditText.Text.ToString().Trim());
                }
                else
                {
                    Log.Info(Tag, "failed ");
                }

            }
            catch(Exception e)
            {
                Log.Error(Tag, "downloadModel failed: " + e.Message);
                ShowToast(e.Message);
            }
        }


        private void ShowProcess(long alreadyDownLength, string buttonText, long totalLength)
        {
            double downDone = alreadyDownLength * 1.0 / M;
            double downTotal = totalLength * 1.0 / M;
            string downD = string.Format("%.2f", downDone);
            string downT = string.Format("%.2f", downTotal);

            string text = downD + "M" + "/" + downT + "M";
            UpdateButton(text, false);
            if (downD.Equals(downT))
            {
                ShowToast(buttonText);
                UpdateButton(buttonText, true);
            }
        }

        private void UpdateButton(string text, bool downloadSuccess)
        {
            this.RunOnUiThread(delegate() {
                tv_download_progress.Text = text;
                if (!downloadSuccess)
                {
                    tv_download_progress.Visibility = ViewStates.Visible;
                }
                else
                {
                    tv_download_progress.Visibility = ViewStates.Gone;
                }

            }); 
        }

        private void ShowToast(string text)
        {
            this.RunOnUiThread(delegate () {
                Toast.MakeText(this, text, ToastLength.Short).Show();

            });
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.mlTtsEngine != null)
            {
                this.mlTtsEngine.Shutdown();
            }
        }

        public void DisplayResult(string str)
        {
            Message msg = new Message();
            Bundle data = new Bundle();
            data.PutString(HandleKey, str);
            msg.Data = data;
            msg.What = HandleCode;
            handler.SendMessage(msg);
        }

        /// <summary>
        /// Implemented from IMLModelDownloadListener interface
        /// </summary>
        public void OnProcess(long alreadyDownLength, long totalLength)
        {
            ShowProcess(alreadyDownLength, "Model downloading...", totalLength);
        }
    }

    /// <summary>
    /// Message handler callback class
    /// </summary>
    public class TtsOfflineHandlerCallBack : Java.Lang.Object, Android.OS.Handler.ICallback
    {
        private TtsOfflineModeActivity mActivity;

        public TtsOfflineHandlerCallBack(TtsOfflineModeActivity activity)
        {
            this.mActivity = activity;
        }

        bool Handler.ICallback.HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case BaseActivity.HandleCode:
                    string text = msg.Data.GetString(BaseActivity.HandleKey);
                    this.mActivity.mTextView.Text = (text + "\n");
                    Log.Error(BaseActivity.Tag, text);
                    break;
                default:
                    break;
            }
            return false;
        }
    }
}