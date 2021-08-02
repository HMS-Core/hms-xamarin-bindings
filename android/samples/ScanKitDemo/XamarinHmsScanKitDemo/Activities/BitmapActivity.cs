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
using System;
using System.Linq;


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Huawei.Hmf.Tasks;
using Huawei.Hms.Hmsscankit;
using Huawei.Hms.Ml.Scan;
using Huawei.Hms.Mlsdk.Common;
using XamarinHmsScanKitDemo.Draw;
using XamarinHmsScanKitDemo.Utils;

namespace XamarinHmsScanKitDemo.Activities
{
    [Activity(Label = "BitmapActivity")]
    public class BitmapActivity : Activity
    {
        public int defaultValue = -1;
        public ISurfaceHolder surfaceHolder;
        public CameraOperation cameraOperation;
        public SurfaceCallback surfaceCallBack;
        public CommonHandler handler;
        public bool isShow;
        public int mode;
        public ImageView backButton, imgButton, scanArs;
        public TextView scanTips;

        public ScanResultView scanResultView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window window = Window;
            window.AddFlags(WindowManagerFlags.KeepScreenOn);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_common);
            mode = Intent.GetIntExtra(Constants.DecodeMode, defaultValue);
            scanArs = FindViewById<ImageView>(Resource.Id.scan_ars);
            scanTips = FindViewById<TextView>(Resource.Id.scan_tip);
            if (mode == Constants.MultiProcessorAsyncCode || mode == Constants.MultiProcessorSyncCode)
            {
                scanArs.Visibility = ViewStates.Invisible;
                scanTips.Text = GetString(Resource.String.scan_showresult);
                AlphaAnimation disappearAnimation = new AlphaAnimation(1, 0);
                disappearAnimation.SetAnimationListener(new AnimationListener(this));

                disappearAnimation.Duration = 3000;
                scanTips.StartAnimation(disappearAnimation);
            }
            cameraOperation = new CameraOperation();
            surfaceCallBack = new SurfaceCallback(this);
            SurfaceView cameraPreview = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            AdjustSurface(cameraPreview);
            surfaceHolder = cameraPreview.Holder;
            isShow = false;
            FindViewById<ImageView>(Resource.Id.img_btn).Click += PictureButton_Click;
            FindViewById<ImageView>(Resource.Id.back_img).Click += BackButton_Click; ;

