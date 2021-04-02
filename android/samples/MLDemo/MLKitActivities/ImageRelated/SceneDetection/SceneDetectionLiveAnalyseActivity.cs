/*
        Copyright 2020-2021. Huawei Technologies Co., Ltd. All rights reserved.

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
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Scd;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.SceneDetection
{
    [Activity(Label = "SceneDetectionLiveAnalyseActivity")]
    public class SceneDetectionLiveAnalyseActivity : AppCompatActivity, View.IOnClickListener,  MLAnalyzer.IMLTransactor
    {
        private const string Tag = "SceneDetectionLiveAnalyseActivity";
        private const int CameraPermissionCode = 0;

        private MLSceneDetectionAnalyzer analyzer;
        private LensEngine mLensEngine;
        private LensEnginePreview mPreview;
        private GraphicOverlay mOverlay;
        private int lensType = LensEngine.FrontLens;
        private bool isFront = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_live_scene_analyse);
            this.mPreview = (LensEnginePreview) this.FindViewById(Resource.Id.scene_preview);
            this.mOverlay = (GraphicOverlay) this.FindViewById(Resource.Id.scene_overlay);
            this.FindViewById(Resource.Id.facingSwitch).SetOnClickListener(this);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }

            this.CreateSegmentAnalyzer();
            // Checking Camera Permissions
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
            {
                this.CreateLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
            }
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
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                this.CreateLensEngine();
                this.StartLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
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
                    Log.Error(Tag, "Failed to start lens engine.", e);
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
                }
            }
        }

        private void CreateSegmentAnalyzer()
        {
            this.analyzer = MLSceneDetectionAnalyzerFactory.Instance.SceneDetectionAnalyzer;
            this.analyzer.SetTransactor(this);
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
                this.analyzer.Stop();
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

        /// <summary>
        /// Implemented from MLAnalyzer.IMLTransactor interface
        /// </summary>
        public void Destroy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implemented from MLAnalyzer.IMLTransactor interface.
        /// Process the results returned by the analyzer.
        /// </summary>
        public void TransactResult(MLAnalyzer.Result result)
        {
            mOverlay.Clear();
            SparseArray imageSegmentationResult = result.AnalyseList;
            IList<MLSceneDetection> list = new List<MLSceneDetection>();
            for (int i = 0; i < imageSegmentationResult.Size(); i++)
            {
                list.Add((MLSceneDetection)imageSegmentationResult.ValueAt(i));
            }
            MLSceneDetectionGraphic sceneDetectionGraphic = new MLSceneDetectionGraphic(mOverlay, list);
            mOverlay.Add(sceneDetectionGraphic);
            mOverlay.PostInvalidate();
        }
    }
}