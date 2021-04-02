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
using Huawei.Hms.Mlplugin.Card.Bcr;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.BankCard
{
    /// <summary>
    /// It provides the identification function of the bank card,
    /// and recognizes formatted text information from the images with bank card information.
    /// Bank Card identification provides on-device API.
    /// </summary>
    [Activity(Label = "BcrAnalyseActivity")]
    public class BcrAnalyseActivity : Activity ,View.IOnClickListener
    {
        private const string Tag = "BcrAnalyseActivity";

        private int CameraPermissionCode = 1;
        private int ReadExternalStorageCode = 2;
        public  int RequestCodeBk = 3;

        private TextView mTextView;
        private ImageView previewImage;
        private MLBcrCaptureCallback banCallback;
        private string cardResultFront = "";
        private Button customViewButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_bcr_analyse);
            this.mTextView = (Android.Widget.TextView)FindViewById(Resource.Id.text_result);
            this.previewImage = (Android.Widget.ImageView)this.FindViewById(Resource.Id.Bank_Card_image);
            this.previewImage.SetScaleType(ImageView.ScaleType.FitXy);
            this.FindViewById(Resource.Id.detect).SetOnClickListener(this);
            this.previewImage.SetOnClickListener(this);
            this.banCallback = new MLBcrCaptureCallback(this);
            this.customViewButton = (Button)FindViewById(Resource.Id.custom_view_button);
            this.customViewButton.SetOnClickListener(this);
            //Checking permissions
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted))
            {
                this.requestPermissions();
            }
            if (!(ActivityCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Android.Content.PM.Permission.Granted))
            {
                this.requestPermissions();
            }
        }

        //Request permissions
        private void requestPermissions()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage };
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, CameraPermissionCode);
                return;
            }
            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadExternalStorage))
            {
                ActivityCompat.RequestPermissions(this, permissions, ReadExternalStorageCode);
                return;
            }
        }

        public void OnClick(View v)
        {
            if(v.Id == Resource.Id.Bank_Card_image
                || v.Id == Resource.Id.detect)
            {

                this.mTextView.Text = "";
                this.StartCaptureActivity(this.banCallback);
            }
            else
            {
                            // Jump to custom interface
            Intent intent = new Intent(this.ApplicationContext, typeof(CustomActivity));
            StartActivityForResult(intent, RequestCodeBk);
            }

        }

        /// <summary>
        /// CustomView activity result
        /// </summary>
        /// <param name="requestCode"></param>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (requestCode == RequestCodeBk)
                {
                    Bitmap bitmap = (Bitmap)data.GetParcelableExtra("bitmap");
                    this.previewImage.SetImageBitmap(bitmap);
                    this.cardResultFront = FormatIdCardResult(data);
                    this.mTextView.Text = this.cardResultFront;
                }
            }
        }

        /// <summary>
        /// Intent formatting from custom view
        /// </summary>
        /// <param name="intent"></param>
        private string FormatIdCardResult(Intent intent)
        {
            StringBuilder resultBuilder = new StringBuilder();

            resultBuilder.Append("Number：");
            resultBuilder.Append(intent.GetStringExtra("number"));
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Issuer：");
            resultBuilder.Append(intent.GetStringExtra("Issuer"));
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Expire: ");
            resultBuilder.Append(intent.GetStringExtra("Expire"));
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Type: ");
            resultBuilder.Append(intent.GetStringExtra("Type"));
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Organization: ");
            resultBuilder.Append(intent.GetStringExtra("Organization"));
            resultBuilder.Append("\r\n");

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Formatting bankCardResult
        /// </summary>
        /// <param name="bankCardResult"></param>
        private string FormatIdCardResult(MLBcrCaptureResult bankCardResult)
        {
            StringBuilder resultBuilder = new StringBuilder();

            resultBuilder.Append("Number：");
            resultBuilder.Append(bankCardResult.Number);
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Issuer：");
            resultBuilder.Append(bankCardResult.Issuer);
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Expire: ");
            resultBuilder.Append(bankCardResult.Expire);
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Type: ");
            resultBuilder.Append(bankCardResult.Type);
            resultBuilder.Append("\r\n");

            resultBuilder.Append("Organization: ");
            resultBuilder.Append(bankCardResult.Organization);
            resultBuilder.Append("\r\n");

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Set the recognition parameters, call the recognizer capture interface for recognition,
        /// and the recognition result will be returned through the callback function.
        /// </summary>
        /// <param name="Callback">The callback of bank cards analyse.</param>
        private void StartCaptureActivity(MLBcrCaptureCallback Callback)
        {
            MLBcrCaptureConfig config = new MLBcrCaptureConfig.Factory()
                    // Set the expected result type of bank card recognition.
                    .SetResultType(MLBcrCaptureConfig.ResultAll)
                    // Set the screen orientation of the plugin page.
                    .SetOrientation(MLBcrCaptureConfig.OrientationAuto)
                    .Create();
            MLBcrCapture bcrCapture = MLBcrCaptureFactory.Instance.GetBcrCapture(config);
            bcrCapture.CaptureFrame(this, Callback);
        }

        /// <summary>
        /// displays failure message in TextView
        /// </summary>
        private void DisplayFailure()
        {
            this.mTextView.Text = "Failure";
        }

        /// <summary>
        /// Use the bank card pre-processing plug-in to identify video stream bank cards.
        /// Create a recognition result callback function to process the identification result of the card.
        /// </summary>
        public class MLBcrCaptureCallback : Java.Lang.Object, MLBcrCapture.ICallback
        {
            private BcrAnalyseActivity bcrAnalyseActivity;
            public MLBcrCaptureCallback(BcrAnalyseActivity BcrAnalyseActivity)
            {
                this.bcrAnalyseActivity = BcrAnalyseActivity;
            }
            public void OnCanceled()
            {
                Log.Info(BcrAnalyseActivity.Tag, "CallBackonRecCanceled");
            }

            public void OnDenied()
            {
                this.bcrAnalyseActivity.DisplayFailure();
                Log.Info(BcrAnalyseActivity.Tag, "CallBackonCameraDenied");
            }

            public void OnFailure(int errCode, Bitmap bitmap)
            {
                this.bcrAnalyseActivity.DisplayFailure();
                Log.Info(BcrAnalyseActivity.Tag, "CallBackonRecFailed");
            }

            public void OnSuccess(MLBcrCaptureResult result)
            {
                Log.Info(BcrAnalyseActivity.Tag, "CallBack onRecSuccess");
                if (result == null)
                {
                    Log.Info(BcrAnalyseActivity.Tag, "CallBack onRecSuccess idCardResult is null");
                    return;
                }
                Bitmap bitmap = result.OriginalBitmap;
                this.bcrAnalyseActivity.previewImage.SetImageBitmap(bitmap);
                this.bcrAnalyseActivity.cardResultFront = this.bcrAnalyseActivity.FormatIdCardResult(result);
                this.bcrAnalyseActivity.mTextView.Text= this.bcrAnalyseActivity.cardResultFront;
            }
        }
    }
}