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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Huawei.Hms.Mlsdk.Common;
using Huawei.Hms.Mlsdk.Skeleton;
using HmsXamarinMLDemo.Camera;

namespace HmsXamarinMLDemo.MLKitActivities.BodyRelated.Skeleton
{
    [Activity(Label = "LiveSkeletonAnalyseActivity")]
    public class LiveSkeletonAnalyseActivity : AppCompatActivity, View.IOnClickListener
    {
        private const string Tag = "LiveSkeletonAnalyseActivity";

        public static readonly int UpdateView = 101;
        private static readonly int CameraPermissionCode = 0;

        private MLSkeletonAnalyzer analyzer;
        private LensEngine mLensEngine;
        private LensEnginePreview mPreview;
        private GraphicOverlay graphicOverlay;
        public ImageView templateImgView;
        public TextView similarityTxt;
        private int lensType = LensEngine.BackLens;
        private bool isFront = false;
        private List<MLSkeleton> templateList;
        private bool isPermissionRequested;
        private LiveSkeletonHandler mHandler;

        // coordinates for the bones of the image template
        public static float[,] TMP_SKELETONS;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Template data
            TMP_SKELETONS = new float[14,4]{
                { 416.6629f, 312.46442f, 101, 0.8042025f}, { 382.3348f, 519.43396f, 102, 0.86383355f}, { 381.0387f, 692.09515f, 103, 0.7551306f}
            , { 659.49194f, 312.24445f, 104, 0.8305682f}, { 693.5356f, 519.4844f, 105, 0.8932837f}, { 694.0054f, 692.4169f, 106, 0.8742422f}
            , { 485.08786f, 726.8787f, 107, 0.6004682f}, { 485.02808f, 935.4897f, 108, 0.7334503f}, { 485.09384f, 1177.127f, 109, 0.67240065f}
            , { 623.7807f, 726.7474f, 110, 0.5483011f}, { 624.5828f, 936.3222f, 111, 0.730425f}, { 625.81915f, 1212.2491f, 112, 0.72417295f}
            , { 521.47363f, 103.95903f, 113, 0.7780853f}, { 521.6231f, 277.2533f, 114, 0.7745689f}};

            this.SetContentView(Resource.Layout.activity_live_skeleton_analyse);
            if (savedInstanceState != null)
            {
                this.lensType = savedInstanceState.GetInt("lensType");
            }

            this.mPreview = (LensEnginePreview)this.FindViewById(Resource.Id.skeleton_preview);
            this.graphicOverlay = (GraphicOverlay)this.FindViewById(Resource.Id.skeleton_overlay);
            templateImgView = (ImageView)this.FindViewById(Resource.Id.template_imgView);
            templateImgView.SetImageResource(Resource.Drawable.skeleton_template);
            similarityTxt = (TextView)this.FindViewById(Resource.Id.similarity_txt);

            //Create analyzer
            this.CreateSkeletonAnalyzer();

            //Camera switch button
            Button facingSwitchBtn = (Button)this.FindViewById(Resource.Id.skeleton_facingSwitch);
            if (Android.Hardware.Camera.NumberOfCameras == 1)
            {
                facingSwitchBtn.Visibility = ViewStates.Gone;
            }
            facingSwitchBtn.SetOnClickListener(this);
            //Initialize template data
            InitTemplateData();

