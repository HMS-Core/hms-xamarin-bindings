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
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Imgseg;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.ImageSegmentation
{
    [Activity(Label = "ImageSegmentationLiveAnalyseActivity")]
    public class ImageSegmentationLiveAnalyseActivity :AppCompatActivity  ,View.IOnClickListener
    {
        private const string Tag = "ImageSegmentationLiveAnalyseActivity";

        private static readonly int CameraPermissionCode = 1;

        private MLImageSegmentationAnalyzer analyzer;
        private LensEngine mLensEngine;
        public LensEnginePreview mPreview;
        public GraphicOverlay mOverlay;
        private int lensType = LensEngine.FrontLens;
        public bool isFront = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_live_segment_analyse);
            this.mPreview = (LensEnginePreview)this.FindViewById(Resource.Id.segment_preview);
            this.mOverlay = (GraphicOverlay)this.FindViewById(Resource.Id.segment_overlay);
            this.FindViewById(Resource.Id.facingSwitch).SetOnClickListener(this);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }
            this.CreateSegmentAnalyzer();
            // Checking Camera Permissions.
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                this.CreateLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
            }
        }

        //Request permission
        private void RequestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, CameraPermissionCode);
                return;
            }
        }

        public void OnClick(View v)
        {
            this.isFront = !this.isFront;
            if (this.isFront)
            {
                this.lensType = LensEngine.FrontLens;
            }
            else
            {
                this.lensType = LensEngine.BackLens;
            }
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Close();
            }
            this.CreateLensEngine();
            this.StartLensEngine();
        }

        private void CreateLensEngine()
        {
            Context context = this.ApplicationContext;
            // Create LensEngine.
            this.mLensEngine = new LensEngine.Creator(context, this.analyzer).SetLensType(this.lensType)
                    .ApplyDisplayDimension(960, 720)
                    .ApplyFps(25.0f)
                    .EnableAutomaticFocus(true)
                    .Create();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode != CameraPermissionCode)
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                return;
            }
            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                this.CreateLensEngine();
                return;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("lensType", this.lensType);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.StartLensEngine();
        }

        private void CreateSegmentAnalyzer()
        {
            // Create a segmentation analyzer. You can create an analyzer using the provided customized face detection parameter: MLImageSegmentationSetting
            MLImageSegmentationSetting setting = new MLImageSegmentationSetting.Factory()
                    .SetExact(false)
                    .SetScene(MLImageSegmentationScene.ForegroundOnly)
                    .SetAnalyzerType(MLImageSegmentationSetting.BodySeg)
                    .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetImageSegmentationAnalyzer(setting);
            this.analyzer.SetTransactor(new MLTransactor(this));
        }

        private void StartLensEngine()
        {
            if (this.mLensEngine != null)
            {
                try
                {
                    this.mPreview.start(this.mLensEngine, this.mOverlay);
                }
                catch (Exception e)
                {
                    Log.Info(Tag, "Failed to start lens engine.", e);
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
                }
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
            this.mPreview.stop();
        }

        /// <summary>
        /// Stop analyzer on OnDestroy() event.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Release();
            }
            if (this.analyzer != null)
            {
                try
                {
                    this.analyzer.Stop();
                }
                catch (Exception e)
                {
                    Log.Info(Tag, "Stop failed: " + e.Message);
                }
            }
        }
    }

    public class MLTransactor : Java.Lang.Object, MLAnalyzer.IMLTransactor
    {
        ImageSegmentationLiveAnalyseActivity imgLiveAnalyseActivity;
        public MLTransactor(ImageSegmentationLiveAnalyseActivity ImgLiveAnalyseActivity)
        {
            this.imgLiveAnalyseActivity = ImgLiveAnalyseActivity;
        }

        public void Destroy()
        {
            
        }

        public void TransactResult(MLAnalyzer.Result result)
        {
            this.imgLiveAnalyseActivity.mOverlay.Clear();
            SparseArray imageSegmentationResult = result.AnalyseList;
            MLSegmentGraphic graphic = new MLSegmentGraphic(this.imgLiveAnalyseActivity.mPreview, this.imgLiveAnalyseActivity.mOverlay, (MLImageSegmentation)imageSegmentationResult.ValueAt(0), this.imgLiveAnalyseActivity.isFront);
            this.imgLiveAnalyseActivity.mOverlay.Add(graphic);
        }
    }
}