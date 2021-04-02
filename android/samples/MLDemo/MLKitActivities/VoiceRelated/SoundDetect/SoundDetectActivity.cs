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
using Android.Icu.Text;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Mlsdk.Sounddect;
using Java.Util;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.SoundDetect
{
    [Activity(Label = "SoundDetectActivity")]
    public class SoundDetectActivity : AppCompatActivity, View.IOnClickListener, IMLSoundDectListener
    {
        private const string Tag = "SoundDetectActivity";

        private const int RcRecordCode = 0x123;
        private string[] perms = { Manifest.Permission.RecordAudio };
        private IList<string> logList;
        private SimpleDateFormat dateFormat;
        private TextView textView;
        private MLSoundDector soundDector;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_sound_detect);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            textView = (TextView) this.FindViewById(Resource.Id.textView);
            this.FindViewById(Resource.Id.start_btn).SetOnClickListener(this);
            this.FindViewById(Resource.Id.stop_btn).SetOnClickListener(this);
            logList = new List<string>();
            dateFormat = new SimpleDateFormat("HH:mm:ss");
            InitModel();
        }

        private void InitModel()
        {
            // Initialize the voice recognizer
            soundDector = MLSoundDector.CreateSoundDector();
            // Setting Recognition Result Listening
            soundDector.SetSoundDectListener(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            soundDector.Destroy();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.start_btn:
                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) == Android.Content.PM.Permission.Granted)
                    {
                        bool startSuccess = soundDector.Start(this);
                        if (startSuccess)
                        {
                            Toast.MakeText(this, Resource.String.sound_dect_start, ToastLength.Long).Show();
                        }
                        return;
                    }
                    ActivityCompat.RequestPermissions(this, perms, RcRecordCode);

                    break;
                case Resource.Id.stop_btn:
                    soundDector.Stop();
                    Toast.MakeText(this, Resource.String.sound_dect_stop, ToastLength.Long).Show();
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Log.Info(Tag, "onRequestPermissionsResult ");
            if (requestCode == RcRecordCode && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                bool startSuccess = soundDector.Start(this);
                if (startSuccess)
                {
                    Toast.MakeText(this, Resource.String.sound_dect_start, ToastLength.Long).Show();
                }
            }
        }

        /// <summary>
        /// Implemented from IMLSoundDectListener interface
        /// </summary>
        public void OnSoundFailResult(int errCode)
        {
            string errCodeDesc = "";
            switch (errCode)
            {
                case MLSoundDectConstants.SoundDectErrorNoMem:
                    errCodeDesc = "no memory error";
                    break;
                case MLSoundDectConstants.SoundDectErrorFatalError:
                    errCodeDesc = "fatal error";
                    break;
                case MLSoundDectConstants.SoundDectErrorAudio:
                    errCodeDesc = "microphone error";
                    break;
                case MLSoundDectConstants.SoundDectErrorInternal:
                    errCodeDesc = "internal error";
                    break;
                default:
                    break;
            }
            Log.Error(Tag, "FailResult errCode: " + errCode + "errCodeDesc:" + errCodeDesc);
        }

        /// <summary>
        /// Implemented from IMLSoundDectListener interface
        /// </summary>
        public void OnSoundSuccessResult(Bundle result)
        {
            string nowTime = dateFormat.Format(new Date());
            int soundType = result.GetInt(MLSoundDector.ResultsRecognized);
            switch (soundType)
            {
                case MLSoundDectConstants.SoundEventTypeLaughter:
                    logList.Add(nowTime + "\tsoundType:laughter");
                    break;
                case MLSoundDectConstants.SoundEventTypeBabyCry:
                    logList.Add(nowTime + "\tsoundType:baby cry");
                    break;
                case MLSoundDectConstants.SoundEventTypeSnoring:
                    logList.Add(nowTime + "\tsoundType:snoring");
                    break;
                case MLSoundDectConstants.SoundEventTypeSneeze:
                    logList.Add(nowTime + "\tsoundType:sneeze");
                    break;
                case MLSoundDectConstants.SoundEventTypeScreaming:
                    logList.Add(nowTime + "\tsoundType:screaming");
                    break;
                case MLSoundDectConstants.SoundEventTypeMeow:
                    logList.Add(nowTime + "\tsoundType:meow");
                    break;
                case MLSoundDectConstants.SoundEventTypeBark:
                    logList.Add(nowTime + "\tsoundType:bark");
                    break;
                case MLSoundDectConstants.SoundEventTypeWater:
                    logList.Add(nowTime + "\tsoundType:water");
                    break;
                case MLSoundDectConstants.SoundEventTypeCarAlarm:
                    logList.Add(nowTime + "\tsoundType:car alarm");
                    break;
                case MLSoundDectConstants.SoundEventTypeDoorBell:
                    logList.Add(nowTime + "\tsoundType:doorbell");
                    break;
                case MLSoundDectConstants.SoundEventTypeKnock:
                    logList.Add(nowTime + "\tsoundType:knock");
                    break;
                case MLSoundDectConstants.SoundEventTypeAlarm:
                    logList.Add(nowTime + "\tsoundType:alarm");
                    break;
                case MLSoundDectConstants.SoundEventTypeSteamWhistle:
                    logList.Add(nowTime + "\tsoundType:steam whistle");
                    break;
                default:
                    logList.Add(nowTime + "\tsoundType:unknown type");
                    break;

            }

            string text = "";
            foreach (string log in logList)
            {
                text += log + "\n";
            }
            if (logList.Count > 10)
            {
                logList.RemoveAt(0);
            }
            textView.Text = text;
        }
    }
}