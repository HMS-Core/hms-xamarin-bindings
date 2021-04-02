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
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Face.Face3d;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Face3d
{
    [Activity(Label = "Live3DFaceAnalyseActivity")]
    public class Live3DFaceAnalyseActivity : AppCompatActivity, View.IOnClickListener, MLAnalyzer.IMLTransactor
    {

        private const string Tag = "Live3DFaceAnalyseActivity";

        private const int CameraPermissionCode = 0;
        private ML3DFaceAnalyzer analyzer;
        private LensEngine mLensEngine;
        private LensEnginePreview mPreview;
        private GraphicOverlay mGraphicOverlay;
        private int lensType = LensEngine.FrontLens;
        private bool isFront = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_live_face3d_analyse);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }
            this.mPreview = (LensEnginePreview) this.FindViewById(Resource.Id.preview);
            this.mGraphicOverlay = (GraphicOverlay) this.FindViewById(Resource.Id.overlay);
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

        private void CreateFaceAnalyzer()
        {
            // Create a face analyzer. You can create an analyzer using the provided customized face detection parameter
            ML3DFaceAnalyzerSetting setting = new ML3DFaceAnalyzerSetting.Factory()
                    // Fast detection of continuous video frames.
                    // MLFaceAnalyzerSetting.TypePrecision: indicating the precision preference mode.
                    // This mode will detect more faces and be more precise in detecting key points and contours, but will run slower.
                    // MLFaceAnalyzerSetting.TypeSpeed: representing a preference for speed.
                    // This will detect fewer faces and be less precise in detecting key points and contours, but will run faster.
                    // .SetPerformanceType(MLFaceAnalyzerSetting.TypeSpeed)
                    .SetPerformanceType(ML3DFaceAnalyzerSetting.TypeSpeed)
                    .SetTracingAllowed(true)
                    .Create();

            this.analyzer = MLAnalyzerFactory.Instance.Get3DFaceAnalyzer(setting);
            this.analyzer.SetTransactor(this);
        }

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

        /// <summary>
        /// After modifying the face analyzer configuration, you need to create a face analyzer again.
        /// </summary>
        private void ReStartAnalyzer()
        {
            if (mPreview != null)
            {
                mPreview.stop();
            }
            if (mLensEngine != null)
            {
                mLensEngine.Release();
            }
            if (analyzer != null)
            {
                try
                {
                    analyzer.Stop();
                }
                catch (Exception e)
                {
                    Log.Error(Tag, e.Message);
                }
            }
            CreateFaceAnalyzer();
            CreateLensEngine();
            StartLensEngine();
        }

        private void StartLensEngine()
        {
            if (this.mLensEngine != null)
            {
                try
                {
                    this.mPreview.start(this.mLensEngine, this.mGraphicOverlay);
                }
                catch (Exception e)
                {
                    Log.Error(Live3DFaceAnalyseActivity.Tag, "Failed to start lens engine.", e);
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
                }
            }
        }

        /// <summary>
        /// Implemented from MLAnalyzer.IMLTransactor interface.
        /// </summary>
        public void Destroy()
        {
            this.mGraphicOverlay.Clear();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.StartLensEngine();
        }

        /// <summary>
        /// Implemented from MLAnalyzer.IMLTransactor interface.
        /// </summary>
        public void TransactResult(MLAnalyzer.Result result)
        {
            this.mGraphicOverlay.Clear();
            SparseArray faceSparseArray = result.AnalyseList;
            for (int i = 0; i < faceSparseArray.Size(); i++)
            {
                ML3DFaceGraphic graphic = new ML3DFaceGraphic(this.mGraphicOverlay, (ML3DFace)faceSparseArray.ValueAt(i), this);
                this.mGraphicOverlay.Add(graphic);
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.mPreview.stop();
        }

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
                    Log.Error(Live3DFaceAnalyseActivity.Tag, "Stop failed: " + e.Message);
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("lensType", this.lensType);
            base.OnSaveInstanceState(outState);
        }

        //Request Camera Permission
        private void RequestCameraPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, Live3DFaceAnalyseActivity.CameraPermissionCode);
                return;
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (requestCode != Live3DFaceAnalyseActivity.CameraPermissionCode)
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