/*
       Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;


namespace XamarinHmsHealthDemo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button LoginBtn;

        private Button DataControlBtn;

        private Button AutoRecorder;

        private Button ActivityRecords;

        private Button SettingConroller;

        private Button BLEConroller;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Main_layout);

            LoginBtn = FindViewById<Button>(Resource.Id.LoginBtn);
            DataControlBtn = FindViewById<Button>(Resource.Id.DataControlBtn);
            AutoRecorder = FindViewById<Button>(Resource.Id.AutoRecorderBtn);
            ActivityRecords = FindViewById<Button>(Resource.Id.ActivityRecordsBtn);
            SettingConroller = FindViewById<Button>(Resource.Id.SettingConrollerBtn);
            BLEConroller = FindViewById<Button>(Resource.Id.BleConrollerBtn);

            LoginBtn.Click += delegate {
                Intent intent = new Intent(this, typeof(AuthClientActivity));
                StartActivity(intent);
            };
            DataControlBtn.Click += delegate {
                Intent intent = new Intent(this, typeof(DataControllerActivity));
                StartActivity(intent);
            };
            AutoRecorder.Click += delegate {
                Intent intent = new Intent(this, typeof(AutoRecorderActivity));
                StartActivity(intent);
            };
            ActivityRecords.Click += delegate {
                Intent intent = new Intent(this, typeof(ActivityRecordsActivity));
                StartActivity(intent);
            };
            SettingConroller.Click += delegate {
                Intent intent = new Intent(this, typeof(SettingControllerActivity));
                StartActivity(intent);
            };
            BLEConroller.Click += delegate {
                Intent intent = new Intent(this, typeof(BLEControllerActivity));
                StartActivity(intent);
            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


    }
}