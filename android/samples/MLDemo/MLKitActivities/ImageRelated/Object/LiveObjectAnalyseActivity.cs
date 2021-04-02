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
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Huawei.Hms.Mlsdk;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Objects;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.ImageRelated.Object
{
    [Activity(Label = "LiveObjectAnalyseActivity")]
    public class LiveObjectAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "LiveObjectAnalyseActivity";

        private const int CameraPermissionCode = 1;
        public const int StopPreview = 1;
        public const int StartPreview = 2;
        private MLObjectAnalyzer analyzer;
        private LensEngine mLensEngine;
        private bool isStarted = true;
        private LensEnginePreview mPreview;
        private GraphicOverlay mOverlay;
        private int lensType = LensEngine.BackLens;
        public bool mlsNeedToDetect = true;
        public ObjectAnalysisHandler mHandler;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_live_object_analyse);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }
            this.mPreview = (LensEnginePreview)this.FindViewById(Resource.Id.object_preview);
            this.mOverlay = (GraphicOverlay)this.FindViewById(Resource.Id.object_overlay);
            this.CreateObjectAnalyzer();
            this.FindViewById(Resource.Id.detect_start).SetOnClickListener(this);

            mHandler = new ObjectAnalysisHandler(this);
            // Checking Camera Permissions
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
        /// <summary>
        /// Start Lens Engine on OnResume() event.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            this.StartLensEngine();
        }
        /// <summary>
        /// Stop Lens Engine on OnPause() event.
        /// </summary>
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
                    Log.Info(LiveObjectAnalyseActivity.Tag, "Stop failed: " + e.Message);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        { 
            if (requestCode != LiveObjectAnalyseActivity.CameraPermissionCode)
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

        private void StopPreviewAction()
        {
            this.mlsNeedToDetect = false;
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
                    Log.Info("object", "Stop failed: " + e.Message);
                }
            }
            this.isStarted = false;
        }

        private void StartPreviewAction()
        {
            if (this.isStarted)
            {
                return;
            }
            this.CreateObjectAnalyzer();
            this.mPreview.release();
            this.CreateLensEngine();
            this.StartLensEngine();
            this.isStarted = true;
        }

        private void CreateLensEngine()
        {
            Context context = this.ApplicationContext;
            // Create LensEngine
            this.mLensEngine = new LensEngine.Creator(context, this.analyzer).SetLensType(this.lensType)
                    .ApplyDisplayDimension(640, 480)
                    .ApplyFps(25.0f)
                    .EnableAutomaticFocus(true)
                    .Create();
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
                    Log.Info(LiveObjectAnalyseActivity.Tag, "Failed to start lens engine.", e);
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
                }
            }
        }

        public void OnClick(View v)
        {
            this.mHandler.SendEmptyMessage(LiveObjectAnalyseActivity.StartPreview);
        }

        private void CreateObjectAnalyzer()
        {
            // Create an object analyzer
            // Use MLObjectAnalyzerSetting.TypeVideo for video stream detection.
            // Use MLObjectAnalyzerSetting.TypePicture for static image detection.
            MLObjectAnalyzerSetting setting =
                    new MLObjectAnalyzerSetting.Factory().SetAnalyzerType(MLObjectAnalyzerSetting.TypeVideo)
                            .AllowMultiResults()
                            .AllowClassification()
                            .Create();
            this.analyzer = MLAnalyzerFactory.Instance.GetLocalObjectAnalyzer(setting);
            this.analyzer.SetTransactor(new ObjectAnalyseMLTransactor(this));
            }

        public class ObjectAnalysisHandler : Android.OS.Handler
        {
            private LiveObjectAnalyseActivity liveObjectAnalyseActivity;

            public ObjectAnalysisHandler(LiveObjectAnalyseActivity LiveObjectAnalyseActivity)
            {
                this.liveObjectAnalyseActivity = LiveObjectAnalyseActivity;
            }

            public override void HandleMessage(Message msg)
            {
                base.HandleMessage(msg);
                switch (msg.What)
                {
                    case LiveObjectAnalyseActivity.StartPreview:
                        this.liveObjectAnalyseActivity.mlsNeedToDetect = true;
                        //Log.d("object", "start to preview");
                        this.liveObjectAnalyseActivity.StartPreviewAction();
                        break;
                    case LiveObjectAnalyseActivity.StopPreview:
                        this.liveObjectAnalyseActivity.mlsNeedToDetect = false;
                        //Log.d("object", "stop to preview");
                        this.liveObjectAnalyseActivity.StopPreviewAction();
                        break;
                    default:
                        break;
                }
            }
        }
        public class ObjectAnalyseMLTransactor : Java.Lang.Object, MLAnalyzer.IMLTransactor
        {
            private LiveObjectAnalyseActivity liveObjectAnalyseActivity;
            public ObjectAnalyseMLTransactor(LiveObjectAnalyseActivity LiveObjectAnalyseActivity)
            {
                this.liveObjectAnalyseActivity = LiveObjectAnalyseActivity;
            }

            public void Destroy()
            {

            }

            public void TransactResult(MLAnalyzer.Result result)
            {
                if (!liveObjectAnalyseActivity.mlsNeedToDetect) {
                    return;
                }
                this.liveObjectAnalyseActivity.mOverlay.Clear();
                SparseArray objectSparseArray = result.AnalyseList;
                for (int i = 0; i < objectSparseArray.Size(); i++)
                {
                    MLObjectGraphic graphic = new MLObjectGraphic(liveObjectAnalyseActivity.mOverlay, ((MLObject)(objectSparseArray.ValueAt(i))));
                    liveObjectAnalyseActivity.mOverlay.Add(graphic);
                }
                // When you need to implement a scene that stops after recognizing specific content
                // and continues to recognize after finishing processing, refer to this code
                for (int i = 0; i < objectSparseArray.Size(); i++)
                {
                    if (((MLObject)(objectSparseArray.ValueAt(i))).TypeIdentity == MLObject.TypeFood)
                    {
                        liveObjectAnalyseActivity.mlsNeedToDetect = true;
                        liveObjectAnalyseActivity.mHandler.SendEmptyMessage(LiveObjectAnalyseActivity.StopPreview);
                    }
                }
            }
        }
    }
}