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
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk.Aft.Cloud;
using Java.IO;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Aft
{
    [Activity(Label = "AftAnalyseActivity")]
    public class AftAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "AftAnalyseActivity";

        private static readonly int RecordRequestCode = 3;
        private static readonly int ThresholdTime = 60000; // 60s

        private TextView tvFileName;
        private TextView tvText;
        private ImageView imgVoice;
        private ImageView imgPlay;
        private string taskId;
        private Android.Net.Uri uri;
        private MLRemoteAftEngine engine;
        private MLRemoteAftSetting setting;
        private static Timer mTimer;
        private AftListener aftListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_voice_aft);
            this.tvFileName = (TextView)FindViewById(Resource.Id.file_name);
            this.tvText = (TextView)FindViewById(Resource.Id.text_output);
            this.imgVoice = (ImageView)FindViewById(Resource.Id.voice_input);
            this.imgPlay = (ImageView)FindViewById(Resource.Id.voice_play);
            this.FindViewById(Resource.Id.voice_input).SetOnClickListener(this);
            //Checking Audio permission
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) == Android.Content.PM.Permission.Granted))
            {
                this.RequestAudioPermission();
            }

            /// <summary>
            /// Audio file transcription callback function. If you want to use AFT,
            /// you need to apply for an agconnect-services.json file 
            /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
            /// add agconnect-services.json to Assets folder in the project.
            /// </summary>
            this.aftListener = new AftListener(this);

            this.setting = new MLRemoteAftSetting.Factory()
            // Set the transcription language code, complying with the BCP 47 standard. Currently, zh (Chinese) and en-US (English) are supported.
            .SetLanguageCode("en-US")
            .Create();
            this.engine = MLRemoteAftEngine.Instance;
            this.engine.Init(this);
            // Pass the listener callback to the audio file transcription engine.
            this.engine.SetAftListener(this.aftListener);
        }

        /// <summary>
        /// Request permissions
        /// </summary>
        private void RequestAudioPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.RecordAudio };
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.RecordAudio))
            {
                ActivityCompat.RequestPermissions(this, permissions, RecordRequestCode);
                return;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.voice_input:
                    this.tvFileName.Text = "add_voice";
                    this.imgPlay.SetImageResource(Resource.Drawable.icon_voice_new);
                    this.tvText.Text = "";
                    this.StartRecord();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Start audio record activity
        /// </summary>
        private void StartRecord()
        {
            Intent intent = new Intent();
            intent.SetAction(MediaStore.Audio.Media.RecordSoundAction);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            intent.PutExtra(MediaStore.ExtraOutput, this.uri);
            this.StartActivityForResult(intent, AftAnalyseActivity.RecordRequestCode);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == AftAnalyseActivity.RecordRequestCode && resultCode == Result.Ok)
            {
                this.uri = data.Data;
                string filePath = this.GetAudioFilePathFromUri(this.uri);
                File file = new File(filePath);
                this.SetFileName(file.Name);
                long audioTime = GetAudioFileTimeFromUri(this.uri);
                int audioTimeInt;
                audioTimeInt = Convert.ToInt32(audioTime);
                if (audioTimeInt < ThresholdTime)
                {
                    // Transfer the audio less than one minute to the audio file transcription engine.
                    this.taskId = this.engine.ShortRecognize(this.uri, this.setting);
                    Log.Info(Tag, "Short audio transcription.");
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Sets voice file name.
        /// </summary>
        /// <param name="path"></param>
        private void SetFileName(string path)
        {
            this.RunOnUiThread(
                delegate 
                {
                    this.tvFileName.Text = path;
                    this.imgPlay.SetImageResource(Resource.Drawable.icon_voice_accent_new);
                });
        }

        private string GetAudioFilePathFromUri(Android.Net.Uri uri)
        {
            ICursor cursor = this.BaseContext.ContentResolver
                    .Query(this.uri, null, null, null, null);
            cursor.MoveToFirst();
            int index = cursor.GetColumnIndex(MediaStore.Audio.AudioColumns.Data);
            return cursor.GetString(index);
        }

        private long GetAudioFileTimeFromUri(Android.Net.Uri uri)
        {
            ICursor cursor = this.BaseContext.ContentResolver
                    .Query(this.uri, null, null, null, null);
            cursor.MoveToFirst();
            long time = cursor.GetLong(cursor.GetColumnIndexOrThrow(MediaStore.Video.Media.InterfaceConsts.Duration));
            return time;
        }

        /// <summary>
        /// When analyze failed,
        /// displays exception message in TextView
        /// </summary>
        public void DisplayFailure(int errorCode)
        {
            this.tvText.Text = "Failure, errorCode is:" + errorCode;
        }

        public void CancelTimer()
        {
            if (mTimer != null)
            {
                mTimer.Dispose();
                mTimer = null;
            }
        }

        /// <summary>
        /// Get class TAG
        /// </summary>
        /// <returns></returns>
        public string GetTAG()
        {
            return Tag;
        }

        /// <summary>
        /// Change text in TextView
        /// </summary>
        /// <param name="newText"></param>
        public void SetText(string newText)
        {
            this.tvText.Text = newText;
        }

        /// <summary>
        /// Stop engine on OnDestroy() event.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.engine.Close();
        }
    }
    /// <summary>
    /// Audio file transcription callback class. If you want to use AFT,
    /// you need to apply for an agconnect-services.json file 
    /// in the developer alliance(https://developer.huawei.com/consumer/en/doc/development/HMS-Guides/ml-add-agc),
    /// add agconnect-services.json to Assets folder in the project.
    /// </summary>
    public class AftListener : Java.Lang.Object, IMLRemoteAftListener
    {
        private AftAnalyseActivity aftAnalyseActivity;
        public AftListener(AftAnalyseActivity AftAnalyseActivity)
        {
            this.aftAnalyseActivity = AftAnalyseActivity;
        }

        // Transliteration error callback function.
        public void OnError(string taskId, int errorCode, string message)
        {
            aftAnalyseActivity.DisplayFailure(errorCode);
            Log.Info(aftAnalyseActivity.GetTAG(), "MLAsrCallBack onError." + errorCode + " task:" + taskId);
        }

        public void OnEvent(string taskId, int eventId, Java.Lang.Object ext)
        {
            // Reserved.
        }

        public void OnInitComplete(string taskId, Java.Lang.Object ext)
        {
            // Reserved.
        }

        // Get notification of transcription results, where developers can process the transcription results.
        public void OnResult(string taskId, MLRemoteAftResult result, Java.Lang.Object ext)
        {
            Log.Info(aftAnalyseActivity.GetTAG(), taskId + " ");
            if (result.Complete)
            {
                Log.Info("AftAnalyseActivity", "result" + result.Text);
                aftAnalyseActivity.CancelTimer();
                this.aftAnalyseActivity.SetText(result.Text);
            }
            else
            {
                this.aftAnalyseActivity.SetText("Loading...");
                return;
            }
            if (result == null || result.TaskId == null || result.Text.Equals(""))
            {
                this.aftAnalyseActivity.SetText("No speech recognized, please re-enter.");
                return;
            }
        }

        public void OnUploadProgress(string taskId, double progress, Java.Lang.Object ext)
        {
            // Reserved.
        }
    }
}