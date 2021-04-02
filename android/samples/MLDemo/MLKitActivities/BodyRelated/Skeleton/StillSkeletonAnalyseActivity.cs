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
using System.IO;
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
using Huawei.Hms.Mlsdk.Skeleton;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Skeleton
{
    [Activity(Label = "StillSkeletonAnalyseActivity")]
    public class StillSkeletonAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "StillSkeletonAnalyseActivity";

        private GraphicOverlay graphicOverlay;
        private ImageView previewView;
        private MLSkeletonAnalyzer analyzer;
        private MLFrame mFrame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_image_skeleton_analyse);
            this.previewView = (ImageView)FindViewById(Resource.Id.skeleton_previewPane);
            this.graphicOverlay = (GraphicOverlay)FindViewById(Resource.Id.skeleton_previewOverlay);
            this.FindViewById(Resource.Id.skeleton_detect_sync).SetOnClickListener(this);
            this.FindViewById(Resource.Id.skeleton_detect_async).SetOnClickListener(this);
        }

        private void CreateAnalyzer()
        {
            // Create an MLFrame by using the bitmap.
            Bitmap originBitmap = BitmapFactory.DecodeResource(this.Resources, Resource.Drawable.skeleton_image);

            // Gets the targeted width / height, only portrait.
            int maxHeight = ((View)previewView.Parent).Height;
            int targetWidth = ((View)previewView.Parent).Width;

            // Determine how much to scale down the image
            float scaleFactor =
                    Math.Max(
                            (float)originBitmap.Width / (float)targetWidth,
                            (float)originBitmap.Height / (float)maxHeight);

            Bitmap resizedBitmap =
                    Bitmap.CreateScaledBitmap(
                            originBitmap,
                            (int)(originBitmap.Width / scaleFactor),
                            (int)(originBitmap.Height / scaleFactor),
                            true);

            mFrame = new MLFrame.Creator().SetBitmap(resizedBitmap).Create();
            analyzer = MLSkeletonAnalyzerFactory.Instance.SkeletonAnalyzer;
        }

        /// <summary>
        /// Synchronous Analyze
        /// </summary>
        private void AnalyzeSync()
        {
            List<MLSkeleton> list = new List<MLSkeleton>();
            SparseArray sparseArray = analyzer.AnalyseFrame(mFrame);
            for (int i = 0; i < sparseArray.Size(); i++)
            {
                list.Add((MLSkeleton)sparseArray.ValueAt(i));
            }
            // Remove invalid point.
            List<MLSkeleton> skeletons = SkeletonUtils.GetValidSkeletons(list);
            if (skeletons != null && !(skeletons.Count == 0))
            {
                ProcessSuccess(skeletons);
            }
            else
            {
                ProcessFailure();
            }
        }

        /// <summary>
        /// Asynchronous Analyze
        /// </summary>
        private async void AnalyzeAsync()
        {
            Task<IList<MLSkeleton>> task = analyzer.AnalyseFrameAsync(mFrame);

            try
            {
                await task;

                if(task.IsCompleted && task.Result != null)
                {
                    //Analyze Success
                    var skeletons = task.Result;
                    if (!(skeletons.Count == 0))
                    {
                        ProcessSuccess(skeletons);
                    }
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
                Log.Error(Tag, " analyzeAsync failed: " + e.Message);
                ProcessFailure();
            }
        }

        public void OnClick(View v)
            {
            switch (v.Id)
            {
                case Resource.Id.skeleton_detect_async:
                    graphicOverlay.Clear();
                    CreateAnalyzer();
                    // Asynchronous analyse.
                    AnalyzeAsync();
                    break;
                case Resource.Id.skeleton_detect_sync:
                    graphicOverlay.Clear();
                    CreateAnalyzer();
                    // Synchronous analyse.
                    AnalyzeSync();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// On Destroy
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.analyzer == null)
            {
                return;
            }
            try
            {
                this.analyzer.Stop();
            }
            catch (IOException e)
            {
                Log.Info(StillSkeletonAnalyseActivity.Tag, "Stop failed: " + e.Message);
            }
        }

        /// <summary>
        /// Display message when process failure
        /// </summary>
        private void ProcessFailure()
        {
            Toast.MakeText(this, "Fail", ToastLength.Short).Show();
        }

        /// <summary>
        /// Handle result when process success
        /// </summary>
        /// <param name="results"></param>
        private void ProcessSuccess(IList<MLSkeleton> results)
        {
            graphicOverlay.Clear();
            SkeletonGraphic skeletonGraphic = new SkeletonGraphic(graphicOverlay, results);
            graphicOverlay.Add(skeletonGraphic);
        }

    }
}