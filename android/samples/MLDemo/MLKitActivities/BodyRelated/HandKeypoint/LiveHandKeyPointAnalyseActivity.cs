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
using Huawei.Hms.Mlsdk.Handkeypoint;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.HandKeypoint
{
    [Activity(Label = "LiveHandKeyPointAnalyseActivity")]
    public class LiveHandKeyPointAnalyseActivity : AppCompatActivity ,View.IOnClickListener
    {
        private const string Tag = "LiveHandKeyPointAnalyseActivity";

        private LensEnginePreview mPreview;
        private GraphicOverlay mOverlay;
        private Button mFacingSwitch;
        private MLHandKeypointAnalyzer mAnalyzer;
        private LensEngine mLensEngine;
        private int lensType = LensEngine.BackLens;
        private int mLensType;
        private bool isFront = false;
        private static readonly int CameraPermissionCode = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_live_handkeypoint_analyse);
            if (savedInstanceState != null)
            {
                mLensType = savedInstanceState.GetInt("lensType");
            }
            InitView();
            CreateHandAnalyzer();
            if (Android.Hardware.Camera.NumberOfCameras == 1)
            {
                mFacingSwitch.Visibility = ViewStates.Gone;
            }
            // Checking Camera Permissions
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
            {
                CreateLensEngine();
            }
            else
            {
                CheckPermission();
            }
        }

        /// <summary>
        /// Check permissions
        /// </summary>
        private void CheckPermission()
        {
            string[] permissions = new string[] { Manifest.Permission.Camera };

            if (!ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                ActivityCompat.RequestPermissions(this, permissions, CameraPermissionCode);
                return;
            }
        }

        /// <summary>
        /// Handle request permission result
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {

            if (requestCode != CameraPermissionCode)
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                return;
            }
            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                //If permission granted, create lens engine.
                this.CreateLensEngine();
                return;
            }
        }

        /// <summary>
        /// Initializing graphic elements
        /// </summary>
        private void InitView()
        {
            mPreview = (LensEnginePreview)FindViewById(Resource.Id.hand_preview);
            mOverlay = (GraphicOverlay)FindViewById(Resource.Id.hand_overlay);
            mFacingSwitch = (Button)FindViewById(Resource.Id.handswitch);
            mFacingSwitch.SetOnClickListener(this);
        }

        /// <summary>
        /// Create Hand Keypoint analyzer
        /// with custom setting.
        /// </summary>
        private void CreateHandAnalyzer()
        {
            // Create a  analyzer. You can create an analyzer using the provided customized handkeypoint detection parameter: MLHandKeypointAnalyzerSetting
            MLHandKeypointAnalyzerSetting setting =
                    new MLHandKeypointAnalyzerSetting.Factory()
                            .SetMaxHandResults(2)
                            .SetSceneType(MLHandKeypointAnalyzerSetting.TypeAll)
                            .Create();
            mAnalyzer = MLHandKeypointAnalyzerFactory.Instance.GetHandKeypointAnalyzer(setting);
            mAnalyzer.SetTransactor(new HandAnalyzerTransactor(this, mOverlay));
        }

        /// <summary>
        /// Create Lens Engine
        /// </summary>
        private void CreateLensEngine()
        {
            Context context = this.ApplicationContext;
            // Create LensEngine.
            mLensEngine = new LensEngine.Creator(context, mAnalyzer)
                    .SetLensType(this.mLensType)
                    .ApplyDisplayDimension(640, 480)
                    .ApplyFps(25.0f)
                    .EnableAutomaticFocus(true)
                    .Create();
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
                    Log.Info(Tag, "Failed to start lens engine." + e.Message );
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
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
            switch (v.Id)
            {
                case Resource.Id.handswitch:
                    SwitchCamera();
                    break;
            }
        }

        /// <summary>
        /// Switch Camera
        /// Front Lens - Back Lens
        /// </summary>
        private void SwitchCamera()
        {
            isFront = !isFront;
            if (this.isFront)
            {
                mLensType = LensEngine.FrontLens;
            }
            else
            {
                mLensType = LensEngine.BackLens;
            }
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Close();
            }
            this.CreateLensEngine();
            this.StartLensEngine();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                CreateLensEngine();
                StartLensEngine();
            }
            else
            {
                CheckPermission();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            mPreview.stop();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this.mLensEngine != null)
            {
                this.mLensEngine.Release();
            }
            if (this.mAnalyzer != null)
            {
                this.mAnalyzer.Stop();
            }
        }


    }
    /// <summary>
    /// Analyzer Transactor Interface
    /// </summary>
    public class HandAnalyzerTransactor : Java.Lang.Object, MLAnalyzer.IMLTransactor
    {
        private GraphicOverlay mGraphicOverlay;
        LiveHandKeyPointAnalyseActivity mActivity;

        public HandAnalyzerTransactor(LiveHandKeyPointAnalyseActivity mActivity, GraphicOverlay ocrGraphicOverlay)
        {
            this.mActivity = mActivity;
            this.mGraphicOverlay = ocrGraphicOverlay;
        }

        public void Destroy()
        {
            this.mGraphicOverlay.Clear();
        }

        public void TransactResult(MLAnalyzer.Result result)
        {
            this.mGraphicOverlay.Clear();

            SparseArray handKeypointsSparseArray = result.AnalyseList;
            List<MLHandKeypoints> list = new List<MLHandKeypoints>();
            for (int i = 0; i < handKeypointsSparseArray.Size(); i++)
            {
                list.Add((MLHandKeypoints)handKeypointsSparseArray.ValueAt(i));
            }
            HandKeypointGraphic graphic = new HandKeypointGraphic(this.mGraphicOverlay, list);
            this.mGraphicOverlay.Add(graphic);
        }
    }

}