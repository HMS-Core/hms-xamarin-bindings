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
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Mlsdk.Livenessdetection;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.LivenessDetection
{
    [Activity(Label = "LiveLivenessDetectionActivity")]
    public class LiveLivenessDetectionActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "LiveLivenessDetectionActivity";

        private static readonly int RcCameraAndExternalStorage = 0x01 << 8;
        private static readonly string[] Permissions = { Manifest.Permission.Camera };

        public Button mBtn;
        public TextView mTextResult;
        public ImageView mImageResult;
        private MLLivenessCaptureCallback callback;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_live_liveness_detection);

            mBtn = (Button) this.FindViewById(Resource.Id.capture_btn);
            mTextResult = (TextView) this.FindViewById(Resource.Id.text_detect_result);
            mImageResult = (ImageView) this.FindViewById(Resource.Id.img_detect_result);
            mBtn.SetOnClickListener(this);

            callback = new MLLivenessCaptureCallback(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.capture_btn:
                    if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
                    {
                        StartCaptureActivity();
                        return;
                    }
                    ActivityCompat.RequestPermissions(this, Permissions, RcCameraAndExternalStorage);
                    break;
            }
        }

        /// <summary>
        /// Start camera stream capturing
        /// </summary>
        private void StartCaptureActivity()
        {
            try
            {
                MLLivenessCapture capture = MLLivenessCapture.Instance;
                capture.StartDetect(this, this.callback);
            }
            catch(Exception e)
            {
                Log.Error(Tag, " startCaptureActivity failed: " + e.Message);
            }
        }

        /// <summary>
        /// Change display text
        /// </summary>
        /// <param name="text"></param>
        public void ChangeText(string text)
        {
            this.mTextResult.Text = text;
        }

        /// <summary>
        /// Handle request permission result.
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Log.Info(Tag, "onRequestPermissionsResult ");
            if (requestCode == RcCameraAndExternalStorage && grantResults.Length > 0 && grantResults[0] == Permission.Granted)
            {
                StartCaptureActivity();
            }
        }

        /// <summary>
        /// Handle Activity result.
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            Log.Info(Tag, "onActivityResult requestCode " + requestCode + ", resultCode " + resultCode);
        }

    }

    /// <summary>
    /// Liveness Detection Capture Callback Class
    /// </summary>
    public class MLLivenessCaptureCallback : Java.Lang.Object, MLLivenessCapture.ICallback
    {
        private LiveLivenessDetectionActivity mActivity;
        public MLLivenessCaptureCallback(LiveLivenessDetectionActivity livenessActivity)
        {
            this.mActivity = livenessActivity;
        }
        public void OnFailure(int errorCode)
        {
            this.mActivity.ChangeText("errorCode:" + errorCode);
        }

        public void OnSuccess(MLLivenessCaptureResult result)
        {
            this.mActivity.ChangeText(result.ToString());
            this.mActivity.mTextResult.SetBackgroundResource(result.IsLive ? Resource.Drawable.bg_blue : Resource.Drawable.bg_red);
            this.mActivity.mImageResult.SetImageBitmap(result.Bitmap);
        }
    }
}