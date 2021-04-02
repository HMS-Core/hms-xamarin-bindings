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
using Huawei.Hms.Mlsdk.Face;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face
{
    [Activity(Label = "LiveFaceAnalyseActivity")]
    public class LiveFaceAnalyseActivity : AppCompatActivity ,View.IOnClickListener
    {
        private const string Tag = "LiveFaceAnalyseActivity";

        private static readonly int CameraPermissionCode = 1;

        private MLFaceAnalyzer analyzer;
        private LensEngine mLensEngine;
        private LensEnginePreview mPreview;
        private GraphicOverlay mOverlay;

        private int lensType = LensEngine.FrontLens;

        private bool isFront = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_live_face_analyse);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }
            this.mPreview = (LensEnginePreview)this.FindViewById(Resource.Id.preview);
            this.mOverlay = (GraphicOverlay)this.FindViewById(Resource.Id.overlay);
            this.CreateFaceAnalyzer();
            this.FindViewById(Resource.Id.facingSwitch).SetOnClickListener(this);
            //Checking Camera permission
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
            {
                this.CreateLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
            }
        }

        //Request Camera Permission
        private void RequestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, LiveFaceAnalyseActivity.CameraPermissionCode);
                return;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (requestCode != LiveFaceAnalyseActivity.CameraPermissionCode)
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

        /// <summary>
        /// Create Face Analyzer
        /// </summary>
        private void CreateFaceAnalyzer()
        {
            // Create a face analyzer. You can create an analyzer using the provided customized face detection parameter
            // MLFaceAnalyzerSetting
            MLFaceAnalyzerSetting setting =
                    new MLFaceAnalyzerSetting.Factory()
                            .SetFeatureType(MLFaceAnalyzerSetting.TypeFeatures)
                            .SetKeyPointType(MLFaceAnalyzerSetting.TypeKeypoints)
                            .SetMinFaceProportion(0.2f)
                            .AllowTracing()
                            .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetFaceAnalyzer(setting);
            this.analyzer.SetTransactor(new FaceAnalyzerTransactor(this.mOverlay));
        }

        /// <summary>
        /// Create Lens Engine for live
        /// face analyzing
        /// </summary>
        private void CreateLensEngine()
        {
            Context context = this.ApplicationContext;
            // Create LensEngine. Recommended image size: large than 320*320, less than 1920*1920.
            this.mLensEngine = new LensEngine.Creator(context, this.analyzer)
                    .SetLensType(this.lensType)
                    .ApplyDisplayDimension(640, 480)
                    .ApplyFps(25.0f)
                    .EnableAutomaticFocus(true)
                    .Create();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.StartLensEngine();
        }

        /// <summary>
        /// Start Lens Engine
        /// </summary>
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
                    //Log.e(LiveFaceAnalyseActivity.TAG, "Failed to start lens engine.", e);
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
        /// Stop engine and analyzer on OnDestroy() event.
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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("lensType", this.lensType);
            base.OnSaveInstanceState(outState);
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
    }
}