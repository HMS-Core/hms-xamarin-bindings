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
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Textimagesuperresolution;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.TextImageSuperResolution
{
    [Activity(Label = "TextImageSuperResolutionActivity")]
    public class TextImageSuperResolutionActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "TextImageSuperResolutionActivity";

        private static readonly int Index3x = 1;
        private static readonly int IndexOriginal = 2;
        private MLTextImageSuperResolutionAnalyzer analyzer;
        private ImageView imageView;
        private Bitmap srcBitmap;
        private Button button3px;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_text_super_resolution);
            imageView = (ImageView) FindViewById(Resource.Id.image);
            srcBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.tisr_image);
            button3px = (Button)FindViewById(Resource.Id.button_3x);
            button3px.SetOnClickListener(this);
            FindViewById(Resource.Id.button_original).SetOnClickListener(this);
            CreateAnalyzer();
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.button_3x)
            {
                DetectImage(Index3x);
            }
            else if (v.Id == Resource.Id.button_original)
            {
                DetectImage(IndexOriginal);
            }
        }

        private async void DetectImage(int type)
        {
            if (type == IndexOriginal)
            {
                SetImage(srcBitmap);
                return;
            }

            if (analyzer == null)
            {
                return;
            }           

            // Create an MLFrame by using the bitmap.
            MLFrame frame = new MLFrame.Creator().SetBitmap(srcBitmap).Create();
            Task<MLTextImageSuperResolution> task = analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;
                if (task.IsCompleted && task.Result != null)
                {
                    MLTextImageSuperResolution result = task.Result;
                    // Detection success
                    Toast.MakeText(ApplicationContext, "Success", ToastLength.Short).Show();
                    SetImage(result.Bitmap);
                }
                else
                {
                    // Detection failed
                    Log.Debug(Tag, "Detection Failed");
                }
            }
            catch(Exception e)
            {
                // Operation failed
                Log.Error(Tag, e.Message);
            }

        }

        private void Release()
        {
            if (analyzer == null)
            {
                return;
            }
            analyzer.Stop();
        }

        private void SetImage(Bitmap bitmap)
        {
            this.RunOnUiThread(() => {
                imageView.SetImageBitmap(bitmap);
            });
        }

        private void CreateAnalyzer()
        {
            this.analyzer = MLTextImageSuperResolutionAnalyzerFactory.Instance.TextImageSuperResolutionAnalyzer;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (srcBitmap != null)
            {
                srcBitmap.Recycle();
            }
            Release();
        }
    }
}