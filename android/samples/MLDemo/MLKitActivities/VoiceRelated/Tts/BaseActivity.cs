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
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk.Tts;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts
{
    [Activity(Label = "BaseActivity")]
    public abstract class BaseActivity : AppCompatActivity , IMLTtsCallback
    {
        public const string Tag = "BaseActivity";

        public const int HandleCode = 1;
        public const string HandleKey = "text";

        public DisplayResultCallback resultCallback = null;
        public interface DisplayResultCallback
        {
            void DisplayResult(string str);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }

        public void SetOnResultCallback(DisplayResultCallback resultCallback)
        {
            this.resultCallback = resultCallback;
        }

        /**
         * TTS callback functions. If you want to use TTS,
         * you need to apply for an agconnect-services.json file in the developer
         * alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
         * replacing the sample-agconnect-services.json in the project.
         */
        public void OnAudioAvailable(string s, MLTtsAudioFragment mlTtsAudioFragment, int i, Pair pair, Bundle bundle)
        {
            //  Audio stream callback API, which is used to return the synthesized audio data to the app.
            //  taskId: ID of an audio synthesis task corresponding to the audio.
            //  audioFragment: audio data.
            //  offset: offset of the audio segment to be transmitted in the queue. One audio synthesis task corresponds to an audio synthesis queue.
            //  range: text area where the audio segment to be transmitted is located; range.first (included): start position; range.second (excluded): end position.
        }

        public void OnError(string taskId, MLTtsError err)
        {
            // Processing logic for TTS failure.
            string str = "TaskID: " + taskId + ", error:" + err;
            if (resultCallback != null)
            {
                resultCallback.DisplayResult(str);
            }
        }

        public void OnEvent(string taskId, int eventId, Bundle bundle)
        {
            String str = "TaskID: " + taskId + ", eventName:" + eventId;
            // Callback method of an audio synthesis event. eventId: event name.
            switch (eventId)
            {
                case MLTtsConstants.EventPlayStart:
                    //  Called when playback starts.
                    break;
                case MLTtsConstants.EventPlayStop:
                    // Called when playback stops.
                    bool isInterrupted = bundle.GetBoolean(MLTtsConstants.EventPlayStopInterrupted);
                    str += " " + isInterrupted;
                    break;
                case MLTtsConstants.EventPlayResume:
                    //  Called when playback resumes.
                    break;
                case MLTtsConstants.EventPlayPause:
                    // Called when playback pauses.
                    break;

                /*//Pay attention to the following callback events when you focus on only synthesized audio data but do not use the internal player for playback:
                case MLTtsConstants.EventSynthesisStart:
                    //  Called when TTS starts.
                    break;
                case MLTtsConstants.EventSynthesisEnd:
                    // Called when TTS ends.
                    break;
                case MLTtsConstants.EventSynthesisComplete:
                    // TTS is complete. All synthesized audio streams are passed to the app.
                    bool isInterrupted = bundle.GetBoolean(MLTtsConstants.EventSynthesisInterrupted);
                    break;*/
                default:
                    break;
            }
            if (resultCallback != null)
            {
                resultCallback.DisplayResult(str);
            }
        }

        public void OnRangeStart(string taskId, int start, int end)
        {
            // Process the mapping between the currently played segment and text.
            String str = "TaskID: " + taskId + ", onRangeStart [" + start + "，" + end + "]";
            if (resultCallback != null)
            {
                resultCallback.DisplayResult(str);
            }
        }

        public void OnWarn(string taskId, MLTtsWarn warn)
        {
            // Alarm handling without affecting service logic.
            string str = "TaskID: " + taskId + ", warn:" + warn;
            if (resultCallback != null)
            {
                resultCallback.DisplayResult(str);
            }
        }
    }
}