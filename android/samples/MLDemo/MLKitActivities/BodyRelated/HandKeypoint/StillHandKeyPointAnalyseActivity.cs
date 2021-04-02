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
using Huawei.Hms.Mlsdk.Handkeypoint;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.HandKeypoint
{
    [Activity(Label = "StillHandKeyPointAnalyseActivity")]
    public class StillHandKeyPointAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "StillHandKeyPointAnalyseActivity";

        private GraphicOverlay mGraphicOverlay;
        private ImageView mPreviewView;
        private Button mDetectSync;
        private Button mDetectAsync;
        private MLFrame mlFrame;
        private MLHandKeypointAnalyzer mAnalyzer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_image_hand_analyse);
            InitView();
        }

        /// <summary>
        /// Initialize elements.
        /// </summary>
        private void InitView()
        {
            mPreviewView = (ImageView) FindViewById(Resource.Id.handstill_previewPane);
            mGraphicOverlay = (GraphicOverlay) FindViewById(Resource.Id.handstill_previewOverlay);
            mDetectSync = (Button) FindViewById(Resource.Id.handstill_detect_sync);
            mDetectSync.SetOnClickListener(this);
            mDetectAsync = (Button) FindViewById(Resource.Id.handstill_detect_async);
            mDetectAsync.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.handstill_detect_async:
                    mGraphicOverlay.Clear();
                    CreateAnalyzer();
                    // Asynchronous analyse.
                    AnalyzeAsync();
                    break;
                case Resource.Id.handstill_detect_sync:
                    mGraphicOverlay.Clear();
                    CreateAnalyzer();
                    // Synchronous analyse.
                    AnalyzeSync();
                    break;
            }
        }
        /// <summary>
        /// Create analyzer
        /// </summary>
        private void CreateAnalyzer()
        {
            // Create an MLFrame by using the bitmap.
            Bitmap originBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.hand);

            // Gets the targeted width / height, only portrait.
            int maxHeight = ((View)mPreviewView.Parent).Height;
            int targetWidth = ((View)mPreviewView.Parent).Width;
            // Determine how much to scale down the image.
            float scaleFactor = Math.Max(
                    (float)originBitmap.Width / (float)targetWidth,
                    (float)originBitmap.Height / (float)maxHeight);

            Bitmap resizedBitmap = Bitmap.CreateScaledBitmap(
                    originBitmap,
                    (int)(originBitmap.Width / scaleFactor),
                    (int)(originBitmap.Height / scaleFactor),
                    true);

            mlFrame = new MLFrame.Creator().SetBitmap(resizedBitmap).Create();
            //Create analyzer setting.
            MLHandKeypointAnalyzerSetting setting =
                    new MLHandKeypointAnalyzerSetting.Factory()
                            .SetMaxHandResults(2)
                            .SetSceneType(MLHandKeypointAnalyzerSetting.TypeAll)
                            .Create();
            //Obtain analyzer instance with custom configuration.
            this.mAnalyzer = MLHandKeypointAnalyzerFactory.Instance.GetHandKeypointAnalyzer(setting);
        }

        /// <summary>
        /// Synchronous analyse.
        /// </summary>
        private void AnalyzeSync()
        {
            IList<MLHandKeypoints> mlHandKeypointsList = new List<MLHandKeypoints>();
            SparseArray mlHandKeypointsSparseArray = mAnalyzer.AnalyseFrame(mlFrame);
            for (int i = 0; i < mlHandKeypointsSparseArray.Size(); i++)
            {
                mlHandKeypointsList.Add((MLHandKeypoints)mlHandKeypointsSparseArray.ValueAt(i));
            }
            if (mlHandKeypointsList != null && mlHandKeypointsList.Count != 0)
            {
                ProcessSuccess(mlHandKeypointsList);
            }
            else
            {
                ProcessFailure();
            }
        }

        /// <summary>
        /// Asynchronous analyse.
        /// </summary>
        private async void AnalyzeAsync()
        {
            Task<IList<MLHandKeypoints>> task = mAnalyzer.AnalyseFrameAsync(mlFrame);
            try
            {
                await task;

                if(task.IsCompleted && task.Result != null)
                {
                    //Analyze Success
                    var results = task.Result;
                    ProcessSuccess(results);
                }
                else
                {
                    //Analyze Failure
                    ProcessFailure();
                }

            }
            catch(Exception e)
            {
                //Operation Failure
                Log.Info(Tag, e.Message);
            }
        }
        private void ProcessFailure()
        {
            Toast.MakeText(this.ApplicationContext, "Fail", ToastLength.Short).Show();
        }

        /// <summary>
        /// Add keypoints to imageView 
        /// </summary>
        private void ProcessSuccess(IList<MLHandKeypoints> results)
        {
            mGraphicOverlay.Clear();
            HandKeypointGraphic handGraphic = new HandKeypointGraphic(mGraphicOverlay, results);
            mGraphicOverlay.Add(handGraphic);
        }
    }
}