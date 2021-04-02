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
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace HmsXamarinMLDemo.MLKitActivities.VoiceRelated.Tts
{
    [Activity(Label = "TtsActivity")]
    public class TtsActivity : AppCompatActivity, View.IOnClickListener
    {
        // tts Online mode
        private Button btn_online_mode;

        // tts Offline mode
        private Button btn_offline_mode;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_tts);

            InitView();
            InitListener();
        }

        private void InitView()
        {
            btn_online_mode = (Button) this.FindViewById(Resource.Id.btn_online_mode);
            btn_offline_mode = (Button) this.FindViewById(Resource.Id.btn_offline_mode);
        }

        private void InitListener()
        {
            btn_online_mode.SetOnClickListener(this);
            btn_offline_mode.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.btn_online_mode:
                    StartActivity(new Intent(this, typeof(TtsOnlineModeActivity)));
                    break;
                case Resource.Id.btn_offline_mode:
                    StartActivity(new Intent(this, typeof(TtsOfflineModeActivity)));
                    break;
            }
        }

    }
}