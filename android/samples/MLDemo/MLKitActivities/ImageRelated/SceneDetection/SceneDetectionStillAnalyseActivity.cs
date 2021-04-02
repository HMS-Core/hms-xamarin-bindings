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
using Huawei.Hms.Mlsdk.Scd;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.SceneDetection
{
    [Activity(Label = "SceneDetectionStillAnalyseActivity")]
    public class SceneDetectionStillAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "SceneDetectionStillAnalyseActivity";

        private MLSceneDetectionAnalyzer analyzer;
        private Bitmap bitmap;
        private TextView textView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_scene_analyse);
            this.FindViewById(Resource.Id.scene_detect).SetOnClickListener(this);
            textView = (TextView) FindViewById(Resource.Id.result_scene);
        }

        public void OnClick(View v)
        {
            this.Analyze();
        }

        private async void Analyze()
        {
            this.analyzer = MLSceneDetectionAnalyzerFactory.Instance.SceneDetectionAnalyzer;
            // Create an MLFrame by using android.graphics.Bitmap. Recommended image size: large than 224*224.
            Bitmap originBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.superresolution_image);
            MLFrame frame = new MLFrame.Creator()
                .SetBitmap(originBitmap)
                .Create();

            Task<IList<MLSceneDetection>> task = this.analyzer.AnalyseFrameAsync(frame);
            try
            {
                await task;
                if (task.IsCompleted && task.Result != null && task.Result.Count !=0)
                {
                    // Analyze success
                    IList<MLSceneDetection> sceneInfos = task.Result;
                    DisplaySuccess(sceneInfos);
                }
                else
                {
                    // Analyze failed
                    Log.Debug(Tag, "Analyze Failed");
                    DisplayFailure();
                }
            }
            catch(Exception ex)
            {
                // Operation failed
                Log.Error(Tag, ex.Message);
            }
        }

        private void DisplaySuccess(IList<MLSceneDetection> sceneInfos)
        {
            string str = "SceneCount：" + sceneInfos.Count + "\n";
            for (int i = 0; i < sceneInfos.Count; i++)
            {
                MLSceneDetection sceneInfo = sceneInfos.ElementAt(i);
                str += "Scene：" + sceneInfo.Result + "\n" + "Confidence：" + sceneInfo.Confidence + "\n";
            }
            textView.Text = str;
        }

        private void DisplayFailure()
        {
            Toast.MakeText(this.ApplicationContext, "Failed", ToastLength.Short).Show();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer != null)
            {
                this.analyzer.Stop();
            }
        }
    }
}