            scanResultView = FindViewById<ScanResultView>(Resource.Id.scan_result_view);
        }

        private void PictureButton_Click(object sender, EventArgs e)
        {
            Intent pickIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
            pickIntent.SetDataAndType(MediaStore.Images.Media.ExternalContentUri, "image/*");
            StartActivityForResult(pickIntent, Constants.RequestCodePhoto);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (mode == Constants.MultiProcessorAsyncCode || mode == Constants.MultiProcessorSyncCode)
            {
                SetResult(Result.Canceled);
            }
            Finish();
        }

        private void AdjustSurface(SurfaceView cameraPreview)
        {
            FrameLayout.LayoutParams paramSurface = (FrameLayout.LayoutParams)cameraPreview.LayoutParameters;
            if (GetSystemService(Context.WindowService) != null)
            {
                var windowManager = GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
                Display defaultDisplay = windowManager.DefaultDisplay;
                Point outPoint = new Point();
                defaultDisplay.GetRealSize(outPoint);
                float sceenWidth = outPoint.X;
                float sceenHeight = outPoint.Y;
                float rate;
                if (sceenWidth / (float)1080 > sceenHeight / (float)1920)
                {
                    rate = sceenWidth / (float)1080;
                    int targetHeight = (int)(1920 * rate);
                    paramSurface.Width = FrameLayout.LayoutParams.MatchParent;
                    paramSurface.Height = targetHeight;
                    int topMargin = (int)(-(targetHeight - sceenHeight) / 2);
                    if (topMargin < 0)
                    {
                        paramSurface.TopMargin = topMargin;
                    }
                }
                else
                {
                    rate = sceenHeight / (float)1920;
                    int targetWidth = (int)(1080 * rate);
                    paramSurface.Width = targetWidth;
                    paramSurface.Height = FrameLayout.LayoutParams.MatchParent;
                    int leftMargin = (int)(-(targetWidth - sceenWidth) / 2);
                    if (leftMargin < 0)
                    {
                        paramSurface.LeftMargin = leftMargin;
                    }
                }
            }
        }


        internal void InitCamera()
        {
            try
            {
                cameraOperation.Open(surfaceHolder);
                if (handler == null)
                {
                    handler = new CommonHandler(this, cameraOperation, mode);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok || data == null || requestCode != Constants.RequestCodePhoto)
            {
                return;
            }
            try
            {
                // Image-based scanning mode
                if (mode == Constants.BitmapCode)
                {
                    DecodeBitmap(MediaStore.Images.Media.GetBitmap(ContentResolver, data.Data), HmsScan.AllScanType);
                }
                else if (mode == Constants.MultiProcessorSyncCode)
                {
                    DecodeMultiSyn(MediaStore.Images.Media.GetBitmap(ContentResolver, data.Data));
                }
                else if (mode == Constants.MultiProcessorAsyncCode)
                {
                    DecodeMultiAsyn(MediaStore.Images.Media.GetBitmap(ContentResolver, data.Data));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DecodeMultiSyn(Bitmap bitmap)
        {
            MLFrame image = MLFrame.FromBitmap(bitmap);
            HmsScanAnalyzer analyzer = new HmsScanAnalyzer.Creator(this).SetHmsScanTypes(HmsScan.AllScanType).Create();
            SparseArray result = analyzer.AnalyseFrame(image);
            if (result != null && result.Size() > 0 && result.ValueAt(0) != null && !string.IsNullOrEmpty(((HmsScan)result.ValueAt(0)).OriginalValue))
            {
                HmsScan[] info = new HmsScan[result.Size()];
                for (int index = 0; index < result.Size(); index++)
                {
                    info[index] = (HmsScan)result.ValueAt(index);
                }
                Intent intent = new Intent();
                intent.PutExtra(Constants.ScanResult, info);
                SetResult(Result.Ok, intent);
                Finish();
            }
        }
        private void DecodeMultiAsyn(Bitmap bitmap)
        {
            MLFrame image = MLFrame.FromBitmap(bitmap);
            HmsScanAnalyzer analyzer = new HmsScanAnalyzer.Creator(this).SetHmsScanTypes(HmsScan.AllScanType).Create();
            analyzer.AnalyzInAsyn(image).AddOnSuccessListener(new DecodeMultiSynSuccessListener(this)).AddOnFailureListener(new DecodeMultiSynFailListener(this));
        }
        private void DecodeBitmap(Bitmap bitmap, int type)
        {
            HmsScan[] hmsScans = ScanUtil.DecodeWithBitmap(this, bitmap, new HmsScanAnalyzerOptions.Creator().SetHmsScanTypes(type).SetPhotoMode(true).Create());
            if (hmsScans != null && hmsScans.Length > 0 && hmsScans.Any() && !string.IsNullOrEmpty(hmsScans.FirstOrDefault().OriginalValue))
            {
                Intent intent = new Intent();
                intent.PutExtra(Constants.ScanResult, hmsScans);
                SetResult(Result.Ok, intent);
                Finish();
            }
        }

        protected override void OnPause()
        {
            if (handler != null)
            {
                handler.Quit();
                handler = null;
            }
            cameraOperation.Close();
            if (!isShow)
            {
                surfaceHolder.RemoveCallback(surfaceCallBack);
            }
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (isShow)
            {
                InitCamera();
            }
            else
            {
                surfaceHolder.AddCallback(surfaceCallBack);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

    public class AnimationListener : Java.Lang.Object, Animation.IAnimationListener
    {
        private BitmapActivity bitmapActivity;

        public AnimationListener(BitmapActivity bitmapActivity)
        {
            this.bitmapActivity = bitmapActivity;
        }

        public void OnAnimationEnd(Animation animation)
        {
            if (bitmapActivity.scanTips != null)
            {
                bitmapActivity.scanTips.Visibility = ViewStates.Gone;
            }
        }

        public void OnAnimationRepeat(Animation animation)
        {
        }

        public void OnAnimationStart(Animation animation)
        {
        }
    }

    public class SurfaceCallback : Java.Lang.Object, ISurfaceHolderCallback
    {
        private BitmapActivity bitmapActivity;

        public SurfaceCallback(BitmapActivity bitmapActivity)
        {
            this.bitmapActivity = bitmapActivity;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {

        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (!bitmapActivity.isShow)
            {
                bitmapActivity.isShow = true;
                bitmapActivity.InitCamera();
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            bitmapActivity.isShow = false;
        }
    }

    public class DecodeMultiSynSuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public BitmapActivity bitmapActivity;

        public DecodeMultiSynSuccessListener(BitmapActivity bitmapActivity)
        {
            this.bitmapActivity = bitmapActivity;
        }

        public void OnSuccess(Java.Lang.Object obj)
        {
            var hmsScans = (obj as System.Collections.IList).Cast<object>().Select(x => x as HmsScan).ToArray();
            if (hmsScans != null && hmsScans.Length > 0 && !string.IsNullOrEmpty(hmsScans[0].OriginalValue))
            {
                Intent intent = new Intent();
                intent.PutExtra(Constants.ScanResult, hmsScans);
                bitmapActivity.SetResult(Result.Ok, intent);
                bitmapActivity.Finish();
            }
        }
    }

    public class DecodeMultiSynFailListener : Java.Lang.Object, IOnFailureListener
    {
        public BitmapActivity bitmapActivity;

        public DecodeMultiSynFailListener(BitmapActivity bitmapActivity)
        {
            this.bitmapActivity = bitmapActivity;
        }

        public void OnFailure(Java.Lang.Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}