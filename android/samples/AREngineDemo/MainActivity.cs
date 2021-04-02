/*
*       Copyright 2020-2021 Huawei Technologies Co., Ltd. All rights reserved.

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
using Android.Views;
using XamarinAREngineDemo.AREngineActivities.Face;
using XamarinAREngineDemo.AREngineActivities.Body3d;
using XamarinAREngineDemo.AREngineActivities.Hand;
using XamarinAREngineDemo.AREngineActivities.World;
using XamarinAREngineDemo.Common;
using Android.Util;
using Android.Content.PM;

namespace XamarinAREngineDemo
{
    /// <summary>
    /// This class provides the permission verification and sub-AR example redirection functions.
    /// </summary>
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity , ViewAnimator.IOnClickListener
    {
        private const string TAG = "MainActivity";

        private bool isFirstClick = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            this.FindViewById(Resource.Id.btn_WorldAR).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_FaceAR).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_body3d).SetOnClickListener(this);
            this.FindViewById(Resource.Id.btn_hand).SetOnClickListener(this);

            // AR Engine requires the camera permission.
            PermissionManager.CheckPermission(this);
        }

        /// <summary>
        /// Choose AR Engine activity.
        /// </summary>
        /// <param name="v">View.</param>
        public void OnClick(View v)
        {
            if (!isFirstClick)
            {
                return;
            }
            else
            {
                isFirstClick = false;
            }
            switch (v.Id)
            {
                case Resource.Id.btn_WorldAR:
                    this.StartActivity(new Android.Content.Intent(this, typeof(WorldActivity)));
                    break;
                case Resource.Id.btn_FaceAR:
                    this.StartActivity(new Android.Content.Intent(this, typeof(FaceActivity)));
                    break;
                case Resource.Id.btn_body3d:
                    this.StartActivity(new Android.Content.Intent(this, typeof(BodyActivity)));
                    break;
                case Resource.Id.btn_hand:
                    this.StartActivity(new Android.Content.Intent(this, typeof(HandActivity)));
                    break;
                default:
                    Log.Error(TAG, "OnClick error!");
                    break;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (!PermissionManager.HasPermission(this))
            {
                Toast.MakeText(this, "This application needs camera permission.", ToastLength.Long).Show();
                Finish();
            }
        }

        protected override void OnResume()
        {
            Log.Debug(TAG, "OnResume");
            base.OnResume();
            isFirstClick = true;
        }

        protected override void OnDestroy()
        {
            Log.Debug(TAG, "OnDestroy start.");
            base.OnDestroy();
            Log.Debug(TAG, "OnDestroy end.");
        }

    }
}