            mHandler = new LiveSkeletonHandler(this);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                this.CreateLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
            }

        }

        /// <summary>
        /// Request permission
        /// </summary>
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
        /// Handle request permission result
        /// </summary>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode != LiveSkeletonAnalyseActivity.CameraPermissionCode)
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                return;
            }
            if (grantResults.Length != 0 && grantResults[0] == Permission.Granted)
            {
                //If permission granted, create lens engine
                this.CreateLensEngine();
                return;
            }
        }

        /// <summary>
        /// OnResume
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Android.Content.PM.Permission.Granted)
            {
                this.CreateLensEngine();
                this.StartLensEngine();
            }
            else
            {
                this.RequestCameraPermission();
            }
        }

        /// <summary>
        /// Create analyzer
        /// </summary>
        private void CreateSkeletonAnalyzer()
        {
            this.analyzer = MLSkeletonAnalyzerFactory.Instance.SkeletonAnalyzer;
            this.analyzer.SetTransactor(new SkeletonAnalyzerTransactor(this, this.graphicOverlay));
        }

        /// <summary>
        /// Create lens engine
        /// </summary>
        private void CreateLensEngine()
        {
            Context context = this.ApplicationContext;
            // Create LensEngine.
            this.mLensEngine = new LensEngine.Creator(context, this.analyzer)
                    .SetLensType(this.lensType)
                    .ApplyDisplayDimension(640, 480)
                    .ApplyFps(20.0f)
                    .EnableAutomaticFocus(true)
                    .Create();
        }

        /// <summary>
        /// Start lens engine
        /// </summary>
        private void StartLensEngine()
        {
            if (this.mLensEngine != null)
            {
                try
                {
                    this.mPreview.start(this.mLensEngine, this.graphicOverlay);
                }
                catch (IOException e)
                {
                    Log.Info(LiveSkeletonAnalyseActivity.Tag, "Failed to start lens engine." + e.Message);
                    this.mLensEngine.Release();
                    this.mLensEngine = null;
                }
            }
        }

        /// <summary>
        /// On Pause
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            this.mPreview.stop();
        }

        /// <summary>
        /// On Destroy
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
                catch (IOException e)
                {
                    Log.Info(LiveSkeletonAnalyseActivity.Tag, "Stop failed: " + e.Message);
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

        /// <summary>
        /// Initialize template data
        /// </summary>
        private void InitTemplateData()
        {
            if (templateList != null)
            {
                return;
            }
            List<MLJoint> mlJointList = new List<MLJoint>();
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.skeleton_template);
            for (int i = 0; i < TMP_SKELETONS.GetLength(0); i++)
            {
                MLJoint mlJoint = new MLJoint(bitmap.Width * TMP_SKELETONS[i,0],
                        bitmap.Height * TMP_SKELETONS[i,1], (int)TMP_SKELETONS[i,2], TMP_SKELETONS[i,3]);
                mlJointList.Add(mlJoint);
            }

            templateList = new List<MLSkeleton>();
            templateList.Add(new MLSkeleton(mlJointList));
        }

        /// <summary>
        /// Compare similarity of skeletons
        /// </summary>
        /// <param name="skeletons"></param>
        public void CompareSimilarity(List<MLSkeleton> skeletons)
        {
            if (templateList == null)
            {
                return;
            }

            float similarity = 0f;
            float result = analyzer.CaluteSimilarity(skeletons, templateList);
            if (result > similarity)
            {
                similarity = result;
            }

            Message msg = Message.Obtain();
            Bundle bundle = new Bundle();
            bundle.PutFloat("similarity", similarity);
            msg.Data = bundle;
            msg.What = LiveSkeletonAnalyseActivity.UpdateView;
            mHandler.SendMessage(msg);
        }
    }

    /// <summary>
    /// Live Skeleton Handler
    /// </summary>
    public class LiveSkeletonHandler : Android.OS.Handler
    {
        private LiveSkeletonAnalyseActivity mActivity;

        public LiveSkeletonHandler(LiveSkeletonAnalyseActivity skeletonActivity)
        {
            this.mActivity = skeletonActivity;
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            if (mActivity == null || mActivity.IsFinishing)
            {
                return;
            }
            if (msg.What == LiveSkeletonAnalyseActivity.UpdateView)
            {
                Bundle bundle = msg.Data;
                float result = bundle.GetFloat("similarity");
                mActivity.similarityTxt.Visibility = ViewStates.Visible;
                mActivity.similarityTxt.Text = ("similarity:" + (int)(result * 100) + "%");
            }

        }

    }
    /// <summary>
    /// Skeleton Analyzer Transactor
    /// </summary>
    public class SkeletonAnalyzerTransactor : Java.Lang.Object, MLAnalyzer.IMLTransactor
    {
        private GraphicOverlay mGraphicOverlay;
        private LiveSkeletonAnalyseActivity mActivity;

        public SkeletonAnalyzerTransactor(LiveSkeletonAnalyseActivity skeletonActivity, GraphicOverlay ocrGraphicOverlay)
        {
            this.mActivity = skeletonActivity;
            this.mGraphicOverlay = ocrGraphicOverlay;
        }
        public void Destroy()
        {
            this.mGraphicOverlay.Clear();
        }

        public void TransactResult(MLAnalyzer.Result result)
        {
            this.mGraphicOverlay.Clear();

            SparseArray sparseArray = result.AnalyseList;
            List<MLSkeleton> list = new List<MLSkeleton>();
            for (int i = 0; i < sparseArray.Size(); i++)
            {
                list.Add((MLSkeleton)sparseArray.ValueAt(i));
            }
            // Remove invalid point.
            List<MLSkeleton> skeletons = SkeletonUtils.GetValidSkeletons(list);
            SkeletonGraphic graphic = new SkeletonGraphic(this.mGraphicOverlay, skeletons);
            this.mGraphicOverlay.Add(graphic);
           
            if (mActivity != null && !mActivity.IsFinishing)
            {
                mActivity.CompareSimilarity(skeletons);
            }
        }
    }
}