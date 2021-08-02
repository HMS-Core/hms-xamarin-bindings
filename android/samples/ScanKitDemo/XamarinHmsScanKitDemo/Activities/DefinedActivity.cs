/**
 * Copyright 2020. Huawei Technologies Co., Ltd. All rights reserved.
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 */
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;
using System;
using XamarinHmsScanKitDemo.Utils;

namespace XamarinHmsScanKitDemo.Activities
{
    [Activity(Label = "DefinedActivity")]
    public class DefinedActivity : Activity
    {
        private FrameLayout frameLayout;
        private RemoteView remoteView;
        private ImageView backButton, imgButton, flashButton;
        int screenWidth;
        int screenHeight;
        private TextView pauseButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_defined);
            // Bind the camera preview layout.
            frameLayout = FindViewById<FrameLayout>(Resource.Id.rim);
            // Set the scanning area. Set the parameters as required.
            DisplayMetrics dm = Resources.DisplayMetrics;
            float density = dm.Density;
            screenWidth = dm.WidthPixels;
            screenHeight = dm.HeightPixels;
            // Set the width and height of the barcode scanning box to 300 dp.
            const int ScanFrameSize = 300;
            int scanFrameSize = (int)(ScanFrameSize * density);
            Rect rect = new Rect();
            rect.Left = screenWidth / 2 - scanFrameSize / 2;
            rect.Right = screenHeight / 2 + scanFrameSize / 2;
            rect.Top = screenHeight / 2 - scanFrameSize / 2;
            rect.Bottom = screenHeight / 2 + scanFrameSize / 2;
            // Initialize the remote view. Use SetContext() to pass the context (mandatory), use SetBoundingBox() to set the scanning area, and use SetFormat() to set the barcode format. Then call the Build() method to create the remote view. Set the non-consecutive scanning mode using the SetContinuouslyScan method (optional).

            remoteView = new RemoteView.Builder().SetContext(this).SetBoundingBox(rect).SetContinuouslyScan(false).SetFormat(HmsScan.QrcodeScanType, HmsScan.DatamatrixScanType).Build();
            Console.WriteLine("RemoteView created");
            //Call back this API under dim lighting. The sample code is used to control the flashlight button.
            remoteView.SetOnLightVisibleCallback(new OnLightVisibleCallback(this));
            remoteView.SetOnResultCallback(new RemoteViewResultCallback(this));
            // Load the customized view to the activity.
            remoteView.OnCreate(savedInstanceState);
            FrameLayout.LayoutParams layoutParams = new FrameLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.MatchParent);
            frameLayout.AddView(remoteView, layoutParams);

            // Set the back, photo scanning, and flashlight operations.
            imgButton = FindViewById<ImageView>(Resource.Id.img_btn);
            imgButton.Click += ImgButton_Click;
            flashButton = FindViewById<ImageView>(Resource.Id.flash_btn);
            flashButton.Click += FlashButton_Click;
            backButton = FindViewById<ImageView>(Resource.Id.back_img);
            backButton.Click += BackButton_Click;
            pauseButton = FindViewById<TextView>(Resource.Id.btn_pause);
            pauseButton.Click += PauseButton_Click;
        }

        private void FlashButton_Click(object sender, EventArgs e)
        {
            if (remoteView.LightStatus)
            {
                remoteView.SwitchLight();
                flashButton.SetImageResource(Resource.Drawable.flashlight_off);
            }
            else
            {
                remoteView.SwitchLight();
                flashButton.SetImageResource(Resource.Drawable.flashlight_on);
            }
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (pauseButton.Text== "Pause Scan")
            {
                pauseButton.Text = "Resume Scan";
                remoteView.PauseContinuouslyScan();
            }
            else
            {
                pauseButton.Text = "Pause Scan";
                remoteView.ResumeContinuouslyScan();
            }
        }

        private void ImgButton_Click(object sender, EventArgs e)
        {
            Intent pickIntent = new Intent(Intent.ActionPick,
                         MediaStore.Images.Media.ExternalContentUri);
            pickIntent.SetDataAndType(MediaStore.Images.Media.ExternalContentUri, "image/*");
            StartActivityForResult(pickIntent, Constants.RequestCodePhoto);
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Finish();
        }

        protected override void OnStart()
        {
            base.OnStart();
            remoteView.OnStart();

        }

        protected override void OnResume()
        {
            base.OnResume();
            remoteView.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            remoteView.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            remoteView.OnDestroy();
        }


        protected override void OnStop()
        {
            base.OnStop();
            remoteView.OnStop();
        }

        //Handle the return results from the album.
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == Constants.RequestCodePhoto)
            {
                try
                {
                    Bitmap bitmap = MediaStore.Images.Media.GetBitmap(this.ContentResolver, data.Data);

                    HmsScan[] hmsScans = ScanUtil.DecodeWithBitmap(this, bitmap, new HmsScanAnalyzerOptions.Creator().SetPhotoMode(true).Create());
                    if (hmsScans != null && hmsScans.Length > 0 && hmsScans[0] != null && !string.IsNullOrEmpty(hmsScans[0].OriginalValue))
                    {
                        Intent intent = new Intent();
                        intent.PutExtra("scanResult", hmsScans[0]);
                        this.SetResult(Result.Ok, intent);
                        this.Finish();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }

    public class RemoteViewResultCallback : Java.Lang.Object, IOnResultCallback
    {
        private DefinedActivity definedActivity;

        public RemoteViewResultCallback(DefinedActivity definedActivity)
        {
            this.definedActivity = definedActivity;
        }

        public void OnResult(HmsScan[] result)
        {
            //Check the result.
            if (result != null && result.Length > 0 && result[0] != null && !string.IsNullOrEmpty(result[0].OriginalValue))
            {
                Intent intent = new Intent();
                intent.PutExtra("scanResult", result[0]);
                definedActivity.SetResult(Result.Ok, intent);
                definedActivity.Finish();
            }
        }
    }

    public class OnLightVisibleCallback : Java.Lang.Object, IOnLightVisibleCallBack
    {
        private DefinedActivity definedActivity;

        public OnLightVisibleCallback(DefinedActivity definedActivity)
        {
            this.definedActivity = definedActivity;
        }

        public void OnVisibleChanged(bool visible)
        {
            if (visible)
            {
                // definedActivity.FindViewById<ImageView>(Resource.Id.flash_btn).Visibility = Android.Views.ViewStates.Visible;
            }
        }
    }
}