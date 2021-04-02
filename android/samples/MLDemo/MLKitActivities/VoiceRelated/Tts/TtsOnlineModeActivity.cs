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
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Tts;
using static HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts.BaseActivity;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts
{
    [Activity(Label = "TtsOnlineModeActivity")]
    public class TtsOnlineModeActivity : BaseActivity, DisplayResultCallback , View.IOnClickListener
    {
        private new const string Tag = "TtsOnlineModeActivity";

        public EditText mEditText;
        public TextView mTextView;

        MLTtsEngine mlTtsEngine;
        MLTtsConfig mlConfigs;
        Handler handler;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_tts_online_mode);
            FindViewById(Resource.Id.btn_speak).SetOnClickListener(this);
            FindViewById(Resource.Id.btn_stop_speak).SetOnClickListener(this);
            FindViewById(Resource.Id.close).SetOnClickListener(this);
            mEditText = (EditText) FindViewById(Resource.Id.edit_input);
            mTextView = (TextView) FindViewById(Resource.Id.textView);

            // Method 1: Use the default parameter settings to create a TTS engine.
            // In the default settings, the source language is Chinese, the Chinese female voice is used,
            // the voice speed is 1.0 (1x), and the volume is 1.0 (1x).
            // MLTtsConfig mlConfigs = new MLTtsConfig();
            // Method 2: Use customized parameter settings to create a TTS engine.
            mlConfigs = new MLTtsConfig()
                    // Setting the language for synthesis.
                    .SetLanguage(MLTtsConstants.TtsEnUs)
                    // Set the timbre.
                    .SetPerson(MLTtsConstants.TtsSpeakerFemaleEn)
                    // Set the speech speed. Range: 0.2–4.0 1.0 indicates 1x speed.
                    .SetSpeed(1.0f)
                    // Set the volume. Range: 0.2–4.0 1.0 indicates 1x volume.
                    .SetVolume(1.0f)
                    // set the synthesis mode.
                    .SetSynthesizeMode(MLTtsConstants.TtsOnlineMode);

            mlTtsEngine = new MLTtsEngine(mlConfigs);
            // Pass the TTS callback to the TTS engine.
            mlTtsEngine.SetTtsCallback(this);
            handler = new Handler(new TtsOnlineHandlerCallBack(this));
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.close:
                    mEditText.Text = "";
                    break;
                case Resource.Id.btn_speak:
                    string text = mEditText.Text.ToString();
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
                    // In queuing mode, the synthesized audio stream is output through onAudioAvailable, and the built-in player of the SDK is used to play the speech.
                    // string id = mlTtsEngine.Speak(text, MLTtsEngine.QueueAppend | MLTtsEngine.OpenStream);
                    // In queuing mode, the synthesized audio stream is output through onAudioAvailable, and the audio stream is not played, but controlled by you.
                    // string id = mlTtsEngine.Speak(text, MLTtsEngine.QueueAppend | MLTtsEngine.OpenStream | MLTtsEngine.ExternalPlayback);
                    break;
                case Resource.Id.btn_stop_speak:
                    mlTtsEngine.Stop();
                    break;
                default:
                    break;
            }
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
    }

    public class TtsOnlineHandlerCallBack : Java.Lang.Object, Android.OS.Handler.ICallback
    {
        private TtsOnlineModeActivity mActivity;

        public TtsOnlineHandlerCallBack(TtsOnlineModeActivity activity)
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