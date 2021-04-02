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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlplugin.Card.Gcr;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.GeneralCard
{
    [Activity(Label = "GcrAnalyseActivity")]
    public class GcrAnalyseActivity : Activity , View.IOnClickListener
    {
        private const string Tag = "GcrAnalyseActivity";

        private int CameraPermissionCode = 1;

        private TextView mTextView;
        private ImageView previewImage;
        private Bitmap cardImage;
        private int processMode;
        private MLGcrCaptureCallback callback;
        private  readonly int HKIdProcess = 1;
        private  readonly int HomeCardProcess = 2;
        private  readonly int PassCardProcess = 3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_gcr_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.text_result);
            this.previewImage = (Android.Widget.ImageView)FindViewById(Resource.Id.Card_image);
            this.previewImage.SetOnClickListener(this);
            this.previewImage.SetScaleType(ImageView.ScaleType.FitXy);
            this.FindViewById(Resource.Id.detect_picture_HKID).SetOnClickListener(this);
            this.FindViewById(Resource.Id.detect_picture_homeCard).SetOnClickListener(this);
            this.FindViewById(Resource.Id.detect_picture_passCard).SetOnClickListener(this);
            this.FindViewById(Resource.Id.detect_video_HKID).SetOnClickListener(this);
            this.FindViewById(Resource.Id.detect_video_homeCard).SetOnClickListener(this);
            this.FindViewById(Resource.Id.detect_video_passCard).SetOnClickListener(this);

            this.callback = new MLGcrCaptureCallback(this);
            //Checking Camera permission
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted))
            {
                this.RequestCameraPermission();
            }
        }
        //Request permission
        private void RequestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage };
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, CameraPermissionCode);
                return;
            }
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.detect_picture_passCard:
                    this.mTextView.Text = "";
                    this.processMode = this.PassCardProcess;
                    this.StartLocalImageActivity(this.cardImage, null, this.callback);
                    break;
                case Resource.Id.detect_video_passCard:
                    this.mTextView.Text = "";
                    this.processMode = this.PassCardProcess;
                    this.StartCaptureActivity(null, this.callback);
                    break;
                case Resource.Id.detect_picture_HKID:
                    this.mTextView.Text = "";
                    this.processMode = this.HKIdProcess;
                    this.StartLocalImageActivity(this.cardImage, null, this.callback);
                    break;
                case Resource.Id.detect_video_HKID:
                    this.mTextView.Text = "";
                    this.processMode = this.HKIdProcess;
                    this.StartCaptureActivity(null, this.callback);
                    break;
                case Resource.Id.detect_picture_homeCard:
                    this.mTextView.Text = "";
                    this.processMode = this.HomeCardProcess;
                    this.StartLocalImageActivity(this.cardImage, null, this.callback);
                    break;
                case Resource.Id.detect_video_homeCard:
                    this.mTextView.Text = "";
                    this.processMode = this.HomeCardProcess;
                    this.StartCaptureActivity(null, this.callback);
                    break;
                case Resource.Id.Card_image:
                    this.mTextView.Text = "";
                    this.processMode = this.PassCardProcess;
                    this.StartTakePhotoActivity(null, this.callback);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Display result in TextView
        /// </summary>
        private void DisplaySuccess(UniversalCardResult mlIdCard)
        {
            System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder();
            resultBuilder.Append("IDNum: " + mlIdCard.number + "\r\n");
            resultBuilder.Append("ValidDate: " + mlIdCard.valid + "\r\n");
            this.mTextView.Text=resultBuilder.ToString();
        }

        private void DisplayFailure()
        {
            this.mTextView.Text = "Failure";
        }

        /// <summary>
        /// Use the card recognition plugin to identify cards.
        /// Create a recognition result callback function to process the identification result of the card.
        /// </summary>
        public class MLGcrCaptureCallback : Java.Lang.Object, MLGcrCapture.ICallback
        {
            private GcrAnalyseActivity gcrAnalyseActivity;
            public MLGcrCaptureCallback(GcrAnalyseActivity GcrAnalyseActivity)
            {
                this.gcrAnalyseActivity = GcrAnalyseActivity;
            }
            public void OnCanceled()
            {
                Log.Info(GcrAnalyseActivity.Tag, "callback onRecCanceled");
            }

            public void OnDenied()
            {
                this.gcrAnalyseActivity.DisplayFailure();
                Log.Info(GcrAnalyseActivity.Tag, "callback onCameraDenied");
            }

            public void OnFailure(int errcode, Bitmap bitmap)
            {
                this.gcrAnalyseActivity.DisplayFailure();
                Log.Info(GcrAnalyseActivity.Tag, "callback onFailure");
            }

            public int OnResult(MLGcrCaptureResult result, Java.Lang.Object obj)
            {
                Log.Info(GcrAnalyseActivity.Tag, "callback onRecSuccess");
                if (result == null)
                {
                    Log.Info(GcrAnalyseActivity.Tag, "callback onRecSuccess result is null");
                    // If result is empty, return MLGcrCaptureResult.CaptureContinue, and the detector will continue to detect.
                    return MLGcrCaptureResult.CaptureContinue;
                }
                UniversalCardResult cardResult = null;

                switch (this.gcrAnalyseActivity.processMode)
                {
                    case 3:
                        PassCardProcess passCard = new PassCardProcess(result.Text);
                        if (passCard != null)
                        {
                            cardResult = passCard.GetResult();
                        }
                        break;
                    case 1:
                        HKIdCardProcess HKIDCard = new HKIdCardProcess(result.Text);
                        if (HKIDCard != null)
                        {
                            cardResult = HKIDCard.GetResult();
                        }
                        break;
                    case 2:
                        HomeCardProcess homeCard = new HomeCardProcess(result.Text);
                        if (homeCard != null)
                        {
                            cardResult = homeCard.GetResult();
                        }
                        break;
                    default:
                        break;
                }

                if (cardResult == null || string.IsNullOrEmpty(cardResult.valid) || string.IsNullOrEmpty(cardResult.number))
                {
                    // If detection is not successful, return MLGcrCaptureResult.CaptureContinue, and the detector will continue to detect.
                    return MLGcrCaptureResult.CaptureContinue;
                }
                this.gcrAnalyseActivity.cardImage = result.CardBitmap;
                this.gcrAnalyseActivity.previewImage.SetImageBitmap(this.gcrAnalyseActivity.cardImage);
                this.gcrAnalyseActivity.DisplaySuccess(cardResult);
                // If detection is successful, return MLGcrCaptureResult.CaptureStop, and the detector will stop to detect.
                return MLGcrCaptureResult.CaptureStop;
            }
        }

        /// <summary>
        /// Use the plug-in to take a picture of the card and recognize.
        /// </summary>
        /// <param name="myObj"></param>
        /// <param name="callback"></param>
        private void StartTakePhotoActivity(Java.Lang.Object myObj, MLGcrCaptureCallback callback)
        {
            MLGcrCaptureConfig cardConfig = new MLGcrCaptureConfig.Factory().Create();
            MLGcrCaptureUIConfig uiConfig = new MLGcrCaptureUIConfig.Factory()
                    .SetScanBoxCornerColor(Color.Blue)
                    .SetTipText("Taking EEP to HK/Macau picture")
                    .SetOrientation(MLGcrCaptureUIConfig.OrientationAuto).Create();
            // Create a general card identification processor using the custom interface.
            MLGcrCapture ocrManager = MLGcrCaptureFactory.Instance.GetGcrCapture(cardConfig, uiConfig);

        // Create a general card identification processor using the default interface.
        // MLGcrCapture ocrManager = MLGcrCaptureFactory.Instance.GetGcrCapture(cardConfig);

        ocrManager.CapturePhoto(this, myObj, callback);
        }
        /// <summary>
        /// Detect input card bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="myObj"></param>
        /// <param name="callback"></param>
        private void StartLocalImageActivity(Bitmap bitmap, Java.Lang.Object myObj, MLGcrCaptureCallback callback)
        {
            if (bitmap == null)
            {
                this.mTextView.Text = "No card image to recognition.";
                return;
            }
            MLGcrCaptureConfig config = new MLGcrCaptureConfig.Factory().Create();
            MLGcrCapture ocrManager = MLGcrCaptureFactory.Instance.GetGcrCapture(config);
            ocrManager.CaptureImage(bitmap, myObj, callback);
        }
        /// <summary>
        /// Set the recognition parameters, call the recognizer capture interface for recognition,
        /// and the recognition result will be returned through the callback function.
        /// </summary>
        /// <param name="myObj"></param>
        /// <param name="callBack">The callback of cards analyse.</param>
        private void StartCaptureActivity(Java.Lang.Object myObj, MLGcrCaptureCallback callBack)
        {
            MLGcrCaptureConfig cardConfig = new MLGcrCaptureConfig.Factory().Create();
            MLGcrCaptureUIConfig uiConfig = new MLGcrCaptureUIConfig.Factory()
                    .SetScanBoxCornerColor(Color.Green)
                    .SetTipText("Recognizing, align edges")
                    .SetOrientation(MLGcrCaptureUIConfig.OrientationAuto).Create();
            MLGcrCapture ocrManager = MLGcrCaptureFactory.Instance.GetGcrCapture(cardConfig, uiConfig);
            ocrManager.CapturePreview(this, myObj, callBack);
        }
    }
}