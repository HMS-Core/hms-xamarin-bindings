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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlplugin.Card.Bcr;

namespace HmsXamarinMLDemo.MLKitActivities.TextRelated.BankCard
{
    /// <summary>
    /// Custom scan interface activity
    /// </summary>
    [Activity(Label = "CustomActivity")]
    public class CustomActivity : Activity
    {
        private const string Tag = "CustomActivity";

        private static readonly double TopOffsetRatio = 0.4;
        private FrameLayout linearLayout;
        private CustomView remoteView;
        private ViewfinderView viewfinderView;
        private View light_layout;
        private ImageView img;
        bool isLight = false;
        CustomViewBcrResultCallback callback;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_bcr_customview);
            linearLayout = (FrameLayout)FindViewById(Resource.Id.rim);
            light_layout = (View)FindViewById(Resource.Id.light_layout);
            img = (ImageView)FindViewById(Resource.Id.imageButton2);
            callback = new CustomViewBcrResultCallback(this);
            // Calculate the coordinate information of the custom interface
            Rect mScanRect = CreateScanRectFromCamera();

            remoteView = new CustomView.Builder()
                    .SetContext(this)
                    // Set the rectangular coordinate setting of the scan frame, required, otherwise it will not be recognized.
                    .SetBoundingBox(mScanRect)
                    // Set the type of result that the bank card identification expects to return.
                    // MLBcrCaptureConfig.ResultSimple：Only identify the card number and validity period information.
                    // MLBcrCaptureConfig.ResultAll：Identify information such as card number, expiration date, issuing bank, issuing organization, and card type.
                    .SetResultType(MLBcrCaptureConfig.ResultSimple)
                    // Set result monitoring
                    .SetOnBcrResultCallback(callback).Build();

            // External calls need to be made explicitly, depending on the life cycle of the current container Activity or ViewGroup
            remoteView.OnCreate(savedInstanceState);
            FrameLayout.LayoutParams lparams = new FrameLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent,
                    LinearLayout.LayoutParams.MatchParent);
            linearLayout.AddView(remoteView, lparams);
            // Draw custom interface according to coordinates
            // In this step, you can also draw other such as scan lines, masks, and draw prompts or other buttons according to your needs.
            AddMainView(mScanRect);

            light_layout.Click += delegate
            {
                remoteView.SwitchLight();
                isLight = !isLight;
                if (isLight)
                {
                    img.SetBackgroundResource(Resource.Drawable.rn_eid_ic_hivision_light_act);
                }
                else
                {
                    img.SetBackgroundResource(Resource.Drawable.rn_eid_ic_hivision_light);
                }
            };
        }

        /// <summary>
        /// OnStart Method
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            Window window = Window;
            View decorView = window.DecorView;
            int option = (int)(Android.Views.SystemUiFlags.LayoutStable
                    | Android.Views.SystemUiFlags.Fullscreen
                    | Android.Views.SystemUiFlags.HideNavigation);
            decorView.SystemUiVisibility = (StatusBarVisibility)option;
            if ((int)Build.VERSION.SdkInt >= 21)
            {
                window.SetStatusBarColor(Color.Transparent);
                window.SetNavigationBarColor(Color.Transparent);
            }
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

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Add View to Main View
        /// </summary>
        private void AddMainView(Rect frameRect)
        {
            this.viewfinderView = new ViewfinderView(this, frameRect);
            this.viewfinderView.LayoutParameters =
                    new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            this.linearLayout.AddView(this.viewfinderView);
        }

        /// <summary>
        /// Get real screen size information
        /// </summary>
        private Point GetRealScreenSize()
        {
            int heightPixels = 0;
            int widthPixels = 0;
            Point point = null;
            IWindowManager manager = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            if (manager != null)
            {
                Display d = manager.DefaultDisplay;
                DisplayMetrics metrics = new DisplayMetrics();
                d.GetMetrics(metrics);
                heightPixels = metrics.HeightPixels;
                widthPixels = metrics.WidthPixels;
                Log.Info(Tag, "heightPixels=" + heightPixels + " widthPixels=" + widthPixels);
                try
                {
                    DisplayMetrics displayMetrics = new DisplayMetrics();
                    manager.DefaultDisplay.GetRealMetrics(displayMetrics);
                    heightPixels = displayMetrics.HeightPixels;
                    widthPixels = displayMetrics.WidthPixels;
                }
                catch(Exception e)
                {

                }
                Log.Info(Tag, "getRealScreenSize widthPixels=" + widthPixels + " heightPixels=" + heightPixels);
                point = new Point(widthPixels, heightPixels);
                return point;
            }
            return point;

        }

        /// <summary>
        /// Create Scan Rect
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        private Rect CreateScanRect(int screenWidth, int screenHeight)
        {
            float heightFactor = 0.8f;
            float CARD_SCALE = 0.63084F;
            int width = (int)Math.Round(screenWidth * heightFactor);
            int height = (int)Math.Round((float)width * CARD_SCALE);
            int leftOffset = (screenWidth - width) / 2;
            int topOffset = (int)(screenHeight * TopOffsetRatio) - height / 2;
            Log.Info(Tag, "screenWidth=" + screenWidth + " screenHeight=" + screenHeight + "  rect width=" + width
                    + " leftOffset " + leftOffset + " topOffset " + topOffset);
            Rect rect = new Rect(leftOffset, topOffset, leftOffset + width, topOffset + height);
            return rect;
        }

        /// <summary>
        /// Create Scan Rect from Camera
        /// </summary>
        private Rect CreateScanRectFromCamera()
        {
            Point point = GetRealScreenSize();
            int screenWidth = point.X;
            int screenHeight = point.Y;
            Rect rect = CreateScanRect(screenWidth, screenHeight);
            return rect;
        }
    }
    public class CustomViewBcrResultCallback : Java.Lang.Object, CustomView.IOnBcrResultCallback
    {
        private CustomActivity mActivity;
        public CustomViewBcrResultCallback(CustomActivity mActivity)
        {
            this.mActivity = mActivity;
        }
        public void OnBcrResult(MLBcrCaptureResult idCardResult)
        {
            Intent intent = new Intent();
            Bitmap bitmap = idCardResult.OriginalBitmap;
            bitmap = Bitmap.CreateScaledBitmap(bitmap, bitmap.Width / 2, bitmap.Height / 2, true);
            // Because the set mode is MLBcrCaptureConfig.RESULT_SIMPLE, only the corresponding data is returned
            intent.PutExtra("bitmap", bitmap);
            intent.PutExtra("number", idCardResult.Number);
            mActivity.SetResult(Result.Ok, intent);
            mActivity.Finish();
        }
    }
}